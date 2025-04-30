namespace WalletApp.Domain.Entities
{
    public class Movement
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; // "Débito" o "Crédito"
        public DateTime CreatedAt { get; set; }

        
        public Wallet? Wallet { get; set; }
    }
}
