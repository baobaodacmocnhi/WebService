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
    [Authorize(Roles = "SWOC")]
    [RoutePrefix("api/SWOC")]
    public class apiSWOCController : ApiController
    {
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);

        [Route("ThongKe_GanMoiDHNNho")]
        [HttpGet]
        public MResult ThongKe_GanMoiDHNNho(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                string[] dates = CreateDate.Split('/');
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @ngay date = '" + dates[2] + dates[1] + dates[0] + "'"
                    + " declare @chitieu int = (SELECT GanMoi FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = year(@ngay))"
                    + " select"
                    + " ma_don_vi = 'TH'"
                    + " , ten_don_vi = N'Tân Hoà'"
                    + " , nam_thong_ke = year(@ngay)"
                    + " , thang_thong_ke = month(@ngay)"
                    + " , gan_moi_tn = count(case when NGAYTHICONG = @ngay then 1 end)"
                    + " , ho_so_nhan_trong_ngay = count(case when t1.NGAYNHAN = @ngay then 1 end)"
                    + " , thang_luy_ke = month(@ngay)"
                    + " , hoso_nhan_trong_thang = count(case when month(t1.NGAYNHAN) = month(@ngay) then 1 end)"
                    + " , ho_so_tro_ngai_trong_thang = count(case when month(t1.NGAYNHAN) = month(@ngay) and(t1.TRONGAITHIETKE = 1 or t2.TRONGAI = 1) then 1 end)"
                    + " , ho_so_tro_ngai_luy_ke = count(case when t1.TRONGAITHIETKE = 1 or t2.TRONGAI = 1 then 1 end)"
                    + " , ho_so_luy_ke = count(case when year(NGAYTHICONG) = year(@ngay) then 1 end)"
                    + " , chi_tieu = @chitieu"
                    + " , ty_le = round(count(case when year(NGAYTHICONG) = year(@ngay) then 1 end) * 100.0 / @chitieu, 2)"
                    + " from TANHOA_WATER.dbo.DON_KHACHHANG t1"
                    + " left join TANHOA_WATER.dbo.KH_HOSOKHACHHANG t2 on t1.SHS = t2.SHS"
                    + " left join TANHOA_WATER.dbo.KH_XINPHEPDAODUONG t3 on t2.MADOTDD = t3.MADOT"
                    + " where t1.LOAIHOSO = 'GM' and(year(NGAYTHICONG) = year(@ngay) or year(t1.NGAYNHAN) = year(@ngay))");
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
                    + " select t1.*,ty_le = t1.tien_thu_tn/ t1.chi_tieu*100 from "
                    + "  (select ma_don_vi = 'TH', ten_don_vi = N'Tân Hòa',nam = @year,thang = @month,sohd_thu_tn = (select count(*) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)"
                    + " , sohd_ton = (select count(*) from hientai where MaNV_DangNgan is null or NGAYGIAITRACH is null),gia_ban_bq = (select GiaBanBinhQuan from HOADON_TA.dbo.TT_GiaBanBinhQuan where Nam = @year and Ky = @month),thang_luy_ke = @month - 1"
                    + " , thu_luy_ke = (select count(*) from luyke where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)"
                    + " , thu_trong_thang = (select count(*) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month)"
                    + " , tien_thu_tn = (select sum(convert(bigint,GIABAN)) from hientai where MaNV_DangNgan is not null and YEAR(NGAYGIAITRACH) = @year and month(NGAYGIAITRACH) = @month and day(NGAYGIAITRACH) = @day)"
                    + " , chi_tieu = (select DoanhThu from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year))t1");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_SuaBe")]
        [HttpGet]
        public MResult ThongKe_SuaBe(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                string[] dates = CreateDate.Split('/');
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @ngay date = '" + dates[2] + dates[1] + dates[0] + "'"
                    + " SELECT"
                    + " 'TH' ma_don_vi"
                    + " , N'Tân Hoà' ten_don_vi"
                    + " , year(@ngay) nam_thong_ke"
                    + " , month(@ngay) thang_thong_ke"
                    + " , day(@ngay) ngay_thong_ke"
                    + " , count(case when NgayBao = @ngay then 1 end) ho_so_nhan_trong_ngay"
                    + " , count(case when NgaySua = @ngay then 1 end) so_luong_sua_trong_ngay"
                    + " , count(case when month(NgayBao) = month(@ngay) then 1 end) ho_so_nhan_trong_thang"
                    + " , count(case when month(NgaySua) = month(@ngay) then 1 end) so_luong_sua_trong_thang"
                    + " , month(@ngay) thang_luy_ke"
                    + " , count(case when year(NgaySua) = year(@ngay) then 1 end) so_luong_sua_luy_ke"
                    + " , count(case when year(NgayBao) = year(@ngay) then 1 end) so_luong_nhan_luy_ke"
                    + " , null chi_tieu"
                    + " , null con_lai"
                    + " , null ty_le"
                    + " FROM server5.tanhoa.dbo.w_BaoBe"
                    + " where year(NgayBao) = year(@ngay) or year(NgaySua) = year(@ngay)");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        //[Route("")]
        //[HttpGet]
        //public MResult ThongKe_ThatThoatNuoc(string CreateDate)
        //{
        //    MResult result = new MResult();
        //    try
        //    {
        //        string[] dates = CreateDate.Split('/');
        //        DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("");
        //        result.data = JsonConvert.SerializeObject(dt);
        //        result.success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.error = ex.Message;
        //    }
        //    return result;
        //}

        [Route("ThongKe_ThayDHNNho")]
        [HttpGet]
        public MResult ThongKe_ThayDHNNho(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                string[] dates = CreateDate.Split('/');
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @ngay date = '" + dates[2] + dates[1] + dates[0] + "'"
                    + " declare @chitieu int = (SELECT ThayDHNNho FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = year(@ngay))"
                    + " select"
                    + " 'TH' ma_don_vi"
                    + " , N'Tân Hoà' ten_don_vi"
                    + " , year(@ngay) nam"
                    + " , month(@ngay) thang"
                    + " , (select count(*) from TB_THAYDHN where DHN_CODH < 40 and HCT_NGAYGAN is null) so_dh_can_thay"
                    + " , count(case when HCT_NGAYGAN = @ngay then 1 end) thay_trong_ngay"
                    + " , count(case when month(HCT_NGAYGAN) = month(@ngay) then 1 end) so_dh_thay_trong_thang"
                    + " , month(@ngay) thang_luy_ke"
                    + " ,count(*) so_dh_thay_luy_ke"
                    + " , @chitieu - count(*) con_lai"
                    + " , @chitieu chi_tieu"
                    + " , round(count(*) * 100.0 / @chitieu, 2) ty_le"
                    + " from CAPNUOCTANHOA.dbo.TB_THAYDHN"
                    + " where year(HCT_NGAYGAN) = year(@ngay) and isnull(HCT_TRONGAI,0)= 0 and HCT_CODHNGAN< 40");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_ThayDHNLon")]
        [HttpGet]
        public MResult ThongKe_ThayDHNLon(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                string[] dates = CreateDate.Split('/');
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @ngay date = '" + dates[2] + dates[1] + dates[0] + "'"
                    + " declare @chitieu int = (SELECT ThayDHNLon FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = year(@ngay))"
                    + " select"
                    + " 'TH' ma_don_vi"
                    + " , N'Tân Hoà' ten_don_vi"
                    + " , year(@ngay) nam"
                    + " , month(@ngay) thang"
                    + " , (select count(*) from TB_THAYDHN where DHN_CODH >= 40 and HCT_NGAYGAN is null) so_dh_can_thay"
                    + " , count(case when HCT_NGAYGAN = @ngay then 1 end) thay_trong_ngay"
                    + " , count(case when month(HCT_NGAYGAN) = month(@ngay) then 1 end) so_dh_thay_trong_thang"
                    + " , month(@ngay) thang_luy_ke"
                    + " ,count(*) so_dh_thay_luy_ke"
                    + " , @chitieu - count(*) con_lai"
                    + " , @chitieu chi_tieu"
                    + " , round(count(*) * 100.0 / @chitieu, 2) ty_le"
                    + " from CAPNUOCTANHOA.dbo.TB_THAYDHN"
                    + " where year(HCT_NGAYGAN) = year(@ngay) and isnull(HCT_TRONGAI,0)= 0 and HCT_CODHNGAN>= 40");
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
                    + " (select ma_don_vi = 'TH', ten_don_vi = N'Tân Hoà', dvt = N'đồng'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year - 1)"
                    + " , khoi_luong_th_1 = (select sum(convert(bigint,GIABAN)) from luyke)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_nam = (select DoanhThu from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year)"
                    + " , thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, (@month - 1)) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_Luong_th_3 = (select sum(convert(bigint,GIABAN)) from hientai where KY = @month - 1)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, (@month - 1)) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_4 = (select sum(convert(bigint,GIABAN)) from hientai where KY <= @month - 1)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @month) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_5 = (select sum(convert(bigint,GIABAN)) from hientai where KY = @month)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_6 = (select sum(convert(bigint,GIABAN)) from hientai where KY <= @month))t1");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_GBBQ")]
        [HttpGet]
        public MResult ThongKe_GBBQ(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @year int = " + Nam + ";"
                    + " declare @month int = " + Thang + ";"
                    + " with luyke as"
                    + "  (select Ky, GiaBanBinhQuan from HOADON_TA.dbo.TT_GiaBanBinhQuan where Nam = @year - 1 and Ky <= @month),"
                    + "  hientai as"
                    + "  (select Ky, GiaBanBinhQuan from HOADON_TA.dbo.TT_GiaBanBinhQuan where Nam = @year and Ky<= @month)"
                    + "  select t1.*,ty_le_6_2 = t1.khoi_luong_th_6 / t1.khoi_luong_th_nam * 100,ty_le_6_1 = t1.khoi_luong_th_1 / t1.khoi_luong_th_nam * 100 from"
                    + "  (select ma_don_vi = 'TH', ten_don_vi = N'Tân Hoà', dvt = N'đồng'"
                    + "  , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year - 1)"
                    + "  , khoi_luong_th_1 = (select sum(GiaBanBinhQuan) / @month from luyke)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_nam = (select GiaBanBinhQuan from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year)"
                    + " , thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, (@month - 1)) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_Luong_th_3 = (select sum(GiaBanBinhQuan) from hientai where KY = @month - 1)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, (@month - 1)) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_4 = (select sum(GiaBanBinhQuan) / (@month - 1) from hientai where KY <= @month - 1)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @month) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_5 = (select sum(GiaBanBinhQuan) from hientai where KY = @month)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_6 = (select sum(GiaBanBinhQuan) / @month from hientai where KY <= @month))t1");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_GanMoiDHNNho")]
        [HttpGet]
        public MResult ThongKe_GanMoiDHNNho(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @thang int = " + Thang
                    + " declare @nam int = " + Nam
                    + " declare @chitieu int = (SELECT GanMoi FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @nam)"
                    + " select"
                    + "  ma_don_vi = 'TH'"
                    + " , ten_don_vi = N'Tân Hoà'"
                    + " , dvt = N'đhn'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam - 1)"
                    + " , khoi_luong_th_1 = count(case when year(NGAYTHICONG) = @nam - 1 and month(NGAYTHICONG) <= @thang then 1 end)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_nam = @chitieu"
                    + " ,thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, @thang - 1) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_3 = count(case when year(NGAYTHICONG) = @nam and month(NGAYTHICONG) = @thang - 1 then 1 end)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, @thang - 1) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_4 = count(case when year(NGAYTHICONG) = @nam and month(NGAYTHICONG) <= @thang - 1 then 1 end)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @thang) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_5 = count(case when year(NGAYTHICONG) = @nam and month(NGAYTHICONG) = @thang then 1 end)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_6 = count(case when year(NGAYTHICONG) = @nam and month(NGAYTHICONG) <= @thang then 1 end)"
                    + " , ty_le_6_2 = round(count(case when year(NGAYTHICONG) = @nam and month(NGAYTHICONG) <= @thang then 1 end) * 100.0 / @chitieu, 2)"
                    + " , ty_le_6_1 = round(count(case when year(NGAYTHICONG) = @nam and month(NGAYTHICONG) <= @thang then 1 end) * 100.0 / count(case when year(NGAYTHICONG) = @nam - 1 and month(NGAYTHICONG) <= @thang then 1 end), 2)"
                    + " , nam_thong_ke = @nam"
                    + " , thang_thong_ke = @thang"
                    + " from TANHOA_WATER.dbo.DON_KHACHHANG t1"
                    + " left join TANHOA_WATER.dbo.KH_HOSOKHACHHANG t2 on t1.SHS = t2.SHS"
                    + " where t1.LOAIHOSO = 'GM' and(year(NGAYTHICONG) >= @nam - 1)");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_ThayDHNNho")]
        [HttpGet]
        public MResult ThongKe_ThayDHNNho(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @thang int = " + Thang
                    + " declare @nam int = " + Nam
                    + " declare @chitieu int = (SELECT ThayDHNNho FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @nam)"
                    + " select"
                    + "  ma_don_vi = 'TH'"
                    + " , ten_don_vi = N'Tân Hoà'"
                    + " , dvt = N'đhn'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam - 1)"
                    + " , khoi_luong_th_1 = count(case when year(HCT_NGAYGAN) = @nam - 1 and month(HCT_NGAYGAN) <= @thang then 1 end)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_nam = @chitieu"
                    + " ,thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, @thang - 1) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_3 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) = @thang - 1 then 1 end)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, @thang - 1) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_4 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang - 1 then 1 end)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @thang) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_5 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) = @thang then 1 end)"
                    + " ,thuc_hien_6 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_6 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang then 1 end)"
                    + " , ty_le_6_2 = round(count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang then 1 end) * 100.0 / @chitieu, 2)"
                    + " , ty_le_6_1 = round(count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang then 1 end) * 100.0 / count(case when year(HCT_NGAYGAN) = @nam - 1 and month(HCT_NGAYGAN) <= @thang then 1 end), 2)"
                    + " , nam_thong_ke = @nam"
                    + " , thang_thong_ke = @thang"
                    + " from CAPNUOCTANHOA.dbo.TB_THAYDHN"
                    + " where HCT_CODHNGAN < 40 and isnull(HCT_TRONGAI,0)= 0");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_ThayDHNLon")]
        [HttpGet]
        public MResult ThongKe_ThayDHNLon(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @thang int = " + Thang
                    + " declare @nam int = " + Nam
                    + " declare @chitieu int = (SELECT ThayDHNLon FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @nam)"
                    + " select"
                    + "  ma_don_vi = 'TH'"
                    + " , ten_don_vi = N'Tân Hoà'"
                    + " , dvt = N'đhn'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam - 1)"
                    + " , khoi_luong_th_1 = count(case when year(HCT_NGAYGAN) = @nam - 1 and month(HCT_NGAYGAN) <= @thang then 1 end)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_nam = @chitieu"
                    + " , thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, @thang - 1) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_3 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) = @thang - 1 then 1 end)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, @thang - 1) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_4 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang - 1 then 1 end)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @thang) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_5 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) = @thang then 1 end)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_6 = count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang then 1 end)"
                    + " , ty_le_6_2 = round(count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang then 1 end) * 100.0 / @chitieu, 2)"
                    + " , ty_le_6_1 = round(count(case when year(HCT_NGAYGAN) = @nam and month(HCT_NGAYGAN) <= @thang then 1 end) * 100.0 / count(case when year(HCT_NGAYGAN) = @nam - 1 and month(HCT_NGAYGAN) <= @thang then 1 end), 2)"
                    + " , nam_thong_ke = @nam"
                    + " , thang_thong_ke = @thang"
                    + " from CAPNUOCTANHOA.dbo.TB_THAYDHN"
                    + " where HCT_CODHNGAN >= 40 and isnull(HCT_TRONGAI,0)= 0");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_ThatThoatNuoc")]
        [HttpGet]
        public MResult ThongKe_ThatThoatNuoc(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @thang int = " + Thang
                    + " declare @nam int = " + Nam
                    + " declare @chitieu float = (SELECT TyLeThatThoatNuoc FROM TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @nam)"
                    + " select"
                    + "  ma_don_vi = 'TH'"
                    + " , ten_don_vi = N'Tân Hoà'"
                    + " , dvt = N'đhn'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam - 1)"
                    + " , khoi_luong_th_1 = avg(case when Nam = @nam - 1 and Ky <= @thang then TyLe end)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_nam = @chitieu"
                    + " , thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, @thang - 1) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_3 = avg(case when Nam = @nam and Ky = @thang - 1 then TyLe end)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, @thang - 1) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_4 = avg(case when Nam = @nam and Ky <= @thang - 1 then TyLe end)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @thang) + N' năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_5 = avg(case when Nam = @nam and Ky = @thang then TyLe end)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @thang) + N' tháng năm ' + convert(varchar, @nam)"
                    + " , khoi_luong_th_6 = avg(case when Nam = @nam and Ky <= @thang then TyLe end)"
                    + " , ty_le_6_2 = round(avg(case when Nam = @nam and Ky <= @thang then TyLe end) * 100 / @chitieu, 2)"
                    + " , ty_le_6_1 = round(avg(case when Nam = @nam and Ky <= @thang then TyLe end) * 100 / avg(case when Nam = @nam - 1 and Ky <= @thang then TyLe end), 2)"
                    + " , nam_thong_ke = @nam"
                    + " , thang_thong_ke = @thang"
                    + " from SERVER5.tanhoa.dbo.g_ThatThoatMangLuoi"
                    + " where Nam >= @nam - 1");
                result.data = JsonConvert.SerializeObject(dt);
                result.success = true;
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
            }
            return result;
        }

        [Route("ThongKe_HoaDon04")]
        [HttpGet]
        public MResult ThongKe_HoaDon04(string Thang, string Nam)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("declare @year int = " + Nam + ";"
                    + " declare @month int = " + Thang + ";"
                    + " with luyke as"
                    + " (select Ky, TIEUTHU=case when dc.TIEUTHU_BD is null then hd.TIEUTHU else dc.TIEUTHU_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON = hd.ID_HOADON where Nam = @year - 1 and Ky <= @month),"
                    + " hientai as"
                    + " (select Ky, TIEUTHU=case when dc.TIEUTHU_BD is null then hd.TIEUTHU else dc.TIEUTHU_BD end from HOADON_TA.dbo.HOADON hd left join HOADON_TA.dbo.DIEUCHINH_HD dc on dc.FK_HOADON = hd.ID_HOADON where Nam = @year and Ky<= @month)"
                    + " select t1.*,ty_le_6_2 = t1.khoi_luong_th_6 / t1.khoi_luong_th_nam * 100,ty_le_6_1 = t1.khoi_luong_th_1 / t1.khoi_luong_th_nam * 100 from"
                    + " (select ma_don_vi = 'TH', ten_don_vi = N'Tân Hoà', dvt = N'hóa đơn'"
                    + " , thuc_hien_1 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year - 1)"
                    + " , khoi_luong_th_1 = (select count(*) from luyke where TIEUTHU = 0)"
                    + " , ke_hoach_nam = N'Kế hoạch năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_nam = (select(select count(*) from hientai where KY = @month) * tylehoadon04 / 100 from TRUNGTAMKHACHHANG.dbo.BC_KeHoach where Nam = @year)"
                    + " , thuc_hien_3 = N'Thực hiện tháng ' + convert(varchar, (@month - 1)) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_Luong_th_3 = (select count(*) from hientai where TIEUTHU = 0 and KY = @month - 1)"
                    + " , thuc_hien_4 = N'Thực hiện ' + convert(varchar, (@month - 1)) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_4 = (select count(*) from hientai where TIEUTHU = 0 and KY <= @month - 1)"
                    + " , thuc_hien_5 = N'Thực hiện tháng ' + convert(varchar, @month) + N' năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_5 = (select count(*) from hientai where TIEUTHU = 0 and KY = @month)"
                    + " , thuc_hien_6 = N'Thực hiện ' + convert(varchar, @month) + N' tháng năm ' + convert(varchar, @year)"
                    + " , khoi_luong_th_6 = (select count(*) from hientai where TIEUTHU = 0 and KY <= @month))t1");
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
