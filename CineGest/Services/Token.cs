using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace CineGest.Services
{
    public class Token
    {
        private string userId, role, name, token;

        private string myIssuer = Environment.GetEnvironmentVariable("ASPNETCORE_CORS_URLS");
        private string myAudience = Environment.GetEnvironmentVariable("ASPNETCORE_CORS_URLS");

        /// <summary>
        /// Construtor para gerar token
        /// </summary>
        /// <param name="userId">Id do utilizador</param>
        /// <param name="role">Cago do utilizador</param>
        /// <param name="name">Nome do utilizador</param>
        public Token(string userId, string role, string name)
        {
            this.userId = userId;
            this.role = role;
            this.name = name;

            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.GivenName, name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = userId,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            this.token = tokenString;
        }

        /// <summary>
        /// Valida o token
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(this.token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = userId,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Devolver o valor da claim pesquisada
        /// </summary>
        /// <param name="claimType">claim</param>
        /// <returns></returns>
        public string GetClaim(string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(this.token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }

        /// <summary>
        /// Devolve o token
        /// </summary>
        /// <returns></returns>
        public string getToken()
        {
            return this.token;
        }
    }
}
