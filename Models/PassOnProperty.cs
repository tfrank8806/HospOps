// ============================================================================
// File: Models/PassOnProperty.cs   (REPLACE ENTIRE FILE)
// ============================================================================
namespace HospOps.Models
{
    /// <summary>Links a PassOnNote to a property (by ID only to avoid missing type).</summary>
    public class PassOnProperty
    {
        public int Id { get; set; }

        public int PassOnNoteId { get; set; }
        public PassOnNote? PassOnNote { get; set; }

        // Keep FK only; navigation removed because HospOps.Models.Property does not exist.
        public int PropertyId { get; set; }
    }
}
