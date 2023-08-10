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
    public class TLMDController : Controller
    {
        private CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TrungTamKhachHang);

        // GET: TLMD
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ThiCong()
        {
            return View();
        }

        public ActionResult DonViThiCong()
        {
            List<MDonViThiCong> lst = new List<MDonViThiCong>();
            DataTable dt = _cDAL_TTKH.ExecuteQuery_DataTable("select * from TLMD_DonViThiCong");
            int STT = 1;
            foreach (DataRow item in dt.Rows)
            {
                MDonViThiCong en = new MDonViThiCong();
                en.STT = STT++;
                en.ID = int.Parse(item["ID"].ToString());
                en.Name = item["Name"].ToString();
                en.DaiDien = item["DaiDien"].ToString();
                en.DienThoai = item["DienThoai"].ToString();
                en.Username = item["Username"].ToString();
                en.Password = item["Password"].ToString();
                en.Active = bool.Parse(item["Active"].ToString());
                int a = -1;
                int.TryParse(item["CreateBy"].ToString(), out a);
                en.CreateBy = a;
                en.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                int.TryParse(item["ModifyBy"].ToString(), out a);
                en.ModifyBy = a;
                DateTime date;
                DateTime.TryParse(item["ModifyDate"].ToString(), out date);
                en.ModifyDate = date;
                lst.Add(en);
            }
            return View(lst);

        }

    }
}