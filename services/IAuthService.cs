using AuthenticationAutherization.data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAutherization.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        List<IdentityRole> GetAllRoles();
        Task<AuthModel> Login([FromBody] LoginModel model);
    }
}
