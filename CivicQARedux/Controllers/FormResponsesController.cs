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

namespace CivicQARedux.Controllers
{
    [Authorize]
    public class FormResponsesController : Controller
    {
        private readonly ApplicationContext _context;

        public FormResponsesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: FormResponses
        public async Task<IActionResult> Index()
        {
            var responses = _context.Responses
                .OrderByDescending(r => r.IsActive)
                .ThenByDescending(r => r.CreatedAt)
                .Include(f => f.Form);

            return View(await responses.ToListAsync());
        }

        // GET: FormResponses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formResponse = await _context.Responses
                .Include(f => f.Form)
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
            if (id is null)
            {
                return NotFound();
            }

            var formResponse = await _context.Responses
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
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok();
        }


        // GET: FormResponses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formResponse = await _context.Responses
                .Include(f => f.Form)
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
            var formResponse = await _context.Responses.FindAsync(id);
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
