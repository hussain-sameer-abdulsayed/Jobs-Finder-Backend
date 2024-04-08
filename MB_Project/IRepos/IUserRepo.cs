using MB_Project.Models;
using MB_Project.Models.DTOS.UserDto;
using Microsoft.AspNetCore.Identity;

namespace MB_Project.IRepos
{
    public interface IUserRepo
    {
        Task<IEnumerable<ViewUserDto>> GetUsers();
        Task<bool> CreateImage(string UserId,string uniqueFileName);
        Task<bool> DeleteUserImage(string UserId);
        void DeleteFile(string uniqueFileName);
        Task<IEnumerable<object>> GetSellers(); 
        Task<IEnumerable<string>> GetUserRoles(string UserId); 
        Task<ViewUserDto> GetUserById(string UserId);
        Task<string> GetNniqueFileName(string UserId);
        Task<string> GetImage(string uniqueFileName);
        Task<UserRefreshTokens> Register(CreateUserDto userDto);
        //Task<bool> IsEmailConfirmed(string email);
        Task<bool> RemoveRoleSeller(string UserId); 
        Task<bool> UpdateUser(string UserId,UpdateUserDto userDto); 
        Task<bool> UpdateUserPassword(string  UserId, string Password);
        Task<bool> DeleteUser(string UserId);
        Task<bool> SellerRole(string UserId);
        Task<bool> Login(UserLogin users);
        Task<UserRefreshTokens> AddUserRefreshTokens(UserRefreshTokens user);
        Task<UserRefreshTokens> GetSavedRefreshTokens(string username, string refreshtoken);
        void DeleteUserRefreshTokens(string username, string refreshToken);








        //Task<UserRefreshTokens> Login(UserLogin loginToken);
        //Task<IEnumerable<IdentityUser>> GetAdmins();
        //Task<bool> RemoveRoleAdmin(string UserId); 
        //Task<bool> CreateClient(CreateUserDto userDto, string uniqueFileName);    
        //Task<IEnumerable<IdentityRole>> GetRoles(); 
        //Task<bool> RoleAdmin(string UserId); 
        //Task<bool> RoleSeller(string UserId); 
        //Task<bool> Login(string username, string password);
        //Task<bool> Logout();
    }
}
