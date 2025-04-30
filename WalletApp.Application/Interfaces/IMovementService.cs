using WalletApp.Domain.Entities;

namespace WalletApp.Application.Interfaces
{
    public interface IMovementService
    {
        Task<bool> TransferAsync(int originWalletId, int destinationWalletId, decimal amount);

        Task<IEnumerable<Movement>> GetMovementsByWalletIdAsync(int walletId);
    }
}
