using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Services
{
    public class LoginManager : ILoginManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LoginManager> _logger;
        private readonly ReadLaterDataContext _readLaterDataContext;

        public LoginManager(IHttpContextAccessor httpContextAccessor, ILogger<LoginManager> logger, ReadLaterDataContext readLaterDataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _readLaterDataContext = readLaterDataContext;
        }

        public async Task<string> GetCurrentUserIdWithOutBasicAuth()
        {
            var result = "";
            try
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(result))
                {
                    _logger.LogInformation($"{nameof(GetCurrentUserIdWithOutBasicAuth)} is empty");
                }

            }
            catch (Exception e)
            {

            }
            return result;
        }

        public async Task<string> GetCurrentUserIdWithBasicAuth()
        {
            var result = "";
            try
            {
                Claim claim = await GetClaimByType("UserId");
                if (claim == null)
                {
                    return null;
                }
                result = claim.Value;
            }
            catch (Exception e)
            {
               
            }

            return result;
        }

        private async Task<Claim> GetClaimByType(string claimType)
        {
            Claim result = null;
            try
            {
                IEnumerable<Claim> claims = null;

                if (_httpContextAccessor.HttpContext != null)
                {
                    var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

                    if (identity != null)
                    {
                        claims = identity.Claims;
                        if (claims != null)
                        {
                            result = claims.Where(x => x.Type == claimType).FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }

        public async Task<List<Claim>> AuthenticateUserByUsernameAndPassword(string username, string password)
        {
            try
            {
                var user = await _readLaterDataContext.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                //TODO:: NEED TO CHECK IF PASSWORD HASHED IS SAME WITH INPUT PASSWORD, we assume its the same

                var claimCollection = new List<Claim>();

                claimCollection.Add(new Claim("UserId", user.Id));

                var checkUserClaim = await _readLaterDataContext.UserClaims.Where(x => x.UserId == user.Id && x.ClaimType == "UserId").FirstOrDefaultAsync();

                if (checkUserClaim == null)
                {
                    var entity = new IdentityUserClaim<string>
                    {
                        UserId = user.Id,
                        ClaimType = "UserId",
                        ClaimValue = user.Id
                    };
                    await _readLaterDataContext.UserClaims.AddAsync(entity);
                    await _readLaterDataContext.SaveChangesAsync();
                }


                return claimCollection;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
