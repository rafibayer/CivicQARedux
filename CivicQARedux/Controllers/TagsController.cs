using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CivicQARedux.Data;
using CivicQARedux.Models.Tags;
using CivicQARedux.Services;
using Microsoft.AspNetCore.Authorization;
using CivicQARedux.Models.FormResponses;

namespace CivicQARedux.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ICurrentUserProvider _userProvider;

        public TagsController(ApplicationContext context, ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _userProvider = currentUserProvider;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            var applicationContext = _context.Tags
                .Where(t => t.FormResponse.Form.UserId == user.Id);

            return View(await applicationContext.ToListAsync());
        }

        // GET: Tags/Details/5
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

            var tag = await _context.Tags
                .Where(t => t.FormResponse.Form.UserId == user.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,FormResponseId")] TagInput input)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                FormResponse responseToTag = await _context.Responses
                    .Where(r => r.Form.UserId == user.Id)
                    .FirstOrDefaultAsync(r => r.Id == input.FormResponseId);

                if (responseToTag is null)
                {
                    return NotFound();
                }

                Tag tag = Tag.FromInput(input);

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "FormResponses", new { id = input.FormResponseId });
            }
            return BadRequest(ModelState);
        }


        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            var tag = await _context.Tags
                .Where(t => t.FormResponse.Form.UserId == user.Id)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag is null)
            {
                return NotFound();
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "FormResponses", new { id = tag.FormResponseId });
        }
    }
}
