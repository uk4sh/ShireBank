namespace ShireBank.Shared
{
    public static class Constants
    {
        public const string BankBaseAddress = "http://localhost:5074";
        public const string ServiceName = "ShireBank";

        public static string FullBankAddress => BankBaseAddress + "/" + ServiceName;
    }
}