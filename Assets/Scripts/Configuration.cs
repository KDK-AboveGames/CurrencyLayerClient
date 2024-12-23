using System;
using Assets.Scripts.CurrencyLayer;
using UnityEngine;

namespace Assets.Scripts
{
	[CreateAssetMenu(fileName = "Configuration", menuName = "ScriptableObjects/Configuration")]
	public class Configuration : ScriptableObject
	{
		[Header("Currencies settings")]
		[Tooltip("Base currency (quote = BaseCurrency/QuoteCurrency)")]
		public Currencies BaseCurrency = Currencies.USD;

		[Tooltip("Quote currency (quote = BaseCurrency/QuoteCurrency)")]
		public Currencies QuoteCurrency = Currencies.EUR;

		[Tooltip("Digits after dot in quote value number")]
		[Range(0, 10)]
		public int QuoteDisplayedPrecision = 2;


		[Header("Service settings")]
		[Tooltip("Access Key CurrencyLayer service API (see https://currencylayer.com/dashboard)")]
		public string CurrencyLayerApiAccessKey;

		[Tooltip("Automatic refresh interval in minutes")]
		[Range(1, 60 * 24)]
		public int UpdateIntervalMinutes = 5;
	}
}
