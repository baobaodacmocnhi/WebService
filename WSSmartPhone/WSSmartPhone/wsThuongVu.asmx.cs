﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsThuongVu
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsThuongVu : System.Web.Services.WebService
    {
        CThuTien _cThuTien = new CThuTien();

        [WebMethod]
        public byte[] get_Hinh(string FolderLoai, string FolderIDCT, string FileName)
        {
            return _cThuTien.get_Hinh_241(CGlobalVariable.pathHinhTV, FolderLoai, FolderIDCT, FileName);
        }

        [WebMethod]
        public bool ghi_Hinh(string FolderLoai, string FolderIDCT, string FileName, byte[] HinhDHN)
        {
            return _cThuTien.ghi_Hinh_241(CGlobalVariable.pathHinhTV, FolderLoai, FolderIDCT, FileName, HinhDHN);
        }

        [WebMethod]
        public bool xoa_Hinh(string FolderLoai, string FolderIDCT, string FileName)
        {
            return _cThuTien.xoa_Hinh_241(CGlobalVariable.pathHinhTV, FolderLoai, FolderIDCT, FileName);
        }

        [WebMethod]
        public bool xoa_Folder_Hinh(string FolderLoai, string FolderIDCT)
        {
            return _cThuTien.xoa_Folder_241(CGlobalVariable.pathHinhTV, FolderLoai, FolderIDCT);
        }

        [WebMethod]
        public bool checkExists_DonTu(string DanhBo, string NoiDung, string SoNgay)
        {
            return _cThuTien.checkExists_DonTu(DanhBo, NoiDung, SoNgay);
        }

        [WebMethod]
        public string getAccess_token_CCCD()
        {
            return _cThuTien.getAccess_token_CCCD();
        }

        [WebMethod]
        public int checkExists_CCCD(string DanhBo, string CCCD, out string result)
        {
            return _cThuTien.checkExists_CCCD(DanhBo, CCCD, out result);
        }

        [WebMethod]
        public int them_CCCD(string DanhBo, string CCCD,out string result)
        {
            //string result = "";
            return _cThuTien.them_CCCD(DanhBo, CCCD, out result);
        }

        [WebMethod]
        public int sua_CCCD(string DanhBo, string CCCD, out string result)
        {
            return _cThuTien.sua_CCCD(DanhBo, CCCD, out result);
        }

        [WebMethod]
        public int xoa_CCCD(string DanhBo, string CCCD, out string result)
        {
            return _cThuTien.xoa_CCCD(DanhBo, CCCD, out result);
        }


    }
}
