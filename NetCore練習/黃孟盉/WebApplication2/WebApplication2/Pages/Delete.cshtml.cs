using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly WebApplication2.Models.JournalDbContext _context;

        public DeleteModel(WebApplication2.Models.JournalDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserRecord UserRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserRecord = await _context.Records.FirstOrDefaultAsync(m => m.Id == id);

            if (UserRecord == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserRecord = await _context.Records.FindAsync(id);

            if (UserRecord != null)
            {
                _context.Records.Remove(UserRecord);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
