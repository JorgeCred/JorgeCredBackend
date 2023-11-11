using JorgeCred.Domain.Shared;

namespace JorgeCred.Domain
{
    public class Account : BaseEntity
    {
        public decimal Balance { get; set; }
        public Guid CardNumber { get; set; }
        public DateTime CardValidity { get; set; }
        public string CardSecurityStamp { get; set; }


        // EF RELATED: Transações associadas a esta conta (seja como origem ou destino)
        public IEnumerable<Transaction> Transactions { get; set; }
        public string ApplicationUserId { get; set; } 
        public ApplicationUser ApplicationUser { get; set; }
    }
}
