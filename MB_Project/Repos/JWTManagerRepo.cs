using MB_Project.AuthJwt;
using MB_Project.Data;
using MB_Project.IRepos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MB_Project.Repos
{
    public class JWTManagerRepo : IJWTManagerRepo
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly MB_ProjectContext _context;

        public JWTManagerRepo(IConfiguration configuration, UserManager<IdentityUser> userManager, MB_ProjectContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public async Task<Token> GenerateRefreshToken(string userName)
        {
            return await GenerateJWTTokens(userName);
        }
        public async Task<Token> GenerateToken(string userName)
        {
            return await GenerateJWTTokens(userName);
        }
        public async Task<Token> GenerateJWTTokens(string userName)
        {
            try
            {
                var user =await _userManager.FindByNameAsync(userName);
                var expireTimeInMinutes = 1;//change it to 1
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                  {
                    new Claim(ClaimTypes.Name, userName,user.Email)
                  }),
                    Expires = DateTime.Now.AddMinutes(expireTimeInMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = GenerateRefreshToken();
                return new Token { AccessToken = tokenHandler.WriteToken(token), RefreshToken = refreshToken };
            }
            catch
            {
                return null;
            }
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public async Task<bool> isValidToken(string token)
        {
            try
            {
                var chk = await _context.UserRefreshTokens.Where(t => t.RefreshToken == token).FirstOrDefaultAsync();
                if (chk is null) 
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }


}

