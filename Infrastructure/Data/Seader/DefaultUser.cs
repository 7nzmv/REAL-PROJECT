using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Seader;

public class DefaultUser
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = "motylek7171@gmail.com",
            Email = "motylek7171@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "919790300",
        };

        var existingUser = await userManager.FindByNameAsync(user.UserName);
        if (existingUser != null)
        {
            return;
        }

        var result = await userManager.CreateAsync(user, "Motylek1718!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    } 
}

