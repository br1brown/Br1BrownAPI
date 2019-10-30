using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Br1BrownAPI {
	public static class Validator {
		/// <summary>
		/// Path.Combine() with URL
		/// </summary>
		/// <param name="Element"></param>
		/// <returns></returns>
		public static string CombineURL(params string[] Element) {
			for (int i = 0; i < Element.Length; i++)
				Element[i] = Element[i].TrimEnd('/', '\\');

			return string.Join("/", Element);
		}

		/// <summary>
		/// Validate MAIL
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public static bool EMAIL(string email) {
			try {
				if (string.IsNullOrEmpty(email))
					throw new Exception();

				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch {
				return false;
			}
		}

		/// <summary>
		/// Validate URL
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static bool URL(string url) {
			return new Regex
				(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase)
				.IsMatch(url);
		}

		/// <summary>
		/// if url exist
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static bool URLExists(string url) {
			if (!URL(url))
				return false;
			try {
				WebRequest webRequest = System.Net.WebRequest.Create(url);
				webRequest.Method = "HEAD";

				try {
					using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse()) {
						return (response.StatusCode.ToString() == "OK");
					}
				}
				catch {
					return false;
				}
			}
			catch (Exception eee) {
				throw new Exception("No internet");
			}
		}

		/// <summary>
		/// if path is local file
		/// </summary>
		/// <param name="_file"></param>
		/// <returns></returns>
		public static bool IsLocalFile(string _file) {
			return new Uri(_file).IsFile;
		}

	}
}
