using AuthService.Domain.Interfaces;

namespace AuthService.Domain.Enities;

public class Company:BaseEntity
{
    public int Unp { get; set; }
    public string Name { get; set; }
    public string LegalAddress { get; set; }
    public string PostalAddress { get; set; }
    public Guid ResponsibleUserId { get; set; }
}