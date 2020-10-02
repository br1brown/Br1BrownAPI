using Br1BrownAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test {
	class Program {
		static void Main(string[] args) {

			Client.CreateDir("TEST/anagrafiche");
			Client.Upload(Path.GetTempFileName(), "TEST/anagrafiche/eeeee.txt");
			var c = Client.AllContents("TEST\\anagrafiche");
			var f = Client.DownloadROWS("TEST/anagrafiche/eeeee.txt");
			Client.IsFile("TEST/anagrafiche/eeeee.txt");
			var b1 =Client.Delete("TEST/anagrafiche/eeeee.txt");
			var b3 = Client.DeleteDIR("TEST/anagrafiche");
			Console.ReadKey();
		}
		private static FTPClient Client { get { return new FTPClient("myfriend.altervista.org/BILANCI8", "myfriend", "9yKetG4XdXf7"); } }


	}
}
