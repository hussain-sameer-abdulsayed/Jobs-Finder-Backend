using AutoMapper;
using MB_Project.AuthJwt;
using MB_Project.Data;
using MB_Project.IRepos;
using MB_Project.Models;
using MB_Project.Models.DTOS;
using MB_Project.Models.DTOS.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Common;
using System.Text.Encodings.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MB_Project.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IJWTManagerRepo _jwtManagerRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPostRepo _postRepo;
        private readonly IEmailService _emailService;
        public UsersController(

                              IMapper mapper,
                              IUserRepo userRepo,
                              IWebHostEnvironment webHostEnvironment,
                              ITransactionRepo transactionRepo,
                              IJWTManagerRepo jwtManagerRepo,
                              UserManager<IdentityUser> userManager,
                              IPostRepo postRepo,
                              IEmailService emailService)
        {
            _mapper = mapper;
            _userRepo = userRepo;
            _webHostEnvironment = webHostEnvironment;
            _transactionRepo = transactionRepo;
            _jwtManagerRepo = jwtManagerRepo;
            _userManager = userManager;
            _postRepo = postRepo;
            _emailService = emailService;
        }





        //[Authorize(Roles = "ADMIN")]
        // GET: api/<UserController>
        [HttpGet()]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var obj =await _userRepo.GetUsers();
                if (obj.Count()==0)
                {
                    return NotFound("not found");
                }
                return Ok(obj);
            }
            catch
            {
                return BadRequest();
            }
        }


        [AllowAnonymous]
        // GET api/<UserController>/5
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUserById(string UserId)
        {
            try
            {
                var obj = await _userRepo.GetUserById(UserId);
                if (obj == null)
                {
                    return NotFound("this user does not exist");
                }
                return Ok(obj);
            }
            catch
            {
                return BadRequest();
            }
        }
        

        [Authorize(Roles ="ADMIN")]
        [HttpGet("UserRoles/{UserId}")]
        public async Task<IActionResult> GetUserRoles(string UserId)
        {
            try
            {
                //var role = _roleManager.Roles.FirstOrDefaultAsync(c=>c.Name=="ADMIN")?.Id;
                //var roleId = _identityUserRole.UserId.ToList();
                var Rolles = await _userRepo.GetUserRoles(UserId);
                if (Rolles.Count()==0)
                {
                    return NotFound();
                }
                return Ok(Rolles);
            }
            catch
            {
                return BadRequest();
            }
        }


        [AllowAnonymous]
        [HttpGet("verify/{verificationToken}")]
        public async Task<IActionResult> VerifiyEmail(string verificationToken)
        {
            try
            {
                //var chk = await _userRepo.VerifiyEmail(request);
                var user = await _userRepo.VerifiyEmail(verificationToken);
                if (user is null)
                {
                    return BadRequest("Invalid email or token.");
                }
                return Ok("Thank you for validation");
            }
            catch
            {
                return BadRequest();
            }
        }


        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromForm]CreateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                var result = await _userRepo.Register(userDto);
                if (!result.IsAuthenticated)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest(result.Message);
                }
                _transactionRepo.CommitTransaction();
                return Ok("Registration successful. Please check your email to verify your account.");
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _userRepo.Login(userLogin);
                if (result.IsAuthenticated is false)
                {
                    return BadRequest(result.Message);
                }
                //if (!await _userRepo.IsEmailConfirmed(userLogin.email))
                //{
                //    return BadRequest("Please check your email for the verification action to login");
                //}
                var user = await _userManager.FindByEmailAsync(userLogin.email);
                var token =await _jwtManagerRepo.GenerateToken(user.UserName);
                if(token == null)
                {
                    return Unauthorized("Invalid attempt...");
                }
                UserRefreshTokens obj = new UserRefreshTokens
                {
                    UserName = user.UserName,
                    RefreshToken = token.RefreshToken,
                    Email = userLogin.email
                };
                await _userRepo.AddUserRefreshTokens(obj);
                //return Ok(token);
                var TestObj = new TestModel
                {
                    Id = 1,
                    token = token.AccessToken,
                    isFreelancer = true,
                    userId = user.Id
                };
                return Ok(TestObj);
            }
            catch
            {
                return BadRequest();
            }
         }


        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(MyToken token)
        {
            var principal = _jwtManagerRepo.GetPrincipalFromExpiredToken(token.AccessToken);
            var username = principal.Identity?.Name;

            var savedRefreshToken = await _userRepo.GetSavedRefreshTokens(username, token.RefreshToken);

            if (savedRefreshToken.RefreshToken != token.RefreshToken)
            {
                return Unauthorized("Invalid attempt!");
            }

            var newJwtToken =await _jwtManagerRepo.GenerateRefreshToken(username);

            if (newJwtToken == null)
            {
                return Unauthorized("Invalid attempt!");
            }

            UserRefreshTokens obj = new UserRefreshTokens
            {
                RefreshToken = newJwtToken.RefreshToken,
                UserName = username
            };

            _userRepo.DeleteUserRefreshTokens(username, token.RefreshToken);
            await _userRepo.AddUserRefreshTokens(obj);

            return Ok(newJwtToken);
        }


        //[Authorize(Roles = "ADMIN")]
        [HttpPost("SetUserToSeller/{userId}")]
        public async Task<IActionResult> SetUserToSeller(string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                if(!await _userRepo.SellerRole(userId))
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Role Was not Added");
                }
                _transactionRepo.CommitTransaction();
                return Ok();

            }
            catch 
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest(); 
            }
        }


        //[Authorize(Roles = "ADMIN")]
        [HttpGet("sellers")]
        public async Task<IActionResult> GetSellers()
        {
            try
            {
                var sellers = await _userRepo.GetSellers();
                if (sellers.Count() == 0)
                {
                    return NotFound();
                }
                return Ok(sellers);
            }
            catch
            {
                return BadRequest();
            }
        }
        
        
        //[Authorize(Roles = "ADMIN")]
        [HttpDelete("removeseller/{UserId}")]
        public async Task<IActionResult> DeleteSeller(string UserId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _transactionRepo.BeginTransaction();
                var result = await _userRepo.RemoveRoleSeller(UserId);
                if (result == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Role Was Not Removed!");
                }
                _transactionRepo.CommitTransaction();
                return Ok();
            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        //[Authorize]
        // PUT api/<UserController>/5
        [HttpPut("{UserId}")]
        public async Task<IActionResult> UpdateUser(string UserId,[FromForm]UpdateUserDto userDto)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    _transactionRepo.BeginTransaction();
                    var result = await _userRepo.UpdateUser(UserId, userDto);
                    if (result == false)
                    {
                        _transactionRepo.RollBackTransaction();
                        return NotFound();
                    }
                    _transactionRepo.CommitTransaction();
                    return Ok();
                }
                catch
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest();
                }
            }

        // this api for verifiy his email then the second(under) api for reset
        //[Authorize]
        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPasswordConfirmation([FromBody]ForgotPasswordModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                /*
                _transactionRepo.BeginTransaction();

                var chk = await _userRepo.ForgotPassword(request);
                if (chk == false)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("Password was not updated");
                }
                _transactionRepo.CommitTransaction();
                return Ok("Password updated");
                */
                _transactionRepo.BeginTransaction();
                var succeeded = await _userRepo.ForgotPassword(request, Request.Scheme, Request.Host.Value);
                if (!succeeded)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest(new { message = "User not found" });
                }
                _transactionRepo.CommitTransaction();
                return Ok(new { message = "Password reset link has been sent to your email." });
            }
            catch 
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest(); 
            }
        }

        // when user verifiy his eamil will come to this api
        [HttpPost("reset-password")]
        public async Task<IActionResult> ForgotPassword([FromBody]ResetPasswordModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                /*
                _transactionRepo.BeginTransaction();
                User user = await _userRepo.ResetPassword(request);
                if (user is null) 
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest();
                }
                _transactionRepo.CommitTransaction();
                return Ok(user);
                */

                _transactionRepo.BeginTransaction();
                var (succeeded, errors) = await _userRepo.ResetPassword(request);
                if (!succeeded)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest(new { errors });
                }
                _transactionRepo.CommitTransaction();
                return Ok(new { message = "Password reset successful" });

            }
            catch
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest();
            }
        }


        //[Authorize(Roles = "ADMIN")]
        // DELETE api/<UserController>/5
        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _transactionRepo.BeginTransaction();
                var imageDeleted = await _userRepo.DeleteUserImage(UserId);
                if (!imageDeleted)
                {
                    return BadRequest("image was not Deleted");
                }
                var deleteUserResult = await _userRepo.DeleteUser(UserId);
                if (!deleteUserResult)
                {
                    _transactionRepo.RollBackTransaction();
                    return BadRequest("User was not Deleted");
                }
                _transactionRepo.CommitTransaction();
                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                _transactionRepo.RollBackTransaction();
                return BadRequest($"User was not Deleted:{ex.Message}");
            }
        }



        /*
        [Authorize]
        [HttpPut("UpdateUserImage")]
        public async Task<IActionResult> UpdateUserImage([FromForm]string UserId, IFormFile File)
        {

            try
            {
                var user = await _userRepo.GetUserById(UserId);
                if (user == null)
                {
                    return BadRequest("user not found");
                }
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/user");
                string oldUniqueFileName = await _userRepo.GetNniqueFileName(UserId);
                var chk1 = await _userRepo.DeleteUserImage(UserId, oldUniqueFileName, Directory);
                if (!chk1)
                {
                    return BadRequest("image was not Updated");
                }
                var newUniqueFileName = await SaveUploadedFile(File);
                if(newUniqueFileName == "size")
                {
                    return BadRequest("The size of the image must be less than 5MB");
                }
                var chk2 = await _userRepo.CreateImage(UserId, newUniqueFileName);
                if (!chk2)
                {
                    return BadRequest("image was not Updated");
                }
                return Ok("image Updated");
            }

            catch { return BadRequest("image was not Updated"); }
        }
        */
        /*
        [Authorize]
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                var admins = await _userRepo.GetAdmins();
                if (admins.Count()==0)
                {
                    return NotFound();
                }
                return Ok(admins);
            }
            catch
            {
                return BadRequest();
            }
        }
        */
        /*
        [Authorize]
        [HttpDelete("removeadmin/{UserId}")]
        public async Task<IActionResult> DeleteAdmin(string UserId)
        {
            try
            {
                var result = await _userRepo.RemoveRoleAdmin(UserId);
                if (result == false)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        */
        /*
        [HttpGet("GetUserImage")]
        public async Task<IActionResult> GetUserImage(string UserId)
        {
            try
            {
                var UserUniqueFileName = await _userRepo.GetNniqueFileName(UserId);
                if(UserUniqueFileName == null)
                {
                    return NotFound("not found");
                }
                var imageUrl = await _userRepo.GetImage(UserUniqueFileName);
                return Ok(imageUrl);
            }
            catch 
            { 
                return BadRequest(); 
            }
        }
        */
        /*
        // POST api/<UserController>
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserDto userDto)
        {
            try
            {
                string uniqueFileName = await SaveUploadedFile(userDto.ProfilePicture);
                var result = await _userRepo.CreateClient(userDto, uniqueFileName);
                if (result)
                {
                    return Ok();
                    
                }
                await _userRepo.Login(userDto.UserName, userDto.Password);
                return NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }
        */
        /*
        [HttpPost("CreateUserImage")]
        public async Task<IActionResult> CreateUserImage([FromForm]string UserId, IFormFile File)
        {
            try
            {
                var user = await _userRepo.GetUserById(UserId);
                if (user == null)
                {
                    return BadRequest("user not found");
                }
                var uniqueFileName = await SaveUploadedFile(File);
                var chk = await _userRepo.CreateImage(UserId, uniqueFileName);
                if (!chk)
                {
                    return BadRequest("image not added");
                }
                return Ok("image added");
            }
            catch { return BadRequest("image added"); }
        }
        */
        /*
        [Authorize]
        [HttpDelete("DeleteUserImage")]
        public async Task<IActionResult> DeleteUserImage(string UserId)
        {
            try
            {
                var user = await _userRepo.GetUserById(UserId);
                if (user == null)
                {
                    return BadRequest("user not found");
                }
                string Directory = Path.Combine(_webHostEnvironment.WebRootPath, "images/user");
                string uniqueFileName = await _userRepo.GetNniqueFileName(UserId);
                var chk = await _userRepo.DeleteUserImage(UserId,uniqueFileName,Directory);
                if (!chk)
                {
                    return BadRequest("image was not deleted");
                }
                return Ok("image deleted");
            }
            catch
            {
                return BadRequest("");
            }
        }
        */
        /*
        [HttpPost("RoleSeller")]
        public async Task<IActionResult> RoleSeller(string UserId)
        {
            try
            {
                var result = await _userRepo.RoleSeller(UserId);
                if (result == false)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        */
        /*
        //[Authorize(Roles ="ADMIN")]
        [HttpPost("RoleAdmin")]
        public async Task<IActionResult> RoleAdmin(string UserId)
        {
            try
            {
                var result = await _userRepo.RoleAdmin(UserId);
                if(result == false)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        */
        /*
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var chk = await _userRepo.Logout();
                if(chk == false)
                {
                    return NotFound("logout failed");
                }
                return Ok();
            }
            catch 
            { 
                return BadRequest(); 
            }
        }
        */
        /*
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string userName, [FromBody] string password)
        {
            try
            {
                var chk = await _userRepo.Login(userName, password);
                if(chk == false)
                {
                    return NotFound("login failed");
                }
                return Ok();
            }
            catch 
            { 
                return BadRequest(); 
            }
        }
        */
        /*
        [Authorize]
        [HttpGet("GetRolles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _userRepo.GetRolles();
                if (roles.Count()==0)
                {
                    return NotFound();
                }
                return Ok(roles);
            }
            catch
            {
                return BadRequest();
            }
        }
        */
    }
} 
