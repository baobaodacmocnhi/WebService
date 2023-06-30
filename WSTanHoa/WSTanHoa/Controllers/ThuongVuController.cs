using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class ThuongVuController : Controller
    {
        private CConnection _cDAL_KinhDoanh = new CConnection(CGlobalVariable.ThuongVu);
        private CConnection _cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private wrThuongVu.wsThuongVu _wsThuongVu = new wrThuongVu.wsThuongVu();

        // GET: ThuongVu
        //public ActionResult viewFile(string TableName, string IDFileName, string IDFileContent)
        //{
        //    if (TableName != null && IDFileName != null && IDFileContent != null && TableName != "" && IDFileName != "" && IDFileContent != "")
        //    {
        //        //byte[] FileContent = getFile(TableName, IDFileName, IDFileContent);
        //        DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select filename=Name+Loai from " + TableName + " where " + IDFileName + "=" + IDFileContent);
        //        if (dt != null && dt.Rows.Count > 0)
        //        {
        //            byte[] FileContent = wsThuongVu.get_Hinh(TableName, IDFileContent, dt.Rows[0]["filename"].ToString());
        //            if (FileContent != null)
        //                return new FileStreamResult(new MemoryStream(FileContent), "image/jpeg");
        //            else
        //                return View();
        //        }
        //        else
        //            return View();
        //    }
        //    else
        //        return null;
        //}

        public ActionResult viewFile(string TableName, string IDFileName, string IDFileContent)
        {
            if (TableName != null && IDFileName != null && IDFileContent != null && TableName != "" && IDFileName != "" && IDFileContent != "")
            {
                string NoiDung = "";
                //byte[] FileContent = getFile(TableName, IDFileName, IDFileContent);
                DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select filename=Name+Loai from " + TableName + " where " + IDFileName + "=" + IDFileContent + " order by CreateDate desc");
                if (dt != null && dt.Rows.Count > 0)
                    foreach (DataRow item in dt.Rows)
                    {
                        byte[] FileContent = _wsThuongVu.get_Hinh(TableName, IDFileContent, item["filename"].ToString());
                        if (item["filename"].ToString().ToLower().Contains(".pdf"))
                        {
                            return viewFilePDF(FileContent);
                        }
                        else
                        {
                            if (FileContent != null)
                                NoiDung += "<img height='100%' src='data:image/jpeg;base64," + Convert.ToBase64String(FileContent) + "'/></br></br>";
                        }
                    }
                else
                    NoiDung = "<head><meta charset='UTF-8'><link rel='shortcut icon' type='image/ico' href='~/Images/logoctycp.png'></head><body><h3>Không có hình ảnh!</h3></body>";
                return Content(NoiDung, "text/html");
            }
            else
                return null;
        }

        private byte[] getFile(string TableName, string IDFileName, string IDFileContent)
        {
            int count = (int)_cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select count(*) from " + TableName + " where " + IDFileName + "=" + IDFileContent);
            if (count > 0)
                return (byte[])_cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select Hinh from " + TableName + " where " + IDFileName + "=" + IDFileContent);
            else
                return null;
        }

        public ActionResult viewFilePDF(byte[] File)
        {
            if (File != null && File.Length > 0)
            {
                return new FileContentResult(File, "application/pdf");
            }
            else
                return null;
        }

        public ContentResult viewViTri(string DanhBo)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://old.cskhtanhoa.com.vn:1803/api/docso/location/" + DanhBo);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                System.Net.HttpWebResponse respuesta = (System.Net.HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == System.Net.HttpStatusCode.Accepted || respuesta.StatusCode == System.Net.HttpStatusCode.OK || respuesta.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    return Content("<head><meta charset='UTF-8'></head><body><h3>Copy tọa độ này vào Google Map: " + ((double)obj["latitude"]).ToString().Replace(",", ".") + "," + ((double)obj["longitude"]).ToString().Replace(",", ".") + "</h3></body>", "text/html");
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ThongTinKhachHang(string DanhBo)
        {
            return Redirect("http://113.161.88.180:8585/index.aspx");
        }

        public ActionResult TinhTienNuoc(FormCollection collection)
        {
            string method = HttpContext.Request.HttpMethod;
            if (method == "POST")
            {
                ViewBag.txtDanhBo = collection["txtDanhBo"].ToString();
                ViewBag.txtNam = collection["txtNam"].ToString();
                ViewBag.txtKy = collection["txtKy"].ToString();
                if (collection["txtGiaBieu"].ToString() == "")
                {
                    getThongTinHoaDon(collection["txtDanhBo"].ToString(), collection["txtNam"].ToString(), collection["txtKy"].ToString());
                }
                else
                {
                    DataTable dtGiaNuoc = getDS_GiaNuoc();
                    //check giảm giá
                    checkExists_GiamGiaNuoc(int.Parse(collection["txtNam"].ToString()), int.Parse(collection["txtKy"].ToString()), int.Parse(collection["txtGiaBieu"].ToString()), ref dtGiaNuoc);

                    int index = -1;
                    for (int i = 0; i < dtGiaNuoc.Rows.Count; i++)
                        if (DateTime.Parse(collection["txtTuNgay"].ToString()).Date < DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date < DateTime.Parse(collection["txtDenNgay"].ToString()).Date)
                        {
                            index = i;
                        }
                        else
                            if (DateTime.Parse(collection["txtTuNgay"].ToString()).Date >= DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date)
                        {
                            index = i;
                        }
                    if (index != -1)
                    {
                        if (DateTime.Parse(collection["txtDenNgay"].ToString()).Date < new DateTime(2019, 11, 15))
                        {
                        }
                        else
                            if (DateTime.Parse(collection["txtTuNgay"].ToString()).Date < DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date < DateTime.Parse(collection["txtDenNgay"].ToString()).Date)
                        {
                            //int TieuThu_DieuChinhGia;
                            int TongSoNgay = (int)((DateTime.Parse(collection["txtDenNgay"].ToString()).Date - DateTime.Parse(collection["txtTuNgay"].ToString()).Date).TotalDays);
                            int SoNgayCu = (int)((DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date - DateTime.Parse(collection["txtTuNgay"].ToString()).Date).TotalDays);
                            int TieuThuCu = (int)Math.Round(double.Parse(collection["txtTieuThu"].ToString()) * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
                            int TieuThuMoi = int.Parse(collection["txtTieuThu"].ToString()) - TieuThuCu;
                            int TongDinhMucCu = (int)Math.Round(double.Parse(collection["txtDinhMuc"].ToString()) * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
                            int TongDinhMucMoi = int.Parse(collection["txtDinhMuc"].ToString()) - TongDinhMucCu;
                            int DinhMucHN_Cu = 0, DinhMucHN_Moi = 0;
                            if (DateTime.Parse(collection["txtTuNgay"].ToString()).Date > new DateTime(2019, 11, 15))
                                if (TongDinhMucCu != 0 && int.Parse(collection["txtDinhMucHN"].ToString()) != 0 && int.Parse(collection["txtDinhMuc"].ToString()) != 0)
                                    DinhMucHN_Cu = (int)Math.Round((double)TongDinhMucCu * int.Parse(collection["txtDinhMucHN"].ToString()) / int.Parse(collection["txtDinhMuc"].ToString()), 0, MidpointRounding.AwayFromZero);
                            if (TongDinhMucMoi != 0 && int.Parse(collection["txtDinhMucHN"].ToString()) != 0 && int.Parse(collection["txtDinhMuc"].ToString()) != 0)
                                DinhMucHN_Moi = (int)Math.Round((double)TongDinhMucMoi * int.Parse(collection["txtDinhMucHN"].ToString()) / int.Parse(collection["txtDinhMuc"].ToString()), 0, MidpointRounding.AwayFromZero);

                            ViewBag.txtTongSoNgay = TongSoNgay.ToString();
                            ViewBag.txtSoNgayCu = SoNgayCu.ToString();
                            ViewBag.txtSoNgayMoi = (TongSoNgay - SoNgayCu).ToString();
                            ViewBag.txtDinhMucCu = TongDinhMucCu.ToString();
                            ViewBag.txtDinhMucMoi = TongDinhMucMoi.ToString();
                            ViewBag.txtDinhMucHNCu = DinhMucHN_Cu.ToString();
                            ViewBag.txtDinhMucHNMoi = DinhMucHN_Moi.ToString();
                            ViewBag.txtTieuThuCu = TieuThuCu.ToString();
                            ViewBag.txtTieuThuMoi = TieuThuMoi.ToString();
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }
                    //

                    ViewBag.txtTuNgay = collection["txtTuNgay"].ToString();
                    ViewBag.txtDenNgay = collection["txtDenNgay"].ToString();
                    ViewBag.txtGiaBieu = collection["txtGiaBieu"].ToString();
                    ViewBag.txtSH = collection["txtSH"].ToString();
                    ViewBag.txtSX = collection["txtSX"].ToString();
                    ViewBag.txtDV = collection["txtDV"].ToString();
                    ViewBag.txtHCSN = collection["txtHCSN"].ToString();
                    ViewBag.txtDinhMucHN = collection["txtDinhMucHN"].ToString();
                    ViewBag.txtDinhMuc = collection["txtDinhMuc"].ToString();
                    ViewBag.txtTieuThu = collection["txtTieuThu"].ToString();
                    //
                    int GiaBanCu = 0, GiaBanMoi = 0, ThueGTGT = 0, PhiBVMTCu = 0, PhiBVMTMoi = 0, GiaBan = 0, TDVTN = 0, ThueGTGTTDVTN = 0, TongCong = 0, TieuThu_DieuChinhGia = 0;
                    string ChiTietCu = "", ChiTietMoi = "", ChiTietPhiBVMTCu = "", ChiTietPhiBVMTMoi = "";
                    wrThuTien.wsThuTien ws = new wrThuTien.wsThuTien();
                    string[] TuNgays = collection["txtTuNgay"].ToString().Split('-');
                    string[] DenNgays = collection["txtDenNgay"].ToString().Split('-');
                    ws.TinhTienNuoc(false, false, false, 0, collection["txtDanhBo"].ToString(), int.Parse(collection["txtKy"].ToString()), int.Parse(collection["txtNam"].ToString()), new DateTime(int.Parse(TuNgays[0]), int.Parse(TuNgays[1]), int.Parse(TuNgays[2])), new DateTime(int.Parse(DenNgays[0]), int.Parse(DenNgays[1]), int.Parse(DenNgays[2])), int.Parse(collection["txtGiaBieu"].ToString()), int.Parse(collection["txtSH"].ToString()), int.Parse(collection["txtSX"].ToString()), int.Parse(collection["txtDV"].ToString()), int.Parse(collection["txtHCSN"].ToString()), int.Parse(collection["txtDinhMuc"].ToString()), int.Parse(collection["txtDinhMucHN"].ToString()), int.Parse(collection["txtTieuThu"].ToString()), ref GiaBanCu, ref ChiTietCu, ref GiaBanMoi, ref ChiTietMoi, ref TieuThu_DieuChinhGia, ref PhiBVMTCu, ref ChiTietPhiBVMTCu, ref PhiBVMTMoi, ref ChiTietPhiBVMTMoi, ref GiaBan, ref ThueGTGT, ref TDVTN, ref ThueGTGTTDVTN);
                    //ThueGTGT = (int)Math.Round((double)(GiaBanCu + GiaBanMoi) * 5 / 100, 0, MidpointRounding.AwayFromZero);

                    ViewBag.txtGiaBanCu = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaBanCu);
                    ViewBag.txtGiaBanMoi = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaBanMoi);
                    ViewBag.txtGiaBan = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaBan);

                    ViewBag.txtThueGTGT = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", ThueGTGT);

                    ViewBag.txtTDVTNCu = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", PhiBVMTCu);
                    ViewBag.txtTDVTNMoi = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", PhiBVMTMoi);
                    ViewBag.txtTDVTN = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TDVTN);

                    ViewBag.txtChiTietCu = ChiTietCu;
                    ViewBag.txtChiTietTDVTNCu = ChiTietPhiBVMTCu;
                    ViewBag.txtChiTietMoi = ChiTietMoi;
                    ViewBag.txtChiTietTDVTNMoi = ChiTietPhiBVMTMoi;

                    TongCong = GiaBan + ThueGTGT + TDVTN + ThueGTGTTDVTN;
                    ViewBag.txtThueGTGTTDVTN = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", ThueGTGTTDVTN);
                    ViewBag.txtTongCong = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", TongCong);
                }
            }
            return View();
        }

        public void getThongTinHoaDon(string DanhBo, string Nam, string Ky)
        {
            try
            {
                DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from HOADON where DANHBA='" + DanhBo + "' and NAM=" + Nam + " and KY=" + Ky);
                if (dt != null & dt.Rows.Count > 0)
                {
                    ViewBag.txtTuNgay = DateTime.Parse(dt.Rows[0]["TUNGAY"].ToString()).ToString("yyyy-MM-dd");
                    ViewBag.txtDenNgay = DateTime.Parse(dt.Rows[0]["DENNGAY"].ToString()).ToString("yyyy-MM-dd");
                    ViewBag.txtGiaBieu = dt.Rows[0]["GB"];
                    if (dt.Rows[0]["TILESH"].ToString() != "")
                        ViewBag.txtSH = dt.Rows[0]["TILESH"];
                    else
                        ViewBag.txtSH = 0;
                    if (dt.Rows[0]["TILESX"].ToString() != "")
                        ViewBag.txtSX = dt.Rows[0]["TILESX"];
                    else
                        ViewBag.txtSX = 0;
                    if (dt.Rows[0]["TILEDV"].ToString() != "")
                        ViewBag.txtDV = dt.Rows[0]["TILEDV"];
                    else
                        ViewBag.txtDV = 0;
                    if (dt.Rows[0]["TILEHCSN"].ToString() != "")
                        ViewBag.txtHCSN = dt.Rows[0]["TILEHCSN"];
                    else
                        ViewBag.txtHCSN = 0;
                    if (dt.Rows[0]["DinhMucHN"].ToString() != "")
                        ViewBag.txtDinhMucHN = dt.Rows[0]["DinhMucHN"];
                    else
                        ViewBag.txtDinhMucHN = "0";
                    if (dt.Rows[0]["DM"].ToString() != "")
                        ViewBag.txtDinhMuc = dt.Rows[0]["DM"];
                    else
                        ViewBag.txtDinhMuc = "0";
                    ViewBag.txtTieuThu = dt.Rows[0]["TIEUTHU"];
                }
                else
                {
                    dt = _cDAL_DocSo.ExecuteQuery_DataTable("select * from DocSo where DanhBa='" + DanhBo + "' and Nam=" + Nam + " and Ky=" + Ky);
                    if (dt != null & dt.Rows.Count > 0)
                    {
                        int Ky2 = 0, Nam2 = 0;
                        if (int.Parse(Ky) == 1)
                        {
                            Ky2 = 12;
                            Nam2 = int.Parse(Nam) - 1;
                        }
                        else
                        {
                            Ky2 = int.Parse(Ky) - 1;
                            Nam2 = int.Parse(Nam);
                        }
                        DataTable dt2 = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from HOADON where DANHBA='" + DanhBo + "' and NAM=" + Nam2 + " and KY=" + Ky2);
                        ViewBag.txtTuNgay = dt2.Rows[0]["TUNGAY"];
                        ViewBag.txtDenNgay = dt2.Rows[0]["DENNGAY"];
                        ViewBag.txtGiaBieu = dt2.Rows[0]["GB"];
                        if (dt.Rows[0]["TILESH"].ToString() != "")
                            ViewBag.txtSH = dt.Rows[0]["TILESH"];
                        else
                            ViewBag.txtSH = 0;
                        if (dt.Rows[0]["TILESX"].ToString() != "")
                            ViewBag.txtSX = dt.Rows[0]["TILESX"];
                        else
                            ViewBag.txtSX = 0;
                        if (dt.Rows[0]["TILEDV"].ToString() != "")
                            ViewBag.txtDV = dt.Rows[0]["TILEDV"];
                        else
                            ViewBag.txtDV = 0;
                        if (dt.Rows[0]["TILEHCSN"].ToString() != "")
                            ViewBag.txtHCSN = dt.Rows[0]["TILEHCSN"];
                        else
                            ViewBag.txtHCSN = 0;
                        if (dt.Rows[0]["DinhMucHN"].ToString() != "")
                            ViewBag.txtDinhMucHN = dt.Rows[0]["DinhMucHN"];
                        else
                            ViewBag.txtDinhMucHN = "0";
                        if (dt2.Rows[0]["DM"].ToString() != "")
                            ViewBag.txtDinhMuc = dt2.Rows[0]["DM"];
                        else
                            ViewBag.txtDinhMuc = "0";
                        ViewBag.txtTieuThu = dt2.Rows[0]["TIEUTHU"];
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable getDS_GiaNuoc()
        {
            return _cDAL_KinhDoanh.ExecuteQuery_DataTable("select * from GiaNuoc2");
        }

        private DataTable getGiaNuocGiam(int Nam, int Ky, int GiaBieu)
        {
            return _cDAL_KinhDoanh.ExecuteQuery_DataTable("select * from GiaNuoc_Giam where Nam like '%" + Nam + "%' and Ky like '%" + Ky.ToString("00") + "%' and GiaBieu like '%" + GiaBieu + "%'");
        }

        private bool checkExists_GiamGiaNuoc(int Nam, int Ky, int GiaBieu, ref DataTable dt)
        {
            DataTable dtGiaNuocGiam = getGiaNuocGiam(Nam, Ky, GiaBieu);

            if (dtGiaNuocGiam != null && dtGiaNuocGiam.Rows.Count > 0)
            {
                double TyLeGiam = double.Parse(dtGiaNuocGiam.Rows[0]["TyLeGiam"].ToString());
                foreach (DataRow item in dt.Rows)
                {
                    item["SHN"] = int.Parse(item["SHN"].ToString()) - (int)(int.Parse(item["SHN"].ToString()) * TyLeGiam / 100);
                    item["SHTM"] = int.Parse(item["SHTM"].ToString()) - (int)(int.Parse(item["SHTM"].ToString()) * TyLeGiam / 100);
                    item["SHVM1"] = int.Parse(item["SHVM1"].ToString()) - (int)(int.Parse(item["SHVM1"].ToString()) * TyLeGiam / 100);
                    item["SHVM2"] = int.Parse(item["SHVM2"].ToString()) - (int)(int.Parse(item["SHVM2"].ToString()) * TyLeGiam / 100);
                    item["SX"] = int.Parse(item["SX"].ToString()) - (int)(int.Parse(item["SX"].ToString()) * TyLeGiam / 100);
                    item["HCSN"] = int.Parse(item["HCSN"].ToString()) - (int)(int.Parse(item["HCSN"].ToString()) * TyLeGiam / 100);
                    item["KDDV"] = int.Parse(item["KDDV"].ToString()) - (int)(int.Parse(item["KDDV"].ToString()) * TyLeGiam / 100);
                }
                return true;
            }
            else
                return false;
        }

        //đăng ký định mức cccd
        public ActionResult DangKyDinhMuc(string function, string DanhBo, string DienThoai, string SoNK, FormCollection form, IEnumerable<HttpPostedFileBase> HinhCCCD, IEnumerable<HttpPostedFileBase> HinhCT)
        {
            if (function == "KiemTra")
            {
                if (DanhBo != null && DanhBo.Replace(" ", "").Replace("-", "") != "")
                {
                    DataTable dt = _cDAL_DHN.ExecuteQuery_DataTable("select MLT=LOTRINH,DanhBo,HoTen,DiaChi = SONHA + ' ' + TENDUONG from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo.Replace(" ", "").Replace("-", "") + "'");
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
                DanhBo = DanhBo.Trim().Replace(" ", "").Replace("-", "");
                DienThoai = DienThoai.Trim().Replace(".", "").Replace("-", "");
                SoNK = SoNK.Trim();
                if (DanhBo == "")
                    ModelState.AddModelError("", "Thiếu Danh Bộ");
                if (DienThoai == "" || DienThoai.Length != 10)
                    ModelState.AddModelError("", "Thiếu Điện Thoại hoặc Điện Thoại Không Đủ 10 số");
                if (SoNK == "")
                    ModelState.AddModelError("", "Thiếu Số Nhân Khẩu");
                for (int i = 0; i < int.Parse(SoNK); i++)
                {
                    if (form["HoTen" + (i + 1) + ""].ToString().Trim() == "")
                    {
                        ModelState.AddModelError("", "Thiếu Họ Tên");
                        return View();
                    }
                    if (form["DCThuongTru" + (i + 1) + ""].ToString().Trim() == "" && form["DCTamTru" + (i + 1) + ""].ToString().Trim() == "")
                    {
                        ModelState.AddModelError("", "Thiếu Địa chỉ thường trụ hoặc tạm trú");
                        return View();
                    }
                    if (form["CCCD" + (i + 1) + ""].ToString().Trim() == "")
                    {
                        ModelState.AddModelError("", "Thiếu số CCCD");
                        return View();
                    }
                    if (HinhCCCD.ElementAt(i) == null || HinhCCCD.ElementAt(i).ContentLength <= 0)
                    {
                        ModelState.AddModelError("", "Thiếu Hình ảnh đính kèm");
                        return View();
                    }
                }
                string checkExists = _cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select case when exists(select ID from DCBD_DKDM_DanhBo where DanhBo='" + DanhBo + "' and cast(createdate as date)=cast(getdate() as date)) then 1 else 0 end").ToString();
                if (checkExists == "1")
                {
                    ModelState.AddModelError("", "Danh Bộ đã nhập trong ngày");
                }
                else
                if (DanhBo != "" && DienThoai.Length == 10 && SoNK != "")
                {
                    string Date = DateTime.Now.ToString("yyyyMMdd");
                    DataTable dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 GB,DM from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
                    string sql = "declare @tbID table (id int);"
                              + " insert into DCBD_DKDM_DanhBo(DanhBo,GiaBieu,DinhMuc,SDT)"
                              + " output inserted.ID into @tbID"
                              + " values('" + DanhBo + "'," + dt.Rows[0]["GB"] + "," + dt.Rows[0]["DM"] + ",'" + DienThoai + "');"
                              + " declare @ID int"
                              + " select @ID=id from @tbID;";
                    for (int i = 0; i < int.Parse(SoNK); i++)
                    {
                        sql += " insert into DCBD_DKDM_CCCD(IDDanhBo,CCCD,HoTen,NgaySinh,DCThuongTru,DCTamTru)values(@ID"
                            + ",N'" + form["CCCD" + (i + 1) + ""].ToString().Trim() + "',N'" + form["HoTen" + (i + 1) + ""].ToString().Trim() + "','" + form["NgaySinh" + (i + 1) + ""].ToString().Trim() + "',N'" + form["DCThuongTru" + (i + 1) + ""].ToString().Trim() + "',N'" + form["DCTamTru" + (i + 1) + ""].ToString().Trim() + "')";
                        Image image = Image.FromStream(HinhCCCD.ElementAt(i).InputStream);
                        Bitmap resizedImage = CGlobalVariable.resizeImage(image, 0.5m);
                        _wsThuongVu.ghi_Hinh("DangKyDinhMuc", DanhBo + "." + Date, "CCCD" + (i + 1).ToString() + Path.GetExtension(HinhCCCD.ElementAt(i).FileName), CGlobalVariable.ImageToByte(resizedImage));
                        if (HinhCT.ElementAt(i) != null && HinhCT.ElementAt(i).ContentLength > 0)
                        {
                            Image imageCT = Image.FromStream(HinhCT.ElementAt(i).InputStream);
                            Bitmap resizedImageCT = CGlobalVariable.resizeImage(imageCT, 0.5m);
                            _wsThuongVu.ghi_Hinh("DangKyDinhMuc", DanhBo + "." + Date, "CT" + (i + 1).ToString() + Path.GetExtension(HinhCT.ElementAt(i).FileName), CGlobalVariable.ImageToByte(resizedImageCT));
                        }
                    }

                    if (_cDAL_KinhDoanh.ExecuteNonQuery(sql))
                        ModelState.AddModelError("", "Đăng Ký Thành Công");
                    else
                        ModelState.AddModelError("", "Đăng Ký Không Thành Công");
                }
            }
            return View();
        }

        public ActionResult viewImageCCCD(string FolderCT)
        {
            if (FolderCT != null && FolderCT != "")
            {
                string NoiDung = "";
                string[] result = _wsThuongVu.get_FileinFolder("DangKyDinhMuc", FolderCT);
                if (result != null && result.Count() > 0)
                    foreach (string item in result)
                    {
                        byte[] FileContent = _wsThuongVu.get_Hinh("DangKyDinhMuc", FolderCT, item.Substring(item.LastIndexOf('\\') + 1, item.Length - item.LastIndexOf('\\') - 1));
                        if (item.ToLower().Contains(".pdf"))
                        {
                            return viewFilePDF(FileContent);
                        }
                        else
                        {
                            if (FileContent != null)
                                NoiDung += "<img height='100%' src='data:image/jpeg;base64," + Convert.ToBase64String(FileContent) + "'/></br></br>";
                        }
                    }
                else
                    NoiDung = "<head><meta charset='UTF-8'><link rel='shortcut icon' type='image/ico' href='~/Images/logoctycp.png'></head><body><h3>Không có hình ảnh!</h3></body>";
                return Content(NoiDung, "text/html");
            }
            else
                return null;
        }

    }
}