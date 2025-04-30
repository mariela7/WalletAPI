using WalletApp.Domain.Entities;

namespace WalletApp.Application.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet?> GetByIdAsync(int id);
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<bool> UpdateAsync(Wallet wallet);
        Task<bool> DeleteAsync(int id);
    }
}
