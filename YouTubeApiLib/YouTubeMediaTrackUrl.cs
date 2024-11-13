using System.Collections.Generic;

namespace YouTubeApiLib
{
	public class YouTubeMediaTrackUrl
	{
		public string Url { get; private set; }
		public string SignatureCipherString { get; }
		public Dictionary<string, string> QueryUrl { get; private set; }
		public Dictionary<string, string> QueryCipher { get; private set; }
		public bool IsCiphered => !string.IsNullOrEmpty(SignatureCipherString) &&
			!string.IsNullOrWhiteSpace(SignatureCipherString);

		private readonly string _originalUrl;
		private string _firstPart = null;

		public YouTubeMediaTrackUrl(string url, string signatureCipherString)
		{
			Url = url;
			SignatureCipherString = signatureCipherString;
			_originalUrl = url;
		}

		public bool SplitUrl()
		{
			string[] splitted = !string.IsNullOrEmpty(Url) ?
				Url.Split(new char[] { '?' }, 2) :
				_originalUrl?.Split(new char[] { '?' }, 2);
			if (splitted != null && splitted.Length > 1)
			{
				_firstPart = splitted[0];
				QueryUrl = Utils.SplitStringToKeyValues(splitted[1], '&', '=');
				return true;
			}

			QueryUrl = null;
			return false;
		}

		public bool MergeUrl()
		{
			if (QueryUrl != null && !string.IsNullOrEmpty(_firstPart) && !string.IsNullOrWhiteSpace(_firstPart))
			{
				string t = JoinDictionary(QueryUrl, '&', '=');
				if (!string.IsNullOrEmpty(t))
				{
					Url = $"{_firstPart}?{t}";
					return true;
				}
			}

			return false;
		}

		public bool SplitCipher()
		{
			if (!string.IsNullOrEmpty(SignatureCipherString))
			{
				QueryCipher = Utils.SplitStringToKeyValues(SignatureCipherString, '&', '=');
				return QueryCipher != null;
			}

			return false;
		}

		public bool MergeCipherUrl(string decryptedCipherSignatureValue, bool useUrlDecode = true)
		{
			if (QueryCipher != null && QueryCipher.ContainsKey("url"))
			{
				string url = useUrlDecode ? Utils.UrlDecode(QueryCipher["url"]) : QueryCipher["url"];
				Url = $"{url}&sig={decryptedCipherSignatureValue}";
				return true;
			}

			return false;
		}

		public void Reset()
		{
			Url = _originalUrl;
			_firstPart = null;
			QueryUrl = QueryCipher = null;
		}

		private static string JoinDictionary(Dictionary<string, string> dictionary,
			char keySeparator, char valueSeparator)
		{
			var keys = dictionary.Keys;
			if (keys != null && keys.Count > 0)
			{
				string result = string.Empty;
				foreach (string key in keys)
				{
					string value = dictionary[key];
					result += $"{keySeparator}{key}{valueSeparator}{value}";
				}

				return result.Length > 1 ? result.Substring(1) : null;
			}

			return null;
		}

		public override string ToString()
		{
			return Url ?? "null";
		}
	}
}
