// ============================================================================
// File: Pages/Admin/Property/RoomNumbers.cshtml.cs  (ADD NEW FILE)
// Stub page for room numbers management
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.Property
{
    [Authorize(Roles = "Manager,Admin")]
    public class RoomNumbersModel : PageModel
    {
        public void OnGet() { }
    }
}
