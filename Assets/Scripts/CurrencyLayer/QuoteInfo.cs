namespace Assets.Scripts.CurrencyLayer
{
	public struct QuoteInfo
	{
		public readonly Currencies BaseCurrency;
		public readonly Currencies QuoteCurrency;
		public readonly float Quote;

		public QuoteInfo(Currencies baseCurrency, Currencies quoteCurrency, float quote)
		{
			BaseCurrency = baseCurrency;
			QuoteCurrency = quoteCurrency;
			Quote = quote;
		}
	}
}
