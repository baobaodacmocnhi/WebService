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

        [Route("")]
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
                    + " (select NGAYGIAITRACH, MaNV_DangNgan, GIABAN from HOADON_TA.dbo.HOADON where Nam= @year and Ky< @month),"
                    + "   hientai as"
                    + " (select NGAYGIAITRACH, MaNV_DangNgan, GIABAN from HOADON_TA.dbo.HOADON where Nam= @year and Ky = @month)"
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

        [Route("")]
        [HttpGet]
        public MResult ThongKe_SanLuong(string Thang, string Nam)
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
        public MResult ThongKe_DoanhThu(string Thang, string Nam)
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
