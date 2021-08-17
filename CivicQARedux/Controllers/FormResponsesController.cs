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

namespace CivicQARedux.Controllers
{
    [Authorize]
    public class FormResponsesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ICurrentUserProvider _userProvider;

        public FormResponsesController(ApplicationContext context, ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _userProvider = currentUserProvider;
        }

        // GET: FormResponses
        public async Task<IActionResult> Index()
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            var responses = _context.Responses
                .OrderByDescending(r => r.IsActive)
                .ThenByDescending(r => r.CreatedAt)
                .Include(r => r.Form)
                .Where(r => r.Form.UserId == user.Id);

            return View(await responses.ToListAsync());
        }

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
                .Include(f => f.Form)
                .Where(f => f.Form.UserId == user.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (formResponse == null)
            {
                return NotFound();
            }

            return View(formResponse);
        }

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

        // POST: FormResponses/Create/
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("FullName,EmailAddress,Subject,Body,FormId")] FormResponseInput input)
        {
            if (ModelState.IsValid)
            {
                Form respondingTo = await _context.Forms.FindAsync(input.FormId);
                if (respondingTo is null)
                {
                    return NotFound();
                }

                FormResponse formResponse = FormResponse.FromInput(input, respondingTo);

                _context.Add(formResponse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateConfirm));
            }
            return View(input);
        }

        [AllowAnonymous]
        public IActionResult CreateConfirm()
        {
            return View();
        }

        // POST: FormResponses/MarkActive/5
        [HttpPost]
        public async Task<IActionResult> MarkActive(int? id)
        {
            return await SetIsActive(id, true);
        }

        // POST: FormResponses/MarkInactive/5
        [HttpPost]
        public async Task<IActionResult> MarkInactive(int? id)
        {
            return await SetIsActive(id, false);
        }

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
