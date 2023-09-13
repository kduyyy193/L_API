using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using L_API.Repos;

namespace L_API.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DbL2Context context;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            DbL2Context context) : base(options, logger, encoder, clock)
        {
            this.context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header is missing");
            }

                var headervalue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                if(headervalue.Parameter != null)
                {
                    var bytes = Convert.FromBase64String(headervalue.Parameter);
                    string credentials = Encoding.UTF8.GetString(bytes);
                    string[] array = credentials.Split(":");
                    string username = array[0];
                    string password = array[1];

                    var user = await this.context.Users.FirstOrDefaultAsync(u => u.Code == username && u.Password == password);

                    if (user != null)
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, user.Code) };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalid username or password");
                    }
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid authorization header");
                }
        }
    }
}