﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class ThuongVuController : Controller
    {
        private CConnection _cDAL_KinhDoanh = new CConnection(CGlobalVariable.KinhDoanh);
        private CConnection _cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);

        // GET: ThuongVu
        public ActionResult viewFile(string TableName, string IDFileName, string IDFileContent)
        {
            if (TableName != null && IDFileName != null && IDFileContent != null && TableName != "" && IDFileName != "" && IDFileContent != "")
            {
                byte[] FileContent = getFile(TableName, IDFileName, IDFileContent);
                if (FileContent != null)
                    return new FileStreamResult(new MemoryStream(FileContent), "image/jpeg");
                else
                    return View();
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
                    int GiaBanCu = 0, GiaBanMoi = 0, ThueGTGT = 0, PhiBVMTCu = 0, PhiBVMTMoi = 0, ThueGTGTTDVTN = 0, TongCong = 0, TieuThu_DieuChinhGia = 0;
                    string ChiTietCu = "", ChiTietMoi = "", ChiTietPhiBVMTCu = "", ChiTietPhiBVMTMoi = "";
                    WebReference.wsThuTien ws = new WebReference.wsThuTien();
                    string[] TuNgays = collection["txtTuNgay"].ToString().Split('-');
                    string[] DenNgays = collection["txtDenNgay"].ToString().Split('-');
                    ws.TinhTienNuoc(false, false, false, 0, collection["txtDanhBo"].ToString(), int.Parse(collection["txtKy"].ToString()), int.Parse(collection["txtNam"].ToString()), new DateTime(int.Parse(TuNgays[0]), int.Parse(TuNgays[1]), int.Parse(TuNgays[2])), new DateTime(int.Parse(DenNgays[0]), int.Parse(DenNgays[1]), int.Parse(DenNgays[2])), int.Parse(collection["txtGiaBieu"].ToString()), int.Parse(collection["txtSH"].ToString()), int.Parse(collection["txtSX"].ToString()), int.Parse(collection["txtDV"].ToString()), int.Parse(collection["txtHCSN"].ToString()), int.Parse(collection["txtDinhMuc"].ToString()), int.Parse(collection["txtDinhMucHN"].ToString()), int.Parse(collection["txtTieuThu"].ToString()), ref GiaBanCu, ref ChiTietCu, ref GiaBanMoi, ref ChiTietMoi, ref TieuThu_DieuChinhGia, ref PhiBVMTCu, ref ChiTietPhiBVMTCu, ref PhiBVMTMoi, ref ChiTietPhiBVMTMoi);
                    ThueGTGT = (int)Math.Round((double)(GiaBanCu + GiaBanMoi) * 5 / 100, 0, MidpointRounding.AwayFromZero);

                    ViewBag.txtGiaBanCu = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaBanCu);
                    ViewBag.txtGiaBanMoi = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaBanMoi);
                    ViewBag.txtGiaBan = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaBanCu + GiaBanMoi));

                    ViewBag.txtThueGTGT = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", ThueGTGT);

                    ViewBag.txtTDVTNCu = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", PhiBVMTCu);
                    ViewBag.txtTDVTNMoi = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", PhiBVMTMoi);
                    ViewBag.txtTDVTN = String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (PhiBVMTCu + PhiBVMTMoi));

                    ViewBag.txtChiTietCu = ChiTietCu;
                    ViewBag.txtChiTietTDVTNCu = ChiTietPhiBVMTCu;
                    ViewBag.txtChiTietMoi = ChiTietMoi;
                    ViewBag.txtChiTietTDVTNMoi = ChiTietPhiBVMTMoi;
                    //Từ 2022 Phí BVMT -> Tiền Dịch Vụ Thoát Nước
                    if ((DateTime.Parse(collection["txtTuNgay"].ToString()).Year < 2021) || (DateTime.Parse(collection["txtTuNgay"].ToString()).Year == 2021 && DateTime.Parse(collection["txtDenNgay"].ToString()).Year == 2021))
                    {
                        TongCong = (GiaBanCu + GiaBanMoi) + ThueGTGT + (PhiBVMTCu + PhiBVMTMoi);
                    }
                    else
                        if (DateTime.Parse(collection["txtTuNgay"].ToString()).Year == 2021 && DateTime.Parse(collection["txtDenNgay"].ToString()).Year == 2022)
                    {
                        ThueGTGTTDVTN = (int)Math.Round((double)PhiBVMTMoi * 10 / 100, 0, MidpointRounding.AwayFromZero);
                        TongCong = (GiaBanCu + GiaBanMoi) + ThueGTGT + (PhiBVMTCu + PhiBVMTMoi) + ThueGTGTTDVTN;
                    }
                    else
                            if (DateTime.Parse(collection["txtTuNgay"].ToString()).Year >= 2022)
                    {
                        ThueGTGTTDVTN = (int)Math.Round((double)(PhiBVMTCu + PhiBVMTMoi) * 10 / 100, 0, MidpointRounding.AwayFromZero);
                        TongCong = (GiaBanCu + GiaBanMoi) + ThueGTGT + (PhiBVMTCu + PhiBVMTMoi) + ThueGTGTTDVTN;
                    }
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

    }
}