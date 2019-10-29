using System;
using System.Net;

namespace Br1BrownAPI {
	public static class ManageString {

		/// <summary>
		/// TrimStart
		/// </summary>
		/// <param name="target"></param>
		/// <param name="trimString"></param>
		/// <returns></returns>
		public static string TrimStart(string target, string trimString) {
			if (string.IsNullOrEmpty(trimString)) return target;

			string result = target;
			while (result.StartsWith(trimString)) {
				result = result.Substring(trimString.Length);
			}

			return result;
		}

		/// <summary>
		/// TrimEnd
		/// </summary>
		/// <param name="target"></param>
		/// <param name="trimString"></param>
		/// <returns></returns>
		public static string TrimEnd(string target, string trimString) {
			if (string.IsNullOrEmpty(trimString)) return target;

			string result = target;
			while (result.EndsWith(trimString)) {
				result = result.Substring(0, result.Length - trimString.Length);
			}

			return result;
		}
	}

	public static class Utils {

		public static bool IS_ON_NET {
			get {
				{
					try {
						using (var client = new WebClient())
						using (client.OpenRead("http://clients3.google.com/generate_204")) {
							return true;
						}
					}
					catch {
						return false;
					}
				}
			}
		}

		public static class User {
			public static string Name {
				get {
					string userName = "";
					try {
						userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
					}
					catch {
						userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
					}

					return userName;
				}
			}
		}

	}
}
