using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WSTanHoa.Models.db;

namespace WSTanHoa.Controllers
{
    public class ZaloChatController : Controller
    {
        dbTrungTamKhachHang db = new dbTrungTamKhachHang();
        CConnection _cDAL_TTKH = new CConnection(System.Configuration.ConfigurationManager.ConnectionStrings["dbTrungTamKhachHang"].ConnectionString);
        // GET: ZaloChat
        public async Task<ActionResult> Index([FromBody]string value)
        {
            string sql = "select distinct zc.IDZalo,zq.Avatar,zq.[Name],zc.CreateDate"
+ " from Zalo_Chat zc left"
+ " join Zalo_QuanTam zq on zc.IDZalo = zq.IDZalo"
+ " where CAST(zc.CreateDate as date) = '" + new DateTime(2021, 08, 21).ToString("yyyyMMdd") + "'";
            DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable(sql);
            List<ZaloView> lst = new List<ZaloView>();
            for (int i= 0; i < dt.Rows.Count; i++)
            {
                ZaloView en = new ZaloView();
                en.STT = (i + 1).ToString();
                en.IDZalo = dt.Rows[i]["IDZalo"].ToString();
                en.Avatar = dt.Rows[i]["Avatar"].ToString();
                en.Name = dt.Rows[i]["Name"].ToString();
                en.CreateDate = dt.Rows[i]["CreateDate"].ToString();
                lst.Add(en);
            }
            return View(lst);
        }
    }
}