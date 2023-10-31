using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WebShopMvcUI.Constants;

namespace WebShopMvcUI.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
            };

            var userIdDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userIdDb is null)
            {
                await userMgr.CreateAsync(admin, "Admin2860@");
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
        public static async Task<bool> DeleteUser(IServiceProvider service, string userName)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var user = await userMgr.FindByNameAsync(userName);

            if (user != null)
            {
                var result = await userMgr.DeleteAsync(user);
                return result.Succeeded;
            }

            return false;
        }

        public static async Task<bool> DeleteUserByEmail(IServiceProvider service, string email)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var user = await userMgr.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await userMgr.DeleteAsync(user);
                return result.Succeeded;
            }

            return false;
        }
    }
}

