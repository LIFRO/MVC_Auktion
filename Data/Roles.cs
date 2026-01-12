using Microsoft.AspNetCore.Identity;

public static class Roles
{
    public static async Task RolesAsync(RoleManager<IdentityRole> roleManager)
    {
            if(!await roleManager.RoleExistsAsync("IsAResponsible"))
            {
                await roleManager.CreateAsync(new IdentityRole("IsAResponsible"));
            }
            if(!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
    }
}