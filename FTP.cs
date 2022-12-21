using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Br1BrownAPI.FTP
{

    public class FTPClient
    {

        public class FTPPath
        {

            /// <summary>
            /// TrimStart
            /// </summary>
            /// <param name="target"></param>
            /// <param name="trimString"></param>
            /// <returns></returns>
            public static string TrimStart(string target, params string[] trimString)
            {
                string result = target;
                foreach (var tString in trimString)
                    if (!string.IsNullOrEmpty(tString))
                    {
                        while (result.StartsWith(tString))
                        {
                            result = result.Substring(tString.Length);
                        }
                    }
                return result;
            }

            /// <summary>
            /// TrimEnd
            /// </summary>
            /// <param name="target"></param>
            /// <param name="trimString"></param>
            /// <returns></returns>
            public static string TrimEnd(string target, params string[] trimString)
            {
                string result = target;
                foreach (var tString in trimString)
                    if (!string.IsNullOrEmpty(tString))
                    {
                        while (result.EndsWith(tString))
                        {
                            result = result.Substring(0, result.Length - tString.Length);
                        }
                    }
                return result;
            }

            /// <summary>
            /// Parti del percorso
            /// </summary>
            public List<string> Vals { set; get; }

            public static implicit operator FTPPath(string input)
            {
                return new FTPPath(input);
            }

            public static implicit operator FTPPath(string[] input)
            {
                return new FTPPath() { Vals = input.ToList() };
            }

            public static implicit operator string(FTPPath input)
            {
                return string.Join("/", input.Vals.Where(rr => !string.IsNullOrWhiteSpace(rr)));
            }

            public FTPPath()
            {
                Vals = new List<string>();
            }

            public FTPPath(string Path)
            {
                Vals = Path.Split('\\', '/').Where(rr => !string.IsNullOrWhiteSpace(rr)).ToList();
            }
        }

        public string MAINFOLDER { get; private set; }
        private string User, Pwd;
        private NetworkCredential NETCREDENTIAL { get { return new NetworkCredential(User, Pwd); } }

        public FTPClient(FTPPath FTP_FOLDER, string FTP_User, string FTP_Pwd)
        {
            FTP_FOLDER = ((string)FTP_FOLDER).TrimEnd('/', '\\');
            FTP_FOLDER += "/";
            FTP_FOLDER = FTPPath.TrimStart(FTP_FOLDER, "ftp://", "ftp.");
            MAINFOLDER = "ftp://ftp." + FTP_FOLDER;
            User = FTP_User;
            Pwd = FTP_Pwd;
        }
        /// <summary>
        /// if path is of file
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <returns></returns>
        public bool IsFile(string ftpPath)
        {

            FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.GetFileSize, ftpPath);

            try
            {
                using (var response = (FtpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    return true;
                }
            }
            catch (WebException ex)
            {
                return Path.HasExtension(ftpPath);
            }
        }

        private FtpWebRequest GetRequest(string method, string folder = "")
        {

            folder = FTPPath.TrimStart(folder, MAINFOLDER);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Validator.Validator.CombineURL(MAINFOLDER, folder));
            request.Credentials = NETCREDENTIAL;

            request.Method = method;

            return request;
        }

        /// <summary>
        /// Get list of al content in a folder 
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public List<string> AllContents(params string[] Listpaths)
        {
            FTPPath paths = (Listpaths);
            FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.ListDirectory, paths);
            try
            {
                var res = new List<string>();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                var tuttiassieme = reader.ReadToEnd();
                var cartelle = tuttiassieme.Split('\r', '\n').Where(rr => !string.IsNullOrWhiteSpace(rr));
                foreach (var item in cartelle)
                {
                    var path = item.Split('/', '\\').LastOrDefault();
                    if (path != null && path.Replace(".", "").Count() > 0)
                        res.Add(path);


                }
                reader.Close();
                response.Close();

                return res;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create directory
        /// </summary>
        /// <param name="pathToCreate"></param>
        public void CreateDir(FTPPath pathToCreate)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;

            FTPPath currentDir = new FTPPath();

            foreach (string subDir in pathToCreate.Vals)
            {

                try
                {
                    if (currentDir != "")
                        currentDir.Vals.Add(subDir);
                    else
                        currentDir = subDir;

                    reqFTP = GetRequest(WebRequestMethods.Ftp.MakeDirectory, currentDir);

                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                    ftpStream.Close();
                    response.Close();
                }
                catch
                {

                }
                currentDir += "/";
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="local"></param>
        /// <param name="path"></param>
        public void Upload(string local, FTPPath path)
        {
            var cartella = Path.GetDirectoryName(path);
            CreateDir(cartella);
            using (WebClient client = new WebClient())
            {
                client.Credentials = NETCREDENTIAL;
                client.UploadFile(Validator.Validator.CombineURL(MAINFOLDER, path), WebRequestMethods.Ftp.UploadFile, local);
            }
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="local"></param>
        public void Download(FTPPath path, string local)
        {
            FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DownloadFile, path);

            using (Stream ftpStream = request.GetResponse().GetResponseStream())
            using (Stream fileStream = File.Create(local))
            {
                ftpStream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Download Stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream DownloadStream(string path)
        {
            try
            {
                FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DownloadFile, path);
                return request.GetResponse().GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Download all rows of file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<string> DownloadROWS(string path)
        {
            var ss = DownloadStream(path);
            if (ss == null)
                return new List<string>();
            return Out.Read.TXT(new StreamReader(ss));
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="filep"></param>
        /// <returns></returns>
        public bool Delete(FTPPath filep)
        {
            FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.DeleteFile, filep);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusDescription.StartsWith("250 ");
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="filep"></param>
        /// <returns></returns>
        public bool DeleteDIR(FTPPath filep)
        {
            FtpWebRequest request = GetRequest(WebRequestMethods.Ftp.RemoveDirectory, filep);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusDescription.StartsWith("250 ");
                }
            }
            catch { return false; }
        }
    }
}
