using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Br1BrownAPI {
	static class Validator {
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

		public static bool URL(string url) {
			return new Regex
				(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase)
				.IsMatch(url);
		}

		public static bool URLExists(string url) {
			if (!Validator.URL(url))
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

		public static bool IsLocalFile(string _file) {
			return new Uri(_file).IsFile;
		}

	}
}
