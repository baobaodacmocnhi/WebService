using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsEContract
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsEContract : System.Web.Services.WebService
    {
        CEContract _cEContract = new CEContract();

        [WebMethod]
        public bool getAccess_token(string checksum)
        {
            return _cEContract.getAccess_token(checksum);
        }

        [WebMethod]
        public bool getAccess_token_Client(string checksum)
        {
            return _cEContract.getAccess_token_Client(checksum);
        }

        [WebMethod]
        public bool getAccess_token_User(string checksum)
        {
            return _cEContract.getAccess_token_User(checksum);
        }

        [WebMethod]
        public bool getAccess_token_Duyet(string checksum)
        {
            return _cEContract.getAccess_token_Duyet(checksum);
        }

        [WebMethod]
        public byte[] renderEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
            , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string NgayHieuLuc, string checksum, out string strResponse)
        {
            return _cEContract.renderEContract(HopDong, DanhBo, CreateDate, HoTen, CCCD, NgayCap, DCThuongTru, DCHienNay
            , DienThoai, Fax, Email, TaiKhoan, Bank, MST, CoDHN, DCLapDat, NgayHieuLuc, checksum, out strResponse);
        }

        [WebMethod]
        public bool createEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
            , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string NgayHieuLuc, bool GanMoi, bool CaNhan, string MaDon, string SHS, string checksum, out string strResponse)
        {
            return _cEContract.createEContract(HopDong, DanhBo, CreateDate, HoTen, CCCD, NgayCap, DCThuongTru, DCHienNay
             , DienThoai, Fax, Email, TaiKhoan, Bank, MST, CoDHN, DCLapDat, NgayHieuLuc, GanMoi, CaNhan, MaDon, SHS, checksum, out strResponse);
        }

        [WebMethod]
        public bool sendEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            return _cEContract.sendEContract(MaDon, SHS, checksum, out strResponse);
        }

        [WebMethod]
        public bool editEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            return _cEContract.editEContract(MaDon, SHS, checksum, out strResponse);
        }

        [WebMethod]
        public bool cancelEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            return _cEContract.cancelEContract(MaDon, SHS, checksum, out strResponse);
        }

        [WebMethod]
        public bool deleteEContract(string MaDon, string SHS, string checksum, out string strResponse)
        {
            return _cEContract.deleteEContract(MaDon, SHS, checksum, out strResponse);
        }

        [WebMethod]
        public bool updateDoiTac(string CCCD, string Email, string DienThoai, string HoTen, string MST, string checksum)
        {
            string strResponse = "";
            return _cEContract.updateDoiTac(CCCD, Email, DienThoai, HoTen, MST, checksum, out  strResponse);
        }

        [WebMethod]
        public bool duyetKhongKy(string IDEContract)
        {
            string strResponse = "";
            return _cEContract.duyetKhongKy(IDEContract, out  strResponse);
        }

    }
}
