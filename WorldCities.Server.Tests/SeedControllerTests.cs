using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldCities.Server.Controllers;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;

namespace WorldCities.Server.Tests;

public class SeedControllerTests
{
    [Fact]
    public async Task CreateDefaultUsers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "WorldCities")
            .Options;

        var mockEnv = Mock.Of<IWebHostEnvironment>();

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.SetupGet(x => x[It.Is<string>(s => 
        s == "DefaultPasswords:RegisteredUser")])
            .Returns("M0ckP$$word");
        mockConfiguration.SetupGet(x => x[It.Is<string>(s =>
        s == "DefaultPasswords:Administrator")])
            .Returns("M0ckP$$word");

        using var context = new ApplicationDbContext(options);

        var roleManager = IdentityHelper.GetRoleManager(
            new RoleStore<IdentityRole>(context));

        var userManager = IdentityHelper.GetUserManager(
            new UserStore<ApplicationUser>(context));

        var controller = new SeedController(
            context,
            roleManager,
            userManager,
            mockEnv,
            mockConfiguration.Object);

        ApplicationUser user_admin = null!;
        ApplicationUser user_user = null!;
        ApplicationUser user_notExisting = null!;

        await controller.CreateDefaultUsers();

        user_admin = await userManager.FindByEmailAsync("admin@email.com");
        user_user = await userManager.FindByEmailAsync("user@email.com");
        user_notExisting = await userManager.FindByEmailAsync("notExisting@email.com");

        Assert.NotNull(user_admin);
        Assert.NotNull(user_user);
        Assert.Null(user_notExisting);  

    }
}
