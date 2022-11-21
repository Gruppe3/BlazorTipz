using BlazorTipz.Data;
using BlazorTipz.ViewModels.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BlazorTipz.Models
{
    public class UserEntity 
    {
        
        private readonly IConfiguration _config;
        
        //database
        public string userName { get; set; } = string.Empty;
        public string employmentId { get; set; } = string.Empty;
        public RoleE userRole { get; set; } = RoleE.User;
        public string? teamId { get; set; } = null;
        public byte[] passwordHash { get; set; } = new byte[64];
        public byte[] passwordSalt { get; set; } = new byte[64];
        public bool active { get; set; } = true;
        public bool firstTimeLogin { get; set; } = false;
        
        //local
        public string AuthToken { get; private set; } = string.Empty; 
        public string password { get; set; } = string.Empty;

        public string passHash { get; set; } = string.Empty;
        public string passSalt { get; set; } = string.Empty;
        //inject _data



        //constructor
        public UserEntity()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }
        public UserEntity(UserViewmodel user)
        {
            _config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", true, true)
                 .Build();
            
            this.employmentId = user.EmploymentId;
            this.teamId = user.TeamId;
            this.userName = user.Name;
            this.userRole = user.UserRole;
            this.firstTimeLogin = user.FirstTimeLogin;
            if (user.Password != null)
            {
                PasswordHashing(user.Password);
            }
            
        }

        public void CreateToken()
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, this.employmentId),
            new Claim(JwtRegisteredClaimNames.GivenName, this.userName),
            new Claim(ClaimTypes.Role, this.userRole.ToString())
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

        public void PasswordHashing(string pass)
        {
            CreatePasswordHash(pass, out byte[] passHash, out byte[] passSalt);
            passwordHash = passHash;
            passwordSalt = passSalt;
            //get database values
            this.passHash = Convert.ToBase64String(passHash);
            this.passSalt = Convert.ToBase64String(passSalt);
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
