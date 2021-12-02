﻿using System;
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
        CKinhDoanh _cKinhDoanh = new CKinhDoanh();

        [WebMethod]
        public bool UpdateUID(string MaNV, string UID)
        {
            return _cThuTien.UpdateUID(MaNV, UID);
        }

        [WebMethod]
        public string DangNhaps(string Username, string Password, string IDMobile, string UID)
        {
            return _cThuTien.DangNhaps(Username, Password, IDMobile, UID);
        }

        [WebMethod]
        public string DangNhaps_Admin(string Username, string Password, string IDMobile, string UID)
        {
            return _cThuTien.DangNhaps_Admin(Username, Password, IDMobile, UID);
        }

        [WebMethod]
        public string DangXuats(string Username, string UID)
        {
            return _cThuTien.DangXuats(Username, UID);
        }

        [WebMethod]
        public string DangXuats_Person(string Username, string UID)
        {
            return _cThuTien.DangXuats_Person(Username, UID);
        }

        [WebMethod]
        public string DangXuats_Admin(string Username, string UID)
        {
            return _cThuTien.DangXuats_Admin(Username, UID);
        }

        [WebMethod]
        public bool updateLogin(string MaNV, string UID)
        {
            return _cThuTien.updateLogin(MaNV, UID);
        }

        [WebMethod]
        public string GetVersion()
        {
            return _cThuTien.GetVersion();
        }

        [WebMethod]
        public string GetDSTo()
        {
            return _cThuTien.getDS_To();
        }

        [WebMethod]
        public string getDS_NhanVien_HanhThu()
        {
            return _cThuTien.getDS_NhanVien_HanhThu();
        }

        [WebMethod]
        public string GetDSNhanVienTo(string MaTo)
        {
            return _cThuTien.getDS_NhanVien(MaTo);
        }

        [WebMethod]
        public string getDS_NhanVien()
        {
            return _cThuTien.getDS_NhanVien();
        }

        //send notification
        [WebMethod]
        public string SendNotificationToClient(string Title, string Content, string UID, string Action, string NameUpdate, string ValueUpdate, string ID)
        {
            return _cThuTien.SendNotificationToClient(Title, Content, UID, Action, NameUpdate, ValueUpdate, ID);
        }

        //hành thu
        [WebMethod]
        public string getDSHoaDonTon_NhanVien(string MaNV, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.getDSHoaDonTon_NhanVien(MaNV, Nam, Ky, FromDot, ToDot);
        }

        [WebMethod]
        public string getDSHoaDonTon_May(string MaNV, string Nam, string Ky, string FromDot, string ToDot, string TuMay, string DenMay)
        {
            return _cThuTien.getDSHoaDonTon_May(MaNV, Nam, Ky, FromDot, ToDot, TuMay, DenMay);
        }

        [WebMethod]
        public string XuLy_HoaDonDienTu(string LoaiXuLy, string MaNV, string MaHDs, string Ngay, string NgayHen, string MaKQDN, string XoaDCHD, string Location)
        {
            IFormatProvider culture = new CultureInfo("en-US", true);
            DateTime date = new DateTime(), date2 = new DateTime();
            if (Ngay != "")
                date = DateTime.ParseExact(Ngay, "dd/MM/yyyy HH:mm:ss", culture);
            if (NgayHen != "")
                date2 = DateTime.ParseExact(NgayHen, "dd/MM/yyyy HH:mm:ss", culture);
            return _cThuTien.XuLy_HoaDonDienTu(LoaiXuLy, MaNV, MaHDs, date, date2, MaKQDN, bool.Parse(XoaDCHD), Location);
        }

        [WebMethod]
        public string get_GhiChu(string DanhBo)
        {
            return _cThuTien.get_GhiChu(DanhBo);
        }

        [WebMethod]
        public string update_GhiChu(string MaNV, string DanhBo, string DienThoai, string GiaBieu, string NiemChi, string DiemBe)
        {
            return _cThuTien.update_GhiChu(MaNV, DanhBo, DienThoai, GiaBieu, NiemChi, DiemBe);
        }

        [WebMethod]
        public string update_DiaChiDHN(string MaNV, string DanhBo, string DiaChiDHN)
        {
            return _cThuTien.update_DiaChiDHN(MaNV, DanhBo, DiaChiDHN);
        }

        //tạm thu
        [WebMethod]
        public string GetDSTamThu(string RutSot, string MaNV, string FromCreateDate, string ToCreateDate)
        {
            return _cThuTien.GetDSTamThu(bool.Parse(RutSot), MaNV, DateTime.ParseExact(FromCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToCreateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        //đóng nước
        [WebMethod]
        public string GetDSDongNuoc(string MaNV_DongNuoc)
        {
            return _cThuTien.GetDSDongNuoc(MaNV_DongNuoc);
        }

        //public string GetDSDongNuoc(string MaNV_DongNuoc, string FromNgayGiao, string ToNgayGiao)
        //{
        //    return _cThuTien.GetDSDongNuoc(MaNV_DongNuoc, DateTime.ParseExact(FromNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        //}

        [WebMethod]
        public string GetDSCTDongNuoc(string MaNV_DongNuoc)
        {
            return _cThuTien.GetDSCTDongNuoc(MaNV_DongNuoc);
        }

        //public string GetDSCTDongNuoc(string MaNV_DongNuoc, string FromNgayGiao, string ToNgayGiao)
        //{
        //    return _cThuTien.GetDSCTDongNuoc(MaNV_DongNuoc, DateTime.ParseExact(FromNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayGiao, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        //}

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
        public string GetTongTon_DenKy(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetTongTon_DenKy(MaTo, Nam, Ky, FromDot, ToDot);
        }

        [WebMethod]
        public string GetTongTon_TrongKy(string MaTo, string Nam, string Ky, string FromDot, string ToDot)
        {
            return _cThuTien.GetTongTon_TrongKy(MaTo, Nam, Ky, FromDot, ToDot);
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

        //admin
        [WebMethod]
        public string truyvan(string sql)
        {
            return _cThuTien.truyvan(sql);
        }

        [WebMethod]
        public string capnhat(string sql)
        {
            return _cThuTien.capnhat(sql);
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

        //nộp tiền
        [WebMethod]
        public string getDS_ChotDangNgan(string FromNgayGiaiTrach, string ToNgayGiaiTrach)
        {
            return _cThuTien.getDS_ChotDangNgan(DateTime.ParseExact(FromNgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(ToNgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string them_ChotDangNgan(string NgayGiaiTrach, string CreateBy)
        {
            return _cThuTien.them_ChotDangNgan(DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture), CreateBy);
        }

        [WebMethod]
        public string chotDangNgan(string ID, string Chot, string CreateBy)
        {
            return _cThuTien.chotDangNgan(ID, bool.Parse(Chot), CreateBy);
        }

        //sync tổng
        [WebMethod]
        public string syncThanhToan(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            return _cThuTien.syncThanhToan(MaHD, GiaiTrach, IDTemp_SyncHoaDon);
        }

        [WebMethod]
        public string syncThanhToan_ThuHo(int MaHD, bool GiaiTrach, int IDTemp_SyncHoaDon)
        {
            return _cThuTien.syncThanhToan_ThuHo(MaHD, GiaiTrach, IDTemp_SyncHoaDon);
        }

        [WebMethod]
        public string syncNopTien(int MaHD)
        {
            return _cThuTien.syncNopTien(MaHD);
        }

        [WebMethod]
        public string syncNopTienLo(string NgayGiaiTrach)
        {
            return _cThuTien.syncNopTienLo(DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string syncNopTienLo_Except12(string NgayGiaiTrach)
        {
            return _cThuTien.syncNopTienLo_Except12(DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string syncNopTienLo_12(string NgayGiaiTrach)
        {
            return _cThuTien.syncNopTienLo_12(DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string showError_NopTien(string NgayGiaiTrach)
        {
            return _cThuTien.showError_NopTien(DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        [WebMethod]
        public string showHDDCBaoCaoThue(string NgayGiaiTrach)
        {
            return _cThuTien.showHDDCBaoCaoThue(DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture));
        }

        //chi tiết tiền nước
        [WebMethod]
        public string updateChiTietTienNuoc(string Nam, string Ky, string Dot)
        {
            return _cThuTien.updateChiTietTienNuoc(int.Parse(Nam), int.Parse(Ky), int.Parse(Dot));
        }

        [WebMethod]
        public void TinhTienNuoc(bool KhongApGiaGiam, bool ApGiaNuocCu, bool DieuChinhGia, int GiaDieuChinh, string DanhBo, int Ky, int Nam, DateTime TuNgay, DateTime DenNgay, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, ref int TienNuocCu, ref string ChiTietCu, ref int TienNuocMoi, ref string ChiTietMoi, ref int TieuThu_DieuChinhGia, ref int PhiBVMTCu, ref string ChiTietPhiBVMTCu, ref int PhiBVMTMoi, ref string ChiTietPhiBVMTMoi)
        {
            _cKinhDoanh.TinhTienNuoc(KhongApGiaGiam, ApGiaNuocCu, DieuChinhGia, GiaDieuChinh, DanhBo, Ky, Nam, TuNgay, DenNgay, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, ref  TienNuocCu, ref  ChiTietCu, ref  TienNuocMoi, ref  ChiTietMoi, ref  TieuThu_DieuChinhGia, ref  PhiBVMTCu, ref  ChiTietPhiBVMTCu, ref  PhiBVMTMoi, ref  ChiTietPhiBVMTMoi);
        }
    }
}
