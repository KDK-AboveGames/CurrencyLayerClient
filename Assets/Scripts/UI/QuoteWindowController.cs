using System;
using System.Threading;
using Assets.Scripts.CurrencyLayer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class QuoteWindowController : MonoBehaviour
	{
		[SerializeField] private Configuration Configuration;
		[SerializeField] private QuoteWindowView View;

		private CurrencyLayerService CurrencyLayerService;

		private CancellationTokenSource AutoUpdateTaskCts;
		private bool UpdateInProgress;

		private void Awake()
		{
			View.ManualUpdateRequested += ManualUpdateQuote;

			CurrencyLayerService = new CurrencyLayerService(Configuration.CurrencyLayerApiAccessKey);
		}

		private void Start()
		{
			UpdateQuote();
		}

		private void OnDestroy()
		{
			View.ManualUpdateRequested -= ManualUpdateQuote;
		}

		private void ManualUpdateQuote()
		{
			if (AutoUpdateTaskCts != null)
			{
				AutoUpdateTaskCts.Cancel();
				AutoUpdateTaskCts = null;
			}

			UpdateQuote();
		}

		private void UpdateQuote()
		{
			if (UpdateInProgress)
			{
				return;
			}

			View.DisplayUpdateInProcess();
			UpdateInProgress = true;

			void FinishUpdate()
			{
				UpdateInProgress = false;

				AutoUpdateTaskCts = new CancellationTokenSource();
				AutoUpdateTaskCts.RegisterRaiseCancelOnDestroy(this);

				AutoUpdateDelayed(Configuration.UpdateIntervalMinutes, AutoUpdateTaskCts.Token)
					.Forget();
			};

			UniTask artificialDelayTask = UniTask.Delay(
				Mathf.CeilToInt(View.UpdateMinDurationSec * 1000),
				ignoreTimeScale: true,
				cancellationToken: destroyCancellationToken);

			CurrencyLayerService.GetQuoteAsync(Configuration.BaseCurrency, Configuration.QuoteCurrency, destroyCancellationToken)
				.ContinueWith(
					async quote =>
					{
						await artificialDelayTask; // wait if min time was not elapsed

						View.DisplayInfo(quote, Configuration.QuoteDisplayedPrecision, Configuration.UpdateIntervalMinutes);
						FinishUpdate();
					})
				.Forget(
					ex =>
					{
						if (ex is OperationCanceledException)
						{
							Debug.LogException(ex);
							return;
						}

						View.DisplayError(ex.Message, Configuration.UpdateIntervalMinutes);
						FinishUpdate();
					});
		}

		private async UniTaskVoid AutoUpdateDelayed(int delayInMinutes, CancellationToken cancellationToken = default)
		{
			await UniTask.Delay(
				delayInMinutes * 60 * 1000,
				ignoreTimeScale: true,
				cancellationToken: cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			UpdateQuote();
		}
	}
}
