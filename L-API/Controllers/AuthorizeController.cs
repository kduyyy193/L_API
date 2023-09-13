using L_API.Modal;
using L_API.Repos;
using L_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace L_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : Controller
    {
        private readonly DbL2Context context;
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshTokenHandler refreshToken;

        public AuthorizeController(DbL2Context context, IOptions<JwtSettings> options, IRefreshTokenHandler refreshToken)
        {
            this.context = context;
            this.jwtSettings = options.Value;
            this.refreshToken = refreshToken;
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCred userCred)
        {
            if (userCred == null || userCred.username == null)
            {
                return Unauthorized();
            }

            var user = await this.context.Users.FirstOrDefaultAsync(item => item.Code == userCred.username && item.Password == userCred.password);
            if (user == null)
            {
                return Unauthorized();
            }

            var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.SecurityKey);
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Code),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDesc);
            var finalToken = tokenHandler.WriteToken(token);

            var refreshTokenValue = await this.refreshToken.GenerateToken(userCred.username);

            return Ok(new TokenResponse
            {
                RefreshToken = refreshTokenValue,
                Token = finalToken
            });
        }
        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] TokenResponse token)
        {
            var _refreshToken = await this.context.Refreshtokens.FirstOrDefaultAsync(item => item.RefreshToken == token.RefreshToken);
            if (_refreshToken == null)
            {
                return Unauthorized();
            }

            var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.SecurityKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token.Token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out securityToken);

            var _token = securityToken as JwtSecurityToken;

            if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                string? username = principal.Identity?.Name;
                if (username != null)
                {
                    var _existData = await this.context.Refreshtokens.FirstOrDefaultAsync(item => item.Userid == username && item.RefreshToken == token.RefreshToken);
                    if (_existData != null)
                    {
                        var _newToken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(
                                new SymmetricSecurityKey(tokenKey),
                                SecurityAlgorithms.HmacSha256
                            )
                        );

                        var _finaltoken = tokenHandler.WriteToken(_newToken);
                        return Ok(new TokenResponse() { Token = _finaltoken, RefreshToken = await this.refreshToken.GenerateToken(username) });
                    }
                }
            }

            return Unauthorized();
        }


    }
}
