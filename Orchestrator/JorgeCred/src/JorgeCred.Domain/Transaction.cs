using JorgeCred.Domain.Shared;

namespace JorgeCred.Domain
{
    public class Transaction : BaseEntity
    {
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public bool Valid { get; set; } = false;
        public DateTime ValidationDate { get; set; }


        // EF RELATED: Conta de origem da transação
        public int TargetAccountId { get; set; }
        public Account? TargetAccount { get; set; }

        // EF RELATED: Conta de destino da transação
        public int OriginAccountId { get; set; }
        public Account? OriginAccount { get; set; }
    }
}
