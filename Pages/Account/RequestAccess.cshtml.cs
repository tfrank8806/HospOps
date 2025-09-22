// ============================================================================
// File: Pages/Account/RequestAccess.cshtml.cs  (ADD NEW FILE)
// Public page to submit an access request.
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Account
{
    public class RequestAccessModel : PageModel
    {
        private readonly HospOpsContext _db;
        public RequestAccessModel(HospOpsContext db) => _db = db;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? MobilePhone { get; set; }
            public string PropertyCode { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty; // stored only after approval
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.FirstName) ||
                string.IsNullOrWhiteSpace(Input.LastName) ||
                string.IsNullOrWhiteSpace(Input.Email) ||
                string.IsNullOrWhiteSpace(Input.PropertyCode) ||
                string.IsNullOrWhiteSpace(Input.Password))
            {
                ModelState.AddModelError(string.Empty, "All fields except Mobile Phone are required.");
                return Page();
            }

            var dupe = _db.AccessRequests.FirstOrDefault(a => a.Email == Input.Email && !a.Approved);
            if (dupe != null)
            {
                TempData["Msg"] = "You already have a pending request. A manager will review it.";
                return RedirectToPage("/Account/RequestAccess");
            }

            _db.AccessRequests.Add(new AccessRequest
            {
                FirstName = Input.FirstName.Trim(),
                LastName = Input.LastName.Trim(),
                Email = Input.Email.Trim(),
                MobilePhone = string.IsNullOrWhiteSpace(Input.MobilePhone) ? null : Input.MobilePhone.Trim(),
                PropertyCode = Input.PropertyCode.Trim(),
                // Password is NOT stored here for security; an approver will create the Identity user.
            });

            await _db.SaveChangesAsync();
            TempData["Msg"] = "Request submitted. A manager will review and approve.";
            return RedirectToPage("/Account/RequestAccess");
        }
    }
}
