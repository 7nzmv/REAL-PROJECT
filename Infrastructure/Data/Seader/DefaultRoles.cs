using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Seader;

public class DefaultRoles
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new List<string>()
        {
            Roles.Admin,
            Roles.User
        };

        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (roleExist)
            {
                continue;
            }

            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
