﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FileClient.ULF {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CustomFileInfo", Namespace="http://schemas.datacontract.org/2004/07/WebApplication1")]
    [System.SerializableAttribute()]
    public partial class CustomFileInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long LengthField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long OffSetField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] SendByteField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Length {
            get {
                return this.LengthField;
            }
            set {
                if ((this.LengthField.Equals(value) != true)) {
                    this.LengthField = value;
                    this.RaisePropertyChanged("Length");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long OffSet {
            get {
                return this.OffSetField;
            }
            set {
                if ((this.OffSetField.Equals(value) != true)) {
                    this.OffSetField = value;
                    this.RaisePropertyChanged("OffSet");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] SendByte {
            get {
                return this.SendByteField;
            }
            set {
                if ((object.ReferenceEquals(this.SendByteField, value) != true)) {
                    this.SendByteField = value;
                    this.RaisePropertyChanged("SendByte");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ULF.IServiceFile")]
    public interface IServiceFile {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceFile/UpLoadFileInfo", ReplyAction="http://tempuri.org/IServiceFile/UpLoadFileInfoResponse")]
        FileClient.ULF.CustomFileInfo UpLoadFileInfo(FileClient.ULF.CustomFileInfo fileInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceFile/GetFileInfo", ReplyAction="http://tempuri.org/IServiceFile/GetFileInfoResponse")]
        FileClient.ULF.CustomFileInfo GetFileInfo(string fileName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceFileChannel : FileClient.ULF.IServiceFile, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceFileClient : System.ServiceModel.ClientBase<FileClient.ULF.IServiceFile>, FileClient.ULF.IServiceFile {
        
        public ServiceFileClient() {
        }
        
        public ServiceFileClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceFileClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceFileClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceFileClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public FileClient.ULF.CustomFileInfo UpLoadFileInfo(FileClient.ULF.CustomFileInfo fileInfo) {
            return base.Channel.UpLoadFileInfo(fileInfo);
        }
        
        public FileClient.ULF.CustomFileInfo GetFileInfo(string fileName) {
            return base.Channel.GetFileInfo(fileName);
        }
    }
}
