using AuthService.BLL.DTOs.Implementations.Responses.User;

namespace AuthService.BLL.DTOs.Implementations.Responses.Company;

public class CompanyResponseDTO
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string Unp { get; set; } 
    public string LegalAddress { get; set; }
    public string PostalAddress { get; set; }
    public UserRepsonseDTO ResponsiblePerson { get; set; }
}