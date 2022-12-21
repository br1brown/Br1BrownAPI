using System;
using System.Net;
using System.Text;

namespace Br1BrownAPI {
	


	public static class Utils {

		/// <summary>
		/// Andare a capo a circa a metà
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
        public static string Spezza(string input)
        {
            // Determina la lunghezza della stringa
            int length = input.Length;

            // Determina il punto di divisione della stringa, circa a metà
            int breakPoint = length / 2;

            // Trova il primo spazio vuoto dopo il punto di divisione
            int breakIndex = input.IndexOf(' ', breakPoint);

            // Se non è stato trovato uno spazio vuoto, usa il punto di divisione come indice di divisione
            if (breakIndex == -1)
            {
                return input;
            }

            // Crea un oggetto StringBuilder per costruire la nuova stringa
            StringBuilder output = new StringBuilder();

            // Aggiungi la prima metà della stringa al nuovo oggetto StringBuilder
            output.Append(input.Substring(0, breakIndex));

            // Aggiungi una nuova riga
            output.AppendLine();

            // Aggiungi la seconda metà della stringa al nuovo oggetto StringBuilder
            output.Append(input.Substring(breakIndex));

            // Restituisci la nuova stringa
            return output.ToString();
        }


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
