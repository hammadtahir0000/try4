//namespace try4.Data
//{
//    public class RoleSeeder
//    {
//    }
//}
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Identity;

namespace RoleBasedAuthAPI.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
