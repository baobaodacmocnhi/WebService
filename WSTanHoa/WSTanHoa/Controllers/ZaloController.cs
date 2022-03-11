using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Net.Http.Headers;
using WSTanHoa.Models.db;

namespace WSTanHoa.Controllers
{
    public class ZaloController : Controller
    {
        decimal IDZalo = -1;
        private dbTrungTamKhachHang db = new dbTrungTamKhachHang();
        private CConnection cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private CConnection cDAL_TrungTam = new CConnection(CGlobalVariable.TrungTamKhachHang);

        // GET: Zalo
        public async Task<ActionResult> Index(decimal? id, [Bind(Include = "IDZalo,DanhBo")] ZaloView zalo, string action)
        {
            if (TempData["IDZalo"] == null)
            {
                if (id != null)
                    TempData["IDZalo"] = id.Value;
            }

            if (TempData["IDZalo"] != null)
                IDZalo = decimal.Parse(TempData["IDZalo"].ToString());

            DataTable dtTTKH = cDAL_TrungTam.ExecuteQuery_DataTable("select IDZalo,z.DienThoai,z.DanhBo,ttkh.HoTen,DiaChi = SONHA + ' ' + TENDUONG from Zalo_DangKy z, [SERVER8].[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh where ttkh.DanhBo=z.DanhBo and IDZalo=" + IDZalo);
            //List<ZaloView> lstZalo = new List<ZaloView>();
            foreach (DataRow item in dtTTKH.Rows)
            {
                ZaloView en = new ZaloView();
                en.IDZalo = item["IDZalo"].ToString();
                en.DanhBo = item["DanhBo"].ToString();
                en.HoTen = item["HoTen"].ToString();
                en.DiaChi = item["DiaChi"].ToString();
                //en.DienThoai = item["DienThoai"].ToString();
                zalo.lst.Add(en);
            }

            if (ModelState.IsValid && !String.IsNullOrWhiteSpace(action))
                if (TempData["IDZalo"] != null)
                {
                    switch (action)
                    {
                        case "Kiểm Tra":
                            if (zalo.DanhBo != null && zalo.DanhBo != "")
                            {
                                DataTable dt = cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + zalo.DanhBo.Replace(" ", "") + "' order by ID_HOADON desc");
                                if (dt.Rows.Count > 0)
                                {
                                    zalo.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                    zalo.HoTen = dt.Rows[0]["HoTen"].ToString();
                                    zalo.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                    return View(zalo);
                                }
                                else
                                {
                                    ModelState.AddModelError("DanhBo", "Danh Bộ không đúng");
                                    return View(zalo);
                                }
                            }
                            break;
                        case "Đăng Ký":
                            if (zalo.DanhBo != null && zalo.DanhBo != "")
                            {
                                //kiểm tra danh bộ
                                DataTable dt = cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG) from HOADON where DANHBA='" + zalo.DanhBo.Replace(" ", "") + "' order by ID_HOADON desc");
                                if (dt.Rows.Count > 0)
                                {
                                    zalo.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                                    zalo.HoTen = dt.Rows[0]["HoTen"].ToString();
                                    zalo.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                                }
                                else
                                {
                                    ModelState.AddModelError("DanhBo", "Danh Bộ không đúng");
                                    return View(zalo);
                                }
                                //kiểm tra trùng
                                if (db.Zalo_DangKy.Count(item => item.IDZalo == IDZalo && item.DanhBo == zalo.DanhBo.Replace(" ", "")) == 0)
                                {
                                    //if (zalo.DienThoai == null || zalo.DienThoai == "" )
                                    //{
                                    //    ModelState.AddModelError("DienThoai", "Vui lòng nhập số điện thoại");
                                    //    return View(zalo);
                                    //}
                                    //else
                                    //    if( zalo.DienThoai.Length < 10)
                                    //{
                                    //    ModelState.AddModelError("DienThoai", "Số điện thoại không đủ 10 ký tự");
                                    //    return View(zalo);
                                    //}
                                    Zalo_DangKy en = new Zalo_DangKy();
                                    en.IDZalo = IDZalo;
                                    en.DanhBo = zalo.DanhBo.Replace(" ", "");
                                    //en.DienThoai = zalo.DienThoai.Replace(" ", "");
                                    en.CreateDate = DateTime.Now;
                                    if (IDZalo != -1)
                                    {
                                        db.Zalo_DangKy.Add(en);
                                    }
                                    await db.SaveChangesAsync();
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    ModelState.AddModelError("DanhBo", "Danh Bộ đã đăng ký rồi");
                                    return View(zalo);
                                }
                            }
                            break;
                    }
                }

            return View(zalo);
        }

        public async Task<ActionResult> Delete(decimal? IDZalo, string DanhBo)
        {
            if (IDZalo == null && (DanhBo == null || DanhBo == ""))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zalo_DangKy zalo = await db.Zalo_DangKy.FindAsync(IDZalo, DanhBo);
            if (zalo == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Zalo_DangKy.Remove(zalo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Zalo", new { id = IDZalo });
            }
            //return View(zalo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //[HttpPost]
        //public async Task<ActionResult> Webhook()
        //{
        //    try
        //    {
        //        Stream req = Request.InputStream;
        //        req.Seek(0, System.IO.SeekOrigin.Begin);
        //        string json = new StreamReader(req, Encoding.UTF8, false).ReadToEnd();
        //        var details = JObject.Parse(json);

        //        string idzalo = "", message = "";
        //        if (details["event_name"].ToString() == "follow" || details["event_name"].ToString() == "unfollow")
        //        {
        //            idzalo = details["follower"]["id"].ToString();
        //            message = "";
        //        }
        //        else
        //        if (details["event_name"].ToString() == "user_send_text" || details["event_name"].ToString() == "user_send_image")
        //        {
        //            idzalo = details["sender"]["id"].ToString();
        //            message = details["message"]["text"].ToString();
        //        }
        //        else
        //        if (details["event_name"].ToString() == "oa_send_text" || details["event_name"].ToString() == "oa_send_image")
        //        {
        //            idzalo = details["recipient"]["id"].ToString();
        //            message = details["message"]["text"].ToString();
        //        }
        //        //log4net.ILog _log = log4net.LogManager.GetLogger("File");
        //        //_log.Debug("link: " + "https://service.cskhtanhoa.com.vn/api/Zalo/webhook?IDZalo=" + idzalo + "&event_name=" + details["event_name"] + "&message=" + message.Replace("#", "$"));

        //        //using (var client = new HttpClient())
        //        //{
        //        //    //Passing service base url  
        //        //    client.BaseAddress = new Uri("https://service.cskhtanhoa.com.vn");

        //        //    client.DefaultRequestHeaders.Clear();
        //        //    //Define request data format  
        //        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
        //        //    HttpResponseMessage Res = await client.PostAsync("api/Zalo/webhook?IDZalo=" + idzalo + "&event_name=" + details["event_name"] + "&message=" + message.Replace("#", "$"), null);

        //        //    //Checking the response is successful or not which is sent using HttpClient  
        //        //    if (Res.IsSuccessStatusCode)
        //        //    {
        //        //        //_log.Debug(Res);
        //        //    }
        //        //    //returning the employee list to view  
        //        //}
        //    }
        //    catch (Exception) { }
        //    return View();
        //}


    }
}
