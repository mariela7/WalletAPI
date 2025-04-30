using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WalletApp.Domain.Entities;
using WalletApp.Infrastructure.Persistence;
using WalletApp.Infrastructure.Services;
using WalletApp.Application.Exceptions;


namespace WalletApp.Tests.Services
{
    public class MovementServiceTests
    {
        private WalletDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new WalletDbContext(options);
        }

        [Fact]
        public async Task TransferAsync_ShouldTransferAndCreateMovements()
        {
            // Arrange
            var context = GetDbContext();
            var service = new MovementService(context);

            var origin = new Wallet
            {
                Name = "Cta origen",
                DocumentId = "1111111111",
                Balance = 100
            };

            var destination = new Wallet
            {
                Name = "Cta Destino",
                DocumentId = "2222222222",
                Balance = 50
            };

            context.Wallets.AddRange(origin, destination);
            await context.SaveChangesAsync();

            // Act
            var result = await service.TransferAsync(origin.Id, destination.Id, 30);

            // Assert
            result.Should().BeTrue();

            var updatedOrigin = await context.Wallets.FindAsync(origin.Id);
            var updatedDestination = await context.Wallets.FindAsync(destination.Id);

            updatedOrigin.Balance.Should().Be(70);
            updatedDestination.Balance.Should().Be(80);

            var movements = await context.Movements.ToListAsync();
            movements.Should().HaveCount(2);

            movements.Should().ContainSingle(m => m.Type == "Débito" && m.WalletId == origin.Id && m.Amount == 30);
            movements.Should().ContainSingle(m => m.Type == "Crédito" && m.WalletId == destination.Id && m.Amount == 30);
        }


        [Fact]
        public async Task TransferAsync_ShouldFail_WhenInsufficientBalance()
        {
            var context = GetDbContext();
            var service = new MovementService(context);

            var origin = new Wallet { Name = "Cta Origen", DocumentId = "1", Balance = 10 };
            var dest = new Wallet { Name = "Cta Destino", DocumentId = "2", Balance = 0 };
            context.Wallets.AddRange(origin, dest);
            await context.SaveChangesAsync();

            Func<Task> act = async () => await service.TransferAsync(origin.Id, dest.Id, 50);

            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Saldo insuficiente en la billetera de origen.");
        }


        [Fact]
        public async Task TransferAsync_ShouldFail_WhenWalletNotFound()
        {
            var context = GetDbContext();
            var service = new MovementService(context);

            var dest = new Wallet { Name = "Dest", DocumentId = "3", Balance = 0 };
            context.Wallets.Add(dest);
            await context.SaveChangesAsync();

            Func<Task> act = async () => await service.TransferAsync(999, dest.Id, 10);
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("*origen no existe*");
        }


        [Fact]
        public async Task GetMovementsByWalletIdAsync_ShouldReturnMovements()
        {
            var context = GetDbContext();
            var service = new MovementService(context);

            var wallet = new Wallet { Name = "MovTest", DocumentId = "4", Balance = 100 };
            context.Wallets.Add(wallet);
            await context.SaveChangesAsync();

            context.Movements.Add(new Movement { WalletId = wallet.Id, Amount = 10, Type = "Débito", CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var result = await service.GetMovementsByWalletIdAsync(wallet.Id);

            result.Should().HaveCount(1);
            result.First().Type.Should().Be("Débito");
        }
    }
}
