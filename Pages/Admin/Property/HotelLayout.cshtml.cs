// ============================================================================
// File: Pages/Admin/Property/HotelLayout.cshtml.cs  (ADD NEW FILE)
// Stub page (authorized). We'll wire a proper editor next.
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.Property
{
    [Authorize(Roles = "Manager,Admin")]
    public class HotelLayoutModel : PageModel
    {
        public void OnGet() { }
    }
}
