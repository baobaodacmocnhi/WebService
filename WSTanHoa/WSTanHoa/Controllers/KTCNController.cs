using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSTanHoa.Models;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/KTCN")]
    public class KTCNController : ApiController
    {
        CConnection _cDAL = new CConnection("Data Source=serverg8-01;Initial Catalog=KTCN;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");

        /// <summary>
        /// Lấy Lịch Cúp Nước chưa được gửi
        /// </summary>
        /// <returns></returns>
        [Route("getLichCupNuoc")]
        private IList<LichCupNuoc> getLichCupNuoc()
        {
            DataTable dt = new DataTable();
            try
            {
                dt = _cDAL.ExecuteQuery_DataTable("select ID,NoiDung,TuNgay,DenNgay,MaQuan,TenQuan,MaPhuong,TenPhuong,Gui from LichCupNuoc where Gui=0");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<LichCupNuoc> lichs = new List<LichCupNuoc>();
                foreach (DataRow item in dt.Rows)
                {
                    LichCupNuoc entity = new LichCupNuoc();
                    entity.ID = int.Parse(item["ID"].ToString());
                    entity.NoiDung = item["NoiDung"].ToString();
                    entity.TuNgay = DateTime.Parse(item["TuNgay"].ToString());
                    entity.DenNgay = DateTime.Parse(item["DenNgay"].ToString());
                    entity.MaQuan = int.Parse(item["MaQuan"].ToString());
                    entity.TenQuan = item["TenQuan"].ToString();
                    entity.MaPhuong = item["MaPhuong"].ToString();
                    entity.TenPhuong = item["TenPhuong"].ToString();
                    entity.Gui = bool.Parse(item["Gui"].ToString());
                    lichs.Add(entity);
                }
                return lichs;
            }
            else
            {
                ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorKhongDung, ErrorResponse.ErrorCodeKhongDung);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            }
        }

        /// <summary>
        /// Cập Nhật đã Gửi Thông Báo
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("updateDaGui")]
        [HttpPost]
        private bool updateDaGui(int ID)
        {
            try
            {
                _cDAL.ExecuteNonQuery("update LichCupNuoc set Gui=1 where ID="+ID);
                return true;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
        }


    }
}
