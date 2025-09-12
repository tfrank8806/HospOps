// File: Models/Department.cs
namespace HospOps.Models
{
    /// <summary>
    /// Hotel departments used across Logbook, Work Orders, etc.
    /// </summary>
    public enum Department
    {
        FrontDesk = 0,
        Housekeeping = 1,
        Maintenance = 2,
        Management = 3,
        FoodAndBeverage = 4,
        Security = 5,
        Other = 9
    }
}