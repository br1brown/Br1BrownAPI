using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Br1BrownAPI {
	public static class Read {

		/// <summary>
		/// Read all lines from URL or LocalFile
		/// </summary>
		/// <param name="_Path">URL or LocalFile</param>
		/// <returns></returns>
		public static List<string> GenericPATH(string _Path) {
			if (Validator.IsLocalFile(_Path))
				return TXT(_Path);
			else
				return URL(_Path);
		}

		/// <summary>
		/// Read all lines from local file
		/// </summary>
		/// <param name="_Path"></param>
		/// <returns></returns>
		public static List<string> TXT(string _Path) {
			List<string> lines = new List<string>();
			if (File.Exists(_Path)) {
				using (StreamReader r = new StreamReader(_Path))
					lines.AddRange(TXT(r));
				return lines;
			}
			return null;
		}

		/// <summary>
		/// Read all lines from file on line
		/// </summary>
		/// <param name="URL"></param>
		/// <returns></returns>
		public static List<string> URL(string URL) {
			if (Utils.IS_ON_NET && Validator.URLExists(URL))
				return TXT(new StreamReader(new WebClient().OpenRead(URL)));
			return null;
		}

		/// <summary>
		/// Read all lines from stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static List<string> TXT(StreamReader stream) {
			return stream.ReadToEnd().Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.None).ToList();
		}
	}
}

