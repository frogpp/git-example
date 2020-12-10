using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Pages
{
    public class EditModel : PageModel
    {
        private readonly WebApplication2.Models.JournalDbContext _context;

        public EditModel(WebApplication2.Models.JournalDbContext context)
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(UserRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DailyRecordExists(UserRecord.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DailyRecordExists(string id)
        {
            return _context.Records.Any(e => e.Id == id);
        }
    }
}
