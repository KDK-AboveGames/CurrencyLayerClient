using System;
using System.Globalization;
using Assets.Scripts.CurrencyLayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class QuoteWindowView : MonoBehaviour
	{
		[SerializeField] private TMP_Text ErrorMessageText;

		[SerializeField] private TMP_Text QuoteText;

		[SerializeField] private TMP_Text AutoUpdateTimerInfoText;

		[SerializeField] private Button ManualUpdateButton;
		[SerializeField] private GameObject ManualUpdateAvailableIndicator;
		[SerializeField] private GameObject ManualUpdateUnavailableIndicator;

		[Tooltip("Small delay just to improve UX by avoiding blinking loading spinner")]
		[SerializeField][Range(0, 10)] private float UpdateMinDuration = 1.5f;
		public float UpdateMinDurationSec => UpdateMinDuration;

		public event Action ManualUpdateRequested;

		private bool UpdateInProcess;

		private float? NextUpdateTime;
		private int NextUpdateSecondsLeft;

		private void Awake()
		{
			QuoteText.text = null; // clear only once. after further updated no need to clear

			ManualUpdateButton.onClick.AddListener(
				() =>
				{
					if (!UpdateInProcess)
					{
						ManualUpdateRequested?.Invoke();
					}
				});
		}


		public void DisplayUpdateInProcess()
		{
			ErrorMessageText.text = null;

			SetNextUpdateTime(null);
			DisplayUpdateStatus(updateInProcess: true);
		}

		public void DisplayInfo(QuoteInfo quoteInfo, int quoteDisplayedPrecision, int nextUpdateInMinutes)
		{
			QuoteText.text = $"{quoteInfo.BaseCurrency}/{quoteInfo.QuoteCurrency}"
				+ $" = {quoteInfo.Quote.ToString("F" + quoteDisplayedPrecision, CultureInfo.InvariantCulture)}";

			ErrorMessageText.text = null;

			DisplayUpdateStatus(updateInProcess: false);
			SetNextUpdateTime(nextUpdateInMinutes);
		}

		public void DisplayError(string errorMessage, int nextUpdateInMinutes)
		{
			ErrorMessageText.text = errorMessage;

			DisplayUpdateStatus(updateInProcess: false);
			SetNextUpdateTime(nextUpdateInMinutes);
		}


		private void Update()
		{
			DisplayNextUpdateTimeLeft();
		}


		private void DisplayUpdateStatus(bool updateInProcess)
		{
			ManualUpdateAvailableIndicator.SetActive(!updateInProcess);
			ManualUpdateUnavailableIndicator.SetActive(updateInProcess);

			UpdateInProcess = updateInProcess;
		}

		private void SetNextUpdateTime(int? nextUpdateInMinutes)
		{
			if (nextUpdateInMinutes.HasValue)
			{
				NextUpdateSecondsLeft = nextUpdateInMinutes.Value * 60;
				NextUpdateTime = Time.realtimeSinceStartup + NextUpdateSecondsLeft;
			}
			else
			{
				NextUpdateTime = null;
			}

			DisplayNextUpdateTimeLeft(force: true);
		}

		private void DisplayNextUpdateTimeLeft(bool force = false)
		{
			if (NextUpdateTime.HasValue)
			{
				int secondsLeft = Mathf.CeilToInt(NextUpdateTime.Value - Time.realtimeSinceStartup);
				if (secondsLeft != NextUpdateSecondsLeft || force)
				{
					AutoUpdateTimerInfoText.text = $"Next auto-refresh in {secondsLeft} sec";
					NextUpdateSecondsLeft = secondsLeft;
				}
			}
			else
			{
				AutoUpdateTimerInfoText.text = null;
			}
		}
	}
}
