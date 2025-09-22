// ============================================================================
// File: Pages/Admin/Property/Locations.cshtml.cs  (ADD NEW FILE)
// Stub page for locations management
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Admin.Property
{
    [Authorize(Roles = "Manager,Admin")]
    public class LocationsModel : PageModel
    {
        public void OnGet() { }
    }
}
