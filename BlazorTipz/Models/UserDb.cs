using BlazorTipz.Components;
using BlazorTipz.Data;
using DataLibrary;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.Models
{
    public class UserDb 
    {
        private readonly IConfiguration _config;
        public string name { get; set; } = string.Empty;
        public string employmentId { get; set; }
        public RoleE role { get; set; } = RoleE.User;
        public string teamId { get; set; }
        public string password { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        public bool active { get; set; } = true;
        public bool firstTimeLogin { get; set; }
        public string AuthToken { get; private set; }

        //inject _data



        //constructor
        public UserDb()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }
        public UserDb(UserViewmodel user)
        {
            _config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", true, true)
                 .Build();
            
            this.employmentId = user.employmentId;
            this.teamId = user.teamId;
            this.name = user.name;
            this.role = user.role;
            this.firstTimeLogin = user.firstTimeLogin;
            if (user.password != null)
            {
                passwordHashing(user.password);
            }
            
        }

        public void CreateToken()
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, this.employmentId),
            new Claim(JwtRegisteredClaimNames.GivenName, this.name),
            new Claim(ClaimTypes.Role, this.role.ToString())
        };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);


            AuthToken = jwt;

        }

        public void passwordHashing(string pass)
        {
            CreatePasswordHash(pass, out byte[] passHash, out byte[] passSalt);
            passwordHash = passHash;
            passwordSalt = passSalt;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
