using System.Collections.Generic;
using System.Text;
using System.Net.Mail;


namespace Br1BrownAPI {
	public class MailClient {

		private SmtpClient client;
		public bool SSL { set { client.EnableSsl = value; } get { return client.EnableSsl; } }

		public MailClient(string Host, int Port) {

			client = new SmtpClient();
			client.Host = Host;
			client.Port = Port;
		}

		public void Credential(string MailUser, string Password) {
			client.UseDefaultCredentials = false;
			client.Credentials = new System.Net.NetworkCredential(MailUser, Password);
		}


		/// <summary>
		/// invio mail con parametri
		/// </summary>
		/// <param name="mittente"></param>
		/// <param name="Destinatari"></param>
		/// <param name="oggetto"></param>
		/// <param name="corpo">Contenuto della mail</param>
		/// <param name="HTML">HTML abilitato</param>
		/// <param name="mailCC">Indirizzi per conoscenza </param>
		/// <param name="mailBCC">Indirizzi per conoscenza nascosta</param>
		public void inviaMail(
			string mittente,
			List<string> Destinatari,

			string oggetto,
			string corpo,

			List<string> mailCC = null,
			List<string> mailBCC = null,

			bool HTML = true
			) {

			MailMessage Messaggio = new MailMessage();

			Messaggio.From = new MailAddress(mittente);

			foreach (var Dest in Destinatari) {
				Messaggio.To.Add(Dest);
			}

			if (mailCC != null)
				foreach (var CC in mailCC) {
					Messaggio.CC.Add(CC);
				}

			if (mailBCC != null)
				foreach (var BCC in mailBCC) {
					Messaggio.Bcc.Add(BCC);
				}

			Messaggio.Subject = oggetto;

			Messaggio.SubjectEncoding = Encoding.UTF8;

			Messaggio.Body = corpo;
			Messaggio.IsBodyHtml = HTML;

			Messaggio.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

			client.Send(Messaggio);
		}

	}
}
