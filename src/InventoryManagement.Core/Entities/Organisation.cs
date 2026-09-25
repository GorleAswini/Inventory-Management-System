namespace InventoryManagement.Core.Entities;

public class Organisation
{
    public int Id { get; set; }
    public string OrganisationName { get; set; } = string.Empty;
    public string OrganisationCode { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoPath { get; set; }

    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
