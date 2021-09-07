using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CivicQARedux.Data;
using CivicQARedux.Models.FormResponses;
using Microsoft.AspNetCore.Authorization;
using CivicQARedux.Models.Forms;
using CivicQARedux.Services;
using Microsoft.AspNetCore.Identity;
using CivicQARedux.Services.Tagging;
using CivicQARedux.Models.Tags;

namespace CivicQARedux.Controllers
{
    /// <summary>
    /// Controls actions for FormResponses,
    /// including anonymous creation
    /// </summary>
    [Authorize]
    public class FormResponsesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ICurrentUserProvider _userProvider;
        private readonly ITagProvider _tagProvider;

        /// <summary>
        /// Create a new FormResponseController
        /// </summary>
        /// <param name="context">Application DB Context</param>
        /// <param name="currentUserProvider">providers logged in <see cref="IdentityUser">User</see> </param>
        public FormResponsesController(
            ApplicationContext context,
            ICurrentUserProvider currentUserProvider,
            ITagProvider tagProvider)
        {
            _context = context;
            _userProvider = currentUserProvider;
            _tagProvider = tagProvider;
        }

        /// <summary>
        /// Returns view of all FormResponses for logged in user's Forms
        /// </summary>
        /// <returns>View</returns>
        // GET: FormResponses
        public async Task<IActionResult> Index()
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            // Response ordering:
            // All actives first
            // Then, order by date, most recent first
            var responses = _context.Responses
                .OrderByDescending(r => r.IsActive)
                .ThenByDescending(r => r.CreatedAt)
                .Include(r => r.Form)
                .Where(r => r.Form.UserId == user.Id);

            return View(await responses.ToListAsync());
        }

        /// <summary>
        /// Return detailed view + response area for FormResponse
        /// </summary>
        /// <param name="id">Id of FormResponse</param>
        /// <returns>View</returns>
        // GET: FormResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var formResponse = await _context.Responses
                .Include(r => r.Tags)
                .Include(r => r.Form)
                .Where(r => r.Form.UserId == user.Id)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (formResponse == null)
            {
                return NotFound();
            }

            return View(formResponse);
        }

        /// <summary>
        /// Creation view for FormResponse, responding to
        /// Form with id.
        /// Allows Anonymous, meant to be public facing.
        /// </summary>
        /// <param name="id">Id of Form to respond to</param>
        /// <returns>View</returns>
        // GET: FormResponses/Create/
        [AllowAnonymous]
        public async Task<IActionResult> Create(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Form respondingTo = await _context.Forms.FindAsync(id);
            if (respondingTo is null)
            {
                return NotFound();
            }

            var input = new FormResponseInput()
            {
                FormId = respondingTo.Id
            };

            return View(input);
        }

        /// <summary>
        /// Accepts creation of FormResponse.
        /// Allows Anonymous, meant to be public facing.
        /// Returns confirmation view.
        /// </summary>
        /// <param name="input">FormResponse creation values</param>
        /// <returns>View</returns>
        // POST: FormResponses/Create/
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create(
            [Bind("FullName,EmailAddress,Subject,Body,FormId")] FormResponseInput input)
        {
            if (ModelState.IsValid)
            {
                Form respondingTo = await _context.Forms.FindAsync(input.FormId);
                if (respondingTo is null)
                {
                    return NotFound();
                }

                FormResponse formResponse = FormResponse.FromInput(input, respondingTo);

                List<string> tagTexts = await _tagProvider.GenerateTags(formResponse);
                foreach (string text in tagTexts)
                {
                    formResponse.Tags.Add(Tag.FromInput(new TagInput {Text = text }));
                }


                _context.Add(formResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateConfirm));
            }
            return View(input);
        }

        /// <summary>
        /// FormResponse creation confirmation view.
        /// Allows Anonymous, meant to be public facing.
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public IActionResult CreateConfirm()
        {
            return View();
        }

        /// <summary>
        /// Marks a FormResponse as active
        /// </summary>
        /// <param name="id">id of FormResponse</param>
        /// <returns>Empty 200</returns>
        // POST: FormResponses/MarkActive/5
        [HttpPost]
        public async Task<IActionResult> MarkActive(int? id)
        {
            return await SetIsActive(id, true);
        }

        /// <summary>
        /// Marks a FormResponse as inactive
        /// </summary>
        /// <param name="id">id of FormResponse</param>
        /// <returns>Empty 200</returns>
        // POST: FormResponses/MarkInactive/5
        [HttpPost]
        public async Task<IActionResult> MarkInactive(int? id)
        {
            return await SetIsActive(id, false);
        }

        /// <summary>
        /// Helper to set IsActive value of FormResponse
        /// </summary>
        /// <param name="id">Id of FormResponse</param>
        /// <param name="isActive">new value for IsActive</param>
        /// <returns>Empty 200</returns>
        private async Task<IActionResult> SetIsActive(int? id, bool isActive)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            if (id is null)
            {
                return NotFound();
            }

            var formResponse = await _context.Responses
                .Where(r => r.Form.UserId == user.Id)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (formResponse is null)
            {
                return NotFound();
            }

            formResponse.IsActive = isActive;

            _context.Responses.Update(formResponse);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException) when (!FormResponseExists((int)id))
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// View for deletion confirmation for a FormResponse
        /// </summary>
        /// <param name="id">Id of FormResponse</param>
        /// <returns>View</returns>
        // GET: FormResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            if (id == null)
            {
                return NotFound();
            }

            var formResponse = await _context.Responses
                .Include(f => f.Form)
                .Where(f => f.Form.UserId == user.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (formResponse == null)
            {
                return NotFound();
            }

            return View(formResponse);
        }

        /// <summary>
        /// Deletes a FormResponse.
        /// Redirects to index view.
        /// </summary>
        /// <param name="id">Id of FormResponse</param>
        /// <returns>view</returns>
        // POST: FormResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }
            var formResponse = await _context.Responses
                .Where(r => r.Form.UserId == user.Id)
                .FirstOrDefaultAsync(r => r.Id == id);

            _context.Responses.Remove(formResponse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormResponseExists(int id)
        {
            return _context.Responses.Any(e => e.Id == id);
        }
    }
}
