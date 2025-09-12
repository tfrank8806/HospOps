// File: HospOps/Pages/Phonebook/Index.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HospOps.Pages.Phonebook;

[Authorize]
public class IndexModel : PageModel
{
    private readonly HospOpsContext _db;
    public IndexModel(HospOpsContext db) => _db = db;

    // ---- Form DTOs (all optional fields) ----
    public class PhoneInput { public string? Label { get; set; } public string? Number { get; set; } }
    public class EmailInput { public string? Label { get; set; } public string? Address { get; set; } }

    public class NewContactInput
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Notes { get; set; }
        public List<PhoneInput> Phones { get; set; } = new() { new PhoneInput() };
        public List<EmailInput> Emails { get; set; } = new() { new EmailInput() };
    }

    [BindProperty]
    public NewContactInput NewContact { get; set; } = new();

    public List<Contact> Contacts { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    // ---- Helpers used by the Razor view ----
    public string DisplayName(Contact c)
    {
        var first = (c.FirstName ?? string.Empty).Trim();
        var last = (c.LastName ?? string.Empty).Trim();
        var name = (first + " " + last).Trim();
        if (string.IsNullOrWhiteSpace(name)) name = c.Company?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(name) ? "—" : name;
    }

    public string PrimaryPhone(Contact c)
    {
        var p = c.Phones?.OrderBy(x => x.SortOrder).FirstOrDefault()?.Number;
        return string.IsNullOrWhiteSpace(p) ? "—" : p!;
    }

    public string PrimaryEmail(Contact c)
    {
        var e = c.Emails?.OrderBy(x => x.SortOrder).FirstOrDefault()?.Address;
        return string.IsNullOrWhiteSpace(e) ? "—" : e!;
    }

    // ---- Handlers ----
    public async Task OnGet()
    {
        var query = _db.Contacts
            .Include(c => c.Phones)
            .Include(c => c.Emails)
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var term = Q.Trim();
            query = query.Where(c =>
                (c.FirstName != null && EF.Functions.Like(c.FirstName, $"%{term}%")) ||
                (c.LastName != null && EF.Functions.Like(c.LastName, $"%{term}%")) ||
                (c.Company != null && EF.Functions.Like(c.Company, $"%{term}%")) ||
                (c.Address != null && EF.Functions.Like(c.Address, $"%{term}%")) ||
                (c.Website != null && EF.Functions.Like(c.Website, $"%{term}%")) ||
                c.Phones.Any(p => p.Number != null && EF.Functions.Like(p.Number, $"%{term}%")) ||
                c.Emails.Any(e => e.Address != null && EF.Functions.Like(e.Address, $"%{term}%"))
            );
        }

        Contacts = await query
            .OrderBy(c => c.Company)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreate()
    {
        var c = new Contact
        {
            FirstName = NewContact.FirstName?.Trim(),
            LastName = NewContact.LastName?.Trim(),
            Company = NewContact.Company?.Trim(),
            Address = NewContact.Address?.Trim(),
            Website = string.IsNullOrWhiteSpace(NewContact.Website) ? null : NewContact.Website!.Trim(),
            Notes = NewContact.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        int pOrder = 0;
        foreach (var p in NewContact.Phones.Where(p => !string.IsNullOrWhiteSpace(p.Number)))
        {
            c.Phones.Add(new ContactPhone
            {
                Label = p.Label?.Trim(),
                Number = p.Number!.Trim(),
                SortOrder = pOrder++
            });
        }
        int eOrder = 0;
        foreach (var e in NewContact.Emails.Where(e => !string.IsNullOrWhiteSpace(e.Address)))
        {
            c.Emails.Add(new ContactEmail
            {
                Label = e.Label?.Trim(),
                Address = e.Address!.Trim(),
                SortOrder = eOrder++
            });
        }

        _db.Contacts.Add(c);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Contact added.";
        return RedirectToPage(new { Q });
    }

    public async Task<IActionResult> OnPostDelete(int id)
    {
        var c = await _db.Contacts.FindAsync(id);
        if (c == null) return NotFound();
        c.IsDeleted = true;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Contact removed (soft delete).";
        return RedirectToPage(new { Q });
    }

    public async Task<IActionResult> OnPostRestore(int id)
    {
        var c = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        if (c == null) return NotFound();
        c.IsDeleted = false;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Contact restored.";
        return RedirectToPage(new { Q });
    }
}
