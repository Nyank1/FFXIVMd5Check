using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using log4net;
using Newtonsoft.Json;

namespace FFXIV_MD5_CHECK
{
    public class FileMd5Hash
    {

        private static readonly log4net.ILog logger = LogManager.GetLogger("FileMd5Hash");
        private static List<Task> tasks = new List<Task>();

        private static async void File(FileInfo file, RecordDataModel.FolderDataModel data)
        {
            RecordDataModel.FileDataModel fileData = null;


            Task t = Task.Run(() =>
            {
                void callback(IAsyncResult are)
                {
                    Func<string, RecordDataModel.FileDataModel> func1 = are.AsyncState as Func<string, RecordDataModel.FileDataModel>;
                    fileData = func1.EndInvoke(are);
                    data.fileDatas.Add(fileData);
                }

                Func<string, RecordDataModel.FileDataModel> func = (path) =>
                {
                    string Md5 = GetFileMd5Hash(file.FullName);
                    
                    return new RecordDataModel.FileDataModel() { fileName = file.Name, Md5Hash = Md5 };
                };

                var ar = func.BeginInvoke(file.FullName, callback, func);

            });
            await t;
            tasks.Add(t);

            
        }
        public static RecordDataModel.FolderDataModel GetFolderMd5Hash(string strFolderFullPATH)
        {
            logger.Debug($"Got follderpath \"{strFolderFullPATH}\", generating FolderData instance");
            RecordDataModel.FolderDataModel data = new RecordDataModel.FolderDataModel();

            DirectoryInfo folder = new DirectoryInfo(strFolderFullPATH);
            data.folderName = folder.Name;
            data.fileDatas = new List<RecordDataModel.FileDataModel>();
            //foreach (FileInfo file in folder.GetFiles())
            for (int c = 0; c < folder.GetFiles().Length; c++)
            {
                FileInfo file = folder.GetFiles()[c];
                logger.Debug($"<Folder: {data.folderName}>Starting generate FileData for file {file.Name}");
                File(file, data);
                //logger.Debug($"<Folder: {data.folderName}>FileData generated: {data.fileDatas[data.fileDatas.Count - 1].fileName}, Md5Hash: {data.fileDatas[data.fileDatas.Count - 1].Md5Hash}");

            }
            bool check = false;

            Task.WhenAll(tasks).ContinueWith((s) => { check = true; });


            while (true)
            {
                if (check)
                {
                    return data;
                }
            }

        }
        public static string GetFileMd5Hash(string strFileFullPATH)
        {
            //logger.Debug($"Got filepath {strFileFullPATH} translate to filestream");
            FileStream stream = new FileStream(strFileFullPATH, FileMode.Open);
            string res = GetFileMd5Hash(stream);
            stream.Close();
            return res;
        }

        public static string GetFileMd5Hash(FileStream fileStream)
        {
            logger.Debug($"Got stream from \"{fileStream.Name}\", calculating Md5Hash");
            byte[] data;
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                data = md5.ComputeHash(fileStream);
                md5.Clear();
            }

            catch (Exception e)
            {
                return e.Message;
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("X2"));
            }

            logger.Debug($"Md5hash of \"{fileStream.Name}\" is {builder.ToString()}");
            return builder.ToString();
        }
    }
}
