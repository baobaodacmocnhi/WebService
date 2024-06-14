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

        [Route("")]
        [HttpGet]
        public MResult ThongKe_KhieuNai(string CreateDate)
        {
            MResult result = new MResult();
            try
            {
                DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select * from KTKS_DonKH.dbo.DonTu dt,KTKS_DonKH.dbo.DonTu_ChiTiet dtct
where CAST(dt.CreateDate as date) >= '20240101' and dt.MaDon = dtct.MaDon
and(select count(*) from KTKS_DonKH.dbo.DonTu_ChiTiet where KTKS_DonKH.dbo.DonTu_ChiTiet.MaDon = dt.MaDon) = 1 and Name_NhomDon_PKH not like ''
and(select stuff((select ';' + cast(NameGroup as nvarchar(100)) FROM[KTKS_DonKH].[dbo].[NhomDon] where KhieuNai = 1 group by NameGroup for xml path('')),1,1,'')) like '%' + Name_NhomDon_PKH + '%'");
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
        public MResult ThongKe_SanLuong(string Thang,string Nam)
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
