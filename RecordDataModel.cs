using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace FFXIV_MD5_CHECK
{
    

    public class RecordDataModel
    {
        public IList<FolderDataModel> folderDatas { get; set; }
        public string gameVersion { get; set; }

        public class FolderDataModel
        {
            public string folderName { get; set; }
            public IList<FileDataModel> fileDatas { get; set; }
        }

        public class FileDataModel
        {
            public string fileName { get; set; }
            public string Md5Hash { get; set; }
        }
    }
}
