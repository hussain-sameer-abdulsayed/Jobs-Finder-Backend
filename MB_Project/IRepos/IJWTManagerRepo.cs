using MB_Project.AuthJwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static MB_Project.Repos.JWTManagerRepo;

namespace MB_Project.IRepos
{
    public interface IJWTManagerRepo
    {
        Task<MyToken> GenerateToken(string userName);
        Task<MyToken> GenerateRefreshToken(string userName);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> isValidToken(string token);
        JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt);
        string GetUserId(string mytoken);
    }
}
