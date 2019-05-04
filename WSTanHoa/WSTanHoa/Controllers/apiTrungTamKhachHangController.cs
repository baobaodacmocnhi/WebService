using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/TrungTamKhachHang")]
    public class apiTrungTamKhachHangController : ApiController
    {
        CConnection _cDAL_DHN = new CConnection(CConstantVariable.DHN);
        CConnection _cDAL_DocSo = new CConnection(CConstantVariable.DocSo);
        CConnection _cDAL_ThuTien = new CConnection(CConstantVariable.ThuTien);

        /// <summary>
        /// Lấy thông tin khách hàng
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getThongTinKhachHang")]
        public ThongTinKhachHang getThongTinKhachHang(string DanhBo)
        {
            DataTable dt = new DataTable();
            //lấy thông tin khách hàng
            string sql = "select DanhBo"
                         + ",HoTen"
                         + ",DiaChi=SoNha+' '+TenDuong+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                         + ",HopDong"
                         + ",DienThoai"
                         + ",MLT=LoTrinh"
                         + ",DinhMuc"
                         + ",GiaBieu"
                         + ",HieuDH"
                         + ",CoDH"
                         + ",Cap"
                         + ",SoThanDH"
                         + ",ViTriDHN"
                         + ",NgayThay"
                         + ",NgayKiemDinh"
                         + ",HieuLuc=convert(varchar(2),Ky)+'/'+convert(char(4),Nam)"
                         + " from TB_DULIEUKHACHHANG where DanhBo=" + DanhBo;
            dt.Merge(_cDAL_DHN.ExecuteQuery_DataTable(sql));
            //lấy thông tin khách hàng đã hủy
            if (dt == null || dt.Rows.Count == 0)
            {
                sql = "select DanhBo"
                             + ",HoTen"
                             + ",DiaChi=SoNha+' '+TenDuong+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                             + ",HopDong"
                             + ",DienThoai=''"
                             + ",MLT=LoTrinh"
                             + ",DinhMuc"
                             + ",GiaBieu"
                             + ",HieuDH"
                             + ",CoDH"
                             + ",Cap"
                             + ",SoThanDH"
                             + ",ViTriDHN"
                             + ",NgayThay"
                             + ",NgayKiemDinh"
                             + ",HieuLuc=convert(varchar(2),Ky)+'/'+convert(char(4),Nam)"
                             + " from TB_DULIEUKHACHHANG_HUYDB where DanhBo=" + DanhBo;
                dt.Merge(_cDAL_DHN.ExecuteQuery_DataTable(sql));
            }
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                en.HoTen = dt.Rows[0]["HoTen"].ToString();
                en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                en.HopDong = dt.Rows[0]["HopDong"].ToString();
                en.DienThoai = dt.Rows[0]["DienThoai"].ToString();
                en.MLT = dt.Rows[0]["MLT"].ToString();
                en.DinhMuc = dt.Rows[0]["DinhMuc"].ToString();
                en.GiaBieu = dt.Rows[0]["GiaBieu"].ToString();
                en.HieuDH = dt.Rows[0]["HieuDH"].ToString();
                en.CoDH = dt.Rows[0]["CoDH"].ToString();
                en.Cap = dt.Rows[0]["Cap"].ToString();
                en.SoThanDH = dt.Rows[0]["SoThanDH"].ToString();
                en.ViTriDHN = dt.Rows[0]["ViTriDHN"].ToString();
                if(dt.Rows[0]["NgayThay"].ToString()!="")
                en.NgayThay = DateTime.Parse( dt.Rows[0]["NgayThay"].ToString());
                if (dt.Rows[0]["NgayKiemDinh"].ToString() != "")
                    en.NgayKiemDinh = DateTime.Parse(dt.Rows[0]["NgayKiemDinh"].ToString());
                en.HieuLuc = dt.Rows[0]["HieuLuc"].ToString();
                return en;
            }
            else
                return null;

        }

        /// <summary>
        /// Lấy danh sách 12 kỳ đọc số gần nhất
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getGhiChiSo")]
        public IList<GhiChiSo> getGhiChiSo(string DanhBo)
        {
            DataTable dt = new DataTable();
            string sql = "select top(12) Ky=CONVERT(char(2),Ky)+'/'+CONVERT(char(4),Nam)"
                           + ",NgayDoc=CONVERT(char(10),DenNgay,103)"
                           + ",CodeMoi"
                           + ",ChiSoCu=CSCu"
                           + ",ChiSoMoi=CSMoi"
                           + ",TieuThu=TieuThuMoi"
                           + " from DocSo"
                           + " where DanhBa=" + DanhBo
                           + " order by Nam desc,CAST(Ky as int) desc";
            dt= _cDAL_DocSo.ExecuteQuery_DataTable(sql);
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<GhiChiSo> lst = new List<GhiChiSo>();
                foreach (DataRow item in dt.Rows)
                {
                    GhiChiSo en = new GhiChiSo();
                    en.Ky = dt.Rows[0]["Ky"].ToString();
                    if (dt.Rows[0]["NgayDoc"].ToString() != "")
                        en.NgayDoc = DateTime.Parse(dt.Rows[0]["NgayDoc"].ToString());
                    en.CodeMoi = dt.Rows[0]["CodeMoi"].ToString();
                    en.ChiSoCu = dt.Rows[0]["ChiSoCu"].ToString();
                    en.ChiSoMoi = dt.Rows[0]["ChiSoMoi"].ToString();
                    en.TieuThu = dt.Rows[0]["TieuThu"].ToString();
                    lst.Add(en);
                }
                return lst;
            }
            else
                return null;

        }

        /// <summary>
        /// Lấy danh sách thông tin ghi chú
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getGhiChu")]
        public IList<GhiChu> getGhiChu(string DanhBo)
        {
            DataTable dt = new DataTable();
            string sql = "select NoiDung,CreateDate from TB_GHICHU where DanhBo=" + DanhBo + " order by CreateDate desc";
            dt = _cDAL_DHN.ExecuteQuery_DataTable(sql);
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<GhiChu> lst = new List<GhiChu>();
                foreach (DataRow item in dt.Rows)
                {
                    GhiChu en = new GhiChu();
                    en.NoiDung = dt.Rows[0]["NoiDung"].ToString();
                    if (dt.Rows[0]["CreateDate"].ToString() != "")
                        en.CreateDate = DateTime.Parse(dt.Rows[0]["CreateDate"].ToString());
                    lst.Add(en);
                }
                return lst;
            }
            else
                return null;

        }

        /// <summary>
        /// Lấy thông tin hóa đơn
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getHoaDon")]
        public IList<HoaDonThuTien> getHoaDon(string DanhBo)
        {
            DataTable dt = new DataTable();
            string sql = "select * from fnTimKiem('" + DanhBo + "','') order by MaHD desc";
            dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<HoaDonThuTien> lst = new List<HoaDonThuTien>();
                foreach (DataRow item in dt.Rows)
                {
                    HoaDonThuTien en = new HoaDonThuTien();
                    en.GiaBieu = dt.Rows[0]["GiaBieu"].ToString();
                    en.DinhMuc = dt.Rows[0]["DinhMuc"].ToString();
                    en.SoHoaDon = dt.Rows[0]["SoHoaDon"].ToString();
                    en.Ky = dt.Rows[0]["Ky"].ToString();
                    en.TieuThu = dt.Rows[0]["TieuThu"].ToString();
                    en.GiaBan = dt.Rows[0]["GiaBan"].ToString();
                    en.ThueGTGT = dt.Rows[0]["ThueGTGT"].ToString();
                    en.PhiBVMT = dt.Rows[0]["PhiBVMT"].ToString();
                    en.TongCong = dt.Rows[0]["TongCong"].ToString();
                    if (dt.Rows[0]["NgayGiaiTrach"].ToString() != "")
                        en.NgayGiaiTrach = DateTime.Parse(dt.Rows[0]["NgayGiaiTrach"].ToString());
                    en.DangNgan = dt.Rows[0]["DangNgan"].ToString();
                    en.HanhThu = dt.Rows[0]["HanhThu"].ToString();
                    en.MaDN = dt.Rows[0]["MaDN"].ToString();
                    if (dt.Rows[0]["NgayDN"].ToString() != "")
                        en.NgayDN = DateTime.Parse(dt.Rows[0]["NgayDN"].ToString());
                    if (dt.Rows[0]["NgayMN"].ToString() != "")
                        en.NgayMN = DateTime.Parse(dt.Rows[0]["NgayMN"].ToString());
                    lst.Add(en);
                }
                return lst;
            }
            else
                return null;

        }
    }
}