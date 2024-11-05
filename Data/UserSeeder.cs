//using Microsoft.AspNetCore.Identity;

//namespace RoleBasedAuthAPI.Data
//{
//    public static class UserSeeder
//    {
//        public static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
//        {
//            var adminEmail = "admin@example.com"; // Change to your admin email
//            var adminPassword = "Admin@123"; // Change to a strong password

//            var existingUser = await userManager.FindByEmailAsync(adminEmail);
//            if (existingUser == null)
//            {
//                var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
//                await userManager.CreateAsync(adminUser, adminPassword);
//                await userManager.AddToRoleAsync(adminUser, "Admin");
//            }
//        }
//    }
//}
