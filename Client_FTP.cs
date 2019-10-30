using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Br1BrownAPI {

	public class ItemFTP {
		
		public Folder_FTP Parent {
			get {
				var spl = _Path.Split('/');
				List<string> newP = new List<string>();
				for (int i = 0; i < spl.Length - 1; i++)
					newP.Add(spl[i]);
				return new Folder_FTP(cl, Validator.CombineURL(newP.ToArray()));
			}
		}

		public string Name { get { return _Path.Split('/').Last(); } }

		/// <summary>
		/// All url of item
		/// </summary>
		public string Position { get { return new Uri(new Uri(cl.MAINFOLDER), _Path).AbsoluteUri; } }

		internal FTPClient cl;
		internal string _Path;

		internal ItemFTP(FTPClient _cl, string sub) {
			cl = _cl;
			_Path = sub;
		}

		public override string ToString() {
			return Name;
		}
	}

	public class Client_FTP {
		FTPClient cl;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="FTP_FOLDER">[domain.org]</param>
		/// <param name="FTP_User"></param>
		/// <param name="FTP_Pwd"></param>
		public Client_FTP(string FTP_FOLDER, string FTP_User, string FTP_Pwd) {
			cl = new FTPClient(FTP_FOLDER, FTP_User, FTP_Pwd);
		}

		public Folder_FTP ROOT { get { return new Folder_FTP(cl, ""); } }

	}

	/// <summary>
	/// Instance of Folder FTP
	/// </summary>
	public class Folder_FTP : ItemFTP {

		public List<Folder_FTP> Folders {
			get {
				var ret = new List<Folder_FTP>();
				foreach (var c in cl.AllContents(_Path))
					if (!cl.IsFile(c))
						ret.Add(new Folder_FTP(cl, Validator.CombineURL(_Path, c)));
				return ret;
			}
		}
		public List<File_FTP> Files {
			get {
				var ret = new List<File_FTP>();
				foreach (var c in cl.AllContents(_Path))
					if (cl.IsFile(c))
						ret.Add(new File_FTP(cl, Validator.CombineURL(_Path, c)));
				return ret;
			}
		}

		internal Folder_FTP(FTPClient _cl, string sub) : base(_cl, sub) { }
	}
	/// <summary>
	/// Instance of File FTP
	/// </summary>
	public class File_FTP : ItemFTP {

		public void Download(string localPath) {
			cl.Download(_Path, localPath);
		}

		internal File_FTP(FTPClient _cl, string sub) : base(_cl, sub) { }
	}

}
