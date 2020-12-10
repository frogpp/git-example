using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication2.Models.JournalDbContext _context;

        public IndexModel(WebApplication2.Models.JournalDbContext context)
        {
            _context = context;
        }

        public IList<UserRecord> DailyRecord { get;set; }

        public async Task OnGetAsync()
        {
            DailyRecord = await _context.Records.ToListAsync();
        }
    }
}
