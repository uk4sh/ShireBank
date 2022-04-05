namespace ShireBank.Server.Models
{
    public class Account
    {
        public uint Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float DebtLimit { get; set; }
        public float Balance { get; set; }
        public bool IsClosed { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public int Version { get; set; }

        public override string? ToString()
        {
            return $"{nameof(Id)}: {Id}\t {nameof(FirstName)}: {FirstName}\t {nameof(LastName)}: {LastName}\t {nameof(DebtLimit)}: {DebtLimit}\t {nameof(Balance)}: {Balance}\t {nameof(IsClosed)}: {(IsClosed ? "Yes" : "No")}\t {nameof(Version)}: {Version}\n"
                + $"Transactions:\n{(Transactions?.Count() > 0 ? string.Join("\n", Transactions.OrderByDescending(t => t.Id)) : string.Empty)}" + "\n";
        }
    }
}
