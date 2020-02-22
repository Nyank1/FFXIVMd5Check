using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;
using log4net.Config;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FFXIV_MD5_CHECK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly log4net.ILog logger = LogManager.GetLogger("MainWindow");
        public static string gameVersion;

        bool ffxiv = false;
        bool ex1 = false;
        bool ex2 = false;
        bool ex3 = false;


        public MainWindow()
        {
            this.Icon = null;
            InitializeComponent();
            logger.Info("Window Initialized");
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "FF14版本文件 (ffxivgame.ver)|ffxivgame.ver|FF14可执行文件 (ffxiv.exe)|ffxiv.exe|FF14_DX11可执行文件(ffxiv.exe)|ffxiv_dx11.exe";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.ShowHelp = true;
                dialog.Title = "请选择\"你的安装目录/最终幻想XIV/game/ffxivgame.ver\"或ffxiv.exe";
                void _(Object _sender, EventArgs _e)
                {
                    MessageBox.Show("请选择\"你的安装目录/最终幻想XIV/game/ffxivgame.ver\"或ffxiv.exe");
                }
                dialog.HelpRequest += new EventHandler(_);

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    logger.Info($"Game file path selected: {dialog.FileName}");
                    pathSelect.Text = dialog.FileName;
                    string GamePath = System.IO.Directory.GetParent(pathSelect.Text).ToString();
                    var versionFile = new System.IO.StreamReader(GamePath + @"/ffxivgame.ver");
                    gameVersion = versionFile.ReadToEnd();
                    if (gameVersion.Length == 20) txtVersion.Text = gameVersion;
                    else
                    {
                        txtVersion.Text = "ERROR";
                        logger.Error($"Incorrect game version file detected");
                    }
                    versionFile.Dispose();
                }
                dialog.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                logger.Error(error.Message);
                this.Close();
            }
        }

        private void SimpleCheck(string GamePath)
        {
            RecordDataModel data = new RecordDataModel();
            data.gameVersion = gameVersion;
            data.folderDatas = new List<RecordDataModel.FolderDataModel>();


            if (ffxiv)
            {
                logger.Info("Starting check ffxiv");
                string path = GamePath + "\\sqpack\\ffxiv\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }
            if (ex1)
            {
                logger.Info("Starting check ex1");
                string path = GamePath + "\\sqpack\\ex1\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }
            if (ex2)
            {
                logger.Info("Starting check ex2");
                string path = GamePath + "\\sqpack\\ex2\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }
            if (ex3)
            {
                logger.Info("Starting check ex3");
                string path = GamePath + "\\sqpack\\ex3\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }

            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\result.json";
            logger.Info($"save file to {filePath}");
            using (System.IO.StreamWriter file = System.IO.File.CreateText(filePath))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(file, data);
            }
        }

        private void SimpleCheckCallBack(IAsyncResult asyncResult)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                btnSimpleCheck.Click -= btnSimpleCheck_Click;
                pb.IsIndeterminate = false;
                btnSimpleCheck.Content = "检查完毕";
            }), System.Windows.Threading.DispatcherPriority.Normal);

        }

        private void SimpleCheck_Old()
        {
            RecordDataModel data = new RecordDataModel();
            data.gameVersion = gameVersion;
            data.folderDatas = new List<RecordDataModel.FolderDataModel>();

            if (pathSelect.Text.Length == 0) return;
            string GamePath = System.IO.Directory.GetParent(pathSelect.Text).ToString();

            if (checkBoxaFFXIV.IsInitialized && (bool)checkBoxaFFXIV.IsChecked)
            {
                string path = GamePath + "\\sqpack\\ffxiv\\";
                //data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));

            }
            if (checkBoxaEX1.IsInitialized && (bool)checkBoxaEX1.IsChecked)
            {
                string path = GamePath + "\\sqpack\\ex1\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }
            if (checkBoxaEX2.IsInitialized && (bool)checkBoxaEX2.IsChecked)
            {
                string path = GamePath + "\\sqpack\\ex2\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }
            if (checkBoxaEX3.IsInitialized && (bool)checkBoxaEX3.IsChecked)
            {
                string path = GamePath + "\\sqpack\\ex3\\";
                data.folderDatas.Add(FileMd5Hash.GetFolderMd5Hash(path));
            }
            MessageBox.Show("MD5计算完毕");

            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\result.json";
            using (System.IO.StreamWriter file = System.IO.File.CreateText(filePath))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Serialize(file, data);
            }

        }

        private void btnSimpleCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (checkBoxaFFXIV.IsInitialized && (bool)checkBoxaFFXIV.IsChecked)
                {
                    ffxiv = true;
                }
                if (checkBoxaEX1.IsInitialized && (bool)checkBoxaEX1.IsChecked)
                {
                    ex1 = true;
                }
                if (checkBoxaEX2.IsInitialized && (bool)checkBoxaEX2.IsChecked)
                {
                    ex2 = true;
                }
                if (checkBoxaEX3.IsInitialized && (bool)checkBoxaEX3.IsChecked)
                {
                    ex3 = true;
                }

                btnSimpleCheck.Click -= btnSimpleCheck_Click;
                btnSimpleCheck.Content = "请等待";
                pb.IsIndeterminate = true;
                string GamePath = System.IO.Directory.GetParent(pathSelect.Text).ToString();
                Action<string> action = SimpleCheck;
                action.BeginInvoke(GamePath, SimpleCheckCallBack, null);

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message + "\n\n" + "请重试！");
                logger.Error(error.Message);
            }
        }
    }
}