using ProjectManagement.API.Dtos;

namespace ProjectManagement.BLL.Interfaces;

public interface IAuthService
{
    LoginResponseDto Login(LoginRequestDto req);
    MeDto Me(System.Security.Claims.ClaimsPrincipal user);
}
