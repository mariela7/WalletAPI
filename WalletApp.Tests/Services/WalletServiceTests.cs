using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WalletApp.Domain.Entities;
using WalletApp.Infrastructure.Persistence;
using WalletApp.Infrastructure.Services;

namespace WalletApp.Tests.Services
{
    public class WalletServiceTests
    {
        private WalletDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new WalletDbContext(options);
        }


        [Fact]
        public async Task CreateAsync_ShouldAddWallet()
        {
            var context = GetDbContext();
            var service = new WalletService(context);

            var wallet = new Wallet
            {
                Name = "Test User",
                DocumentId = "999999999",
                Balance = 50
            };

            var result = await service.CreateAsync(wallet);
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Test User");

            var count = await context.Wallets.CountAsync();
            count.Should().Be(1);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnWallet_WhenExists()
        {
            var context = GetDbContext();
            var service = new WalletService(context);

            var wallet = new Wallet { Name = "Get Test", DocumentId = "777", Balance = 20 };
            context.Wallets.Add(wallet);
            await context.SaveChangesAsync();

            var result = await service.GetByIdAsync(wallet.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Get Test");
        }


        [Fact]
        public async Task UpdateAsync_ShouldUpdateWallet_WhenExists()
        {
            var context = GetDbContext();
            var service = new WalletService(context);

            var wallet = new Wallet { Name = "Old Name", DocumentId = "888", Balance = 10 };
            context.Wallets.Add(wallet);
            await context.SaveChangesAsync();

            wallet.Name = "Updated Name";
            var result = await service.UpdateAsync(wallet);

            result.Should().BeTrue();
            (await context.Wallets.FindAsync(wallet.Id))!.Name.Should().Be("Updated Name");
        }


        [Fact]
        public async Task DeleteAsync_ShouldRemoveWallet_WhenExists()
        {
            var context = GetDbContext();
            var service = new WalletService(context);

            var wallet = new Wallet { Name = "To Delete", DocumentId = "999", Balance = 0 };
            context.Wallets.Add(wallet);
            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(wallet.Id);

            result.Should().BeTrue();
            var exists = await context.Wallets.FindAsync(wallet.Id);
            exists.Should().BeNull();
        }
    }
}
