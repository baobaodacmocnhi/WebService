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
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class ZaloChatController : Controller
    {
        private CConnection _cDAL_TrungTam = new CConnection(CGlobalVariable.TrungTamKhachHang);
        // GET: ZaloChat
        public ActionResult Index()
        {
            if (Session["ID"] == null)
            {
                Session["Url"] = Request.Url;
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public string getTimKiem([FromBody] string NoiDungTimKiem)
        {
            try
            {
                string sql = "select zc.IDZalo,zq.Avatar,zq.[Name],zc.CreateDate,zc.NguoiGui,zc.NoiDung"
                        + " from Zalo_Chat zc left"
                        + " join Zalo_QuanTam zq on zc.IDZalo = zq.IDZalo"
                        + " where zc.NoiDung like N'%" + NoiDungTimKiem + "%'"
                        + " order by zc.CreateDate desc";
                DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable(sql);
                List<ZaloView> lst = new List<ZaloView>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ZaloView en = new ZaloView();
                    en.STT = (i + 1).ToString();
                    en.IDZalo = dt.Rows[i]["IDZalo"].ToString();
                    en.Avatar = dt.Rows[i]["Avatar"].ToString();
                    en.Name = dt.Rows[i]["Name"].ToString();
                    en.CreateDate = dt.Rows[i]["CreateDate"].ToString();
                    en.NguoiGui = dt.Rows[i]["NguoiGui"].ToString();
                    en.NoiDung = dt.Rows[i]["NoiDung"].ToString();
                    lst.Add(en);
                }
                return CGlobalVariable.jsSerializer.Serialize(lst);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string getChat([FromBody] string IDZalo)
        {
            try
            {
                string sql = "select zc.IDZalo,zq.Avatar,zq.[Name],zc.CreateDate,zc.NguoiGui,zc.NoiDung,zc.Image"
                        + " ,DienThoai=(select top 1 DienThoai from Zalo_DangKy where IDZalo=zq.IDZalo)"
                        + " from Zalo_Chat zc left"
                        + " join Zalo_QuanTam zq on zc.IDZalo = zq.IDZalo"
                        + " where zc.IDZalo = " + IDZalo
                        + " order by zc.CreateDate desc";
                DataTable dt = _cDAL_TrungTam.ExecuteQuery_DataTable(sql);
                List<ZaloView> lst = new List<ZaloView>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ZaloView en = new ZaloView();
                    en.STT = (i + 1).ToString();
                    en.IDZalo = dt.Rows[i]["IDZalo"].ToString();
                    en.Avatar = dt.Rows[i]["Avatar"].ToString();
                    en.Name = dt.Rows[i]["Name"].ToString();
                    en.CreateDate = dt.Rows[i]["CreateDate"].ToString();
                    en.NguoiGui = dt.Rows[i]["NguoiGui"].ToString();
                    en.NoiDung = dt.Rows[i]["NoiDung"].ToString();
                    en.Image = dt.Rows[i]["Image"].ToString();
                    en.DienThoai = dt.Rows[i]["DienThoai"].ToString();
                    lst.Add(en);
                }
                return CGlobalVariable.jsSerializer.Serialize(lst);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}