using Microsoft.EntityFrameworkCore;
using WalletApp.Application.Exceptions;
using WalletApp.Application.Interfaces;
using WalletApp.Domain.Entities;
using WalletApp.Infrastructure.Persistence;


namespace WalletApp.Infrastructure.Services
{
    public class MovementService : IMovementService
    {
        private readonly WalletDbContext _context;

        public MovementService(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TransferAsync(int originId, int destinationId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new BusinessException("El monto debe ser mayor que cero.");
            }

            var origin = await _context.Wallets.FindAsync(originId);
            var destination = await _context.Wallets.FindAsync(destinationId);

            if (origin == null)
            {
                throw new BusinessException("La billetera de origen no existe.");
            }

            if (destination == null)
            {
                throw new BusinessException("La billetera de destino no existe.");
            }

            if (origin.Balance < amount)
            {
                throw new BusinessException("Saldo insuficiente en la billetera de origen.");
            }

            origin.Balance -= amount;
            destination.Balance += amount;

            var now = DateTime.UtcNow;

            var debit = new Movement
            {
                WalletId = originId,
                Amount = amount,
                Type = "Débito",
                CreatedAt = now
            };

            var credit = new Movement
            {
                WalletId = destinationId,
                Amount = amount,
                Type = "Crédito",
                CreatedAt = now
            };

            _context.Movements.AddRange(debit, credit);
            origin.UpdatedAt = now;
            destination.UpdatedAt = now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Movement>> GetMovementsByWalletIdAsync(int walletId)
        {
            return await _context.Movements
                .Where(m => m.WalletId == walletId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
