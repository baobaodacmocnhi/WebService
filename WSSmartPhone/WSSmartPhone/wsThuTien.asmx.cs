using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Globalization;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsThuTien
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsThuTien : System.Web.Services.WebService
    {
        CThuTien _cThuTien = new CThuTien();

        [WebMethod]
        public bool UpdateUID(string MaNV, string UID)
        {
            return _cThuTien.UpdateUID(MaNV, UID);
        }

        [WebMethod]
        public string DangNhap(string Username, string Password, string UID)
        {
            return _cThuTien.DangNhap(Username, Password, UID);
        }

        [WebMethod]
        public string DangNhaps(string Username, string Password, string UID)
        {
            return _cThuTien.DangNhaps(Username, Password, UID);
        }

        [WebMethod]
        public bool DangXuat(string Username)
        {
            return _cThuTien.DangXuat(Username);
        }

        [WebMethod]
        public bool DangXuats(string Username, string UID)
        {
            return _cThuTien.DangXuats(Username, UID);
        }

        [WebMethod]
        public string GetVersion()
        {
            return _cThuTien.GetVersion();
        }

        [WebMethod]
        public string GetDSTo()
        {
            return _cThuTien.GetDSTo();
        }

        [WebMethod]
        public string GetDSNhanVienDoi()
        {
            return _cThuTien.GetDSNhanVien();
        }

        [WebMethod]
        public string GetDSNhanVienTo(string MaTo)
        {
            return _cThuTien.GetDSNhanVien(MaTo);
        }

        [WebMethod]
        public string GetDSHoaDonTon(string MaNV, string NgayDi)
        {
            return _cThuTien.GetDSHoaDonTon(MaNV, DateTime.ParseExact(NgayDi, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string GetDSHoaDonTon_Dot(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetDSHoaDonTon(MaNV, Nam, Ky, FromDot, ToDot);
        }

        [WebMethod]
        public string SendNotificationToClient(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            return _cThuTien.SendNotificationToClient(Title, Content, UID, Action, NameUpdate, ValueUpdate, ID);
        }

        [WebMethod]
        public string GetDSDongNuoc(string MaNV_DongNuoc, string FromNgayGiao, string ToNgayGiao)
        {
            return _cThuTien.GetDSDongNuoc(MaNV_DongNuoc, DateTime.ParseExact(FromNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string GetDSCTDongNuoc(string MaNV_DongNuoc, string FromNgayGiao, string ToNgayGiao)
        {
            return _cThuTien.GetDSCTDongNuoc(MaNV_DongNuoc, DateTime.ParseExact(FromNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public bool CheckExist_DongNuoc(string MaDN)
        {
            return _cThuTien.CheckExist_DongNuoc(MaDN);
        }

        [WebMethod]
        public bool CheckExist_DongNuoc2(string MaDN)
        {
            return _cThuTien.CheckExist_DongNuoc2(MaDN);
        }

        [WebMethod]
        public bool ThemDongNuoc(string MaDN, string DanhBo, string MLT, string HoTen, string DiaChi, string HinhDN, string NgayDN, string ChiSoDN, string Hieu, string Co, string SoThan, string ChiMatSo, string ChiKhoaGoc, string LyDo, string CreateBy)
        {
            return _cThuTien.ThemDongNuoc(MaDN, DanhBo, MLT, HoTen, DiaChi, HinhDN, DateTime.ParseExact(NgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture), ChiSoDN, Hieu, Co, SoThan, ChiMatSo, ChiKhoaGoc, LyDo, CreateBy);
        }

        [WebMethod]
        public bool SuaDongNuoc(string MaDN, string HinhDN, string NgayDN, string ChiSoDN, string ChiMatSo, string ChiKhoaGoc, string LyDo, string CreateBy)
        {
            return _cThuTien.SuaDongNuoc(MaDN, HinhDN, DateTime.ParseExact(NgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture), ChiSoDN, ChiMatSo, ChiKhoaGoc, LyDo, CreateBy);
        }

        [WebMethod]
        public bool ThemDongNuoc2(string MaDN, string HinhDN, string NgayDN, string ChiSoDN, string CreateBy)
        {
            return _cThuTien.ThemDongNuoc2(MaDN, HinhDN, DateTime.ParseExact(NgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture), ChiSoDN, CreateBy);
        }

        [WebMethod]
        public bool CheckExist_MoNuoc(string MaDN)
        {
            return _cThuTien.CheckExist_MoNuoc(MaDN);
        }

        [WebMethod]
        public bool ThemMoNuoc(string MaDN, string HinhMN, string NgayMN, string ChiSoMN, string CreateBy)
        {
            return _cThuTien.ThemMoNuoc(MaDN, HinhMN, DateTime.ParseExact(NgayMN, "dd/MM/yyyy", CultureInfo.InvariantCulture), ChiSoMN, CreateBy);
        }

        [WebMethod]
        public bool SuaMoNuoc(string MaDN, string HinhMN, string NgayMN, string ChiSoMN, string CreateBy)
        {
            return _cThuTien.SuaMoNuoc(MaDN, HinhMN, DateTime.ParseExact(NgayMN, "dd/MM/yyyy", CultureInfo.InvariantCulture), ChiSoMN, CreateBy);
        }

        [WebMethod]
        public bool DangNganDongNuoc(string MaNV, string MaHDs)
        {
            return _cThuTien.DangNganDongNuoc(MaNV, MaHDs);
        }

        [WebMethod]
        public string GetDSHoaDonTon_DongNuoc(string DanhBo, string MaHDs)
        {
            return _cThuTien.GetDSHoaDonTon_DongNuoc(DanhBo, MaHDs);
        }

        [WebMethod]
        public string GetDSHoaDon_DanhBo(string DanhBo)
        {
            return _cThuTien.GetDSHoaDon(DanhBo);
        }

        [WebMethod]
        public string GetDSHoaDon_TTKH(string HoTen, string SoNha, string TenDuong)
        {
            return _cThuTien.GetDSHoaDon(HoTen, SoNha, TenDuong);
        }

        [WebMethod]
        public string GetTongGiaoHoaDon(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetTongGiaoHoaDon( MaTo,  Nam,  Ky,  FromDot,  ToDot);
        }

        [WebMethod]
        public string GetTongDangNgan(string MaTo, string FromNgayGiaiTrach, string ToNgayGiaiTrach)
        {
            return _cThuTien.GetTongDangNgan(MaTo, DateTime.ParseExact(FromNgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string GetTongTon(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetTongTon(MaTo, Nam, Ky, FromDot, ToDot);
        }
    }
}
