﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Service : System.Web.Services.WebService
    {
        CBaoBao _cBaoBao = new CBaoBao();
        CThuTien _cThuTien = new CThuTien();

        #region BaoBao

        [WebMethod]
        public bool ThemKhachHang(string HoTen, string GioiTinh, string DienThoai, string BienSoXe, string MaPhong)
        {
            return _cBaoBao.ThemKhachHang(HoTen, GioiTinh, DienThoai, BienSoXe, MaPhong);
        }

        [WebMethod]
        public bool SuaKhachHang(string ID, string HoTen, string GioiTinh, string DienThoai, string BienSoXe, string MaPhong)
        {
            return _cBaoBao.SuaKhachHang(ID, HoTen, GioiTinh, DienThoai, BienSoXe, MaPhong);
        }

        [WebMethod]
        public bool XoaKhachHang(string ID)
        {
            return _cBaoBao.XoaKhachHang(ID);
        }

        [WebMethod]
        public DataTable GetDSKhachHang()
        {
            return _cBaoBao.GetDSKhachHang();
        }

        [WebMethod]
        public bool SuaPhong(string ID, string Name, string GiaTien, string SoNKNuoc, string ChiSoDien, string ChiSoNuoc, string NgayThue, string Thue)
        {
            return _cBaoBao.SuaPhong(ID, Name, GiaTien, SoNKNuoc, ChiSoDien, ChiSoNuoc, NgayThue, Thue);
        }

        [WebMethod]
        public DataTable GetDSPhong()
        {
            return _cBaoBao.GetDSPhong();
        }

        [WebMethod]
        public bool SuaGiaDien(string ID, string Name, string GiaTien)
        {
            return _cBaoBao.SuaGiaDien(ID, Name, GiaTien);
        }

        [WebMethod]
        public DataTable GetDSGiaDien()
        {
            return _cBaoBao.GetDSGiaDien();
        }

        [WebMethod]
        public bool SuaGiaNuoc(string ID, string Name, string GiaTien)
        {
            return _cBaoBao.SuaGiaNuoc(ID, Name, GiaTien);
        }

        [WebMethod]
        public DataTable GetDSGiaNuoc()
        {
            return _cBaoBao.GetDSGiaNuoc();
        }

        [WebMethod]
        public bool ThemHoaDon(string MaPhong, string ChiSoDien, string ChiSoNuoc)
        {
            return _cBaoBao.ThemHoaDon(MaPhong, int.Parse(ChiSoDien), int.Parse(ChiSoNuoc));
        }

        [WebMethod]
        public bool SuaHoaDon(string ID, string ChiSoDien, string ChiSoNuoc)
        {
            return _cBaoBao.SuaHoaDon(ID, int.Parse(ChiSoDien), int.Parse(ChiSoNuoc));
        }

        [WebMethod]
        public bool XoaHoaDon(string ID)
        {
            return _cBaoBao.XoaHoaDon(ID);
        }

        [WebMethod]
        public DataTable GetDSHoaDonBB()
        {
            return _cBaoBao.GetDSHoaDon();
        }

        [WebMethod]
        public DataTable GetDSHoaDonBBByMaPhong(string MaPhong)
        {
            return _cBaoBao.GetDSHoaDon(MaPhong);
        }

        #endregion

        #region DocSo

        //[WebMethod]
        //public object DS_GetCurrentVersion()
        //{
        //    return _cDocSo.GetCurrentVersion();
        //}

        //[WebMethod]
        //public bool DS_CheckDangNhap(string TaiKhoan, string MatKhau)
        //{
        //    return _cDocSo.CheckDangNhap(TaiKhoan, MatKhau);
        //}

        //[WebMethod]
        //public DataTable DS_DangNhap(string TaiKhoan, string MatKhau)
        //{
        //    return _cDocSo.DangNhap(TaiKhoan, MatKhau);
        //}

        //[WebMethod]
        //public DataTable DS_GetDSCode()
        //{
        //    return _cDocSo.GetDSCode();
        //}

        //[WebMethod]
        //public DataTable DS_GetDSDocSo(string Nam, string Ky, string Dot, string May)
        //{
        //    return _cDocSo.GetDSDocSo(Nam, Ky, Dot, May);
        //}

        //[WebMethod]
        //public int DS_TinhTieuThu(string DanhBo, string Nam, string Ky, string CodeMoi, string CSMoi)
        //{
        //    return _cDocSo.TinhTieuThu(DanhBo, int.Parse(Nam), int.Parse(Ky), CodeMoi, int.Parse(CSMoi));
        //}

        //[WebMethod]
        //public void DS_TinhTienNuoc(string DanhBo, string GiaBieu, string DinhMuc, string TieuThu, out int GiaBan, out int PhiBVMT, out int ThueGTGT, out int TongCong, out string ChiTiet)
        //{
        //    _cDocSo.TinhTienNuoc(DanhBo, int.Parse(GiaBieu), int.Parse(DinhMuc), int.Parse(TieuThu), out GiaBan, out PhiBVMT, out ThueGTGT, out TongCong, out ChiTiet);
        //}

        //[WebMethod]
        //public bool DS_CapNhat(string ID, string DanhBo,string Nam,string Ky, string CodeMoi, string TTDHNMoi, string CSMoi, string GiaBieu,string DinhMuc, string Latitude, string Longitude,out int TieuThu,out int TongCong)
        //{
        //   return _cDocSo.CapNhat(ID, DanhBo, int.Parse(Nam), int.Parse(Ky), CodeMoi, TTDHNMoi, int.Parse(CSMoi), int.Parse(GiaBieu), int.Parse(DinhMuc), Latitude, Longitude,out TieuThu,out TongCong);
        //}

        //[WebMethod]
        //public bool DS_ThemHinhDHN(string DanhBo, string CreateBy, string imageStr, string Latitude, string Longitude)
        //{
        //    return _cDocSo.ThemHinhDHN(DanhBo, CreateBy, imageStr, Latitude,  Longitude);
        //}

        #endregion

    }
}