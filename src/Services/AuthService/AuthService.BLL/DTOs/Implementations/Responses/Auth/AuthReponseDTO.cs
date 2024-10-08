namespace AuthService.BLL.DTOs.Implementations.Responses.Auth;

public class AuthReponseDTO
{
    public string AccessToken { get; set; }
    public Guid UserId { get; set; }
}