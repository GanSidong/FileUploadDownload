using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileApp.FileService;

namespace FileApp
{
    public partial class Form1 : Form
    {
        FileServiceClient client;
        public Form1()
        {
            InitializeComponent();
            client = new FileServiceClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Fdialog = new OpenFileDialog();

            if (Fdialog.ShowDialog() == DialogResult.OK)
            {

                using (Stream fs = new FileStream(Fdialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    string message;
                    this.filepath.Text = Fdialog.SafeFileName;
                    bool result = client.UpLoadFile(Fdialog.SafeFileName, fs.Length, fs, out message);

                    if (result == true)
                    {
                        MessageBox.Show("上传成功！");
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = this.filename.Text;
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\client\";
            bool issuccess = false;
            string message = "";
            Stream filestream = new MemoryStream();
            long filesize = client.DownLoadFile(filename, out issuccess, out message, out filestream);

            if (issuccess)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                byte[] buffer = new byte[filesize];
                FileStream fs = new FileStream(path + filename, FileMode.Create, FileAccess.Write);
                int count = 0;
                while ((count = filestream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, count);
                }

                //清空缓冲区
                fs.Flush();
                //关闭流
                fs.Close();
                MessageBox.Show("下载成功！");

            }
            else
            {

                MessageBox.Show(message);

            }
        }
    }
}
