﻿using BlazorTipz.Components;
using BlazorTipz.Data;
using DataLibrary;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;

namespace BlazorTipz.Models
{
    public class UserDb : UserA
    {
        private readonly IConfiguration _config ;
        private readonly AuthenticationComponent _auth;

        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }

        public string AuthToken { get; private set; }

        //inject _data



        //constructor
        public UserDb()
        {
                

        }
        public UserDb(UserA user)
        {
           
            this.employmentId = user.employmentId;
            this.teamId = user.teamId;
            this.name = user.name;
            this.role = user.role;
            passwordHashing(user.password);
            CreateToken();
        }

        public void CreateToken()
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, this.employmentId),
            new Claim(JwtRegisteredClaimNames.GivenName, this.name),
            new Claim(JwtRegisteredClaimNames.UniqueName, this.teamId),
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
            _auth.CreatePasswordHash(pass, out byte[] passHash, out byte[] passSalt);
            passwordHash = passHash;
            passwordSalt = passSalt;
        }
    }
}
