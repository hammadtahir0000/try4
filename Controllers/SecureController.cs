using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace try4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecureController : ControllerBase
    {
        // Endpoint accessible only by Admins
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            // Instruct the frontend to redirect to the Admin Dashboard
            return Ok(new { Message = "Welcome, Admin!", RedirectTo = "/admin/dashboard" });
        }

        // Endpoint accessible only by Users
        [Authorize(Roles = "User")]
        [HttpGet("user-only")]
        public IActionResult UserOnly()
        {
            // Instruct the frontend to redirect to the User Profile
            return Ok(new { Message = "Welcome, User!", RedirectTo = "/user/profile" });
        }

        // Endpoint accessible by both Admins and Users
        [Authorize(Roles = "Admin,User")]
        [HttpGet("admin-or-user")]
        public IActionResult AdminOrUser()
        {
            // Check the role of the current user and provide the appropriate redirect
            if (User.IsInRole("Admin"))
            {
                return Ok(new { Message = "Welcome back, Admin!", RedirectTo = "/admin/dashboard" });
            }
            else if (User.IsInRole("User"))
            {
                return Ok(new { Message = "Welcome back, User!", RedirectTo = "/user/profile" });
            }

            return Unauthorized();
        }

        // Public endpoint accessible by anyone (no authorization required)
        [AllowAnonymous]
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            // Instruct the frontend to redirect to the Home page
            return Ok(new { Message = "This is a public endpoint.", RedirectTo = "/home" });
        }
    }
}
