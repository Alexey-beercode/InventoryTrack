namespace AuthService.BLL.DTOs.Implementations.Requests.Company;

public class UpdateCompanyDTO
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string LegalAddress { get; set; }
    public string PostalAddress { get; set; }
    public Guid ResponsiblePersonId { get; set; }
}