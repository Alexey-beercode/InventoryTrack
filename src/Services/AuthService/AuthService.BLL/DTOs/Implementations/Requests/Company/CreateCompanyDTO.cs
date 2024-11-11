namespace AuthService.BLL.DTOs.Implementations.Requests.Company;

public class CreateCompanyDTO
{
    public string Name { get; set; }
    public int Unp { get; set; } 
    public string LegalAddress { get; set; }
    public string PostalAddress { get; set; }
    public Guid ResponsibleUserId { get; set; }
}