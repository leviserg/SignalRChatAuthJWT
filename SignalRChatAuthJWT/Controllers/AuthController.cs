using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SignalRChatAuthJWT.Auth;
using SignalRChatAuthJWT.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SignalRChatAuthJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsersList fakeUsers;

        public AuthController(IOptions<UsersList> fakeUsers)
        {
            this.fakeUsers = fakeUsers.Value;
        }

        [HttpPost("token")]
        public IActionResult Token([FromBody] AuthModel authModel)
        {
            var user = fakeUsers.Users.FirstOrDefault(x => x.Login == authModel.Login);

            if (user == null)
                return BadRequest();

            if (user.PasswordHash != authModel.Password.GetMd5Hash()) //GetSha1()//
                return BadRequest();

            var token = GetJwt(user);

            return Ok(token);
        }

        private string GetJwt(FakeUser fakeUser)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                notBefore: now,
                claims: GetClaims(fakeUser),
                expires: now.AddMinutes(AuthOptions.Lifetime),
                signingCredentials: new SigningCredentials(AuthOptions.PrivateKey, SecurityAlgorithms.RsaSha256)
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

            return tokenString;
        }

        private IEnumerable<Claim> GetClaims(FakeUser fakeUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, fakeUser.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, fakeUser.Role),
                new Claim(ClaimTypes.NameIdentifier, fakeUser.Login)
            };

            return claims;
        }
    }
}
