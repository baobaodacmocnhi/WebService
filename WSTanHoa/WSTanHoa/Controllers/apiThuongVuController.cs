using System;
using System.Data;
using System.Web.Http;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/ThuongVu")]
    public class apiThuongVuController : ApiController
    {
        private CConnection _cDAL_ThuongVu = new CConnection(CGlobalVariable.ThuongVu);
        private wrThuongVu.wsThuongVu _wsThuongVu = new wrThuongVu.wsThuongVu();

        [Route("insertCCCDtoTCT")]
        [HttpGet]
        public bool insertCCCDtoTCT(string checksum)
        {
            try
            {
                if (CGlobalVariable.checksum == checksum)
                {
                    DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select * from ChungTu_ChiTiet where len(MaCT)=12 and malct=15 and Cat=0 and mact not in(select CCCD from CCCD_Temp) and DanhBo not like ''");
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            string result = "";
                            _wsThuongVu.them_CCCD(item["DanhBo"].ToString(), item["MaCT"].ToString(), out result);
                            _cDAL_ThuongVu.ExecuteNonQuery("insert into CCCD_Temp(CCCD,DanhBo,Result,ModifyDate)values('" + item["MaCT"].ToString() + "','" + item["DanhBo"].ToString() + "',N'" + result + "',getdate())");
                        }
                        catch
                        {
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}