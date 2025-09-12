using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirect authenticated users straight to Logbook for convenience
            if (User?.Identity?.IsAuthenticated == true)
                return Redirect("/Logbook");

            return Page();
        }
    }
}
