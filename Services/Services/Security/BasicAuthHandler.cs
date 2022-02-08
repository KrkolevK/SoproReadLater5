using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Abstractions;

namespace Services.Security
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILoginManager _loginManager;
        private readonly IHttpContextAccessor _httpContext;
        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ILoginManager loginManager, IHttpContextAccessor httpContext) : base(options, logger, encoder, clock)
        {
            _loginManager = loginManager;
            _httpContext = httpContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            if (Request.Headers.ContainsKey("Authorization"))
            {
                //! Get the auth header content
                var authorizationHeader = Request.Headers["Authorization"];

                var credentialsBase64 = authorizationHeader.ToString().Substring("Basic ".Length)?.Trim();

                string credentials;

                try
                {
                    credentials = Encoding.UTF8.GetString(Convert.FromBase64String(credentialsBase64));
                }
                catch
                {
                    return AuthenticateResult.Fail(new Exception("Invalid Credentials"));
                }

                var credentialsSplit = credentials?.Split(":");

                if (credentialsSplit?.Length != 2)
                {
                    return AuthenticateResult.Fail(new Exception("Invalid Credentials"));
                }

                var username = credentialsSplit[0];
                var password = credentialsSplit[1];

                var claims = await _loginManager.AuthenticateUserByUsernameAndPassword(username, password);

                if (claims != null)
                {
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));

                    var ticket = new AuthenticationTicket(principal, "BasicAuth");

                    return AuthenticateResult.Success(ticket);
                }

                return AuthenticateResult.Fail(new Exception("Did not authenticate"));
            }
            return AuthenticateResult.NoResult();

        }
    }
  
}
