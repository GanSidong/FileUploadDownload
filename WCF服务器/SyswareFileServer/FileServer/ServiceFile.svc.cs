using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FileServer
{
    public class ServiceFile : IServiceFile
    {
        public CustomFileInfo UpLoadFileInfo(CustomFileInfo fileInfo)
        {
            // 获取服务器文件上传路径
            string fileUpLoadPath = System.Web.Hosting.HostingEnvironment.MapPath("~/UpLoadFile/");
            // 如需指定新的文件夹，需要进行创建操作。
            // 创建FileStream对象
            FileStream fs = new FileStream(fileUpLoadPath + fileInfo.Name, FileMode.OpenOrCreate);

            long offSet = fileInfo.OffSet;
            // 使用提供的流创建BinaryWriter对象
            var binaryWriter = new BinaryWriter(fs, Encoding.UTF8);

            binaryWriter.Seek((int)offSet, SeekOrigin.Begin);
            binaryWriter.Write(fileInfo.SendByte);
            fileInfo.OffSet = fs.Length;
            fileInfo.SendByte = null;

            binaryWriter.Close();
            fs.Close();
            return fileInfo;
        }

        public CustomFileInfo GetFileInfo(string fileName)
        {
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/UpLoadFile/") + fileName;
            if (File.Exists(filePath))
            {
                var fs = new FileStream(filePath, FileMode.OpenOrCreate);
                CustomFileInfo fileInfo = new CustomFileInfo
                {
                    Name = fileName,
                    OffSet = fs.Length,
                };
                fs.Close();
                return fileInfo;
            }
            return null;
        }
    }
}
