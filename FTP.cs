using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Br1BrownAPI {

	public class FTPClient {

		public string MAINFOLDER { get; private set; }
		private string User, Pwd;
		private NetworkCredential NETCREDENTIAL { get { return new NetworkCredential(User, Pwd); } }

		public FTPClient(string FTP_FOLDER, string FTP_User, string FTP_Pwd) {
			FTP_FOLDER = FTP_FOLDER.TrimEnd('/', '\\');
			FTP_FOLDER += "/";
			FTP_FOLDER = ManageString.TrimStart(FTP_FOLDER, "ftp://");
			FTP_FOLDER = ManageString.TrimStart(FTP_FOLDER, "ftp.");
			FTP_FOLDER = "ftp://ftp." + FTP_FOLDER;
			MAINFOLDER = FTP_FOLDER;
			User = FTP_User;
			Pwd = FTP_Pwd;
		}

		public bool IsFile(string ftpPath) {

			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.GetFileSize, ftpPath);

			try {
				using (var response = (FtpWebResponse)request.GetResponse())
				using (var responseStream = response.GetResponseStream()) {
					return true;
				}
			}
			catch (WebException ex) {
				return Path.HasExtension(ftpPath);
			}
		}

		private FtpWebRequest GetRequest(string method, string folder = "") {

			folder = ManageString.TrimStart(folder, MAINFOLDER);

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Validator.CombineURL(MAINFOLDER, folder));
			request.Credentials = NETCREDENTIAL;

			request.Method = method;

			return request;
		}

		public List<string> AllContents(string sub = "") {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.ListDirectory, sub);

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();

			Stream responseStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(responseStream);
			var cartelle = reader.ReadToEnd().Replace("\r", "").Split('\n');

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
				client.UploadFile(Validator.CombineURL(MAINFOLDER , path), WebRequestMethods.Ftp.UploadFile, local);
			}
		}

		public void Download(string path, string local) {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DownloadFile, path);

			using (Stream ftpStream = request.GetResponse().GetResponseStream())
			using (Stream fileStream = File.Create(local)) {
				ftpStream.CopyTo(fileStream);
			}
		}

		public Stream DownloadStream(string path) {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DownloadFile, path);
			return request.GetResponse().GetResponseStream();
		}

		public List<string> DownloadROWS(string path) {
			FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DownloadFile, path);
			return Read.TXT(new StreamReader(request.GetResponse().GetResponseStream()));
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
