using Microsoft.EntityFrameworkCore;
using WalletApp.Application.Interfaces;
using WalletApp.Domain.Entities;
using WalletApp.Infrastructure.Persistence;

namespace WalletApp.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly WalletDbContext _context;

        public WalletService(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await _context.Wallets.FindAsync(id);
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            wallet.CreatedAt = DateTime.UtcNow;
            wallet.UpdatedAt = DateTime.UtcNow;
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<bool> UpdateAsync(Wallet wallet)
        {
            var existing = await _context.Wallets.FindAsync(wallet.Id);
            if (existing == null) 
            { 
                return false; 
            }

            existing.Name = wallet.Name;
            existing.DocumentId = wallet.DocumentId;
            existing.Balance = wallet.Balance;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null) 
            { 
                return false; 
            }

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
