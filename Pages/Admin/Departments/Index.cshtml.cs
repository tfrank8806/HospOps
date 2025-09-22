// ============================================================================
// File: Pages/Admin/Departments/Index.cshtml.cs  (ADD NEW FILE)
// Stub list page (we'll add CRUD later)
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.Departments
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        public void OnGet() { }
    }
}
