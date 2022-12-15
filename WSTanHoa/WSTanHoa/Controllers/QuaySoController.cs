using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class QuaySoController : Controller
    {
        private CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=DH_CODONG;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        // GET: QuaySo
        public ActionResult Index()
        {
            //Random random = new Random();
            //DataTable dt = null;
            //while (dt == null || dt.Rows.Count == 0)
            //{
            //    dt = _cDAL.ExecuteQuery_DataTable("select top 1 STT=right('000' + cast(STT as varchar(3)), 3),DonVi,HoTen from QuaySo where Quay=0 ORDER BY NEWID()");
            //}
            ////_cDAL.ExecuteNonQuery("update QuaySo set Quay=1 where STT=" + dt.Rows[0]["STT"].ToString());
            //ViewBag.num1 = dt.Rows[0]["STT"].ToString().Substring(0, 1);
            //ViewBag.num2 = dt.Rows[0]["STT"].ToString().Substring(1, 1);
            //ViewBag.num3 = dt.Rows[0]["STT"].ToString().Substring(2, 1);
            //ViewBag.donvi = dt.Rows[0]["DonVi"].ToString();
            //ViewBag.hoten = dt.Rows[0]["HoTen"].ToString();
            return View();
        }

        public string getQuay()
        {
            try
            {
                List<MView> vKhongTinHieu = new List<MView>();
                DataTable dt = null;
                while (dt == null || dt.Rows.Count == 0)
                {
                    dt = _cDAL.ExecuteQuery_DataTable("select top 1 STT=right('000' + cast(STT as varchar(3)), 3),DonVi,HoTen from QuaySo where Quay=0 ORDER BY NEWID()");
                }
                _cDAL.ExecuteNonQuery("update QuaySo set Quay=1 where STT=" + dt.Rows[0]["STT"].ToString());
                MView en = new MView();
                en.ChiSo = dt.Rows[0]["STT"].ToString().Substring(0, 1);
                en.DanhBo = dt.Rows[0]["STT"].ToString().Substring(1, 1);
                en.DiaChi = dt.Rows[0]["STT"].ToString().Substring(2, 1);
                en.HoTen = dt.Rows[0]["DonVi"].ToString();
                en.NoiDung = dt.Rows[0]["HoTen"].ToString().ToUpper();
                vKhongTinHieu.Add(en);
                return CGlobalVariable.jsSerializer.Serialize(vKhongTinHieu);
            }
            catch
            {
                return null;
            }
        }

    }
}