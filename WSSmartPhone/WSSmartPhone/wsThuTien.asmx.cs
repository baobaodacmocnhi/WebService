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

        //send notification
        [WebMethod]
        public string SendNotificationToClient(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            return _cThuTien.SendNotificationToClient(Title, Content, UID, Action, NameUpdate, ValueUpdate, ID);
        }

        //hành thu
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
        public string GetDSHoaDonTon_Dot_HoaDonDienTu(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetDSHoaDonTon_HoaDonDienTu(MaNV, Nam, Ky, FromDot, ToDot);
        }

        [WebMethod]
        public string XuLy_HoaDonDienTu(string LoaiXuLy, string MaNV, string MaHDs, string Ngay, string NgayHen)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = new DateTime(), date2 = new DateTime();
            if (Ngay != "")
                date = DateTime.ParseExact(Ngay, "dd/MM/yyyy HH:mm:ss", culture);
            if (NgayHen != "")
                date2 = DateTime.ParseExact(NgayHen, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.XuLy_HoaDonDienTu(LoaiXuLy, MaNV, MaHDs, date, date2);
        }

        [WebMethod]
        public string get_GhiChu(string DanhBo)
        {
            return _cThuTien.get_GhiChu(DanhBo);
        }

        [WebMethod]
        public bool update_GhiChu(string MaNV, string DanhBo, string DienThoai, string GiaBieu, string NiemChi, string DiemBe)
        {
            return _cThuTien.update_GhiChu(MaNV, DanhBo, DienThoai, GiaBieu, NiemChi, DiemBe);
        }

        //đóng nước
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
        public bool ThemDongNuoc(string MaDN, string DanhBo, string MLT, string HoTen, string DiaChi, string HinhDN, string NgayDN, string ChiSoDN, string ButChi, string KhoaTu, string NiemChi, string KhoaKhac, string KhoaKhac_GhiChu, string Hieu, string Co, string SoThan, string ChiMatSo, string ChiKhoaGoc, string ViTri, string LyDo, string CreateBy)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = DateTime.ParseExact(NgayDN, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.ThemDongNuoc(MaDN, DanhBo, MLT, HoTen, DiaChi, HinhDN, date, ChiSoDN, ButChi, KhoaTu, NiemChi, KhoaKhac, KhoaKhac_GhiChu, Hieu, Co, SoThan, ChiMatSo, ChiKhoaGoc, ViTri, LyDo, CreateBy);
        }

        [WebMethod]
        public bool SuaDongNuoc(string MaDN, string HinhDN, string NgayDN, string ChiSoDN, string ChiMatSo, string ChiKhoaGoc, string LyDo, string CreateBy)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = DateTime.ParseExact(NgayDN, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.SuaDongNuoc(MaDN, HinhDN, date, ChiSoDN, ChiMatSo, ChiKhoaGoc, LyDo, CreateBy);
        }

        [WebMethod]
        public bool ThemDongNuoc2(string MaDN, string HinhDN, string NgayDN, string ChiSoDN, string ButChi, string KhoaTu, string NiemChi, string KhoaKhac, string KhoaKhac_GhiChu, string CreateBy)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = DateTime.ParseExact(NgayDN, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.ThemDongNuoc2(MaDN, HinhDN, date, ChiSoDN, ButChi, KhoaTu, NiemChi, KhoaKhac, KhoaKhac_GhiChu, CreateBy);
        }

        [WebMethod]
        public bool CheckExist_MoNuoc(string MaDN)
        {
            return _cThuTien.CheckExist_MoNuoc(MaDN);
        }

        [WebMethod]
        public bool ThemMoNuoc(string MaDN, string HinhMN, string NgayMN, string ChiSoMN, string CreateBy)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = DateTime.ParseExact(NgayMN, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.ThemMoNuoc(MaDN, HinhMN, date, ChiSoMN, CreateBy);
        }

        [WebMethod]
        public bool SuaMoNuoc(string MaDN, string HinhMN, string NgayMN, string ChiSoMN, string CreateBy)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = DateTime.ParseExact(NgayMN, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.SuaMoNuoc(MaDN, HinhMN, date, ChiSoMN, CreateBy);
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

        //tìm kiếm
        [WebMethod]
        public string GetDSTimKiem(string DanhBo)
        {
            return _cThuTien.GetDSTimKiem(DanhBo);
        }

        [WebMethod]
        public string GetDSTimKiemTTKH(string HoTen, string SoNha, string TenDuong)
        {
            return _cThuTien.GetDSTimKiemTTKH(HoTen, SoNha, TenDuong);
        }

        //quản lý
        [WebMethod]
        public string GetTongGiaoHoaDon(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetTongGiaoHoaDon(MaTo, Nam, Ky, FromDot, ToDot);
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

        [WebMethod]
        public string GetTongDongMoNuoc_Tong(string DongNuoc, string MaTo, string FromNgayDN, string ToNgayDN)
        {
            return _cThuTien.GetTongDongMoNuoc_Tong(Boolean.Parse(DongNuoc), MaTo, DateTime.ParseExact(FromNgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string GetTongDongMoNuoc_ChiTiet(string DongNuoc, string MaTo, string FromNgayDN, string ToNgayDN)
        {
            return _cThuTien.GetTongDongMoNuoc_ChiTiet(Boolean.Parse(DongNuoc), MaTo, DateTime.ParseExact(FromNgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayDN, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string GetTongThuHo_Tong(string MaTo, string FromCreateDate, string ToCreateDate, string Loai)
        {
            return _cThuTien.GetTongThuHo_Tong(MaTo, DateTime.ParseExact(FromCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), Loai);
        }

        [WebMethod]
        public string GetTongThuHo_ChiTiet(string MaTo, string FromCreateDate, string ToCreateDate, string Loai)
        {
            return _cThuTien.GetTongThuHo_ChiTiet(MaTo, DateTime.ParseExact(FromCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), Loai);
        }

        //lệnh hủy
        [WebMethod]
        public string GetDSHoaDon_LenhHuy(string LoaiCat, string ID)
        {
            return _cThuTien.GetDSHoaDon_LenhHuy(LoaiCat, ID);
        }

        [WebMethod]
        public bool Sua_LenhHuy(string MaHDs, string Cat, string TinhTrang, string CreateBy)
        {
            return _cThuTien.Sua_LenhHuy(MaHDs, Cat, TinhTrang, CreateBy);
        }
    }
}
