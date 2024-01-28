using AuthenticationAutherization.data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAutherization.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Jwt _jwt;
        SignInManager<IdentityUser> signInManager;
        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<Jwt> jwt, SignInManager<IdentityUser> _signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            signInManager = _signInManager;

        }
        public List<IdentityRole> GetAllRoles()
        {
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            return roles;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already register!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already register!" };

            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, model.RoleName);

            var jwtSecurityToken = await CreateJwtToken(user);
            AddRoleModel roleModel = new AddRoleModel();
            roleModel.UserId = user.Id;
            roleModel.Role = model.RoleName;
            AddRoleAsync(roleModel);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                ISAuthenticated = true,
                Roles = new List<string> { model.RoleName },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };


        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || await _roleManager.RoleExistsAsync(model.Role))
            {
                return "Invalid User Id or Role";
            }
            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                return "User already assigned to this role";
            }
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
                return string.Empty;

            return "Something went wrong";

        }

        private async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleCliams = new List<Claim>();
            foreach (var role in roles)
                roleCliams.Add(new Claim("roles", role));

            var cliams = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleCliams);
            var symmetricSecurrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurrityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: cliams,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;

        }
        public async Task<AuthModel> Login([FromBody] LoginModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var role = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var jwtSecurityToken = await CreateJwtToken(user);
                //var jwtSecurityToken =  CreateToken(user);
                return new AuthModel
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),

                };
             
            }
            else
            {
                 return new AuthModel
                 {
               

                 };
            }
        }

        //Old way to generate token
        //private string CreateToken(IdentityUser user)
        //{
        //    List<Claim> claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name ,user.UserName)
        //    };
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authnetication , this is my custom Secret key for authnetication"));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.Aes256CbcHmacSha512);
        //    var token = new JwtSecurityToken(
        //        claims: claims,
        //         expires: DateTime.Now.AddDays(1),
        //         signingCredentials: creds
        //         );
        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        //    return jwt;
        //}
    }
}
