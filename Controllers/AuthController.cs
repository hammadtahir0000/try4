using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using try4.Models;
using try4.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return Conflict(new { Status = "Error", Message = "User already exists!" });

        IdentityUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        // Assign 'User' role by default
        if (!await _roleManager.RoleExistsAsync("User"))
            await _roleManager.CreateAsync(new IdentityRole("User"));
        await _userManager.AddToRoleAsync(user, "User");

        // Commenting out email confirmation logic for now
        /*
        // Generate email confirmation token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { userId = user.Id, token }, Request.Scheme);

        // Send email (you would replace this with your actual email sending logic)
        // Here we're simulating an email service
        Console.WriteLine($"Confirmation Link: {confirmationLink}");
        */

        // Automatically confirm the user's email
        await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user)); // Automatically confirm the email

        return Ok(new { Status = "Success", Message = "User registered successfully!" });
    }




    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found!");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return Ok("Email confirmed successfully!");

        return BadRequest("Email confirmation failed.");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return Unauthorized(new { Message = "Invalid username or password." });

        // Remove the email confirmation check
        // if (!await _userManager.IsEmailConfirmedAsync(user))
        //     return BadRequest(new { Message = "Email is not confirmed!" });

        if (await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = await _jwtTokenService.GenerateToken(user);

            // After successful login, you can include a redirection to the profile
            var userProfile = new
            {
                Username = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return Ok(new { Token = token, Profile = userProfile });
        }

        return Unauthorized();
    }


    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        // Find the user by username
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return NotFound(new { Status = "Error", Message = "User not found!" });

        // Attempt to change the password
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { Status = "Error", Message = $"Password change failed! {errors}" });
        }

        return Ok(new { Status = "Success", Message = "Password changed successfully!" });
    }


    [HttpGet("check-roles/{username}")]
    public async Task<IActionResult> CheckUserRoles(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound("User not found!");

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { Username = username, Roles = roles });
    }


    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RoleModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
            return NotFound("User not found!");

        if (!await _roleManager.RoleExistsAsync(model.Role))
            await _roleManager.CreateAsync(new IdentityRole(model.Role));

        await _userManager.AddToRoleAsync(user, model.Role);
        return Ok($"Role {model.Role} assigned to user {model.Username}");
    }
}
