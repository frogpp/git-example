using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly WebApplication2.Models.JournalDbContext _context;

        public DetailsModel(WebApplication2.Models.JournalDbContext context)
        {
            _context = context;
        }

        public UserRecord DailyRecord { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DailyRecord = await _context.Records.FirstOrDefaultAsync(m => m.Id == id);

            if (DailyRecord == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
