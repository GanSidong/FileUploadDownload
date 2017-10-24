using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.ServiceModel.Configuration;
using System.Configuration;
using FileClient.ULF;

namespace FileClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.dgvFiles.AutoGenerateColumns = false;
            this.dgvFiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (Directory.Exists(Helper.ServerFolderPath) == false)
            {
                Directory.CreateDirectory(Helper.ServerFolderPath);
            }

            LoadUpLoadFile();
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadUpLoadFile()
        {

            //string address1 = Helper.ServerFilePath;
            //if (address1 == "")
            //{
            //    MessageBox.Show("请配置客户端终结点");
            //    return;
            //}
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Size", typeof(string)));
            dt.Columns.Add(new DataColumn("CreatedTime", typeof(DateTime)));


            string[] files = Directory.GetFiles(Helper.ServerFolderPath);
            foreach (string filePath in files)
            {
                FileInfo info = new FileInfo(filePath);
                if (info.Length > Int32.MaxValue)
                {
                    continue;
                }
                DataRow dr = dt.NewRow();
                dr[0] = info.Name;

                dr[1] = humanReadableByteCount(info.Length,false);
                dr[2] = info.CreationTime;
                dt.Rows.Add(dr);
            }
            if (dgvFiles.InvokeRequired)
            {
                dgvFiles.Invoke((ThreadStart)delegate
                {
                    this.dgvFiles.DataSource = dt;
                });
            }
            else
            {
                this.dgvFiles.DataSource = dt;
            }
        }

        /// <summary>
        /// upLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;
            if (this.txtFileName.Text == "")
            {
                MessageBox.Show("请选择文件路径");
                btnUpload.Enabled = true;
                return;
            }

            string filePath = this.txtFileName.Text;
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(filePath);
        }
        #region 多线程
        void upLoad(object parame)
        {
            try
            {
                string path = parame.ToString();
                int maxSiz = 1024 * 100;

                System.IO.FileInfo fileInfoIO = new System.IO.FileInfo(path);
                // 要上传的文件地址
                FileStream fs = File.OpenRead(fileInfoIO.FullName);
                // 实例化服务客户的
                ServiceFileClient client = new ServiceFileClient();

                // 根据文件名获取服务器上的文件

                CustomFileInfo file = client.GetFileInfo(fileInfoIO.Name);

                if (file == null)
                {
                    file = new CustomFileInfo();
                    file.OffSet = 0;
                }
                file.Name = fileInfoIO.Name;
                file.Length = fs.Length;

                if (file.Length == file.OffSet) //如果文件的长度等于文件的偏移量，说明文件已经上传完成
                {
                    MessageBox.Show("该文件已存在");
                }
                else
                {
                    //MessageBox.Show("1");

                    while (file.Length != file.OffSet)
                    {
                        file.SendByte = new byte[file.Length - file.OffSet <= maxSiz ? file.Length - file.OffSet : maxSiz]; //设置传递的数据的大小

                        fs.Position = file.OffSet; //设置本地文件数据的读取位置
                        fs.Read(file.SendByte, 0, file.SendByte.Length);//把数据写入到file.Data中
                        // MessageBox.Show("2");
                        file = client.UpLoadFileInfo(file); //上传
                        //int percent = (int)((double)file.OffSet / (double)((long)file.Length)) * 100;
                        int percent = (int)(((double)file.OffSet / (double)((long)file.Length)) * 100);

                        if (progressBar.InvokeRequired)
                        {
                            // MessageBox.Show("3");
                            progressBar.Invoke((ThreadStart)delegate
                            {
                                progressBar.Value = percent;
                                progressBar.Update();
                            });
                        }
                        else
                        {
                            //MessageBox.Show("4");
                            progressBar.Value = percent;
                            progressBar.Update();
                        }


                    }
                    // 移动文件到临时目录（此部分创建可以使用sqlserver数据库代替）
                    string address = string.Format(@"{0}\{1}", Helper.ServerFolderPath, file.Name);

                    fileInfoIO.CopyTo(address, true);
                    LoadUpLoadFile();
                    MessageBox.Show("success");

                }
                fs.Close();
                client.Close();
                //if (this.btnUpload.InvokeRequired)
                //{
                //    this.btnUpload.Invoke((ThreadStart)delegate
                //    {
                //        this.btnUpload.Enabled = true;
                //    });
                //}
                //else
                //{
                this.btnUpload.Enabled = true;
                this.btnUpload.Update();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion


        #region 独立线程
        /// <summary>
        /// 进度条更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((ThreadStart)delegate
                {
                    progressBar.Value = e.ProgressPercentage;
                });
            }
            else
            {
                progressBar.Value = e.ProgressPercentage;
                progressBar.Update();
            }
        }
        /// <summary>
        /// 执行操作完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result == false)
            {

            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((ThreadStart)delegate
                    {
                        MessageBox.Show("上传成功");
                    });
                }
                else
                {
                    MessageBox.Show("上传成功");
                }
            }

            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((ThreadStart)delegate
                {
                    progressBar.Value = 0;
                });
            }
            else
            {
                progressBar.Value = 0;
            }

            if (txtFileName.InvokeRequired)
            {
                txtFileName.Invoke((ThreadStart)delegate
                {
                    this.txtFileName.Clear();
                });
            }
            else
            {
                this.txtFileName.Clear();
            }
            if (this.btnUpload.InvokeRequired)
            {
                this.btnUpload.Invoke((ThreadStart)delegate
                {
                    this.btnUpload.Enabled = true;
                });
            }
            else
            {
                this.btnUpload.Enabled = true;
            }

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = e.Argument.ToString();
            System.IO.FileInfo fileInfoIO = new System.IO.FileInfo(path);
            // 要上传的文件地址
            FileStream fs = File.OpenRead(fileInfoIO.FullName);
            // 实例化服务客户的
            ServiceFileClient client = new ServiceFileClient();
            try
            {
                int maxSiz = 1024 * 100;
                // 根据文件名获取服务器上的文件
                CustomFileInfo file = client.GetFileInfo(fileInfoIO.Name);
                if (file == null)
                {
                    file = new CustomFileInfo();
                    file.OffSet = 0;
                }
                file.Name = fileInfoIO.Name;
                file.Length = fs.Length;
                if (file.Length > Int32.MaxValue)
                {
                    MessageBox.Show("上传文件大小不能超过2G");

                }
                else
                {
                    if (file.Length == file.OffSet) //如果文件的长度等于文件的偏移量，说明文件已经上传完成
                    {
                        MessageBox.Show("该文件已存在");
                        e.Result = false;   // 设置异步操作结果为false
                    }
                    else
                    {
                        while (file.Length != file.OffSet)
                        {
                            file.SendByte = new byte[file.Length - file.OffSet <= maxSiz ? file.Length - file.OffSet : maxSiz]; //设置传递的数据的大小

                            fs.Position = file.OffSet; //设置本地文件数据的读取位置
                            fs.Read(file.SendByte, 0, file.SendByte.Length);//把数据写入到file.Data中
                            file = client.UpLoadFileInfo(file); //上传
                            //int percent = (int)((double)file.OffSet / (double)((long)file.Length)) * 100;
                            int percent = (int)(((double)file.OffSet / (double)((long)file.Length)) * 100);
                            (sender as BackgroundWorker).ReportProgress(percent);
                        }
                        // 移动文件到临时目录（此部分创建可以使用sqlserver数据库代替）
                        string address = string.Format(@"{0}\{1}", Helper.ServerFolderPath, file.Name);
                        fileInfoIO.CopyTo(address, true);
                        LoadUpLoadFile();
                        e.Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                fs.Close();
                fs.Dispose();
                client.Close();
                client.Abort();

            }
        }

        /// <summary>
        /// 选择文件对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = "C:";
            //dialog.Multiselect = true;
            //dialog.Filter = "";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            this.txtFileName.Text = dialog.FileName;
        }
        #endregion
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmDownLoad_Click(object sender, EventArgs e)
        {

            if (this.dgvFiles.SelectedRows == null || this.dgvFiles.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择文件");
                return;
            }

            //worker = new BackgroundWorker();
            //worker.WorkerReportsProgress = true;
            //worker.ProgressChanged += downLoadWorker_ProgressChanged;
            //worker.DoWork += downLoadWorker_DoWork;
            //worker.RunWorkerCompleted += downLoadWorker_RunWorkerCompleted;
            //worker.RunWorkerAsync();

            Thread downLoadThread = new Thread(new ThreadStart(DownLoadFile));
            downLoadThread.IsBackground = true;
            downLoadThread.SetApartmentState(ApartmentState.STA);
            downLoadThread.Name = "downLoadThreade";
            downLoadThread.Start();
        }
        #region 下载独立线程
        void downLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("下载成功");
            if (pbDownLoad.InvokeRequired)
            {
                pbDownLoad.Invoke((ThreadStart)delegate
                {
                    pbDownLoad.Value = 0;
                });
            }
        }

        void downLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            DownLoadFile();
        }



        void downLoadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbDownLoad.Value = e.ProgressPercentage;
        }
        #endregion

        #region 下载多线程
        private void DownLoadFile()
        {

            // 测试使用
            //string fileFullpath = "http://localhost:29700//UpLoadFile";
            string fileFullpath = Helper.ServerFilePath;
            // 获得DataGridView选中行
            DataGridViewRow selectedRow = this.dgvFiles.SelectedRows[0];
            string fileName = selectedRow.Cells[0].Value.ToString(); // 文件名称

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = "C:";
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileFullpath += string.Format("//{0}", fileName);
                if (txtDownLoadPath.InvokeRequired)
                {
                    txtDownLoadPath.Invoke((ThreadStart)delegate
                    {
                        this.txtDownLoadPath.Text = sfd.FileName;
                    });
                }

                WebRequest request = WebRequest.Create(fileFullpath);
                WebResponse fs = null;
                try
                {
                    fs = request.GetResponse();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "/r/n请确保文件名不存在特殊字符");
                }
                long contentLength = fs.ContentLength;
                if (pbDownLoad.InvokeRequired)
                {
                    pbDownLoad.Invoke((ThreadStart)delegate
                    {
                        pbDownLoad.Maximum = (int)contentLength;
                    });
                }
                Stream st = fs.GetResponseStream();
                try
                {
                    byte[] byteLength = new byte[contentLength];
                    int allByte = (int)contentLength;
                    int startByte = 0;
                    while (contentLength > 0)
                    {
                        int downByte = st.Read(byteLength, startByte, allByte);
                        if (downByte == 0)
                        {
                            break;
                        }
                        startByte += downByte;
                        // 计算下载进度
                        //int percent = (int)(((double)startByte / ((long)(double)allByte)) * 100);
                        //(sender as BackgroundWorker).ReportProgress(percent);
                        if (pbDownLoad.InvokeRequired)
                        {
                            pbDownLoad.Invoke((ThreadStart)delegate
                            {
                                pbDownLoad.Value = startByte;
                            });
                        }
                        allByte -= downByte;
                    }
                    // 保存路径
                    string downLoadPath = sfd.FileName;
                    FileStream stream = new FileStream(downLoadPath, FileMode.OpenOrCreate, FileAccess.Write);
                    stream.Write(byteLength, 0, byteLength.Length);
                    stream.Close();
                    fs.Close();
                    Thread.Sleep(500);
                    MessageBox.Show("下载成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    st.Close();
                    st.Dispose();
                    request.Abort();

                    if (pbDownLoad.InvokeRequired)
                    {
                        pbDownLoad.Invoke((ThreadStart)delegate
                        {
                            pbDownLoad.Maximum = 0;
                        });
                    }
                }
            }
        #endregion
        }

        private void dgvFiles_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            //显示在HeaderCell上
            for (int i = 0; i < this.dgvFiles.Rows.Count; i++)
            {
                DataGridViewRow r = this.dgvFiles.Rows[i];
                r.HeaderCell.Value = string.Format("{0}", i + 1);
            }
            this.dgvFiles.Refresh();
        }

        public static String humanReadableByteCount(long bytes, bool si)
        {
            int unit = si ? 1000 : 1024;
            if (bytes < unit) return bytes + " B";
            int exp = (int)(Math.Log(bytes) / Math.Log(unit));
            string pre = (si ? "kMGTPE" : "KMGTPE").CharAt(exp - 1);
            return string.Format("{0}{1}B", Math.Round(bytes / Math.Pow(unit, exp), 1), pre);
        }
    }
     public static class CharAtExtention{
         public static string CharAt(this string s,int index){
             if((index >= s.Length)||(index<0))
                 return "";
            return s.Substring(index,1);
        }
    }
}
