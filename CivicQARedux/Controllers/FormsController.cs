using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CivicQARedux.Data;
using CivicQARedux.Models.Forms;
using Microsoft.AspNetCore.Authorization;

using CivicQARedux.Services;

namespace CivicQARedux.Controllers
{
    [Authorize]
    public class FormsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ICurrentUserProvider _userProvider;

        public FormsController(ApplicationContext context, ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _userProvider = currentUserProvider;
        }

        // GET: Forms
        public async Task<IActionResult> Index()
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            List<FormDTO> results = await _context.Forms
                .Include(f => f.FormResponses)
                .Where(f => f.UserId == user.Id)
                .Select(f => FormDTO.FromForm(f))
                .ToListAsync();

            return View(results);
        }

        // GET: Forms/Details/5
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

            var form = await _context.Forms
                .Include(f => f.FormResponses
                    .OrderByDescending(r => r.IsActive)
                    .ThenByDescending(r => r.CreatedAt))
                .Where(f => f.UserId == user.Id)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (form == null)
            {
                return NotFound();
            }

            return View(FormDTO.FromForm(form));
        }

        //// Get: Forms/Respond/{id}
        // [AllowAnonymous]
        //public IActionResult Respond(int? id)
        //{

        //}

        // GET: Forms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title")] FormInput formInput)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                _context.Forms.Add(Form.FromInput(formInput, user));
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(formInput);
        }

        // GET: Forms/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var form = await _context.Forms
                .Where(f => f.UserId == user.Id)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (form == null)
            {
                return NotFound();
            }

            return View(new FormInput { Title = form.Title});
        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title")] FormInput input)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Form formToEdit = await _context.Forms
                        .Where(f => f.UserId == user.Id)
                        .FirstOrDefaultAsync(f => f.Id == id);

                    formToEdit.Title = input.Title;

                    _context.Forms.Update(formToEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!FormExists(id))
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: Forms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .Include(f => f.FormResponses)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (form == null)
            {
                return NotFound();
            }

            return View(FormDTO.FromForm(form));
        }

        // POST: Forms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            Form form = await _context.Forms
                .Where(f => f.UserId == user.Id)
                .FirstOrDefaultAsync(f => f.Id == id); 

            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormExists(int id)
        {
            return _context.Forms.Any(e => e.Id == id);
        }
    }
}
