using ShireBank.Server.Models.Enums;

namespace ShireBank.Server.Models
{
    public class Transaction
    {
        public uint Id { get; set; }
        public uint AccountId { get; set; }
        public float Value { get; set; }
        public float Balance { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}\t {nameof(AccountId)}: {AccountId}\t {nameof(Value)}: {Value}\t {nameof(Balance)}: {Balance}\t {nameof(Type)}: {Enum.GetName(Type)}\t {nameof(Timestamp)}: {Timestamp.ToString("O")}";
        }
    }
}
