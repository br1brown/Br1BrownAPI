using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Br1BrownAPI {

	public class ItemFTP {

		protected string CalcPath(params string []Element) {
			for (int i = 0; i < Element.Length; i++)
				Element[i] = Element[i].TrimEnd('/', '\\');

			return string.Join("/", Element);
		}

		public Folder_FTP Parent {
			get {
				var spl = _Path.Split('/');
				List<string> newP = new List<string>();
				for (int i = 0; i < spl.Length - 1; i++)
					newP.Add( spl[i]);
				return new Folder_FTP(cl, CalcPath(newP.ToArray()));
			}
		}

		public string Name { get { return _Path.Split('/').Last(); } }

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
		/// <param name="FTP_FOLDER">ftp://ftp.domain.org/</param>
		/// <param name="FTP_User"></param>
		/// <param name="FTP_Pwd"></param>
		public Client_FTP(string FTP_FOLDER, string FTP_User, string FTP_Pwd) {
			cl = new FTPClient(FTP_FOLDER, FTP_User, FTP_Pwd);
		}

		public Folder_FTP ROOT { get { return new Folder_FTP(cl, ""); } }

	}

	public class Folder_FTP : ItemFTP {

		public List<Folder_FTP> Folders {
			get {
				var ret = new List<Folder_FTP>();
				foreach (var c in cl.AllContents(_Path))
					if (!cl.IsFile(c))
						ret.Add(new Folder_FTP(cl, CalcPath(_Path, c)));
				return ret;
			}
		}
		public List<File_FTP> Files {
			get {
				var ret = new List<File_FTP>();
				foreach (var c in cl.AllContents(_Path))
					if (cl.IsFile(c))
						ret.Add(new File_FTP(cl, CalcPath (_Path , c)));
				return ret;
			}
		}

		internal Folder_FTP(FTPClient _cl, string sub) : base(_cl, sub) { }
	}


	public class File_FTP : ItemFTP {

		public void Download(string localPath) {
			cl.Download(_Path, localPath);
		}

		internal File_FTP(FTPClient _cl, string sub) : base(_cl, sub) { }
	}

	public class FTPClient {

		public string MAINFOLDER { get; private set; }
		private string User, Pwd;
		private NetworkCredential NETCREDENTIAL { get { return new NetworkCredential(User, Pwd); } }

		public FTPClient(string FTP_FOLDER, string FTP_User, string FTP_Pwd) {
			MAINFOLDER = FTP_FOLDER;
			User = FTP_User;
			Pwd = FTP_Pwd;
		}

		public bool IsFile(string requestUrl) {

			var ftpWebRequest = GetRequest(WebRequestMethods.Ftp.GetFileSize, requestUrl);

			try { return ((FtpWebResponse)ftpWebRequest.GetResponse()).ContentLength != default(long); }
			catch (Exception) { return false; }
		}


		private FtpWebRequest GetRequest(string method, string folder = "") {
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(MAINFOLDER + folder);
			request.Credentials = NETCREDENTIAL;

			if (!string.IsNullOrEmpty(method))
				request.Method = method;

			return request;
		}

		public List<string> AllContents(string sub = "") {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.ListDirectory, sub);

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();

			Stream responseStream = response.GetResponseStream();
			StreamReaderer = new StreamReader(responseStream);
			var cartelle =er.ReadToEnd().Replace("\r", "").Split('\n');

			reader.Close();
			response.Close();

			return cartelle.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
		}

		public void CreateDir(string pathToCreate) {
			FtpWebRequest reqFTP = null;
			Stream ftpStream = null;

			string[] subDirs = pathToCreate.Split('/');

			string currentDir = "";

			foreach (string subDir in subDirs) {

				try {
					if (currentDir != "")
						currentDir = currentDir + "/" + subDir;
					else
						currentDir = subDir;

					reqFTP = GetRequest(WebRequestMethods.Ftp.MakeDirectory, currentDir);

					reqFTP.UseBinary = true;
					FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
					ftpStream = response.GetResponseStream();
					ftpStream.Close();
					response.Close();
				}
				catch {

				}
				currentDir += "/";
			}
		}

		public void Upload(string local, string path) {
			var cartella = Path.GetDirectoryName(path).Replace("\\", "/");
			CreateDir(cartella);
			using (WebClient client = new WebClient()) {
				client.Credentials = NETCREDENTIAL;
				client.UploadFile(MAINFOLDER + path, WebRequestMethods.Ftp.UploadFile, local);
			}
		}

		public void Download(string path, string local) {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DownloadFile, path);

			using (Stream ftpStream = request.GetResponse().GetResponseStream())
			using (Stream fileStream = File.Create(local)) {
				ftpStream.CopyTo(fileStream);
			}
		}

		public bool Delete(string filep) {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DeleteFile, filep);

			try {
				using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) {
					return response.StatusDescription.StartsWith("250 ");
				}
			}
			catch { return false; }
		}
	}
}
