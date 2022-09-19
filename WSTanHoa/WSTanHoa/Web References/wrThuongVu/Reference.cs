﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace WSTanHoa.wrThuongVu {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="wsThuongVuSoap", Namespace="http://tempuri.org/")]
    public partial class wsThuongVu : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback get_HinhOperationCompleted;
        
        private System.Threading.SendOrPostCallback ghi_HinhOperationCompleted;
        
        private System.Threading.SendOrPostCallback xoa_HinhOperationCompleted;
        
        private System.Threading.SendOrPostCallback xoa_Folder_HinhOperationCompleted;
        
        private System.Threading.SendOrPostCallback getAccess_token_CCCDOperationCompleted;
        
        private System.Threading.SendOrPostCallback checkExists_CCCDOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public wsThuongVu() {
            this.Url = global::WSTanHoa.Properties.Settings.Default.WSTanHoa_wrThuongVu_wsThuongVu;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event get_HinhCompletedEventHandler get_HinhCompleted;
        
        /// <remarks/>
        public event ghi_HinhCompletedEventHandler ghi_HinhCompleted;
        
        /// <remarks/>
        public event xoa_HinhCompletedEventHandler xoa_HinhCompleted;
        
        /// <remarks/>
        public event xoa_Folder_HinhCompletedEventHandler xoa_Folder_HinhCompleted;
        
        /// <remarks/>
        public event getAccess_token_CCCDCompletedEventHandler getAccess_token_CCCDCompleted;
        
        /// <remarks/>
        public event checkExists_CCCDCompletedEventHandler checkExists_CCCDCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/get_Hinh", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] get_Hinh(string FolderLoai, string FolderIDCT, string FileName) {
            object[] results = this.Invoke("get_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT,
                        FileName});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void get_HinhAsync(string FolderLoai, string FolderIDCT, string FileName) {
            this.get_HinhAsync(FolderLoai, FolderIDCT, FileName, null);
        }
        
        /// <remarks/>
        public void get_HinhAsync(string FolderLoai, string FolderIDCT, string FileName, object userState) {
            if ((this.get_HinhOperationCompleted == null)) {
                this.get_HinhOperationCompleted = new System.Threading.SendOrPostCallback(this.Onget_HinhOperationCompleted);
            }
            this.InvokeAsync("get_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT,
                        FileName}, this.get_HinhOperationCompleted, userState);
        }
        
        private void Onget_HinhOperationCompleted(object arg) {
            if ((this.get_HinhCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.get_HinhCompleted(this, new get_HinhCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ghi_Hinh", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool ghi_Hinh(string FolderLoai, string FolderIDCT, string FileName, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] HinhDHN) {
            object[] results = this.Invoke("ghi_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT,
                        FileName,
                        HinhDHN});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void ghi_HinhAsync(string FolderLoai, string FolderIDCT, string FileName, byte[] HinhDHN) {
            this.ghi_HinhAsync(FolderLoai, FolderIDCT, FileName, HinhDHN, null);
        }
        
        /// <remarks/>
        public void ghi_HinhAsync(string FolderLoai, string FolderIDCT, string FileName, byte[] HinhDHN, object userState) {
            if ((this.ghi_HinhOperationCompleted == null)) {
                this.ghi_HinhOperationCompleted = new System.Threading.SendOrPostCallback(this.Onghi_HinhOperationCompleted);
            }
            this.InvokeAsync("ghi_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT,
                        FileName,
                        HinhDHN}, this.ghi_HinhOperationCompleted, userState);
        }
        
        private void Onghi_HinhOperationCompleted(object arg) {
            if ((this.ghi_HinhCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ghi_HinhCompleted(this, new ghi_HinhCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/xoa_Hinh", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool xoa_Hinh(string FolderLoai, string FolderIDCT, string FileName) {
            object[] results = this.Invoke("xoa_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT,
                        FileName});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void xoa_HinhAsync(string FolderLoai, string FolderIDCT, string FileName) {
            this.xoa_HinhAsync(FolderLoai, FolderIDCT, FileName, null);
        }
        
        /// <remarks/>
        public void xoa_HinhAsync(string FolderLoai, string FolderIDCT, string FileName, object userState) {
            if ((this.xoa_HinhOperationCompleted == null)) {
                this.xoa_HinhOperationCompleted = new System.Threading.SendOrPostCallback(this.Onxoa_HinhOperationCompleted);
            }
            this.InvokeAsync("xoa_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT,
                        FileName}, this.xoa_HinhOperationCompleted, userState);
        }
        
        private void Onxoa_HinhOperationCompleted(object arg) {
            if ((this.xoa_HinhCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.xoa_HinhCompleted(this, new xoa_HinhCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/xoa_Folder_Hinh", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool xoa_Folder_Hinh(string FolderLoai, string FolderIDCT) {
            object[] results = this.Invoke("xoa_Folder_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void xoa_Folder_HinhAsync(string FolderLoai, string FolderIDCT) {
            this.xoa_Folder_HinhAsync(FolderLoai, FolderIDCT, null);
        }
        
        /// <remarks/>
        public void xoa_Folder_HinhAsync(string FolderLoai, string FolderIDCT, object userState) {
            if ((this.xoa_Folder_HinhOperationCompleted == null)) {
                this.xoa_Folder_HinhOperationCompleted = new System.Threading.SendOrPostCallback(this.Onxoa_Folder_HinhOperationCompleted);
            }
            this.InvokeAsync("xoa_Folder_Hinh", new object[] {
                        FolderLoai,
                        FolderIDCT}, this.xoa_Folder_HinhOperationCompleted, userState);
        }
        
        private void Onxoa_Folder_HinhOperationCompleted(object arg) {
            if ((this.xoa_Folder_HinhCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.xoa_Folder_HinhCompleted(this, new xoa_Folder_HinhCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/getAccess_token_CCCD", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getAccess_token_CCCD() {
            object[] results = this.Invoke("getAccess_token_CCCD", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getAccess_token_CCCDAsync() {
            this.getAccess_token_CCCDAsync(null);
        }
        
        /// <remarks/>
        public void getAccess_token_CCCDAsync(object userState) {
            if ((this.getAccess_token_CCCDOperationCompleted == null)) {
                this.getAccess_token_CCCDOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetAccess_token_CCCDOperationCompleted);
            }
            this.InvokeAsync("getAccess_token_CCCD", new object[0], this.getAccess_token_CCCDOperationCompleted, userState);
        }
        
        private void OngetAccess_token_CCCDOperationCompleted(object arg) {
            if ((this.getAccess_token_CCCDCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getAccess_token_CCCDCompleted(this, new getAccess_token_CCCDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/checkExists_CCCD", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int checkExists_CCCD(string DanhBo, string CCCD, string CMND) {
            object[] results = this.Invoke("checkExists_CCCD", new object[] {
                        DanhBo,
                        CCCD,
                        CMND});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void checkExists_CCCDAsync(string DanhBo, string CCCD, string CMND) {
            this.checkExists_CCCDAsync(DanhBo, CCCD, CMND, null);
        }
        
        /// <remarks/>
        public void checkExists_CCCDAsync(string DanhBo, string CCCD, string CMND, object userState) {
            if ((this.checkExists_CCCDOperationCompleted == null)) {
                this.checkExists_CCCDOperationCompleted = new System.Threading.SendOrPostCallback(this.OncheckExists_CCCDOperationCompleted);
            }
            this.InvokeAsync("checkExists_CCCD", new object[] {
                        DanhBo,
                        CCCD,
                        CMND}, this.checkExists_CCCDOperationCompleted, userState);
        }
        
        private void OncheckExists_CCCDOperationCompleted(object arg) {
            if ((this.checkExists_CCCDCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.checkExists_CCCDCompleted(this, new checkExists_CCCDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void get_HinhCompletedEventHandler(object sender, get_HinhCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class get_HinhCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal get_HinhCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void ghi_HinhCompletedEventHandler(object sender, ghi_HinhCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ghi_HinhCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ghi_HinhCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void xoa_HinhCompletedEventHandler(object sender, xoa_HinhCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class xoa_HinhCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal xoa_HinhCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void xoa_Folder_HinhCompletedEventHandler(object sender, xoa_Folder_HinhCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class xoa_Folder_HinhCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal xoa_Folder_HinhCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void getAccess_token_CCCDCompletedEventHandler(object sender, getAccess_token_CCCDCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getAccess_token_CCCDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getAccess_token_CCCDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void checkExists_CCCDCompletedEventHandler(object sender, checkExists_CCCDCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class checkExists_CCCDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal checkExists_CCCDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591