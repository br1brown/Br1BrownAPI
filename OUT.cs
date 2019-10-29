using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Br1BrownAPI {
	public static class Read {

		public static List<string> TXT(string _Path) {
			List<string> lines = new List<string>();
			if (File.Exists(_Path)) {
				using (StreamReader r = new StreamReader(_Path)) {
					lines.AddRange(TXT(r));
				}
			}
			return lines;
		}

		public static List<string> URL(string URL) {
			return TXT(new StreamReader(new WebClient().OpenRead(URL)));
		}

		public static List<string> TXT(StreamReader stream) {
			return stream.ReadToEnd().Split(new[] { "\r\n", "\r", "\n",Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
		}
	}
}

