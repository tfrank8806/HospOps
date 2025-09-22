// ============================================================================
// File: Pages/Admin/WorkOrders/Types.cshtml.cs  (ADD NEW FILE)
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.WorkOrders
{
    [Authorize(Roles = "Manager,Admin")]
    public class TypesModel : PageModel
    {
        public void OnGet() { }
    }
}
