using ClosedXML.Excel;
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
        private CConnection cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
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
                    SqlCommand command = new SqlCommand("insert into DocSo_Web(DanhBo,Nam,Ky,Dot,ChiSo,Hinh)values('" + dt.Rows[0]["DanhBo"].ToString() + "'," + drLich["Nam"].ToString() + "," + drLich["Ky"].ToString() + "," + drLich["Dot"].ToString() + "," + ChiSo + ",@Hinh)");
                    command.Parameters.Add("@Hinh", SqlDbType.Image).Value = ImageToByte(resizedImage);
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

        public ActionResult sDHN(string action, string TuNgay, string DenNgay, string NgayXem)
        {
            object soluong = cDAL_DocSo.ExecuteQuery_ReturnOneValue("select SoLuong=COUNT(*) from sDHN_NCC a,sDHN b,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG c"
                                    + " where a.ID = b.IDNCC and b.DanhBo = c.DANHBO and Valid = 1");
            ViewBag.SoLuong = soluong;
            DataTable dt = cDAL_DocSo.ExecuteQuery_DataTable("select Name,SoLuong=COUNT(*) from sDHN_NCC a,sDHN b,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG c"
                                    + " where a.ID = b.IDNCC and b.DanhBo = c.DANHBO and Valid = 1"
                                    + " group by Name"
                                    + " order by SoLuong");
            List<ThongTinKhachHang> lstTong = new List<ThongTinKhachHang>();
            foreach (DataRow item in dt.Rows)
            {
                ThongTinKhachHang en = new ThongTinKhachHang();
                en.DanhBo = item["Name"].ToString();
                en.HoTen = item["SoLuong"].ToString();
                lstTong.Add(en);
            }
            ViewBag.lstTong = lstTong;
            //Lịch Sử
            DateTime date = DateTime.Now.AddDays(-1);
            if (NgayXem != null && NgayXem != "")
            {
                string[] datestr = NgayXem.Split('/');
                date = new DateTime(int.Parse(datestr[2]), int.Parse(datestr[1]), int.Parse(datestr[0]));
            }
            DataTable dtBinhThuong = getDS_sDHN_BinhThuong(date);
            DataTable dtBatThuong = getDS_sDHN_BatThuong(date);
            DataTable dtKhongTinHieu = getDS_sDHN_KhongTinHieu(date);
            List<ThongTinKhachHang> lstLichSu = new List<ThongTinKhachHang>();
            ThongTinKhachHang enLichSu = new ThongTinKhachHang();
            enLichSu.DanhBo = dtBinhThuong.Rows[0]["Loai"].ToString();
            enLichSu.HoTen = dtBinhThuong.Rows.Count.ToString();
            enLichSu.DiaChi = dtBinhThuong.Rows[0]["Loai2"].ToString();
            lstLichSu.Add(enLichSu);
            enLichSu = new ThongTinKhachHang();
            enLichSu.DanhBo = dtBatThuong.Rows[0]["Loai"].ToString();
            enLichSu.HoTen = dtBatThuong.Rows.Count.ToString();
            enLichSu.DiaChi = dtBatThuong.Rows[0]["Loai2"].ToString();
            lstLichSu.Add(enLichSu);
            enLichSu = new ThongTinKhachHang();
            enLichSu.DanhBo = dtKhongTinHieu.Rows[0]["Loai"].ToString();
            enLichSu.HoTen = dtKhongTinHieu.Rows.Count.ToString();
            enLichSu.DiaChi = dtKhongTinHieu.Rows[0]["Loai2"].ToString();
            lstLichSu.Add(enLichSu);
            ViewBag.lstLichSu = lstLichSu;
            ViewBag.NgayXem = date;
            if (action == "Xem" && TuNgay != "" && DenNgay != "")
            {
                DateTime dateTu = DateTime.Parse(TuNgay), dateDen = DateTime.Parse(DenNgay);

            }
            return View();
        }

        public DataTable getDS_sDHN_KhongTinHieu(DateTime date)
        {
            return cDAL_DocSo.ExecuteQuery_DataTable("select Loai=N'Không Tín Hiệu',Loai2=0,* from"
                                                    + " (select NCC=ncc.Name,ttkh.DANHBO, SoLuong = (select COUNT(ID) from sDHN_LichSu ls where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and ls.DanhBo = ttkh.DANHBO)"
                                                    + " from sDHN_NCC ncc,sDHN dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1"
                                                    + " where t1.SoLuong = 0 order by NCC");
        }

        public DataTable getDS_sDHN_BinhThuong(DateTime date)
        {
            return cDAL_DocSo.ExecuteQuery_DataTable("select Loai=N'Bình Thường',Loai2=2,* from"
                                                    + " (select NCC=ncc.Name,ttkh.DANHBO, SoLuong = (select COUNT(ID) from sDHN_LichSu ls where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and ls.DanhBo = ttkh.DANHBO)"
                                                    + " from sDHN_NCC ncc,sDHN dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1"
                                                    + " where t1.SoLuong >= 24 order by NCC");
        }

        public DataTable getDS_sDHN_BatThuong(DateTime date)
        {
            return cDAL_DocSo.ExecuteQuery_DataTable("select Loai=N'Bất Thường',Loai2=1,* from"
                                                    + " (select NCC=ncc.Name,ttkh.DANHBO, SoLuong = (select COUNT(ID) from sDHN_LichSu ls where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and ls.DanhBo = ttkh.DANHBO)"
                                                    + " from sDHN_NCC ncc,sDHN dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1"
                                                    + " where t1.SoLuong > 0 and t1.SoLuong < 24 order by NCC");
        }

        public ActionResult ExportData(string function, string Ngay)
        {
            DataTable dt = new DataTable();
            DateTime date = DateTime.Parse(Ngay);
            switch (function)
            {
                case "0":
                    dt = getDS_sDHN_KhongTinHieu(date);
                    dt.TableName = dt.Rows[0]["Loai"].ToString();
                    break;
                case "1":
                    dt = getDS_sDHN_BatThuong(date);
                    dt.TableName = dt.Rows[0]["Loai"].ToString();
                    break;
                default:
                    dt = getDS_sDHN_BinhThuong(date);
                    dt.TableName = dt.Rows[0]["Loai"].ToString();
                    break;
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", "attachment;filename= TanHoa.sDHN.xlsx");

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

        public Bitmap resizeImage(Image image, decimal percentage)
        {
            int width = (int)Math.Round(image.Width * percentage, MidpointRounding.AwayFromZero);
            int height = (int)Math.Round(image.Height * percentage, MidpointRounding.AwayFromZero);
            return resizeImage(image, width, height);
        }

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