using Azure.Core;
using MB_Project.AuthJwt;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS;
using MB_Project.Models.DTOS.UserDto;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;

namespace MB_Project.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly MB_ProjectContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Jwt _jwt;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;
        public UserRepo(
                              MB_ProjectContext context,
                              UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              SignInManager<IdentityUser> signInManager,
                              IOptions<Jwt> jwt,
                              IConfiguration configuration,
                              IWebHostEnvironment webHostEnvironment
,
                              IEmailService emailService
                              /*IdentityUserRole<string> identityUserRole*/
                              )
        {
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwt = jwt.Value;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
            //_identityUserRole = identityUserRole;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            try
            {
                const int ExpiresDuration = 30;
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                    roleClaims.Add(new Claim("roles", role));

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
                .Union(userClaims)
                .Union(roleClaims);
                //if (string.IsNullOrEmpty(_jwt.Key))
                //{
                //    throw new ArgumentNullException(nameof(_jwt.Key), "JWT key cannot be null.");
                //}


                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Jwt:Issuer").Value!,
                    audience: _configuration.GetSection("Jwt:Audience").Value!,
                    claims: claims,
                    expires: DateTime.Now.AddDays(ExpiresDuration), // tset in minutes
                    signingCredentials: signingCredentials);

                return jwtSecurityToken;
            }
            catch
            {
                return null;
            }

        }
        // I create this because i can't convert from user to identity user
        private async Task<JwtSecurityToken> CreateJwtTokenIdentity(IdentityUser user)
        {
            try
            {
                const int ExpiresDuration = 30;
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                    roleClaims.Add(new Claim("roles", role));

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
                .Union(userClaims)
                .Union(roleClaims);
                //if (string.IsNullOrEmpty(_jwt.Key))
                //{
                //    throw new ArgumentNullException(nameof(_jwt.Key), "JWT key cannot be null.");
                //}


                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Jwt:Issuer").Value!,
                    audience: _configuration.GetSection("Jwt:Audience").Value!,
                    claims: claims,
                    expires: DateTime.Now.AddDays(ExpiresDuration), // tset in minutes
                    signingCredentials: signingCredentials);

                return jwtSecurityToken;
            }
            catch
            {
                return null;
            }
        }
        private async Task<string> SaveUploadedFile(IFormFile file)
        {
            // Check if a file was uploaded
            if (file != null && file.Length > 0)
            {
                const int maxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB
                if (file.Length > maxFileSizeInBytes)
                {
                    return ("size");
                }
                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString()+".png";

                // Construct the file path
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/user");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the file system
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return the unique file name
                return uniqueFileName;
            }
            else
            {
                // Return null if no file was uploaded
                return null;
            }
        }
        



        public async Task<string> GetImage(string uniqueFileName)
        {
            try
            {
                const string _baseImageUrl = "https://localhost:7181/images/user/";
                if (string.IsNullOrEmpty(uniqueFileName))
                {
                    return null;
                }
                string imageUrl = $"{_baseImageUrl}{uniqueFileName}";
                return imageUrl;
            }
            catch
            {
                return null;
            }
        }
        public async Task<string> GetNniqueFileName(string UserId)
        {
            try
            {
                var UserUniqueFileName = await _context.Users.
                    Where(x => x.Id == UserId).
                    Select(x => x.ProfileImage).
                    FirstOrDefaultAsync();
                ;
                if (UserUniqueFileName == null)
                {
                    return null;
                }
                return UserUniqueFileName;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<IdentityUser>> GetAdmins()
        {
            try
            {
                var user = await _userManager.GetUsersInRoleAsync("ADMIN");
                if (user.Count() == 0)
                {
                    return Enumerable.Empty<IdentityUser>();
                }
                return (IEnumerable<IdentityUser>)user;
            }
            catch
            {
                return Enumerable.Empty<IdentityUser>();
            }
        }
        public async Task<IEnumerable<object>> GetSellers()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync("SELLER");
                if (users.Count() == 0)
                {
                    return Enumerable.Empty<IdentityUser>();
                }
                var sellers = users.Select(user => new
                {
                    userId = user.Id,
                    email = user.Email,
                    userName = user.UserName
                });
                return (IEnumerable<object>)sellers;
            }
            catch
            {
                return Enumerable.Empty<object>();
            }
        }
        public async Task<ViewUserDto> GetUserById(string UserId)
        {
            try
            {
                var uniqueFileName = await GetNniqueFileName(UserId);
                var image = await GetImage(uniqueFileName);

                var obj = await(from e in _context.Users
                                join p in _userManager.Users
                                on e.Id equals p.Id
                                where p.Id == UserId
                                select new ViewUserDto
                                {
                                    Id = p.Id,
                                    UserName = p.UserName,
                                    Name = e.Name,
                                    Email = p.Email,
                                    Password = p.PasswordHash,
                                    EmailConfirmed = p.EmailConfirmed,
                                    ProfileImage = image,
                                    Bio = e.Bio,
                                    Orders = e.Orders,
                                    Reviews = e.Reviews,
                                    Posts = e.Posts
                                }).FirstOrDefaultAsync();

                return obj;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<string>> GetUserRoles(string UserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserId);
                if (user == null)
                {
                    return Enumerable.Empty<string>();
                }

                var roleNames = await _userManager.GetRolesAsync(user);
                return roleNames;
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
        public async Task<IEnumerable<ViewUserDto>> GetUsers()
        {
            try
            {
                var obj = await (from e in _context.Users
                                 join p in _userManager.Users
                                 on e.Id equals p.Id
                                 select new ViewUserDto
                                 {
                                     Id = p.Id,
                                     UserName = p.UserName,
                                     ProfileImage = e.ProfileImage
                                 }).ToListAsync();
                foreach (var user in obj)
                {
                    var uniqueFileName = await GetNniqueFileName(user.Id);
                    user.ProfileImage = await GetImage(uniqueFileName);
                }
                if (obj == null)
                {
                    return Enumerable.Empty<ViewUserDto>();
                }
                return (IEnumerable<ViewUserDto>)obj;
            }
            catch
            {
                return Enumerable.Empty<ViewUserDto>();
            }
        }
        public async Task<bool> CreateImage(string UserId, string uniqueFileName)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                user.ProfileImage = uniqueFileName;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<UserRefreshTokens> Register(CreateUserDto userDto)
        {
            if (await _userManager.FindByEmailAsync(userDto.Email) is not null)
            {
                return new UserRefreshTokens { Message = "Email is already Register!", IsAuthenticated = false };
                // isAuthenticated in Auth Model is false by default i handle it to understand code
            }
            //if (await _userManager.FindByNameAsync(userDto.UserName) is not null)
            //{
            //    return new UserRefreshTokens { Message = "UserName is already Register!", IsAuthenticated = false };
            //    // isAuthenticated in Auth Model is false by default i handle it to understand code
            //}

            //var uniqueFileName = await SaveUploadedFile(userDto.profileImage);
            var newUser = new User
            {
                UserName = userDto.Email,
                Name = userDto.Name,
                Email = userDto.Email,
                //ProfileImage = uniqueFileName,
                Bio = userDto.Bio,
                // this is for email validation
                ValidationEmailToken = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, userDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},"; 
                }
                //DeleteFile(uniqueFileName);// delete image from system
                return new UserRefreshTokens { Message = errors, IsAuthenticated = false };
            }
            var uniqueFileName = await SaveUploadedFile(userDto.profileImage);
            _context.Attach(newUser);
            newUser.ProfileImage = uniqueFileName;
            await _context.SaveChangesAsync();
            EmailDto email = new EmailDto()
            {
                To = newUser.Email,
                Subject = "Email Verification",
                Body = $"Please click the following link to verify your email: <a href='https://localhost:7181/api/users/verify/{Uri.EscapeDataString(newUser.ValidationEmailToken)}'>Verify Email</a>"
            };
            //_emailService.SendValidationEmail(email);
            if (userDto.isFreelancer == true)
            {
                await _userManager.AddToRoleAsync(newUser, "SELLER");
            }
            await _userManager.AddToRoleAsync(newUser, "CLIENT");
            var jwtSecurityToken = await CreateJwtToken(newUser);
            var Uroles = new List<string>(await _userManager.GetRolesAsync(newUser));
            return new UserRefreshTokens
            {
                IsAuthenticated = true,
            };
        }
        
        public async Task<bool> RoleAdmin(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return false;
                }
                await _userManager.AddToRoleAsync(user, "ADMIN");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateUser(string UserId, UpdateUserDto userDto)
        {
            try
            {
                var obj = await _context.Users.FindAsync(UserId);
                if (obj == null)
                {
                    return false;
                }
                obj.Name = userDto.Name;
                obj.Bio = userDto.Bio;
                if (userDto.profileImage != null)
                {
                    var uniqueFileName = await UpdateUserImage(UserId, userDto.profileImage);
                    if (uniqueFileName == "error")
                    {
                        return false;
                    }
                    obj.ProfileImage = uniqueFileName;
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> ForgotPassword(ForgotPasswordModel request, string requestScheme, string requestHost)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return false;
                };
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"{requestScheme}://{requestHost}/api/users/reset-password?token={token}&email={user.Email}";
                EmailDto email = new EmailDto()
                {
                    To = user.Email,
                    Subject = "Reset Password",
                    Body = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>"
                };
                _emailService.SendValidationEmail(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPassword(ResetPasswordModel request)
        {
            /*
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                if (user == null)
                    return (false, new[] { "User not found" });
                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: request.Password!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                _context.Attach(user);
                user.PasswordHash = hashed;
                await _context.SaveChangesAsync();
                return user;
                */

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (false, new[] { "User not found" });

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
                return (true, Enumerable.Empty<string>());

            return (false, result.Errors.Select(e => e.Description));

        } 
        public async Task<bool> RemoveRoleAdmin(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return false;
                }
                await _userManager.RemoveFromRoleAsync(user, "ADMIN");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> RemoveRoleSeller(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return false;
                }
                await _userManager.RemoveFromRoleAsync(user, "SELLER");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteUser(string UserId)
        {
            try
            {
                var obj = await _context.Users.FindAsync(UserId);
                if (obj == null)
                {
                    return false;
                }
                _context.Users.Remove(obj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DeleteUserImage(string UserId)
        {
            try
            {
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/user");
                string uniqueFileName = await GetNniqueFileName(UserId);
                string filePath = Path.Combine(Directory, uniqueFileName);
                if (!File.Exists(filePath))
                {
                    return false;
                }
                File.Delete(filePath);
                var user = await _context.Users.FindAsync(UserId);
                user.ProfileImage = null;
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }
        //this RollBack for image to delete it 
        public void DeleteFile(string uniqueFileName)
        {
            string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/user");
            string filePath = Path.Combine(Directory, uniqueFileName);

            // Check if the file exists before attempting to delete it
            if (File.Exists(filePath))
            {
                // Delete the file
                File.Delete(filePath);
            }
        }
        public async Task<string> UpdateUserImage(string UserId, IFormFile File)
        {

            try
            {
                var user = await GetUserById(UserId);
                if (user == null)
                {
                    return "error";
                }
                var chk1 = await DeleteUserImage(UserId);
                if (!chk1)
                {
                    return "error";
                }
                var newUniqueFileName = await SaveUploadedFile(File);
                if (newUniqueFileName == "size")
                {
                    return "error";
                }
                var chk2 = await CreateImage(UserId, newUniqueFileName);
                if (!chk2)
                {
                    return "error";
                }
                return newUniqueFileName;
            }

            catch { return "error"; }
        }
        public async Task<bool> SellerRole(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return false;
                }
                await _userManager.AddToRoleAsync(user, "SELLER");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserLogin> Login(UserLogin users)
        {
            var user = await _userManager.FindByEmailAsync(users.email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, users.password))
            {
                return new UserLogin { IsAuthenticated=false,Message="Email or Password is incorrect" };
            }
            if (!user.EmailConfirmed)
            {
                return new UserLogin { IsAuthenticated = false, Message = "You need to verifiy your email" };
            }
            return new UserLogin{IsAuthenticated=true };
        }
        
            
        public async Task<UserRefreshTokens> AddUserRefreshTokens(UserRefreshTokens user)
        {
            try
            {
                await _context.UserRefreshTokens.AddAsync(user);
                _context.SaveChanges();
                return user;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserRefreshTokens> GetSavedRefreshTokens(string username, string refreshToken)
        {
            var token = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive == true);
            return token;
        }

        public void DeleteUserRefreshTokens(string username, string refreshToken)
        {
            var item = _context.UserRefreshTokens.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _context.UserRefreshTokens.Remove(item);
            }
        }

        public async Task<User> VerifiyEmail(/*VerifiyEmail req*/ string verificationToken)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email && x.ValidationEmailToken == req.Token);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.ValidationEmailToken == verificationToken);
            if (user is null)
            {
                return null;
            }
            _context.Attach(user);
            user.EmailConfirmed = true;
            user.ValidationEmailToken = null;
            await _context.SaveChangesAsync();
            return user;
        }




        //public async Task<bool> IsEmailConfirmed(string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (!await _userManager.IsEmailConfirmedAsync(user))
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        /*
        public async Task<UserRefreshTokens> Login(UserLogin loginToken)
        {
            var user = await _userManager.FindByEmailAsync(loginToken.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginToken.Password))
            {
                return new UserRefreshTokens { Message= "Email or Password is inccorrect" };
            }
            var jwtSecurityToken = await CreateJwtTokenIdentity(user);
            var Uroles = new List<string>(await _userManager.GetRolesAsync(user));
            return new UserRefreshTokens
            {
                Email = user.Email,
                IsAuthenticated = true,
                ExpiresOn = jwtSecurityToken.ValidTo,
                UserName = user.UserName,
                Roles = Uroles,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }
        */
        /*
        public async Task<bool> CreateClient(CreateUserDto userDto,string uniqueFileName)
        {
            try
            {
                var newUser = new User
                {
                    UserName = userDto.UserName,
                    Name = userDto.Name,
                    Email = userDto.Email,
                    ProfileImage = uniqueFileName,
                    Bio = userDto.Bio,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(newUser, userDto.Password);
                if (result.Succeeded)
                {
                    if(userDto.IsSeller ==  true)
                    {
                        await _userManager.AddToRoleAsync(newUser, "SELLER");
                        await _userManager.AddToRoleAsync(newUser, "CLIENT");
                        return true;
                    }
                    await _userManager.AddToRoleAsync(newUser, "CLIENT");
                    return true;

                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        */
        /*
        public async Task<bool> Login(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return false;
                }
                var login = await _signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);
                return true;
            }
            catch
            {
                return false;
            }
        }
        */
        /*
        public async Task<bool> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        */
        /*
        public async Task<bool> RoleSeller(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return false;
                }
                await _userManager.AddToRoleAsync(user, "SELLER");
                return true;
            }
            catch
            {
                return false;
            }
        }
        */
        /*
        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            try
            {
                var roles = new List<IdentityRole>();
                roles = await _roleManager.Roles.ToListAsync();
                if (!roles.Any())
                {
                    return Enumerable.Empty<IdentityRole>();
                }
                return (IEnumerable<IdentityRole>)roles;
            }
            catch
            {
                return Enumerable.Empty<IdentityRole>();
            }
        }
        */
    }
}
