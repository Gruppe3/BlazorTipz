using BlazorTipz.Data;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using System.Text;

namespace BlazorTipz.Components
{
    public class AuthenticationComponent
    {

        private readonly IConfiguration _configuration;
        //Get IConfiguration from DI
        public AuthenticationComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        // get Claimvalue Identity.Name from encoded JwtSecurityToken with key from appsettings.json
        public string GetClaimValue(string token)
        {
            //claimtype name
            String claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            return tokenS.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.Ordinal)).Value;
        }






        public String readTokenForId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            return tokenS.Claims.First(claim => claim.Type == "nameid").Value;
        }   
    }
}
