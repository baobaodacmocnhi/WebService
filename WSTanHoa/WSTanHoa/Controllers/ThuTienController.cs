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
    [RoutePrefix("api/ThuTien")]
    public class ThuTienController : ApiController
    {
        CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");

       /// <summary>
       /// Lấy Hóa Đơn theo số lượng kỳ truyền vào
       /// </summary>
       /// <param name="DanhBo"></param>
       /// <param name="SoKy">Số Kỳ cần lấy</param>
       /// <returns></returns>
        [Route("getHoaDon")]
        public IList<HoaDon> getHoaDon(string DanhBo,string SoKy)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = _cDAL.ExecuteQuery_DataTable("select top("+SoKy+") HoTen=TENKH,DiaChi=(SO+' '+DUONG),MaHD=ID_HOADON,SOHOADON,DanhBo=DANHBA,NAM,KY,GIABAN,ThueGTGT=THUE,PhiBVMT=PHI,TONGCONG,NGAYGIAITRACH from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<HoaDon> hoadons = new List<HoaDon>();
                foreach (DataRow item in dt.Rows)
                {
                    HoaDon entity = new HoaDon();
                    entity.HoTen = item["HoTen"].ToString();
                    entity.DiaChi = item["DiaChi"].ToString();
                    entity.MaHD = int.Parse(item["MaHD"].ToString());
                    entity.SoHoaDon = item["SoHoaDon"].ToString();
                    entity.DanhBo = (string)item["DanhBo"];
                    entity.Nam = int.Parse(item["Nam"].ToString());
                    entity.Ky = int.Parse(item["Ky"].ToString());
                    entity.GiaBan = int.Parse(item["GiaBan"].ToString());
                    entity.ThueGTGT = int.Parse(item["ThueGTGT"].ToString());
                    entity.PhiBVMT = int.Parse(item["PhiBVMT"].ToString());
                    entity.TongCong = int.Parse(item["TongCong"].ToString());
                    DateTime? date = null;
                    if (item["NGAYGIAITRACH"].ToString()!=null)
                    entity.NgayGiaiTrach = DateTime.Parse(item["NGAYGIAITRACH"].ToString());
                    hoadons.Add(entity);
                }
                return hoadons;
            }
            else
            {
                ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorKhongDung, ErrorResponse.ErrorCodeKhongDung);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            }
        }
    }
}
