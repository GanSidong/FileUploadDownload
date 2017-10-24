using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FileServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IServiceFile”。
    [ServiceContract]
    public interface IServiceFile
    {
        /// <summary>
        /// 上传操作
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        [OperationContract]
        CustomFileInfo UpLoadFileInfo(CustomFileInfo fileInfo);

        /// <summary>
        /// 获取文件操作
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [OperationContract]
        CustomFileInfo GetFileInfo(string fileName);
    }

    /// <summary>
    /// 自定义文件属性类
    /// </summary>
    [DataContract]
    public class CustomFileInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [DataMember]
        public long Length { get; set; }

        /// <summary>
        /// 文件偏移量
        /// </summary>
        [DataMember]
        public long OffSet { get; set; }

        /// <summary>
        /// 发送的字节
        /// </summary>
        [DataMember]
        public byte[] SendByte { get; set; }
    }
}
