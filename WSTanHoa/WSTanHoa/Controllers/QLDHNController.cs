using ClosedXML.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class QLDHNController : Controller
    {
        private CConnection cDAL_DHN = new CConnection(CGlobalVariable.DHNWFH);
        private CConnection cDAL_DocSo = new CConnection(CGlobalVariable.DocSoWFH);
        private CConnection cDAL_sDHN = new CConnection(CGlobalVariable.sDHNWFH);
        apiTrungTamKhachHangController apiTTKH = new apiTrungTamKhachHangController();

        public ActionResult BaoChiSoNuoc(string function, string DanhBo, string ChiSo, HttpPostedFileBase Hinh)
        {
            if (function == "KiemTra")
            {
                if (DanhBo != null && DanhBo.Replace(" ", "").Replace("-", "") != "")
                {
                    DataTable dt = cDAL_DHN.ExecuteQuery_DataTable("select MLT=LOTRINH,DanhBo,HoTen,DiaChi = SONHA + ' ' + TENDUONG from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo.Replace(" ", "").Replace("-", "") + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ViewBag.DanhBo = dt.Rows[0]["DanhBo"];
                        ViewBag.HoTen = dt.Rows[0]["HoTen"];
                        ViewBag.DiaChi = dt.Rows[0]["DiaChi"];
                    }
                    else
                        ModelState.AddModelError("", "Danh Bộ không tồn tại");
                }
            }
            else
                if (function == "Gui")
            {
                if (DanhBo == "")
                    ModelState.AddModelError("", "Thiếu Danh Bộ");
                if (ChiSo == "")
                    ModelState.AddModelError("", "Thiếu Chỉ Số Nước");
                if (Hinh == null || Hinh.ContentLength <= 0)
                    ModelState.AddModelError("", "Thiếu Chỉ Hình ĐHN");
                if (DanhBo != "" && ChiSo != "" && Hinh != null && Hinh.ContentLength > 0)
                {
                    DataTable dt = cDAL_DHN.ExecuteQuery_DataTable("select MLT=LOTRINH,DanhBo,HoTen,DiaChi = SONHA + ' ' + TENDUONG from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo.Replace(" ", "").Replace("-", "") + "'");
                    DataRow drLich = apiTTKH.getLichDocSo_Func_DataRow(dt.Rows[0]["DanhBo"].ToString(), dt.Rows[0]["MLT"].ToString());
                    //nếu trước 1 ngày
                    if (DateTime.Now.Date < DateTime.Parse(drLich["NgayDoc"].ToString()).Date.AddDays(-1))
                    {
                        ModelState.AddModelError("", "Chưa đến kỳ đọc số tiếp theo, Vui lòng tra cứu lịch đọc số");
                        return View();
                    }
                    else
                    //nếu sau 12h ngày chuyển listing
                    if (DateTime.Now.Date == DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date && DateTime.Now.Hour > 11)
                    {
                        ModelState.AddModelError("", "Đã quá thời gian ghi chỉ số");
                        return View();
                    }
                    else
                    //nếu sau ngày chuyển listing
                    if (DateTime.Now.Date > DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date)
                    {
                        ModelState.AddModelError("", "Chưa đến kỳ đọc số tiếp theo, Vui lòng tra cứu lịch đọc số");
                        return View();
                    }
                    //kiểm tra đã gửi chỉ số nước rồi
                    DataTable dtResult = cDAL_DocSo.ExecuteQuery_DataTable("select top 1 * from DocSo where DocSoID='" + drLich["Nam"].ToString() + int.Parse(drLich["Ky"].ToString()).ToString("00") + dt.Rows[0]["DanhBo"].ToString() + "' and (CodeMoi not like '' or ChuBao=1)");
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    //if (DateTime.Parse(dtResult.Rows[0]["CreateDate"].ToString()).Date >= DateTime.Parse(drLich["NgayDoc"].ToString()).Date.AddDays(-1)
                    //    || DateTime.Parse(dtResult.Rows[0]["CreateDate"].ToString()).Date == DateTime.Parse(drLich["NgayChuyenListing"].ToString()).Date)
                    {
                        ModelState.AddModelError("", "Danh Bộ này đã gửi/ghi chỉ số nước rồi");
                        return View();
                    }

                    Image image = Image.FromStream(Hinh.InputStream);
                    Bitmap resizedImage = resizeImage(image, 0.5m);
                    SqlCommand command = new SqlCommand("insert into DocSo_Web(DanhBo,Nam,Ky,Dot,ChiSo)values('" + dt.Rows[0]["DanhBo"].ToString() + "'," + drLich["Nam"].ToString() + "," + drLich["Ky"].ToString() + "," + drLich["Dot"].ToString() + "," + ChiSo + ")");
                    //SqlCommand command = new SqlCommand("insert into DocSo_Web(DanhBo,Nam,Ky,Dot,ChiSo,Hinh)values('" + dt.Rows[0]["DanhBo"].ToString() + "'," + drLich["Nam"].ToString() + "," + drLich["Ky"].ToString() + "," + drLich["Dot"].ToString() + "," + ChiSo + ",@Hinh)");
                    //command.Parameters.Add("@Hinh", SqlDbType.Image).Value = ImageToByte(resizedImage);
                    bool result = cDAL_DocSo.ExecuteNonQuery(command);
                    wrDHN.wsDHN wsDHN = new wrDHN.wsDHN();
                    string jsonresult = wsDHN.ghiChiSo(drLich["Nam"].ToString() + int.Parse(drLich["Ky"].ToString()).ToString("00") + dt.Rows[0]["DanhBo"].ToString(), "40", ChiSo, Convert.ToBase64String(ImageToByte(resizedImage)), int.Parse(drLich["Dot"].ToString()).ToString("00"), "Chủ Báo", "0");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(jsonresult);
                    if (obj["success"] == true)
                    {
                        cDAL_DocSo.ExecuteNonQuery("update DocSo set ChuBao=1 where DocSoID='" + drLich["Nam"].ToString() + int.Parse(drLich["Ky"].ToString()).ToString("00") + dt.Rows[0]["DanhBo"].ToString() + "'");
                        ModelState.AddModelError("", "Thành Công, Cám ơn Quý Khách Hàng đã cung cấp chỉ số nước");
                        object UID = cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 UID from NguoiDung where May=(select PhanMay from DocSo where DocSoID='" + drLich["Nam"].ToString() + int.Parse(drLich["Ky"].ToString()).ToString("00") + dt.Rows[0]["DanhBo"].ToString() + "') order by MaND asc");
                        wsDHN.SendNotificationToClient("Chủ Báo", dt.Rows[0]["MLT"].ToString() + "-" + dt.Rows[0]["DanhBo"].ToString() + "-" + dt.Rows[0]["DiaChi"].ToString(), UID.ToString(), "ChuBao", "name", obj["message"], drLich["Nam"].ToString() + int.Parse(drLich["Ky"].ToString()).ToString("00") + dt.Rows[0]["DanhBo"].ToString());
                    }
                    else
                        ModelState.AddModelError("", "Thất Bại, Vui lòng thử lại");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult BaoChiSoNuoc_Insert(string DanhBo, string ChiSo, HttpPostedFileBase Hinh)
        {
            //string DanhBo = this.Request.QueryString["DanhBo"];
            //string ChiSo = this.Request.QueryString["ChiSo"];
            //string Hinh = this.Request.QueryString["Hinh"];

            return View();
        }

        [NonAction]
        public SelectList ToSelectList(DataTable table, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new SelectListItem()
                {
                    Text = row[textField].ToString(),
                    Value = row[valueField].ToString()
                });
            }

            return new SelectList(list, "Value", "Text");
        }

        public ActionResult sDHN(FormCollection collection, string function, string TuNgay, string DenNgay, string NgayXem)
        {
            DataTable dtNCC = cDAL_sDHN.ExecuteQuery_DataTable("select ID,Name from sDHN_NCC");
            ViewBag.NCC = ToSelectList(dtNCC, "ID", "Name");
            dtNCC = cDAL_sDHN.ExecuteQuery_DataTable("select DMA=MADMA from sDHN sdhn,[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh"
                 + " where Valid = 1 and sdhn.DanhBo = ttkh.DANHBO"
                 + " group by MADMA"
                 + " order by MADMA");
            ViewBag.DMA = ToSelectList(dtNCC, "DMA", "DMA");

            object soluong = cDAL_sDHN.ExecuteQuery_ReturnOneValue("select SoLuong=COUNT(*) from sDHN_NCC a,sDHN b,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG c"
                                    + " where a.ID = b.IDNCC and b.DanhBo = c.DANHBO and Valid = 1");
            ViewBag.SoLuong = soluong;
            DataTable dt = cDAL_sDHN.ExecuteQuery_DataTable("select b.IDNCC,Name,SoLuong=COUNT(*) from sDHN_NCC a,sDHN b,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG c"
                                    + " where a.ID = b.IDNCC and b.DanhBo = c.DANHBO and Valid = 1"
                                    + " group by IDNCC,Name"
                                    + " order by IDNCC");
            List<MView> vTong = new List<MView>();
            foreach (DataRow item in dt.Rows)
            {
                MView en = new MView();
                en.TieuDe = item["Name"].ToString();
                en.SoLuong = item["SoLuong"].ToString();
                vTong.Add(en);
            }
            ViewBag.vTong = vTong;
            //Lịch Sử
            DateTime date = DateTime.Now.AddDays(-1);
            if (NgayXem != null && NgayXem != "")
            {
                string[] datestr = NgayXem.Split('/');
                date = new DateTime(int.Parse(datestr[2]), int.Parse(datestr[1]), int.Parse(datestr[0]));
            }
            @ViewBag.NgayXem = date.ToString("dd/MM/yyyy");
            //List<MView> vLichSu = new List<MView>();
            //int count = 7;
            //while (count > 0)
            //{
            //MView enLichSuChild;
            //if (dtBinhThuong != null && dtBinhThuong.Rows.Count > 0)
            //{
            //    enLichSuChild = new MView();
            //    enLichSuChild.TieuDe = dtBinhThuong.Rows[0]["Loai"].ToString();
            //    enLichSuChild.SoLuong = dtBinhThuong.Rows.Count.ToString();
            //    enLichSuChild.NoiDung = dtBinhThuong.Rows[0]["Loai2"].ToString();
            //    enLichSu.lst.Add(enLichSuChild);
            //}
            //if (dtBatThuong != null && dtBatThuong.Rows.Count > 0)
            //{
            //    enLichSuChild = new MView();
            //    enLichSuChild.TieuDe = dtBatThuong.Rows[0]["Loai"].ToString();
            //    enLichSuChild.SoLuong = dtBatThuong.Rows.Count.ToString();
            //    enLichSuChild.NoiDung = dtBatThuong.Rows[0]["Loai2"].ToString();
            //    enLichSu.lst.Add(enLichSuChild);
            //}
            //if (dtKhongTinHieu != null && dtKhongTinHieu.Rows.Count > 0)
            //{
            //    enLichSuChild = new MView();
            //    enLichSuChild.TieuDe = dtKhongTinHieu.Rows[0]["Loai"].ToString();
            //    enLichSuChild.SoLuong = dtKhongTinHieu.Rows.Count.ToString();
            //    enLichSuChild.NoiDung = dtKhongTinHieu.Rows[0]["Loai2"].ToString();
            //    enLichSu.lst.Add(enLichSuChild);
            //}
            //    count--;
            //    date = date.AddDays(-1);
            //    vLichSu.Add(enLichSu);
            //}
            //ViewBag.vLichSu = vLichSu;
            if (function == "Excel")
            {
                string filename = "";
                string sql = "select ttkh.DanhBo,DiaChi=SoNha+' '+TenDuong,HoTen,MaDMA,CoDHN=CoDH,Hieu_DHTM,Loai_DHTM,SoThanDH,KieuPhatSong,IDLogger"
                    + ",DVLAPDAT,NHA_CCDHN,NHA_TICHHOP,XUATXU,NGAYKIEMDINH = CONVERT(varchar(10), NGAYKIEMDINH, 103),NGAYTHAY = CONVERT(varchar(10), NGAYTHAY, 103)";
                if (collection["KyHD"].ToString() == "")
                {
                    string[] fromdatestr = collection["TuNgay"].ToString().Split('/');
                    DateTime fromdate = new DateTime(int.Parse(fromdatestr[2]), int.Parse(fromdatestr[1]), int.Parse(fromdatestr[0]));
                    string[] todatestr = collection["DenNgay"].ToString().Split('/');
                    DateTime todate = new DateTime(int.Parse(todatestr[2]), int.Parse(todatestr[1]), int.Parse(todatestr[0]));
                    if (collection["Hour"].ToString() == "")
                    {
                        while (fromdate.Date <= todate.Date)
                        {
                            sql += ",'" + fromdate.ToString("dd/MM/yyyy") + "'=(select count(DanhBo) from sDHN_LichSu where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)='" + fromdate.ToString("yyyyMMdd") + "')";
                            fromdate = fromdate.AddDays(1);
                        }
                    }
                    else
                    {
                        while (fromdate.Date <= todate.Date)
                        {
                            sql += ",ChiSo=(select top 1 ChiSo from sDHN_LichSu where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)='" + fromdate.ToString("yyyyMMdd") + "' and DATEPART(HOUR, ThoiGianCapNhat)=" + collection["Hour"].ToString() + ")"
                            + ",ThoiGian=(select top 1 ThoiGianCapNhat from sDHN_LichSu where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)='" + fromdate.ToString("yyyyMMdd") + "' and DATEPART(HOUR, ThoiGianCapNhat)=" + collection["Hour"].ToString() + ")";
                            fromdate = fromdate.AddDays(1);
                        }
                    }
                }
                else
                {
                    string[] KyHDs = collection["KyHD"].ToString().Split(';');
                    foreach (string item in KyHDs)
                    {
                        string[] KyHD = item.Split('/');
                        sql += ",CSDocSo=(select CSMoi from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + ")"
                            + ",ThoiGianDocSo=(select GioGhi from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + ")"
                            + ",CSsDHN=(select top 1 ChiSo from sDHN_LichSu where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)=(select cast(GioGhi as date) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + ") and DATEPART(HOUR, ThoiGianCapNhat)=(select DATEPART(HOUR, gioghi) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "))"
                            + ",ThoiGiansDHN=(select top 1 ThoiGianCapNhat from sDHN_LichSu where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)=(select cast(GioGhi as date) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + ") and DATEPART(HOUR, ThoiGianCapNhat)=(select DATEPART(HOUR, gioghi) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "))";
                    }
                }
                sql += " from sDHN sdhn,[DHTM_THONGTIN] ttdhn,[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh"
                    + " where Valid=1 and sdhn.IDNCC=ttdhn.ID and sdhn.DanhBo=ttkh.DANHBO";
                if (collection.AllKeys.Contains("chkBoDaNghiemThu"))
                    sql += " and sdhn.DanhBo not in (select DanhBo from DHTM_NGHIEMTHU)";
                if (collection["radLoai"].ToString() == "radNCC")
                {
                    sql += " and sdhn.IDNCC=" + collection["NCC"].ToString();
                    filename = cDAL_sDHN.ExecuteQuery_ReturnOneValue("select Name from sDHN_NCC where ID=" + collection["NCC"].ToString()).ToString();
                }
                else
                    if (collection["radLoai"].ToString() == "radDMA")
                {
                    sql += " and madma='" + collection["DMA"].ToString() + "'";
                    filename = collection["DMA"].ToString();
                }
                else
                    if (collection["radLoai"].ToString() == "radNghiemThuTD")
                {
                    sql += " and sdhn.DanhBo in (select DanhBo from DHTM_NGHIEMTHU_TD)";
                    filename = "NghiemThuTD";
                }
                sql += " order by IDNCC";
                dt = cDAL_sDHN.ExecuteQuery_DataTable(sql);
                dt.TableName = "Sheet1";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename= TanHoa.sDHN." + filename + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return View();
        }

        public string getKhongTinHieu_sDHN(string SoNgay, string Ngay)
        {
            try
            {
                List<MView> vKhongTinHieu = new List<MView>();
                DataTable dt = getDS_sDHN_KhongTinHieu("", "", SoNgay, Ngay);
                DataTable dtNCC = cDAL_sDHN.ExecuteQuery_DataTable("select * from sDHN_NCC");
                foreach (DataRow item in dtNCC.Rows)
                {
                    DataRow[] dr = dt.Select("IDNCC=" + item["ID"]);
                    if (dr != null && dr.Count() > 0)
                    {
                        MView en = new MView();
                        en.TieuDe = item["Name"].ToString();
                        en.SoLuong = dr.Count().ToString();
                        en.NoiDung = item["ID"].ToString();
                        vKhongTinHieu.Add(en);
                    }
                }
                return CGlobalVariable.jsSerializer.Serialize(vKhongTinHieu);
            }
            catch
            {
                return null;
            }
        }

        [NonAction]
        public DataTable getDS_sDHN_KhongTinHieu(string function, string NCC, string SoNgay, string Ngay)
        {
            string[] datestr = Ngay.Split('/');
            DateTime date = new DateTime(int.Parse(datestr[2]), int.Parse(datestr[1]), int.Parse(datestr[0]));
            List<MView> vKhongTinHieu = new List<MView>();
            int count = int.Parse(SoNgay);
            string sql = "";
            if (function == "export")
            {
                sql = "select t1.*,HieuDHN=ttsdhn.HIEU_DHTM,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA from"
                    + " (select IDNCC,NCC=ncc.Name,t1.DanhBo,SoLuongNgay=COUNT(t1.DanhBo) from (";
                while (count > 0)
                {
                    sql += " select IDNCC,DanhBo,SoLuong=(select COUNT(*) from sDHN_LichSu where DanhBo=sDHN.DanhBo and CAST(ThoiGianCapNhat as date)='" + date.ToString("yyyyMMdd") + "')"
                        + " from sDHN where Valid = 1";
                    if (count != 1)
                        sql += " union all";
                    count--;
                    date = date.AddDays(-1);
                }
                sql += ")t1,sDHN_NCC ncc"
                        + " where t1.SoLuong = 0 and t1.IDNCC = ncc.ID and t1.IDNCC like '%" + NCC + "%'"
                        + " group by IDNCC,ncc.Name,t1.DanhBo"
                        + " having COUNT(t1.DanhBo) = " + SoNgay
                        + " )t1,DHTM_THONGTIN ttsdhn,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                        + " where t1.IDNCC=ttsdhn.ID and t1.DanhBo=ttkh.DANHBO";
            }
            else
            {
                sql = "select IDNCC,NCC=ncc.Name,t1.DanhBo,SoLuongNgay=COUNT(t1.DanhBo) from (";
                while (count > 0)
                {
                    sql += " select IDNCC,DanhBo,SoLuong=(select COUNT(*) from sDHN_LichSu where DanhBo=sDHN.DanhBo and CAST(ThoiGianCapNhat as date)='" + date.ToString("yyyyMMdd") + "')"
                        + " from sDHN where Valid = 1";
                    if (count != 1)
                        sql += " union all";
                    count--;
                    date = date.AddDays(-1);
                }
                sql += ")t1,sDHN_NCC ncc"
                        + " where t1.SoLuong = 0 and t1.IDNCC = ncc.ID and t1.IDNCC like '%" + NCC + "%'"
                        + " group by IDNCC,ncc.Name,t1.DanhBo"
                        + " having COUNT(t1.DanhBo) = " + SoNgay
                        + " order by IDNCC";
            }
            return cDAL_sDHN.ExecuteQuery_DataTable(sql);
        }

        public string getLichSu_sDHN(string Ngay)
        {
            try
            {
                string[] datestr = Ngay.Split('/');
                DateTime date = new DateTime(int.Parse(datestr[2]), int.Parse(datestr[1]), int.Parse(datestr[0]));
                List<MView> vLichSu = new List<MView>();
                int count = 7;
                while (count > 0)
                {
                    MView enLichSu = new MView();
                    enLichSu.ThoiGian = date.ToString("dd/MM/yyyy");
                    DataTable dtBinhThuong = getDS_sDHN_BinhThuong("", date);
                    DataTable dtBatThuong = getDS_sDHN_BatThuong("", date);
                    DataTable dtKhongTinHieu = getDS_sDHN_KhongTinHieu("", date);

                    MView enLichSuChild;
                    if (dtBinhThuong != null && dtBinhThuong.Rows.Count > 0)
                    {
                        enLichSuChild = new MView();
                        enLichSuChild.TieuDe = dtBinhThuong.Rows[0]["Loai"].ToString();
                        enLichSuChild.SoLuong = dtBinhThuong.Rows.Count.ToString();
                        enLichSuChild.NoiDung = dtBinhThuong.Rows[0]["Loai2"].ToString();
                        enLichSu.lst.Add(enLichSuChild);
                    }
                    if (dtBatThuong != null && dtBatThuong.Rows.Count > 0)
                    {
                        enLichSuChild = new MView();
                        enLichSuChild.TieuDe = dtBatThuong.Rows[0]["Loai"].ToString();
                        enLichSuChild.SoLuong = dtBatThuong.Rows.Count.ToString();
                        enLichSuChild.NoiDung = dtBatThuong.Rows[0]["Loai2"].ToString();
                        enLichSu.lst.Add(enLichSuChild);
                    }
                    if (dtKhongTinHieu != null && dtKhongTinHieu.Rows.Count > 0)
                    {
                        enLichSuChild = new MView();
                        enLichSuChild.TieuDe = dtKhongTinHieu.Rows[0]["Loai"].ToString();
                        enLichSuChild.SoLuong = dtKhongTinHieu.Rows.Count.ToString();
                        enLichSuChild.NoiDung = dtKhongTinHieu.Rows[0]["Loai2"].ToString();
                        enLichSu.lst.Add(enLichSuChild);
                    }
                    count--;
                    date = date.AddDays(-1);
                    vLichSu.Add(enLichSu);
                }
                return CGlobalVariable.jsSerializer.Serialize(vLichSu);
            }
            catch
            {
                return null;
            }
        }

        [NonAction]
        public DataTable getDS_sDHN_KhongTinHieu(string function, DateTime date)
        {
            if (function == "export")
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Không Tín Hiệu',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='0' from"
                                                        + " (select DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                        + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong = 0 and t1.IDNCC=ttsdhn.ID order by NCC");
            else
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Không Tín Hiệu',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='0' from"
                                                        + " (select DanhBo=dhn.DanhBo, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN dhn"
                                                        + " where ncc.ID=dhn.IDNCC and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong = 0 and t1.IDNCC=ttsdhn.ID order by NCC");
        }

        [NonAction]
        public DataTable getDS_sDHN_BinhThuong(string function, DateTime date)
        {
            if (function == "export")
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bình Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='2' from"
                                                        + " (select DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                        + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong >= 24 and t1.IDNCC=ttsdhn.ID order by NCC");
            else
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bình Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='2' from"
                                              + " (select DanhBo=dhn.DanhBo, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                              + " from sDHN_NCC ncc,sDHN dhn"
                                              + " where ncc.ID=dhn.IDNCC and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                              + " where t1.SoLuong >= 24 and t1.IDNCC=ttsdhn.ID order by NCC");
        }

        [NonAction]
        public DataTable getDS_sDHN_BatThuong(string function, DateTime date)
        {
            if (function == "export")
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bất Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='1' from"
                                                        + " (select DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                        + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong > 0 and t1.SoLuong < 24 and t1.IDNCC=ttsdhn.ID order by NCC");
            else
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bất Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='1' from"
                                                      + " (select DanhBo=dhn.DanhBo, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                      + " from sDHN_NCC ncc,sDHN dhn"
                                                      + " where ncc.ID=dhn.IDNCC and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                      + " where t1.SoLuong > 0 and t1.SoLuong < 24 and t1.IDNCC=ttsdhn.ID order by NCC");
        }

        public ActionResult ExportData(string function1, string function2, string SoNgay, string Ngay)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] datestr = Ngay.Split('/');
                DateTime date = new DateTime(int.Parse(datestr[2]), int.Parse(datestr[1]), int.Parse(datestr[0]));
                string filename = "";
                if (function1 == "KhongTinHieu")
                {
                    dt = getDS_sDHN_KhongTinHieu("export", function2, SoNgay, Ngay);
                    dt.TableName = "Không Tín Hiệu";
                    filename = dt.TableName + "." + Ngay.Replace("/", ".");
                }
                else
                    if (function1 == "LichSu")
                {
                    if (function2 == "0")
                    {
                        dt = getDS_sDHN_KhongTinHieu("export", date);
                    }
                    else
                    if (function2 == "1")
                    {
                        dt = getDS_sDHN_BatThuong("export", date);
                    }
                    else
                    if (function2 == "2")
                    {
                        dt = getDS_sDHN_BinhThuong("export", date);
                    }
                    dt.TableName = dt.Rows[0]["Loai"].ToString();
                    filename = dt.Rows[0]["Loai"].ToString() + "." + dt.Rows[0]["ThoiGian"].ToString().Replace("/", ".");
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename= TanHoa.sDHN." + filename + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
                return RedirectToAction("Index", "QLDHN");
            }
            catch (Exception ex)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('" + ex.Message + "');</script>");
            }
        }

        public bool updateDS_sDHN()
        {
            try
            {
                apiDocSoController apiDocSo = new apiDocSoController();
                apiDocSo.updateDS_sDHN("tanho@2022");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool getChiSo_sDHN_Day_Back(string Ngay)
        {
            try
            {
                if (Ngay != "")
                {
                    apiDocSoController apiDocSo = new apiDocSoController();
                    apiDocSo.getChiSo_sDHN_Day_Back(Ngay.Replace("/", "-"), "tanho@2022");
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        [NonAction]
        public Bitmap resizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        [NonAction]
        public Bitmap resizeImage(Image image, decimal percentage)
        {
            int width = (int)Math.Round(image.Width * percentage, MidpointRounding.AwayFromZero);
            int height = (int)Math.Round(image.Height * percentage, MidpointRounding.AwayFromZero);
            return resizeImage(image, width, height);
        }

        [NonAction]
        public byte[] ImageToByte(Bitmap image)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        public ActionResult TaiApp()
        {
            return Redirect("http://113.161.88.180:81/app/docso.apk");
        }
    }
}