using System;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Assets.Scripts.Utils
{
	public static class UnityWebRequestExtensions
	{
		/// <summary>
		/// Appends query string parameter to uri.
		/// </summary>
		public static Uri AddQueryStringParameter(this Uri uri, string key, string value)
		{
			UriBuilder responseUri = new UriBuilder(uri.ToString());

			NameValueCollection query = HttpUtility.ParseQueryString(responseUri.Query);
			query.Add(key, value);

			responseUri.Query = query.ToString();
			return responseUri.Uri;
		}

		/// <summary>
		/// Deserializes model from response content.
		/// </summary>
		public static T GetResponseContent<T>(this UnityWebRequest webRequest)
		{
			switch (webRequest.result)
			{
				case UnityWebRequest.Result.Success:
				{
					try
					{
						if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
						{
							throw new Exception("Response content is empty");
						}

						return JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
					}
					catch (Exception ex)
					{
						throw new Exception(
							$"Unable to deserialise {nameof(UnityWebRequest)} response content into {typeof(T).FullName}. {ex.Message}");
					}
				}

				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
				case UnityWebRequest.Result.ProtocolError:
				{
					throw new Exception(
						$"Error while sending {nameof(UnityWebRequest)}: {webRequest.result} - {webRequest.error}.");
				}

				default:
				{
					throw new Exception(
						$"{nameof(UnityWebRequest)} response content is not ready to read.");
				}
			}
		}
	}
}
