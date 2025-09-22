// ============================================================================
// File: Pages/Admin/Calendar/Categories.cshtml.cs  (ADD NEW FILE)
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.Calendar
{
    [Authorize(Roles = "Manager,Admin")]
    public class CategoriesModel : PageModel
    {
        public void OnGet() { }
    }
}
