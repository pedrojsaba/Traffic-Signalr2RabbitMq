using System.Collections.Generic;

namespace UpgFisi.Common.Domain
{
    public sealed class Money : ValueObject<Money>
    {
        public decimal Amount { get; }
        public Currency Currency { get; }

        public Money()
        {
            Amount = 0;
            Currency = Currency.USD;
        }

        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Dollars(decimal amount)
        {
            return new Money(amount, Currency.USD);
        }

        public static Money Soles(decimal amount)
        {
            return new Money(amount, Currency.PEN);
        }

        public static Money Euros(decimal amount)
        {
            return new Money(amount, Currency.EUR);
        }

        public Money Add(Money money)
        {
            decimal total = Amount + money.Amount;
            return new Money(total, money.Currency);
        }

        public Money Subtract(Money money)
        {
            decimal total = Amount - money.Amount;
            return new Money(total, money.Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}