using Newtonsoft.Json;
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
    //[Authorize(Roles = "SWOC")]
    [RoutePrefix("api/SWOC")]
    public class apiSWOCController : ApiController
    {
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);

        [Route("")]
        [HttpGet]
        public MResult ThongKe_GanMoiDHNNho(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_KhieuNai")]
        [HttpGet]
        public MResult ThongKe_KhieuNai(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                string[] dates = CreateDate.Split('/');
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @year int=" + dates[2] + ";"
                    + " declare @month int = " + dates[1] + ";"
                    + " declare @day int= " + dates[0] + ";"
                    + " with luyke as"
                    + " (select dtct.* from KTKS_DonKH.dbo.DonTu dt,KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                    + "  where YEAR(dt.CreateDate) = @year and month(dt.CreateDate) >= 1 and  month(dt.CreateDate) < @month and dt.MaDon = dtct.MaDon"
                    + "  and(select count(*) from KTKS_DonKH.dbo.DonTu_ChiTiet where KTKS_DonKH.dbo.DonTu_ChiTiet.MaDon = dt.MaDon) = 1 and Name_NhomDon_PKH not like ''"
                    + "  and(select stuff((select ';' + cast(NameGroup as nvarchar(100)) FROM[KTKS_DonKH].[dbo].[NhomDon] where KhieuNai = 1 group by NameGroup for xml path('')),1,1,'')) like '%' + Name_NhomDon_PKH + '%'),"
                    + "   hientai as"
                    + " (select dtct.* from KTKS_DonKH.dbo.DonTu dt,KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                    + "  where YEAR(dt.CreateDate) = @year and month(dt.CreateDate) = @month and dt.MaDon = dtct.MaDon"
                    + "  and(select count(*) from KTKS_DonKH.dbo.DonTu_ChiTiet where KTKS_DonKH.dbo.DonTu_ChiTiet.MaDon = dt.MaDon) = 1 and Name_NhomDon_PKH not like ''"
                    + "  and(select stuff((select ';' + cast(NameGroup as nvarchar(100)) FROM[KTKS_DonKH].[dbo].[NhomDon] where KhieuNai = 1 group by NameGroup for xml path('')),1,1,'')) like '%' + Name_NhomDon_PKH + '%')"
                    + "  select ma_don_vi = 'TH', ten_don_vi = N'Tân Hòa',nam = @year,thang = @month,so_khkn_tn = (select count(*) from hientai where DAY(CreateDate) = @day)"
                    + "  ,xu_ly_trong_thang = (select count(*) from hientai where HoanThanh = 1),so_khkn_trong_thang = (select count(*) from hientai),chua_xu_ly = (select count(*) from hientai where HoanThanh = 0)"
                    + "  ,khkn_luy_ke = (select count(*) from luyke),thang_luy_ke = (select @month - 1),xu_ly_tn = (select count(*) from hientai where DAY(CreateDate) = @day and HoanThanh = 1),chi_tieu = null,ty_le = null,xu_ly_luy_ke = (select count(*) from luyke where HoanThanh = 1)");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_SXKD")]
        [HttpGet]
        public MResult ThongKe_SXKD(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                string[] dates = CreateDate.Split('/');
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @year int=" + dates[2] + ";"
                    + " declare @month int = " + dates[1] + ";"
                    + " declare @day int= " + dates[0] + ";"
                    + " with luyke as"
                    + " (select NGAYGIAITRACH, MaNV_DangNgan, GIABAN=case when dc.GIABAN_BD is null then hd.GIABAN else dc.GIABAN_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON=hd.ID_HOADON where Nam= @year and Ky< @month),"
                    + "   hientai as"
                    + " (select NGAYGIAITRACH, MaNV_DangNgan, GIABAN=case when dc.GIABAN_BD is null then hd.GIABAN else dc.GIABAN_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON=hd.ID_HOADON where Nam= @year and Ky = @month)"
                    + "  select ma_don_vi = 'TH', ten_don_vi = N'Tân Hòa',nam = @year,thang = @month,sohd_thu_tn = (select count(*) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)"
                    + " , sohd_ton = (select count(*) from hientai where MaNV_DangNgan is null or NGAYGIAITRACH is null),gia_ban_bq = (select GiaBanBinhQuan from HOADON_TA.dbo.TT_GiaBanBinhQuan where Nam = @year and Ky = @month),thang_luy_ke = @month - 1"
                    + " , thu_luy_ke = (select count(*) from luyke where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)"
                    + " , thu_trong_thang = (select count(*) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month)"
                    + " , tien_thu_tn = (select sum(giaban) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)"
                    + " , chi_tieu = (select DoanhThu from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year),ty_le = (select sum(giaban) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)/ (select DoanhThu from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year)*100");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_SuaBe(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_ThatThoatNuoc(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_ThayDHNNho(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_ThayDHNLon(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_SanLuong")]
        [HttpGet]
        public MResult ThongKe_SanLuong(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @year int = " + Nam + ";"
                    + " declare @month int = " + Thang + ";"
                    + " with luyke as"
                    + " (select Ky, TIEUTHU=case when dc.TIEUTHU_BD is null then hd.TIEUTHU else dc.TIEUTHU_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON = hd.ID_HOADON where Nam = @year - 1 and Ky<= @month),"
                    + " hientai as"
                    + " (select Ky, TIEUTHU=case when dc.TIEUTHU_BD is null then hd.TIEUTHU else dc.TIEUTHU_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON = hd.ID_HOADON where Nam = @year and Ky<= @month)"
                    + " select t1.*,ty_le_6_2=cast(t1.khoi_luong_th_6 as float)/t1.khoi_luong_th_nam*100,ty_le_6_1=cast(t1.khoi_luong_th_1 as float)/t1.khoi_luong_th_nam*100 from"
                    + " (select ma_don_vi = 'TH', ten_don_vi = N'Tân Hoà',dvt = 'm3'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year - 1)"
                    + " , khoi_luong_th_1 = (select sum(TieuThu) from luyke)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_nam = (select SanLuong from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year)"
                    + " , thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, (@month - 1)) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_Luong_th_3 = (select sum(TieuThu) from hientai where KY = @month - 1)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, (@month - 1)) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_4 = (select sum(TieuThu) from hientai where KY <= @month - 1)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @month) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_5 = (select sum(TieuThu) from hientai where KY = @month)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_6 = (select sum(TieuThu) from hientai where KY <= @month))t1");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_DoanhThu")]
        [HttpGet]
        public MResult ThongKe_DoanhThu(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @year int = " + Nam + ";"
                    + " declare @month int = " + Thang + ";"
                    + " with luyke as"
                    + " (select Ky, NGAYGIAITRACH, MaNV_DangNgan, GIABAN=case when dc.GIABAN_BD is null then hd.GIABAN else dc.GIABAN_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON = hd.ID_HOADON where Nam = @year - 1 and Ky<= @month),"
                    + " hientai as"
                    + " (select Ky, NGAYGIAITRACH, MaNV_DangNgan, GIABAN=case when dc.GIABAN_BD is null then hd.GIABAN else dc.GIABAN_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON = hd.ID_HOADON where Nam = @year and Ky<= @month)"
                    + " select t1.*,ty_le_6_2 = t1.khoi_luong_th_6 / t1.khoi_luong_th_nam * 100,ty_le_6_1 = t1.khoi_luong_th_1 / t1.khoi_luong_th_nam * 100 from"
                    + " (select ma_don_vi = 'TH', ten_don_vi = N'Tân Hoà', dvt = 'đồng'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year - 1)"
                    + " , khoi_luong_th_1 = (select sum(GIABAN) from luyke)"
                    + ", ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @year)"
                    + ", khoi_luong_th_nam = (select DoanhThu from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year)"
                    + ", thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, (@month - 1)) + N' năm ' + convert(varchar, @year)"
                    + ", khoi_Luong_th_3 = (select sum(GIABAN) from hientai where KY = @month - 1)"
                    + ", thuc_hien_4 = N'Thực hiện ' + convert(varchar, (@month - 1)) + N' tháng năm ' + convert(varchar, @year)"
                    + ", khoi_luong_th_4 = (select sum(GIABAN) from hientai where KY <= @month - 1)"
                    + ", thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @month) + N' năm ' + convert(varchar, @year)"
                    + ", khoi_luong_th_5 = (select sum(GIABAN) from hientai where KY = @month)"
                    + ", thuc_hien_6 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year)"
                    + ", khoi_luong_th_6 = (select sum(GIABAN) from hientai where KY <= @month))t1");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_GBBQ(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_GanMoiDHNNho(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_ThayDHNNho(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_ThayDHNLon(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_ThatThoatNuoc(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("")]
        [HttpGet]
        public MResult ThongKe_HoaDon04(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

    }
}
