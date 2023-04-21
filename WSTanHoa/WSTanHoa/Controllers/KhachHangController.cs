using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class KhachHangController : Controller
    {
        private CConnection cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        private CConnection cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private CConnection cDAL_ThuongVu = new CConnection(CGlobalVariable.ThuongVu);
        private CConnection cDAL_GanMoi = new CConnection(CGlobalVariable.GanMoi);
        private CConnection cDAL_TTKH = new CConnection(CGlobalVariable.TrungTamKhachHang);
        private apiTrungTamKhachHangController apiTTKH = new apiTrungTamKhachHangController();

        // GET: KhachHang
        public ActionResult ThongTin(string DanhBo)
        {
            //if (Session["LoginQRCode"] == null)
            //{
            //    Session["Url"] = Request.Url;
            //    Session["DanhBo"] = DanhBo;
            //    return RedirectToAction("Login", "KhachHang");
            //}
            ThongTinKhachHang en = new ThongTinKhachHang();
            if (DanhBo.ToUpper().Contains("THW"))
            {
                object result = cDAL_TTKH.ExecuteQuery_ReturnOneValue("select DanhBo from QR_Dong where KyHieu='THW' and ID=" + DanhBo.Substring(3, DanhBo.Length - 3));
                if (result != null && result.ToString() != "")
                    DanhBo = result.ToString();
            }
            //lấy thông tin khách hàng
            string sql = "select DanhBo"
                         + ",HoTen"
                         + ",DiaChi=SoNha+' '+TenDuong+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                         + ",DiaChiHoaDon=DiaChiHoaDon+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                         + ",HopDong"
                         + ",DienThoai"
                         + ",MLT=LoTrinh"
                         + ",DinhMuc"
                         + ",DinhMucHN"
                         + ",GiaBieu"
                         + ",HieuDH"
                         + ",CoDH"
                         + ",Cap"
                         + ",SoThanDH"
                         + ",ViTriDHN"
                         + ",NgayThay"
                         + ",NgayKiemDinh,DMA=MADMA"
                         + ",HieuLuc=convert(varchar(2),Ky)+'/'+convert(char(4),Nam)"
                         + ",Quan,Phuong"
                         + " from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo + "'";
            DataTable dt = cDAL_DHN.ExecuteQuery_DataTable(sql);
            if (dt.Rows.Count > 0)
            {
                string Quan = dt.Rows[0]["Quan"].ToString(), Phuong = dt.Rows[0]["Phuong"].ToString();
                en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                en.HoTen = dt.Rows[0]["HoTen"].ToString();
                en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                en.HopDong = dt.Rows[0]["HopDong"].ToString();
                en.MLT = dt.Rows[0]["MLT"].ToString();
                en.GiaBieu = dt.Rows[0]["GiaBieu"].ToString();
                en.DinhMuc = dt.Rows[0]["DinhMuc"].ToString();
                en.DinhMucHN = dt.Rows[0]["DinhMucHN"].ToString();
                en.DMA = dt.Rows[0]["DMA"].ToString().Replace("TH-", "");
                //
                object result = cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 N''+HoTen+' : '+DienThoai from NguoiDung where May=" + dt.Rows[0]["MLT"].ToString().Substring(2, 2));
                if (result != null)
                {
                    en.NVDocSo = result.ToString();
                    en.NVDocSo += " ; " + apiTTKH.getLichDocSo_Func_String(DanhBo, dt.Rows[0]["MLT"].ToString());
                }
                //
                string KyNo = "";
                int TongNo = 0;
                sql = "select top 12 * from fnTimKiem('" + DanhBo + "','') order by MaHD desc";
                DataTable dt2 = cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                //lấy thu hộ
                DataTable dt3 = cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnThuHoChuaDangNgan(" + DanhBo + ")");
                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    en.NVThuTien = dt2.Rows[0]["NhanVien"].ToString();
                    en.NVThuTien += " ; " + apiTTKH.getLichThuTien_Func_String(DanhBo, dt2.Rows[0]["MLT"].ToString());

                    foreach (DataRow item in dt2.Rows)
                    {
                        HoaDonThuTien enCT = new HoaDonThuTien();
                        enCT.GiaBieu = item["GiaBieu"].ToString();
                        enCT.DinhMuc = item["DinhMuc"].ToString();
                        enCT.DinhMucHN = item["DinhMucHN"].ToString();
                        enCT.Ky = item["Ky"].ToString();
                        enCT.CSC = item["CSC"].ToString();
                        enCT.CSM = item["CSM"].ToString();
                        enCT.TieuThu = item["TieuThu"].ToString();
                        enCT.GiaBan = String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(item["GiaBan"].ToString()));
                        enCT.ThueGTGT = String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(item["ThueGTGT"].ToString()));
                        enCT.PhiBVMT = String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(item["PhiBVMT"].ToString()));
                        enCT.TongCong = String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", int.Parse(item["TongCong"].ToString()));

                        if (item["NgayGiaiTrach"].ToString() != "")
                            if (item["DangNgan"].ToString() != "CNKĐ")
                                enCT.NgayGiaiTrach = DateTime.Parse(item["NgayGiaiTrach"].ToString());
                            else
                            {
                                if (dt3 != null && dt3.Rows.Count > 0)
                                {
                                    if (dt3.Rows[0]["Kys"].ToString().Contains(enCT.Ky) == false)
                                        TongNo += int.Parse(item["TongCong"].ToString());
                                }
                                else
                                    TongNo += int.Parse(item["TongCong"].ToString());
                            }
                        else
                        {
                            KyNo += enCT.Ky + ", ";
                            if (dt3 != null && dt3.Rows.Count > 0)
                            {
                                if (dt3.Rows[0]["Kys"].ToString().Contains(enCT.Ky) == false)
                                    TongNo += int.Parse(item["TongCong"].ToString());
                            }
                            else
                                TongNo += int.Parse(item["TongCong"].ToString());
                        }

                        en.lstHoaDon.Add(enCT);
                        //
                        ChartData enChartData = new ChartData();
                        enChartData.Ky = enCT.Ky;
                        enChartData.TieuThu = int.Parse(enCT.TieuThu);
                        enChartData.ThanhTien = decimal.Parse(item["TongCong"].ToString());
                        en.ChartData.Add(enChartData);
                    }
                }
                //
                en.ThongTin = "Đã thanh toán hết";
                if (TongNo > 0)
                    en.ThongTin = "Số tiền chưa thanh toán: " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", TongNo);
                //
                int PhiMoNuoc = (int)cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select PhiMoNuoc=dbo.fnGetPhiMoNuoc(" + DanhBo + ")");
                if (PhiMoNuoc > 0)
                    en.ThongTin += "; Phí mở nước: " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", PhiMoNuoc);
                //
                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    en.ThongTin += "; Đã thu hộ " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", int.Parse(dt3.Rows[0]["TongCong"].ToString())) + " - Kỳ " + dt3.Rows[0]["Kys"].ToString() + " - ngày " + dt3.Rows[0]["CreateDate"].ToString() + " - qua " + dt3.Rows[0]["TenDichVu"].ToString();
                }
                //lấy thông tin đóng nước
                sql = "select top 1 CONVERT(varchar(10),NgayDN,103)+' '+CONVERT(varchar(10),NgayDN,108) from TT_KQDongNuoc where MoNuoc=0 and TroNgaiMN=0 and DanhBo='" + DanhBo + "' order by NgayDN desc";
                dt = cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                    en.ThongTinDongNuoc = "Địa chỉ đang tạm ngưng cung cấp nước từ " + dt.Rows[0][0].ToString() + " do chưa thanh toán tiền nước kỳ " + KyNo;
                if (en.ThongTinDongNuoc == "")
                {
                    sql = "select top 1 * from SuCoNgungCungCapNuoc where CAST(GETDATE() as date)>=CAST(DATEADD(DAY,-1,DateStart) as date) and CAST(GETDATE() as date)<=CAST(DateEnd as date) and (DMAs like ('%" + en.DMA + "%') or DanhBos like ('%" + DanhBo + "%') or (Quan like ('%" + Quan + "%') and Phuong like ('%" + Phuong + "%')))";
                    dt = cDAL_TTKH.ExecuteQuery_DataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                        en.ThongTinDongNuoc = dt.Rows[0]["NoiDung"].ToString();
                }
                cDAL_TTKH.ExecuteNonQuery("insert into QR_Log(DanhBo) values ('" + DanhBo + "')");
            }
            return View(en);
        }

        public ActionResult LichSuBamChi(string DanhBo)
        {
            List<ThongTinKhachHang> model = new List<ThongTinKhachHang>();
            if (DanhBo != null && DanhBo.Replace(" ", "").Replace("-", "").Length == 11)
            {
                DataTable dtNiemChi = apiTTKH.getDS_NiemChi(DanhBo.Replace(" ", "").Replace("-", ""));
                for (int i = 0; i < dtNiemChi.Rows.Count; i++)
                {
                    ThongTinKhachHang en = new ThongTinKhachHang();
                    en.MLT = (i + 1).ToString();
                    if (bool.Parse(dtNiemChi.Rows[i]["KhoaTu"].ToString()))
                        en.DanhBo = dtNiemChi.Rows[i]["NoiDung"].ToString() + ", Khóa Từ";
                    else
                        en.DanhBo = dtNiemChi.Rows[i]["NoiDung"].ToString() + ", Khóa Chì: " + dtNiemChi.Rows[i]["NiemChi"].ToString() + " " + dtNiemChi.Rows[i]["MauSac"].ToString();
                    model.Add(en);
                }
            }
            return View(model);
        }

        public ActionResult Login(string function, string DienThoai, string DienThoaiDK)
        {
            if (Session["DanhBo"] != null)
            {
                if (function == "login")
                {
                    if (DienThoai == "tanhoa123")
                    {
                        Session["LoginQRCode"] = true;
                        return Redirect(Session["Url"].ToString());
                    }
                    else
                    {
                        DataTable dt = cDAL_DHN.ExecuteQuery_DataTable("select DienThoai,CreateDate from SDT_DHN where DanhBo='" + Session["DanhBo"] + "' and DienThoai='" + DienThoai + "'"
                            + " union all"
                            + " select DienThoai,CreateDate from SDT_DHN_QRCode where DanhBo='" + Session["DanhBo"] + "' and DienThoai='" + DienThoai + "'"
                            + " order by CreateDate desc");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            Session["LoginQRCode"] = true;
                            return Redirect(Session["Url"].ToString());
                        }
                        else
                            ModelState.AddModelError("", "Sai số điện thoại xác nhận");
                    }
                }
                else
            if (function == "dangkydienthoai")
                {
                    if (DienThoaiDK.Length != 10)
                        ModelState.AddModelError("", "Số điện thoại di động 10 số");
                    if (DienThoaiDK.Substring(0, 1) != "0")
                        ModelState.AddModelError("", "Số điện thoại di động SAI");
                    if (DienThoaiDK.Length == 10 && DienThoaiDK.Substring(0, 1) == "0")
                        cDAL_DHN.ExecuteQuery_DataTable("insert into SDT_DHN_QRCode(DanhBo,DienThoai)values('" + Session["DanhBo"] + "','" + DienThoaiDK + "')");
                }
                List<MView> vDienThoai = new List<MView>();
                DataTable dt2 = cDAL_DHN.ExecuteQuery_DataTable("select DienThoai,CreateDate from SDT_DHN where DanhBo='" + Session["DanhBo"] + "'"
                    + " union all"
                    + " select DienThoai,CreateDate from SDT_DHN_QRCode where DanhBo='" + Session["DanhBo"] + "'"
                    + " order by CreateDate desc");
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    MView en = new MView();
                    en.TieuDe = (i + 1).ToString();
                    en.NoiDung = dt2.Rows[i]["DienThoai"].ToString().Substring(0, 7) + "xxx";
                    vDienThoai.Add(en);
                }
                if (vDienThoai.Count > 0)
                    ViewBag.vDienThoai = vDienThoai;
            }
            return View();
        }

        public ActionResult TienTrinhDon(string id)
        {
            MView enView = new MView();
            DataTable dt = new DataTable();
            if (id != null)
                if (id.ToLower().Contains("g"))
                {
                    dt = cDAL_GanMoi.ExecuteQuery_DataTable("SELECT  biennhan.SHS, biennhan.HOTEN,(SONHA + '  ' + DUONG + ',  P.' + p.TENPHUONG + ',  Q.' + q.TENQUAN) as 'DIACHI',"
                                + " CONVERT(char(10),biennhan.NGAYNHAN,103)+' '+CONVERT(char(5),biennhan.NGAYNHAN,108) AS 'CreateDate',lhs.TENLOAI as 'LOAIHS'"
                                + " FROM QUAN q,PHUONG p, BIENNHANDON biennhan, LOAI_HOSO lhs"
                                + " WHERE biennhan.QUAN = q.MAQUAN AND q.MAQUAN = p.MAQUAN  AND biennhan.PHUONG = p.MAPHUONG AND lhs.MALOAI = biennhan.LOAIDON"
                                + " AND biennhan.SHS = '" + id.ToLower().Replace("g", "") + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        MView en = new MView();
                        en.NoiDung = "Ngày nhận đơn";
                        en.ThoiGian = dt.Rows[0]["CreateDate"].ToString();
                        enView.lst.Add(en);

                        string sqlCT = "select NgayChuyenThietKe=CONVERT(char(10),NGAYCHUYEN_HOSO,103)+' '+CONVERT(char(5),NGAYCHUYEN_HOSO,108),NGAYDONGTIEN=CONVERT(char(10),NGAYDONGTIEN,103)+' '+CONVERT(char(5),NGAYDONGTIEN,108),SOTIEN"
                            + ",TRONGAITHIETKE,NOIDUNGTRONGAI,NGAYHOANTATTK=(select CONVERT(char(10),NGAYHOANTATTK,103)+' '+CONVERT(char(5),NGAYHOANTATTK,108) from TOTHIETKE where SOHOSO=donkh.SOHOSO)"
                            + " ,TaiXet=(select GHICHUTR from TMP_TAIXET where MAHOSO=donkh.SOHOSO)"
                                      + " ,NgayXinPhepDaoDuong = (select CONVERT(char(10),NGAYLAP,103)+' '+CONVERT(char(5),NGAYLAP,108) from KH_XINPHEPDAODUONG where MADOT = (select MADOTDD from KH_HOSOKHACHHANG where SHS = donkh.SHS))"
                                      + " ,NgayCoPhepDaoDuong = (select CONVERT(char(10),NGAYCOPHEP,103)+' '+CONVERT(char(5),NGAYCOPHEP,108) from KH_XINPHEPDAODUONG where MADOT = (select MADOTDD from KH_HOSOKHACHHANG where SHS = donkh.SHS))"
                                      + " ,CoPhep = (select CoPhep from KH_XINPHEPDAODUONG where MADOT = (select MADOTDD from KH_HOSOKHACHHANG where SHS = donkh.SHS))"
                                      + " ,NgayThiCong = (select CONVERT(char(10),NGAYTHICONG,103)+' '+CONVERT(char(5),NGAYTHICONG,108) from KH_HOSOKHACHHANG where SHS = donkh.SHS)"
                                      + " from DON_KHACHHANG donkh where SHS = '" + id.ToLower().Replace("g", "") + "'";
                        DataTable dtCT = cDAL_GanMoi.ExecuteQuery_DataTable(sqlCT);
                        if (dtCT != null && dtCT.Rows.Count > 0)
                        {
                            if (dtCT.Rows[0]["NgayChuyenThietKe"].ToString() != "")
                            {
                                en = new MView();
                                en.NoiDung = "Ngày chuyển thiết kế";
                                en.ThoiGian = dtCT.Rows[0]["NgayChuyenThietKe"].ToString();
                                enView.lst.Add(en);
                            }
                            if (dtCT.Rows[0]["SOTIEN"].ToString() != "")
                            {
                                en = new MView();
                                en.NoiDung = "Ngày đóng tiền";
                                en.ThoiGian = dtCT.Rows[0]["NGAYDONGTIEN"].ToString();
                                enView.lst.Add(en);
                            }
                            if (dtCT.Rows[0]["NgayXinPhepDaoDuong"].ToString() != "")
                            {
                                en = new MView();
                                en.NoiDung = "Ngày xin phép đào đường";
                                en.ThoiGian = dtCT.Rows[0]["NgayXinPhepDaoDuong"].ToString();
                                enView.lst.Add(en);
                            }
                            if (dtCT.Rows[0]["NgayCoPhepDaoDuong"].ToString() != "")
                            {
                                en = new MView();
                                en.NoiDung = "Ngày có phép đào đường";
                                en.ThoiGian = dtCT.Rows[0]["NgayCoPhepDaoDuong"].ToString();
                                enView.lst.Add(en);
                            }
                            if (dtCT.Rows[0]["NgayThiCong"].ToString() != "")
                            {
                                en = new MView();
                                en.NoiDung = "Ngày thi công dự kiến";
                                en.ThoiGian = dtCT.Rows[0]["NgayThiCong"].ToString();
                                enView.lst.Add(en);
                            }
                            if (dtCT.Rows[0]["TRONGAITHIETKE"].ToString() != "" && bool.Parse(dtCT.Rows[0]["TRONGAITHIETKE"].ToString()))
                            {
                                en = new MView();
                                en.NoiDung = "Hồ sơ trở ngại thiết kế";
                                en.ThoiGian = dtCT.Rows[0]["NOIDUNGTRONGAI"].ToString();
                                enView.lst.Add(en);
                            }
                            if (dtCT.Rows[0]["COPHEP"].ToString() != "" && !bool.Parse(dtCT.Rows[0]["COPHEP"].ToString()))
                            {
                                en = new MView();
                                en.NoiDung = "Hồ sơ trở ngại xin phép đào đường";
                                enView.lst.Add(en);
                            }
                            DataTable dtTN = cDAL_GanMoi.ExecuteQuery_DataTable("select NoiDungTN from KH_HOSOKHACHHANG where TroNgai=1 and SHS='" + id.ToLower().Replace("g", "") + "'");
                            if (dtTN != null && dtTN.Rows.Count > 0)
                            {
                                en = new MView();
                                en.NoiDung = "Hồ sơ trở ngại";
                                en.ThoiGian = dtCT.Rows[0]["NoiDungTN"].ToString();
                                enView.lst.Add(en);
                            }
                        }
                        ViewBag.DanhBo = dt.Rows[0]["HoTen"].ToString();
                        ViewBag.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                    }
                }
                else
                {
                    dt = cDAL_ThuongVu.ExecuteQuery_DataTable("select CreateDate=CONVERT(char(10),a.CreateDate,103)+' '+CONVERT(char(5),a.CreateDate,108),HieuLucKy,DinhMuc=SoNK*4,b.DanhBo,b.DiaChi"
        + " , NgayKTXM = (select top 1 CONVERT(char(10),NgayKTXM,103)+' '+CONVERT(char(5),NgayKTXM,108) from KTXM c, KTXM_ChiTiet d where c.MaKTXM = d.MaKTXM and c.MaDonMoi = a.MaDon and d.STT = b.STT order by NgayKTXM desc)"
        + " , NgayTTTL = (select top 1 CONVERT(char(10),d.CreateDate,103)+' '+CONVERT(char(5),d.CreateDate,108) from ThuTraLoi c, ThuTraLoi_ChiTiet d where c.MaTTTL = d.MaTTTL and c.MaDonMoi = a.MaDon and d.STT = b.STT order by d.CreateDate desc)"
        + " , IDTTTL = (select top 1 d.MaCTTTTL from ThuTraLoi c, ThuTraLoi_ChiTiet d where c.MaTTTL = d.MaTTTL and c.MaDonMoi = a.MaDon and d.STT = b.STT order by d.CreateDate desc)"
        + " from DonTu a,DonTu_ChiTiet b where a.MaDon = b.MaDon and a.MaDon = " + id);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        MView en = new MView();
                        en.NoiDung = "Ngày nhận đơn";
                        en.ThoiGian = dt.Rows[0]["CreateDate"].ToString();
                        enView.lst.Add(en);

                        if (dt.Rows[0]["NgayKTXM"].ToString() != "")
                        {
                            en = new MView();
                            en.NoiDung = "Ngày kiểm tra";
                            en.ThoiGian = dt.Rows[0]["NgayKTXM"].ToString();
                            enView.lst.Add(en);
                        }
                        if (dt.Rows[0]["HieuLucKy"].ToString() != "")
                        {
                            en = new MView();
                            en.NoiDung = "Hiệu lực kỳ";
                            en.ThoiGian = dt.Rows[0]["HieuLucKy"].ToString();
                            enView.lst.Add(en);

                            en = new MView();
                            en.NoiDung = "Định mức";
                            en.ThoiGian = dt.Rows[0]["DinhMuc"].ToString() + " m³";
                            enView.lst.Add(en);
                        }
                        if (dt.Rows[0]["IDTTTL"].ToString() != "")
                        {
                            en = new MView();
                            en.NoiDung = "Thư trả lời";
                            en.ThoiGian = dt.Rows[0]["NgayTTTL"].ToString();
                            en.DanhBo = dt.Rows[0]["IDTTTL"].ToString();
                            enView.lst.Add(en);
                        }
                        ViewBag.DanhBo = dt.Rows[0]["DanhBo"].ToString().Insert(7, " ").Insert(4, " ");
                        ViewBag.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                    }
                }
            return View(enView);
        }
    }
}