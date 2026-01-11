using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Auktioner.Controllers;
using MVC_Auktioner.Data;
using MVC_Auktioner.Models;
using Xunit;

namespace MVC_Auktioner.Tests;

public class AuctionsControllerTests
{
    private static ApplicationDbContext BuildContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Create_AddsAuction_AndRedirectsToIndex()
    {
        // Arrange
        using var context = BuildContext(nameof(Create_AddsAuction_AndRedirectsToIndex));
        var category = new CategoryModel { CategoryName = "Cars" };
        context.Category.Add(category);
        await context.SaveChangesAsync();

        var controller = new AuctionsController(context);
        var auction = new AuctionModel
        {
            InventoryId = "abc123456",
            Decade = 1990,
            CategoryId = category.Id,
            Title = "Test title",
            Description = "Desc",
            Location = "Loc",
            Seller = "Seller",
            PurchaseCost = 10,
            StartingPrice = 20
        };

        // Act
        var result = await controller.Create(auction);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var saved = Assert.Single(context.Auctions);
        Assert.Equal("ABC123456", saved.InventoryId); // controller uppercases it
        Assert.Equal(category.Id, saved.CategoryId);
    }

    [Fact]
    public void SellPost_IsLimitedTo_IsAResponsible_Role()
    {
        // Arrange
        var method = typeof(AuctionsController).GetMethod(nameof(AuctionsController.Sell), new[] { typeof(SellAuctionModel) });
        Assert.NotNull(method);

        // Act
        var authorize = method!.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        Assert.NotNull(authorize);
        Assert.Equal("IsAResponsible", authorize!.Roles);
    }

    [Fact]
    public async Task Create_WithStartingPriceBelowPurchaseCost_ReturnsViewWithError()
    {
        // Arrange
        using var context = BuildContext(nameof(Create_WithStartingPriceBelowPurchaseCost_ReturnsViewWithError));
        var category = new CategoryModel { CategoryName = "Decor" };
        context.Category.Add(category);
        await context.SaveChangesAsync();

        var controller = new AuctionsController(context);
        var auction = new AuctionModel
        {
            InventoryId = "AAA000000",
            Decade = 1980,
            CategoryId = category.Id,
            Title = "Test",
            Description = "Desc",
            Location = "Loc",
            Seller = "Seller",
            PurchaseCost = 100,
            StartingPrice = 50 // below cost
        };

        // Act
        var result = await controller.Create(auction);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(auction, view.Model);
        Assert.True(controller.ModelState.ContainsKey(nameof(auction.StartingPrice)));
        Assert.Empty(context.Auctions);
    }

    [Fact]
    public async Task Sell_WithFinalPriceBelowStartingPrice_ReturnsViewWithError()
    {
        // Arrange
        using var context = BuildContext(nameof(Sell_WithFinalPriceBelowStartingPrice_ReturnsViewWithError));
        var category = new CategoryModel { CategoryName = "Household" };
        context.Category.Add(category);
        await context.SaveChangesAsync();
        var auction = new AuctionModel
        {
            InventoryId = "BBB111111",
            Decade = 1970,
            CategoryId = category.Id,
            Title = "Test",
            Location = "Loc",
            Seller = "Seller",
            PurchaseCost = 10,
            StartingPrice = 200
        };
        context.Auctions.Add(auction);
        await context.SaveChangesAsync();

        var controller = new AuctionsController(context);
        var model = new SellAuctionModel
        {
            Id = auction.Id,
            InventoryId = auction.InventoryId,
            StartingPrice = auction.StartingPrice,
            FinalPrice = 50 // below starting price
        };

        // Act
        var result = await controller.Sell(model);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey(nameof(model.FinalPrice)));
        var stored = await context.Auctions.AsNoTracking().FirstAsync();
        Assert.False(stored.Sold);
        Assert.Null(stored.FinalPrice);
    }
}
