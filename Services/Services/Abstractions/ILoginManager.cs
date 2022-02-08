using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ILoginManager
    {
        /// <summary>
        /// Gets the current logged NameIdentifier that is our UserId in Db
        /// </summary>
        /// <returns></returns>
        Task<string> GetCurrentUserIdWithOutBasicAuth();
        /// <summary>
        /// Authenticate user by username and password and return Claims
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<List<Claim>> AuthenticateUserByUsernameAndPassword(string username, string password);

        /// <summary>
        /// Gets userId from Claims if it was authenticated with basic auth
        /// </summary>
        /// <returns></returns>
        Task<string> GetCurrentUserIdWithBasicAuth();
    }
}
