﻿using System;
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
        private apiTrungTamKhachHangController apiTTKH = new apiTrungTamKhachHangController();

        // GET: KhachHang
        public ActionResult ThongTin(string DanhBo)
        {
            ThongTinKhachHang en = new ThongTinKhachHang();
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
                         + ",NgayKiemDinh"
                         + ",HieuLuc=convert(varchar(2),Ky)+'/'+convert(char(4),Nam)"
                         + " from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo + "'";
            DataTable dt = cDAL_DHN.ExecuteQuery_DataTable(sql);
            if (dt.Rows.Count > 0)
            {
                en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                en.HoTen = dt.Rows[0]["HoTen"].ToString();
                en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                en.HopDong = dt.Rows[0]["HopDong"].ToString();
                en.MLT = dt.Rows[0]["MLT"].ToString();
                en.GiaBieu = dt.Rows[0]["GiaBieu"].ToString();
                en.DinhMuc = dt.Rows[0]["DinhMuc"].ToString();
                en.DinhMucHN = dt.Rows[0]["DinhMucHN"].ToString();
                //
                en.NVDocSo = cDAL_DocSo.ExecuteQuery_ReturnOneValue("select N''+NhanVienID+' : '+DienThoai from MayDS where May=" + dt.Rows[0]["MLT"].ToString().Substring(2, 2)).ToString();
                en.NVDocSo += " ; " + apiTTKH.getLichDocSo_Func_String(DanhBo, dt.Rows[0]["MLT"].ToString());
                //
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
            }

            return View(en);
        }
    }
}