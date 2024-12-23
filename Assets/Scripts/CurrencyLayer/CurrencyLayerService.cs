using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Assets.Scripts.CurrencyLayer
{
	public class CurrencyLayerService
	{
		private const string ApiAccessKeyQueryParameter = "access_key";
		private const string BaseCurrencyQueryParameter = "source";
		private const string QuoteCurrencyQueryParameter = "currencies";

		private readonly Uri QuotesUriBase;

		public CurrencyLayerService(string apiAccessKey)
		{
			QuotesUriBase = new Uri("https://apilayer.net/api/live")
				.AddQueryStringParameter(ApiAccessKeyQueryParameter, apiAccessKey);
		}

		public async UniTask<QuoteInfo> GetQuoteAsync(
			Currencies baseCurrency,
			Currencies quoteCurrency,
			CancellationToken cancellationToken = default)
		{
			using UnityWebRequest webRequest = UnityWebRequest.Get(
				QuotesUriBase
					.AddQueryStringParameter(BaseCurrencyQueryParameter, baseCurrency.ToString())
					.AddQueryStringParameter(QuoteCurrencyQueryParameter, quoteCurrency.ToString()));

			await webRequest.SendWebRequest().WithCancellation(cancellationToken);

			QuotesResponse response = webRequest.GetResponseContent<QuotesResponse>();
			if (!response.success)
			{
				throw new Exception($"Error while getting currency quotes data: {response.error.code} - {response.error.info}.");
			}

			string quoteKey = $"{baseCurrency}{quoteCurrency}";
			if (response.quotes == null
				|| !response.quotes.TryGetValue(quoteKey, out float quote))
			{
				throw new Exception($"Unable to get requested quote {baseCurrency}->{quoteCurrency} from response.");
			}

			return new QuoteInfo(baseCurrency, quoteCurrency, quote);
		}

		#region DTO

		private class QuotesResponse
		{
			public bool success;
			public ResponseError error;

			public string terms;
			public string privacy;
			public int timestamp;

			public Currencies source;
			public Dictionary<string, float> quotes;
		}

		private class ResponseError
		{
			public int code;
			public string info;
		}

		#endregion DTO
	}
}
