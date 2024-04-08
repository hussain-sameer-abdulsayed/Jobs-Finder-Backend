using MB_Project.AuthJwt;
using System.Security.Claims;

namespace MB_Project.IRepos
{
    public interface IJWTManagerRepo
    {
        Task<Token> GenerateToken(string userName);
        Task<Token> GenerateRefreshToken(string userName);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> isValidToken(string token);

    }
}
