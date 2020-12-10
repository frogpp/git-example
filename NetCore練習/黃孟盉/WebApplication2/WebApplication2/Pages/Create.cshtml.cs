using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2.Models;

namespace WebApplication2.Pages
{
    public class CreateModel : PageModel
    {
        private readonly WebApplication2.Models.JournalDbContext _context;

        public CreateModel(WebApplication2.Models.JournalDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public UserRecord UserRecord { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Records.Add(UserRecord);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
