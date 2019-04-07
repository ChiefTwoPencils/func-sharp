namespace WebFunc.Models
{
    public class Account
    {
        public decimal Balance { get; }

        public Account(decimal balance) => Balance = balance;
        
        public Account WithBalance(decimal balance) => new Account(balance);
        
        public Account FromDeposit(decimal amount) => new Account(Balance + amount);
        
        public Account FromWithdrawal(decimal amount) => new Account(Balance - amount);
    }
}