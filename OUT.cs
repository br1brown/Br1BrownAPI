using System.Collections.Generic;
using System.IO;
using System.Net;

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
			List<string> lines = new List<string>();
			string line;
			while ((line = stream.ReadLine()) != null) {
				lines.Add(line);
			}
			return lines;

		}
	}
}

