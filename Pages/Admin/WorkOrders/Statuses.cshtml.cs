// ============================================================================
// File: Pages/Admin/WorkOrders/Statuses.cshtml.cs  (ADD NEW FILE)
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.WorkOrders
{
    [Authorize(Roles = "Manager,Admin")]
    public class StatusesModel : PageModel
    {
        public void OnGet() { }
    }
}
