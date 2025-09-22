// ============================================================================
// File: Pages/Admin/MyTeam/Users.cshtml.cs  (ADD NEW FILE)
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.MyTeam
{
    [Authorize(Roles = "Manager,Admin")]
    public class UsersModel : PageModel
    {
        public void OnGet() { }
    }
}
