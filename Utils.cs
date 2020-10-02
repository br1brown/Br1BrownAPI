using System;
using System.Net;

namespace Br1BrownAPI {
	

	public static class Utils {

		/// <summary>
		/// if you can connect on net
		/// </summary>
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

			/// <summary>
			/// Get username
			/// </summary>
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
