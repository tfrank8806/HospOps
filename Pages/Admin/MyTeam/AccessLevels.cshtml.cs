// ============================================================================
// File: Pages/Admin/MyTeam/AccessLevels.cshtml.cs  (ADD NEW FILE)
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.MyTeam
{
    [Authorize(Roles = "Manager,Admin")]
    public class AccessLevelsModel : PageModel
    {
        public void OnGet() { }
    }
}
