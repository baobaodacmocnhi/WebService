using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WSSmartPhone
{
    public class CKinhDoanh
    {
        CConnection _DAL = new CConnection("Data Source=113.161.88.180,1133;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");
        //CConnection _DAL = new CConnection("Data Source=serverg8-01;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");

        public DataTable getDS_GiaNuoc()
        {
            return _DAL.ExecuteQuery_DataTable("select * from GiaNuoc2");
        }

        public bool checkExists_GiaNuocGiam(int Nam, int Ky, int GiaBieu)
        {
            return (bool)_DAL.ExecuteQuery_ReturnOneValue("if exists(select * from GiaNuoc_Giam where Nam like '%" + Nam + "%' and Ky like '%" + Ky + "%' and GiaBieu like '%" + GiaBieu + "%') select 'true' else select 'false'");
        }

        public DataTable getGiaNuocGiam(int Nam, int Ky, int GiaBieu)
        {
            return _DAL.ExecuteQuery_DataTable("select * from GiaNuoc_Giam where Nam like '%" + Nam + "%' and Ky like '%" + Ky + "%' and GiaBieu like '%" + GiaBieu + "%'");
        }

        //chi tiết tiền nước
        private const int _GiamTienNuoc = 10;

        //public void TinhTienNuoc(bool ApGiaNuocCu, bool DieuChinhGia, int GiaDieuChinh, string DanhBo, int Ky, int Nam, DateTime TuNgay, DateTime DenNgay, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out int TienNuocCu, out string ChiTietCu, out int TienNuocMoi, out string ChiTietMoi, out int TieuThu_DieuChinhGia)
        //{
        //    List<GiaNuoc2> lst = _dbThuTien.GiaNuoc2s.ToList();
        //    int index = -1;
        //    TienNuocCu = TienNuocMoi = 0;
        //    ChiTietCu = ChiTietMoi = "";
        //    TieuThu_DieuChinhGia = 0;
        //    for (int i = 0; i < lst.Count; i++)
        //        if (TuNgay.Date < lst[i].NgayTangGia.Value.Date && lst[i].NgayTangGia.Value.Date < DenNgay.Date)
        //        {
        //            index = i;
        //        }
        //        else
        //            if (TuNgay.Date >= lst[i].NgayTangGia.Value.Date)
        //            {
        //                index = i;
        //            }
        //    if (index != -1)
        //    {
        //        if (DenNgay.Date < new DateTime(2019, 11, 15))
        //        {
        //            //int TieuThu_DieuChinhGia;
        //            List<int> lstGiaNuoc = new List<int> { lst[index].SHTM.Value, lst[index].SHVM1.Value, lst[index].SHVM2.Value, lst[index].SX.Value, lst[index].HCSN.Value, lst[index].KDDV.Value, lst[index].SHN.Value };
        //            TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, 0, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
        //        }
        //        else
        //            if (TuNgay.Date < lst[index].NgayTangGia.Value.Date && lst[index].NgayTangGia.Value.Date < DenNgay.Date)
        //            {
        //                if (ApGiaNuocCu == false)
        //                {
        //                    //int TieuThu_DieuChinhGia;
        //                    int TongSoNgay = (int)((DenNgay.Date - TuNgay.Date).TotalDays);

        //                    int SoNgayCu = (int)((lst[index].NgayTangGia.Value.Date - TuNgay.Date).TotalDays);
        //                    int TieuThuCu = (int)Math.Round((double)TieuThu * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
        //                    int TieuThuMoi = TieuThu - TieuThuCu;
        //                    int TongDinhMucCu = (int)Math.Round((double)TongDinhMuc * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
        //                    int TongDinhMucMoi = TongDinhMuc - TongDinhMucCu;
        //                    int DinhMucHN_Cu = 0, DinhMucHN_Moi = 0;
        //                    if (TuNgay.Date > new DateTime(2019, 11, 15))
        //                        if (TongDinhMucCu != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
        //                            DinhMucHN_Cu = (int)Math.Round((double)TongDinhMucCu * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
        //                    if (TongDinhMucMoi != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
        //                        DinhMucHN_Moi = (int)Math.Round((double)TongDinhMucMoi * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
        //                    List<int> lstGiaNuocCu = new List<int> { lst[index - 1].SHTM.Value, lst[index - 1].SHVM1.Value, lst[index - 1].SHVM2.Value, lst[index - 1].SX.Value, lst[index - 1].HCSN.Value, lst[index - 1].KDDV.Value, lst[index - 1].SHN.Value };
        //                    List<int> lstGiaNuocMoi = new List<int> { lst[index].SHTM.Value, lst[index].SHVM1.Value, lst[index].SHVM2.Value, lst[index].SX.Value, lst[index].HCSN.Value, lst[index].KDDV.Value, lst[index].SHN.Value };
        //                    //lần đầu áp dụng giá biểu 10, tổng áp giá mới luôn
        //                    if (TuNgay.Date < new DateTime(2019, 11, 15) && new DateTime(2019, 11, 15) < DenNgay.Date && GiaBieu == 10)
        //                        TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietCu, out TieuThu_DieuChinhGia);
        //                    else
        //                        TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietCu, out TieuThu_DieuChinhGia);
        //                    TienNuocMoi = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucMoi, DinhMucHN_Moi, TieuThuMoi, out ChiTietMoi, out TieuThu_DieuChinhGia);
        //                }
        //                else
        //                {
        //                    List<int> lstGiaNuocCu = new List<int> { lst[index - 1].SHTM.Value, lst[index - 1].SHVM1.Value, lst[index - 1].SHVM2.Value, lst[index - 1].SX.Value, lst[index - 1].HCSN.Value, lst[index - 1].KDDV.Value, lst[index - 1].SHN.Value };
        //                    TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
        //                }
        //            }
        //            else
        //            {
        //                //int TieuThu_DieuChinhGia;
        //                List<int> lstGiaNuoc = new List<int> { lst[index].SHTM.Value, lst[index].SHVM1.Value, lst[index].SHVM2.Value, lst[index].SX.Value, lst[index].HCSN.Value, lst[index].KDDV.Value, lst[index].SHN.Value };
        //                TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
        //            }
        //    }
        //    else
        //    {

        //    }
        //}

        //public int TinhTienNuoc(bool DieuChinhGia, int GiaDieuChinh, List<int> lstGiaNuoc, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out string ChiTiet, out int TieuThu_DieuChinhGia)
        //{
        //    try
        //    {
        //        string _chiTiet = "";
        //        int DinhMuc = TongDinhMuc - DinhMucHN, _SH = 0, _SX = 0, _DV = 0, _HCSN = 0;
        //        TieuThu_DieuChinhGia = 0;
        //        //HOADON hoadon = _cThuTien.Get(DanhBo, Ky, Nam);
        //        //List<GiaNuoc> lstGiaNuoc = db.GiaNuocs.ToList();
        //        ///Table GiaNuoc được thiết lập theo bảng giá nước
        //        ///1. Đến 4m3/người/tháng
        //        ///2. Trên 4m3 đến 6m3/người/tháng
        //        ///3. Trên 6m3/người/tháng
        //        ///4. Đơn vị sản xuất
        //        ///5. Cơ quan, đoàn thể HCSN
        //        ///6. Đơn vị kinh doanh, dịch vụ
        //        ///7. Hộ nghèo, cận nghèo
        //        ///List bắt đầu từ phần tử thứ 0
        //        int TongTien = 0;
        //        switch (GiaBieu)
        //        {
        //            ///TƯ GIA
        //            case 10:
        //                DinhMucHN = TongDinhMuc;
        //                if (TieuThu <= DinhMucHN)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[6];
        //                    if (TieuThu > 0)
        //                        _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                        if (TieuThu - DinhMucHN <= Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + ((TieuThu - DinhMucHN) * lstGiaNuoc[1]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if ((TieuThu - DinhMucHN) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + ((int)Math.Round((double)DinhMuc / 2) * lstGiaNuoc[1])
        //                                        + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if ((int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            if ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                    + ((TieuThu - DinhMucHN) * GiaDieuChinh);
        //                        if (DinhMucHN > 0)
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if ((TieuThu - DinhMucHN) > 0)
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                        if (lstGiaNuoc[6] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = TieuThu;
        //                        else
        //                            TieuThu_DieuChinhGia = TieuThu - DinhMucHN;
        //                    }
        //                break;
        //            case 11:
        //            case 21:///SH thuần túy
        //                //if (TieuThu <= DinhMucHN)
        //                //{
        //                //    TongTien = TieuThu * lstGiaNuoc[6];
        //                //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                //}
        //                //else
        //                //    if (TieuThu - DinhMucHN <= DinhMuc)
        //                //    {
        //                //        TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                //                    + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
        //                //        _chiTiet = (DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
        //                //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                //    }
        //                if (TieuThu <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = TieuThu - TieuThuHN;
        //                    TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                + (TieuThuDC * lstGiaNuoc[0]);
        //                    if (TieuThuHN > 0)
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    if (TieuThuDC > 0)
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                        if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6])
        //                                        + (DinhMuc * lstGiaNuoc[0])
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                        + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0])
        //                                    + ((TieuThu - DinhMuc) * GiaDieuChinh);
        //                        if (DinhMucHN > 0)
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (DinhMuc > 0)
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = TieuThu;
        //                        else
        //                            TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                    }
        //                break;
        //            case 12:
        //            case 22:
        //            case 32:
        //            case 42:///SX thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[3];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 13:
        //            case 23:
        //            case 33:
        //            case 43:///DV thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[5];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 14:
        //            case 24:///SH + SX
        //                ///Nếu không có tỉ lệ
        //                if (TyLeSH == 0 && TyLeSX == 0)
        //                {
        //                    //if (TieuThu <= DinhMucHN)
        //                    //{
        //                    //    TongTien = TieuThu * lstGiaNuoc[6];
        //                    //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (TieuThu - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
        //                    //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (TieuThu <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = TieuThu - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[3]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = TieuThu;
        //                            else
        //                                TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                        }
        //                }
        //                else
        //                ///Nếu có tỉ lệ SH + SX
        //                {
        //                    //int _SH = 0, _SX = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _SX = TieuThu - _SH;

        //                    //if (_SH <= DinhMucHN)
        //                    //{
        //                    //    TongTien = _SH * lstGiaNuoc[6];
        //                    //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (_SH - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH - DinhMucHN * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
        //                    //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                            if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            }
        //                            else
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                            + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                    updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = _SH;
        //                            else
        //                                TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                        }
        //                    TongTien += _SX * lstGiaNuoc[3];
        //                    if (_SX > 0)
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                }
        //                break;
        //            case 15:
        //            case 25:///SH + DV
        //                ///Nếu không có tỉ lệ
        //                if (TyLeSH == 0 && TyLeDV == 0)
        //                {
        //                    //if (TieuThu <= DinhMucHN)
        //                    //{
        //                    //    TongTien = TieuThu * lstGiaNuoc[6];
        //                    //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (TieuThu - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (TieuThu * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
        //                    //                    + TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (TieuThu <= DinhMucHN + DinhMuc)
        //                    {
        //                        //double TyLe = Math.Round((double)DinhMucHN / (DinhMucHN + DinhMuc), 2);
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = TieuThu - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[5]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = TieuThu;
        //                            else
        //                                TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                        }
        //                }
        //                else
        //                ///Nếu có tỉ lệ SH + DV
        //                {
        //                    //int _SH = 0, _DV = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SH;

        //                    //if (_SH <= DinhMucHN)
        //                    //{
        //                    //    TongTien = _SH * lstGiaNuoc[6];
        //                    //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (_SH - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH - DinhMucHN * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
        //                    //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                            if (_SH - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            }
        //                            else
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                            + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                    updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = _SH;
        //                            else
        //                                TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                        }
        //                    TongTien += _DV * lstGiaNuoc[5];
        //                    if (_DV > 0)
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                }
        //                break;
        //            case 16:
        //            case 26:///SH + SX + DV
        //                ///Nếu chỉ có tỉ lệ SX + DV mà không có tỉ lệ SH, không xét Định Mức
        //                if (TyLeSX != 0 && TyLeDV != 0 && TyLeSH == 0)
        //                {
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SX;
        //                    TongTien = (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
        //                    if (_SX > 0)
        //                        _chiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
        //                    if (_DV > 0)
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                }
        //                else
        //                ///Nếu có đủ 3 tỉ lệ SH + SX + DV
        //                {
        //                    //int _SH = 0, _SX = 0, _DV = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SH - _SX;

        //                    //if (_SH <= DinhMucHN)
        //                    //{
        //                    //    TongTien = _SH * lstGiaNuoc[6];
        //                    //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (_SH - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
        //                    //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                            if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMuc) * lstGiaNuoc[1]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            }
        //                            else
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                            + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                    updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = _SH;
        //                            else
        //                                TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                        }
        //                    TongTien += (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
        //                    if (_SX > 0)
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                    if (_DV > 0)
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                }
        //                break;
        //            case 17:
        //            case 27:///SH ĐB
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[0];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            case 18:
        //            case 28:
        //            case 38:///SH + HCSN
        //                ///Nếu không có tỉ lệ
        //                if (TyLeSH == 0 && TyLeHCSN == 0)
        //                {
        //                    //if (TieuThu <= DinhMucHN)
        //                    //{
        //                    //    TongTien = TieuThu * lstGiaNuoc[6];
        //                    //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (TieuThu - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
        //                    //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (TieuThu <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = TieuThu - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * lstGiaNuoc[4]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = TieuThu;
        //                            else
        //                                TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                        }
        //                }
        //                else
        //                ///Nếu có tỉ lệ SH + HCSN
        //                {
        //                    //int _SH = 0, _HCSN = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _HCSN = TieuThu - _SH;

        //                    //if (_SH <= DinhMucHN)
        //                    //{
        //                    //    TongTien = _SH * lstGiaNuoc[6];
        //                    //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    //}
        //                    //else
        //                    //    if (_SH - DinhMucHN <= DinhMuc)
        //                    //    {
        //                    //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((_SH - DinhMucHN) * lstGiaNuoc[0]);
        //                    //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
        //                    //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                    //    }
        //                    if (_SH <= DinhMucHN + DinhMuc)
        //                    {
        //                        double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                        int TieuThuHN = 0, TieuThuDC = 0;
        //                        TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                        TieuThuDC = _SH - TieuThuHN;
        //                        TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                    + (TieuThuDC * lstGiaNuoc[0]);
        //                        if (TieuThuHN > 0)
        //                            _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (TieuThuDC > 0)
        //                            updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                    }
        //                    else
        //                        if (!DieuChinhGia)
        //                            if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[0]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            }
        //                            else
        //                            {
        //                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                            + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                                if (DinhMucHN > 0)
        //                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                                if (DinhMuc > 0)
        //                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                                if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                    updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                                if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                            }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                            if (lstGiaNuoc[0] == GiaDieuChinh)
        //                                TieuThu_DieuChinhGia = _SH;
        //                            else
        //                                TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                        }
        //                    TongTien += _HCSN * lstGiaNuoc[4];
        //                    if (_HCSN > 0)
        //                        updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
        //                }
        //                break;
        //            case 19:
        //            case 29:
        //            case 39:///SH + HCSN + SX + DV
        //                //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeHCSN != 0)
        //                    _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeSX != 0)
        //                    _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH - _HCSN - _SX;

        //                //if (_SH <= DinhMucHN)
        //                //{
        //                //    TongTien = _SH * lstGiaNuoc[6];
        //                //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                //}
        //                //else
        //                //    if (_SH - DinhMucHN <= DinhMuc)
        //                //    {
        //                //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((_SH - DinhMucHN) * lstGiaNuoc[0]);
        //                //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
        //                //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
        //                //    }
        //                if (_SH <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = _SH - TieuThuHN;
        //                    TongTien = (TieuThuHN * lstGiaNuoc[6])
        //                                + (TieuThuDC * lstGiaNuoc[0]);
        //                    if (TieuThuHN > 0)
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                    if (TieuThuDC > 0)
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                        if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
        //                        if (DinhMucHN > 0)
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
        //                        if (DinhMuc > 0)
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

        //                        if (lstGiaNuoc[0] == GiaDieuChinh)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                    }
        //                TongTien += (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
        //                if (_HCSN > 0)
        //                    updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
        //                if (_SX > 0)
        //                    updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                if (_DV > 0)
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                break;
        //            ///TẬP THỂ
        //            //case 21:///SH thuần túy
        //            //    if (TieuThu <= DinhMuc)
        //            //        TongTien = TieuThu * lstGiaNuoc[0];
        //            //    else
        //            //        if (TieuThu - DinhMuc <= DinhMuc / 2)
        //            //            TongTien = (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * lstGiaNuoc[1]);
        //            //        else
        //            //            TongTien = (DinhMuc * lstGiaNuoc[0]) + (DinhMuc / 2 * lstGiaNuoc[1]) + ((TieuThu - DinhMuc - DinhMuc / 2) * lstGiaNuoc[2]);
        //            //    break;
        //            //case 22:///SX thuần túy
        //            //    TongTien = TieuThu * lstGiaNuoc[3];
        //            //    break;
        //            //case 23:///DV thuần túy
        //            //    TongTien = TieuThu * lstGiaNuoc[5];
        //            //    break;
        //            //case 24:///SH + SX
        //            //    hoadon = _cThuTien.GetMoiNhat(DanhBo);
        //            //    if (hoadon != null)
        //            //        ///Nếu không có tỉ lệ
        //            //        if (hoadon.TILESH==null && hoadon.TILESX==null)
        //            //        {

        //            //        }
        //            //    break;
        //            //case 25:///SH + DV

        //            //    break;
        //            //case 26:///SH + SX + DV

        //            //    break;
        //            //case 27:///SH ĐB
        //            //    TongTien = TieuThu * lstGiaNuoc[0];
        //            //    break;
        //            //case 28:///SH + HCSN

        //            //    break;
        //            //case 29:///SH + HCSN + SX + DV

        //            //    break;
        //            ///CƠ QUAN
        //            case 31:///SHVM thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[4];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            //case 32:///SX
        //            //    TongTien = TieuThu * lstGiaNuoc[3];
        //            //    break;
        //            //case 33:///DV
        //            //    TongTien = TieuThu * lstGiaNuoc[5];
        //            //    break;
        //            case 34:///HCSN + SX
        //                ///Nếu không có tỉ lệ
        //                if (TyLeHCSN == 0 && TyLeSX == 0)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[3];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
        //                }
        //                else
        //                ///Nếu có tỉ lệ
        //                {
        //                    //int _HCSN = 0, _SX = 0;
        //                    if (TyLeHCSN != 0)
        //                        _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                    _SX = TieuThu - _HCSN;

        //                    TongTien = (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]);
        //                    if (_HCSN > 0)
        //                        _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                    if (_SX > 0)
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                }
        //                break;
        //            case 35:///HCSN + DV
        //                ///Nếu không có tỉ lệ
        //                if (TyLeHCSN == 0 && TyLeDV == 0)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[5];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
        //                }
        //                else
        //                ///Nếu có tỉ lệ
        //                {
        //                    //int _HCSN = 0, _DV = 0;
        //                    if (TyLeHCSN != 0)
        //                        _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _HCSN;

        //                    TongTien = (_HCSN * lstGiaNuoc[4]) + (_DV * lstGiaNuoc[5]);
        //                    if (_HCSN > 0)
        //                        _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                    if (_DV > 0)
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                }
        //                break;
        //            case 36:///HCSN + SX + DV
        //                {
        //                    //int _HCSN = 0, _SX = 0, _DV = 0;
        //                    if (TyLeHCSN != 0)
        //                        _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _HCSN - _SX;

        //                    TongTien = (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
        //                    if (_HCSN > 0)
        //                        _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
        //                    if (_SX > 0)
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                    if (_DV > 0)
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                }
        //                break;
        //            //case 38:///SH + HCSN

        //            //    break;
        //            //case 39:///SH + HCSN + SX + DV

        //            //    break;
        //            ///NƯỚC NGOÀI
        //            case 41:///SHVM thuần túy
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * lstGiaNuoc[2];
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * GiaDieuChinh;
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                break;
        //            //case 42:///SX
        //            //    TongTien = TieuThu * lstGiaNuoc[3];
        //            //    break;
        //            //case 43:///DV
        //            //    TongTien = TieuThu * lstGiaNuoc[5];
        //            //    break;
        //            case 44:///SH + SX
        //                {
        //                    //int _SH = 0, _SX = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    _SX = TieuThu - _SH;

        //                    TongTien = (_SH * lstGiaNuoc[2]) + (_SX * lstGiaNuoc[3]);
        //                    if (_SH > 0)
        //                        _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                    if (_SX > 0)
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                }
        //                break;
        //            case 45:///SH + DV
        //                //int _SH = 0, _DV = 0;
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH;

        //                TongTien = (_SH * lstGiaNuoc[2]) + (_DV * lstGiaNuoc[5]);
        //                if (_SH > 0)
        //                    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                if (_DV > 0)
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                break;
        //            case 46:///SH + SX + DV
        //                {
        //                    //int _SH = 0, _SX = 0, _DV = 0;
        //                    if (TyLeSH != 0)
        //                        _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                    if (TyLeSX != 0)
        //                        _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                    _DV = TieuThu - _SH - _SX;

        //                    TongTien = (_SH * lstGiaNuoc[2]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
        //                    if (_SH > 0)
        //                        _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
        //                    if (_SX > 0)
        //                        updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
        //                    if (_DV > 0)
        //                        updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                }
        //                break;
        //            ///BÁN SỈ
        //            case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
        //                //if (TieuThu <= DinhMuc)
        //                //    TongTien = TieuThu * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100));
        //                //else
        //                //    if (TieuThu - DinhMuc <= DinhMuc / 2)
        //                //        TongTien = (DinhMuc * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100))) + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - (lstGiaNuoc[1] * 10 / 100)));
        //                //    else
        //                //        TongTien = (DinhMuc * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100))) + (DinhMuc / 2 * (lstGiaNuoc[1] - (lstGiaNuoc[1] * 10 / 100))) + ((TieuThu - DinhMuc - DinhMuc / 2) * (lstGiaNuoc[2] - (lstGiaNuoc[2] * 10 / 100)));
        //                //if (TieuThu <= DinhMucHN)
        //                //{
        //                //    TongTien = TieuThu * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
        //                //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                //}
        //                //else
        //                //    if (TieuThu - DinhMucHN <= DinhMuc)
        //                //    {
        //                //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (TieuThu - DinhMucHN * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                //    }
        //                if (TieuThu <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = TieuThu - TieuThuHN;
        //                    TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    if (TieuThuHN > 0)
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                    if (TieuThuDC > 0)
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                        if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)) + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
        //                                        + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((TieuThu - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                        if (DinhMucHN > 0)
        //                            _chiTiet = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        if (DinhMuc > 0)
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        if ((TieuThu - DinhMucHN - DinhMuc) > 0)
        //                            updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

        //                        if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
        //                            TieuThu_DieuChinhGia = TieuThu;
        //                        else
        //                            TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
        //                    }
        //                //TongTien -= TongTien * 10 / 100;
        //                break;
        //            case 52:///sỉ khu công nghiệp
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100);
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100));
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                //TongTien -= TongTien * 10 / 100;
        //                break;
        //            case 53:///sỉ KD - TM
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100);
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                //TongTien -= TongTien * 10 / 100;
        //                break;
        //            case 54:///sỉ HCSN
        //                if (!DieuChinhGia)
        //                {
        //                    TongTien = TieuThu * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100);
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100));
        //                }
        //                else
        //                {
        //                    TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
        //                    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                    TieuThu_DieuChinhGia = TieuThu;
        //                }
        //                //TongTien -= TongTien * 10 / 100;
        //                break;
        //            case 58:
        //                //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeHCSN != 0)
        //                    _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeSX != 0)
        //                    _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH - _HCSN - _SX;

        //                TongTien += (_SH * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                            + (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
        //                            + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
        //                            + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
        //                if (_SH > 0)
        //                    _chiTiet += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                if (_HCSN > 0)
        //                    updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
        //                if (_SX > 0)
        //                    updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
        //                if (_DV > 0)
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
        //                break;
        //            case 59:///sỉ phức tạp
        //                //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeHCSN != 0)
        //                    _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
        //                if (TyLeSX != 0)
        //                    _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH - _HCSN - _SX;

        //                //if (_SH <= DinhMucHN)
        //                //{
        //                //    TongTien = _SH * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
        //                //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                //}
        //                //else
        //                //    if (_SH - DinhMucHN <= DinhMuc)
        //                //    {
        //                //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (_SH - DinhMucHN * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + "\r\n"
        //                //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                //    }
        //                if (_SH <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = _SH - TieuThuHN;
        //                    TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    if (TieuThuHN > 0)
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                    if (TieuThuDC > 0)
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                        if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                        + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                        + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                        if (DinhMucHN > 0)
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        if (DinhMuc > 0)
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

        //                        if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
        //                    }
        //                TongTien += (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)) + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)) + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
        //                if (_HCSN > 0)
        //                    updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
        //                if (_SX > 0)
        //                    updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
        //                if (_DV > 0)
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
        //                //TongTien -= TongTien * 10 / 100;
        //                break;
        //            case 68:///SH giá sỉ - KD giá lẻ
        //                //int _SH = 0, _DV = 0;
        //                if (TyLeSH != 0)
        //                    _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
        //                _DV = TieuThu - _SH;

        //                //if (_SH <= DinhMucHN)
        //                //{
        //                //    TongTien = _SH * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
        //                //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                //}
        //                //else
        //                //    if (_SH - DinhMucHN <= DinhMuc)
        //                //    {
        //                //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + ((_SH - DinhMucHN) * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                //    }
        //                if (_SH <= DinhMucHN + DinhMuc)
        //                {
        //                    double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
        //                    int TieuThuHN = 0, TieuThuDC = 0;
        //                    TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
        //                    TieuThuDC = _SH - TieuThuHN;
        //                    TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
        //                    if (TieuThuHN > 0)
        //                        _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                    if (TieuThuDC > 0)
        //                        updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                }
        //                else
        //                    if (!DieuChinhGia)
        //                        if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
        //                        {
        //                            TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                        + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                        + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                        }
        //                        else
        //                        {
        //                            TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                        + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                        + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
        //                                        + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
        //                            if (DinhMucHN > 0)
        //                                _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                            if (DinhMuc > 0)
        //                                updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                            if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
        //                                updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
        //                            if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
        //                                updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
        //                        }
        //                    else
        //                    {
        //                        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
        //                                    + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
        //                                    + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
        //                        if (DinhMucHN > 0)
        //                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
        //                        if (DinhMuc > 0)
        //                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
        //                        if ((_SH - DinhMucHN - DinhMuc) > 0)
        //                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

        //                        if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
        //                            TieuThu_DieuChinhGia = _SH;
        //                        else
        //                            TieuThu_DieuChinhGia = _SH - DinhMuc;
        //                    }
        //                TongTien += _DV * lstGiaNuoc[5];
        //                if (_DV > 0)
        //                    updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
        //                //TongTien -= TongTien * 10 / 100;
        //                break;
        //            default:
        //                _chiTiet = "";
        //                TongTien = 0;
        //                break;
        //        }
        //        ChiTiet = _chiTiet;
        //        return TongTien;
        //    }
        //    catch (Exception)
        //    {
        //        ChiTiet = "";
        //        TieuThu_DieuChinhGia = 0;
        //        return 0;
        //    }
        //}

        public int TinhTienNuoc(bool DieuChinhGia, int GiaDieuChinh, List<int> lstGiaNuoc, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, out string ChiTiet, out int TieuThu_DieuChinhGia)
        {
            try
            {
                string _chiTiet = "";
                int DinhMuc = TongDinhMuc - DinhMucHN, _SH = 0, _SX = 0, _DV = 0, _HCSN = 0;
                TieuThu_DieuChinhGia = 0;
                //HOADON hoadon = _cThuTien.Get(DanhBo, Ky, Nam);
                //List<GiaNuoc> lstGiaNuoc = db.GiaNuocs.ToList();
                ///Table GiaNuoc được thiết lập theo bảng giá nước
                ///1. Đến 4m3/người/tháng
                ///2. Trên 4m3 đến 6m3/người/tháng
                ///3. Trên 6m3/người/tháng
                ///4. Đơn vị sản xuất
                ///5. Cơ quan, đoàn thể HCSN
                ///6. Đơn vị kinh doanh, dịch vụ
                ///7. Hộ nghèo, cận nghèo
                ///List bắt đầu từ phần tử thứ 0
                int TongTien = 0;
                switch (GiaBieu)
                {
                    ///TƯ GIA
                    case 10:
                        DinhMucHN = TongDinhMuc;
                        if (TieuThu <= DinhMucHN)
                        {
                            TongTien = TieuThu * lstGiaNuoc[6];
                            if (TieuThu > 0)
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN <= Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + ((TieuThu - DinhMucHN) * lstGiaNuoc[1]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if ((TieuThu - DinhMucHN) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + ((int)Math.Round((double)DinhMuc / 2) * lstGiaNuoc[1])
                                                + ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if ((int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    if ((TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - (int)Math.Round((double)DinhMucHN / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6])
                                            + ((TieuThu - DinhMucHN) * GiaDieuChinh);
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if ((TieuThu - DinhMucHN) > 0)
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                if (lstGiaNuoc[6] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN;
                            }
                        break;
                    case 11:
                    case 21:///SH thuần túy
                        //if (TieuThu <= DinhMucHN)
                        //{
                        //    TongTien = TieuThu * lstGiaNuoc[6];
                        //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                        //}
                        //else
                        //    if (TieuThu - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * lstGiaNuoc[6])
                        //                    + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
                        //        _chiTiet = (DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                        //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                        //    }
                        if (TieuThu <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = TieuThu - TieuThuHN;
                            TongTien = (TieuThuHN * lstGiaNuoc[6])
                                        + (TieuThuDC * lstGiaNuoc[0]);
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6])
                                                + (DinhMuc * lstGiaNuoc[0])
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0])
                                            + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                if (lstGiaNuoc[0] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                            }
                        break;
                    case 12:
                    case 22:
                    case 32:
                    case 42:///SX thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[3];
                            if (TieuThu > 0)
                                _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 13:
                    case 23:
                    case 33:
                    case 43:///DV thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[5];
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 14:
                    case 24:///SH + SX
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeSX == 0)
                        {
                            //if (TieuThu <= DinhMucHN)
                            //{
                            //    TongTien = TieuThu * lstGiaNuoc[6];
                            //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (TieuThu - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                            //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[3]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = TieuThu;
                                    else
                                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                                }
                        }
                        else
                        ///Nếu có tỉ lệ SH + SX
                        {
                            //int _SH = 0, _SX = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _SH;

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH - DinhMucHN * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _SX * lstGiaNuoc[3];
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        }
                        break;
                    case 15:
                    case 25:///SH + DV
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeDV == 0)
                        {
                            //if (TieuThu <= DinhMucHN)
                            //{
                            //    TongTien = TieuThu * lstGiaNuoc[6];
                            //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (TieuThu - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (TieuThu * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                            //                    + TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                //double TyLe = Math.Round((double)DinhMucHN / (DinhMucHN + DinhMuc), 2);
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * lstGiaNuoc[5]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = TieuThu;
                                    else
                                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                                }
                        }
                        else
                        ///Nếu có tỉ lệ SH + DV
                        {
                            //int _SH = 0, _DV = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH;

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH - DinhMucHN * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _DV * lstGiaNuoc[5];
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    case 16:
                    case 26:///SH + SX + DV
                        ///Nếu chỉ có tỉ lệ SX + DV mà không có tỉ lệ SH, không xét Định Mức
                        if (TyLeSX != 0 && TyLeDV != 0 && TyLeSH == 0)
                        {
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SX;
                            TongTien = (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_SX > 0)
                                _chiTiet = _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        else
                        ///Nếu có đủ 3 tỉ lệ SH + SX + DV
                        {
                            //int _SH = 0, _SX = 0, _DV = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH - _SX;

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (_SH * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    case 17:
                    case 27:///SH ĐB
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[0];
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    case 18:
                    case 28:
                    case 38:///SH + HCSN
                        ///Nếu không có tỉ lệ
                        if (TyLeSH == 0 && TyLeHCSN == 0)
                        {
                            //if (TieuThu <= DinhMucHN)
                            //{
                            //    TongTien = TieuThu * lstGiaNuoc[6];
                            //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (TieuThu - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN) * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (TieuThu <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = TieuThu - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * lstGiaNuoc[4]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = TieuThu;
                                    else
                                        TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                                }
                        }
                        else
                        ///Nếu có tỉ lệ SH + HCSN
                        {
                            //int _SH = 0, _HCSN = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _HCSN = TieuThu - _SH;

                            //if (_SH <= DinhMucHN)
                            //{
                            //    TongTien = _SH * lstGiaNuoc[6];
                            //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            //}
                            //else
                            //    if (_SH - DinhMucHN <= DinhMuc)
                            //    {
                            //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((_SH - DinhMucHN) * lstGiaNuoc[0]);
                            //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6])
                            //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                            //    }
                            if (_SH <= DinhMucHN + DinhMuc)
                            {
                                double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                                int TieuThuHN = 0, TieuThuDC = 0;
                                TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                                TieuThuDC = _SH - TieuThuHN;
                                TongTien = (TieuThuHN * lstGiaNuoc[6])
                                            + (TieuThuDC * lstGiaNuoc[0]);
                                if (TieuThuHN > 0)
                                    _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (TieuThuDC > 0)
                                    updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                            }
                            else
                                if (!DieuChinhGia)
                                    if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[0]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((_SH - DinhMucHN - DinhMuc) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    }
                                    else
                                    {
                                        TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                    + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                        if (DinhMucHN > 0)
                                            _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                        if (DinhMuc > 0)
                                            updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                        if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                            updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                        if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                            updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                    }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                    if (lstGiaNuoc[0] == GiaDieuChinh)
                                        TieuThu_DieuChinhGia = _SH;
                                    else
                                        TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                                }
                            TongTien += _HCSN * lstGiaNuoc[4];
                            if (_HCSN > 0)
                                updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                        }
                        break;
                    case 19:
                    case 29:
                    case 39:///SH + HCSN + SX + DV
                        //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;

                        //if (_SH <= DinhMucHN)
                        //{
                        //    TongTien = _SH * lstGiaNuoc[6];
                        //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                        //}
                        //else
                        //    if (_SH - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * lstGiaNuoc[6]) + ((_SH - DinhMucHN) * lstGiaNuoc[0]);
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]) + "\r\n"
                        //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]);
                        //    }
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * lstGiaNuoc[6])
                                        + (TieuThuDC * lstGiaNuoc[0]);
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * lstGiaNuoc[1]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * lstGiaNuoc[1])
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * lstGiaNuoc[2]);
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[1]));
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * lstGiaNuoc[6]) + (DinhMuc * lstGiaNuoc[0]) + ((_SH - DinhMucHN - DinhMuc) * GiaDieuChinh);
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[6]);
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[0]));
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh));

                                if (lstGiaNuoc[0] == GiaDieuChinh)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                            }
                        TongTien += (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                        if (_HCSN > 0)
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]));
                        if (_SX > 0)
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        break;
                    ///TẬP THỂ
                    //case 21:///SH thuần túy
                    //    if (TieuThu <= DinhMuc)
                    //        TongTien = TieuThu * lstGiaNuoc[0];
                    //    else
                    //        if (TieuThu - DinhMuc <= DinhMuc / 2)
                    //            TongTien = (DinhMuc * lstGiaNuoc[0]) + ((TieuThu - DinhMuc) * lstGiaNuoc[1]);
                    //        else
                    //            TongTien = (DinhMuc * lstGiaNuoc[0]) + (DinhMuc / 2 * lstGiaNuoc[1]) + ((TieuThu - DinhMuc - DinhMuc / 2) * lstGiaNuoc[2]);
                    //    break;
                    //case 22:///SX thuần túy
                    //    TongTien = TieuThu * lstGiaNuoc[3];
                    //    break;
                    //case 23:///DV thuần túy
                    //    TongTien = TieuThu * lstGiaNuoc[5];
                    //    break;
                    //case 24:///SH + SX
                    //    hoadon = _cThuTien.GetMoiNhat(DanhBo);
                    //    if (hoadon != null)
                    //        ///Nếu không có tỉ lệ
                    //        if (hoadon.TILESH==null && hoadon.TILESX==null)
                    //        {

                    //        }
                    //    break;
                    //case 25:///SH + DV

                    //    break;
                    //case 26:///SH + SX + DV

                    //    break;
                    //case 27:///SH ĐB
                    //    TongTien = TieuThu * lstGiaNuoc[0];
                    //    break;
                    //case 28:///SH + HCSN

                    //    break;
                    //case 29:///SH + HCSN + SX + DV

                    //    break;
                    ///CƠ QUAN
                    case 31:///SHVM thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[4];
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    //case 32:///SX
                    //    TongTien = TieuThu * lstGiaNuoc[3];
                    //    break;
                    //case 33:///DV
                    //    TongTien = TieuThu * lstGiaNuoc[5];
                    //    break;
                    case 34:///HCSN + SX
                        ///Nếu không có tỉ lệ
                        if (TyLeHCSN == 0 && TyLeSX == 0)
                        {
                            TongTien = TieuThu * lstGiaNuoc[3];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]);
                        }
                        else
                        ///Nếu có tỉ lệ
                        {
                            //int _HCSN = 0, _SX = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _HCSN;

                            TongTien = (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]);
                            if (_HCSN > 0)
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        }
                        break;
                    case 35:///HCSN + DV
                        ///Nếu không có tỉ lệ
                        if (TyLeHCSN == 0 && TyLeDV == 0)
                        {
                            TongTien = TieuThu * lstGiaNuoc[5];
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]);
                        }
                        else
                        ///Nếu có tỉ lệ
                        {
                            //int _HCSN = 0, _DV = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _HCSN;

                            TongTien = (_HCSN * lstGiaNuoc[4]) + (_DV * lstGiaNuoc[5]);
                            if (_HCSN > 0)
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    case 36:///HCSN + SX + DV
                        {
                            //int _HCSN = 0, _SX = 0, _DV = 0;
                            if (TyLeHCSN != 0)
                                _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _HCSN - _SX;

                            TongTien = (_HCSN * lstGiaNuoc[4]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_HCSN > 0)
                                _chiTiet = _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[4]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    //case 38:///SH + HCSN

                    //    break;
                    //case 39:///SH + HCSN + SX + DV

                    //    break;
                    ///NƯỚC NGOÀI
                    case 41:///SHVM thuần túy
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * lstGiaNuoc[2];
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                        }
                        else
                        {
                            TongTien = TieuThu * GiaDieuChinh;
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", GiaDieuChinh);
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        break;
                    //case 42:///SX
                    //    TongTien = TieuThu * lstGiaNuoc[3];
                    //    break;
                    //case 43:///DV
                    //    TongTien = TieuThu * lstGiaNuoc[5];
                    //    break;
                    case 44:///SH + SX
                        {
                            //int _SH = 0, _SX = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            _SX = TieuThu - _SH;

                            TongTien = (_SH * lstGiaNuoc[2]) + (_SX * lstGiaNuoc[3]);
                            if (_SH > 0)
                                _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                        }
                        break;
                    case 45:///SH + DV
                        //int _SH = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH;

                        TongTien = (_SH * lstGiaNuoc[2]) + (_DV * lstGiaNuoc[5]);
                        if (_SH > 0)
                            _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        break;
                    case 46:///SH + SX + DV
                        {
                            //int _SH = 0, _SX = 0, _DV = 0;
                            if (TyLeSH != 0)
                                _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                            if (TyLeSX != 0)
                                _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                            _DV = TieuThu - _SH - _SX;

                            TongTien = (_SH * lstGiaNuoc[2]) + (_SX * lstGiaNuoc[3]) + (_DV * lstGiaNuoc[5]);
                            if (_SH > 0)
                                _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[2]);
                            if (_SX > 0)
                                updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[3]));
                            if (_DV > 0)
                                updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        }
                        break;
                    ///BÁN SỈ
                    case 51:///sỉ khu dân cư - Giảm % tiền nước cho ban quản lý chung cư
                        //if (TieuThu <= DinhMuc)
                        //    TongTien = TieuThu * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100));
                        //else
                        //    if (TieuThu - DinhMuc <= DinhMuc / 2)
                        //        TongTien = (DinhMuc * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100))) + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - (lstGiaNuoc[1] * 10 / 100)));
                        //    else
                        //        TongTien = (DinhMuc * (lstGiaNuoc[0] - (lstGiaNuoc[0] * 10 / 100))) + (DinhMuc / 2 * (lstGiaNuoc[1] - (lstGiaNuoc[1] * 10 / 100))) + ((TieuThu - DinhMuc - DinhMuc / 2) * (lstGiaNuoc[2] - (lstGiaNuoc[2] * 10 / 100)));
                        //if (TieuThu <= DinhMucHN)
                        //{
                        //    TongTien = TieuThu * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
                        //    _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                        //}
                        //else
                        //    if (TieuThu - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (TieuThu - DinhMucHN * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                        //                    + (TieuThu - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //    }
                        if (TieuThu <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(TieuThu * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = TieuThu - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                        }
                        else
                            if (!DieuChinhGia)
                                if (TieuThu - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)) + ((TieuThu - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                    if ((TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((TieuThu - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                if (DinhMucHN > 0)
                                    _chiTiet = +DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                if ((TieuThu - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (TieuThu - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = TieuThu;
                                else
                                    TieuThu_DieuChinhGia = TieuThu - DinhMucHN - DinhMuc;
                            }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 52:///sỉ khu công nghiệp
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100);
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100));
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 53:///sỉ KD - TM
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100);
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 54:///sỉ HCSN
                        if (!DieuChinhGia)
                        {
                            TongTien = TieuThu * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100);
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100));
                        }
                        else
                        {
                            TongTien = TieuThu * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100);
                            if (TieuThu > 0)
                            _chiTiet = TieuThu + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                            TieuThu_DieuChinhGia = TieuThu;
                        }
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 58:
                        //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;

                        TongTien += (_SH * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                    + (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100))
                                    + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100))
                                    + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        if (_SH > 0)
                            _chiTiet += _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        if (_HCSN > 0)
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
                        if (_SX > 0)
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
                        break;
                    case 59:///sỉ phức tạp
                        //int _SH = 0, _HCSN = 0, _SX = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeHCSN != 0)
                            _HCSN = (int)Math.Round((double)TieuThu * TyLeHCSN / 100, 0, MidpointRounding.AwayFromZero);
                        if (TyLeSX != 0)
                            _SX = (int)Math.Round((double)TieuThu * TyLeSX / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH - _HCSN - _SX;

                        //if (_SH <= DinhMucHN)
                        //{
                        //    TongTien = _SH * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
                        //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                        //}
                        //else
                        //    if (_SH - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + (_SH - DinhMucHN * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + "\r\n"
                        //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //    }
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMucHN - DinhMuc;
                            }
                        TongTien += (_HCSN * (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)) + (_SX * (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)) + (_DV * (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100));
                        if (_HCSN > 0)
                            updateChiTiet(ref _chiTiet, _HCSN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[4] - lstGiaNuoc[4] * _GiamTienNuoc / 100)));
                        if (_SX > 0)
                            updateChiTiet(ref _chiTiet, _SX + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[3] - lstGiaNuoc[3] * _GiamTienNuoc / 100)));
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[5] - lstGiaNuoc[5] * _GiamTienNuoc / 100)));
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    case 68:///SH giá sỉ - KD giá lẻ
                        //int _SH = 0, _DV = 0;
                        if (TyLeSH != 0)
                            _SH = (int)Math.Round((double)TieuThu * TyLeSH / 100, 0, MidpointRounding.AwayFromZero);
                        _DV = TieuThu - _SH;

                        //if (_SH <= DinhMucHN)
                        //{
                        //    TongTien = _SH * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100);
                        //    _chiTiet = _SH + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                        //}
                        //else
                        //    if (_SH - DinhMucHN <= DinhMuc)
                        //    {
                        //        TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100)) + ((_SH - DinhMucHN) * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                        //                    + (_SH - DinhMucHN) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                        //    }
                        if (_SH <= DinhMucHN + DinhMuc)
                        {
                            double TyLe = (double)DinhMucHN / (DinhMucHN + DinhMuc);
                            int TieuThuHN = 0, TieuThuDC = 0;
                            TieuThuHN = (int)Math.Round(_SH * TyLe, 0, MidpointRounding.AwayFromZero);
                            TieuThuDC = _SH - TieuThuHN;
                            TongTien = (TieuThuHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                        + (TieuThuDC * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100));
                            if (TieuThuHN > 0)
                                _chiTiet = TieuThuHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                            if (TieuThuDC > 0)
                                updateChiTiet(ref _chiTiet, TieuThuDC + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                        }
                        else
                            if (!DieuChinhGia)
                                if (_SH - DinhMucHN - DinhMuc <= Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero))
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                }
                                else
                                {
                                    TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                                + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                                + ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) * (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100))
                                                + ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) * (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100));
                                    if (DinhMucHN > 0)
                                        _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                    if (DinhMuc > 0)
                                        updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                    if ((int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) > 0)
                                        updateChiTiet(ref _chiTiet, (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[1] - lstGiaNuoc[1] * _GiamTienNuoc / 100)));
                                    if ((_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) > 0)
                                        updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc - (int)Math.Round((double)(DinhMucHN + DinhMuc) / 2, 0, MidpointRounding.AwayFromZero)) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[2] - lstGiaNuoc[2] * _GiamTienNuoc / 100)));
                                }
                            else
                            {
                                TongTien = (DinhMucHN * (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100))
                                            + (DinhMuc * (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100))
                                            + ((_SH - DinhMucHN - DinhMuc) * (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100));
                                if (DinhMucHN > 0)
                                    _chiTiet = DinhMucHN + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[6] - lstGiaNuoc[6] * _GiamTienNuoc / 100));
                                if (DinhMuc > 0)
                                    updateChiTiet(ref _chiTiet, DinhMuc + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100)));
                                if ((_SH - DinhMucHN - DinhMuc) > 0)
                                    updateChiTiet(ref _chiTiet, (_SH - DinhMucHN - DinhMuc) + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", (GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)));

                                if (lstGiaNuoc[0] - lstGiaNuoc[0] * _GiamTienNuoc / 100 == GiaDieuChinh - GiaDieuChinh * _GiamTienNuoc / 100)
                                    TieuThu_DieuChinhGia = _SH;
                                else
                                    TieuThu_DieuChinhGia = _SH - DinhMuc;
                            }
                        TongTien += _DV * lstGiaNuoc[5];
                        if (_DV > 0)
                            updateChiTiet(ref _chiTiet, _DV + " x " + String.Format(System.Globalization.CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##}", lstGiaNuoc[5]));
                        //TongTien -= TongTien * 10 / 100;
                        break;
                    default:
                        _chiTiet = "";
                        TongTien = 0;
                        break;
                }
                ChiTiet = _chiTiet;
                return TongTien;
            }
            catch (Exception ex)
            {
                ChiTiet = "";
                TieuThu_DieuChinhGia = 0;
                throw ex;
            }
        }

        public void TinhTienNuoc(bool KhongApGiaGiam, bool ApGiaNuocCu, bool DieuChinhGia, int GiaDieuChinh, string DanhBo, int Ky, int Nam, DateTime TuNgay, DateTime DenNgay, int GiaBieu, int TyLeSH, int TyLeSX, int TyLeDV, int TyLeHCSN, int TongDinhMuc, int DinhMucHN, int TieuThu, ref int TienNuocCu, ref string ChiTietCu, ref int TienNuocMoi, ref string ChiTietMoi, ref int TieuThu_DieuChinhGia)
        {
            try
            {
                DataTable dtGiaNuoc = getDS_GiaNuoc();
                //check giảm giá
                if (KhongApGiaGiam == false)
                    checkExists_GiaNuocGiam(Nam, Ky, GiaBieu, ref dtGiaNuoc);

                int index = -1;
                TienNuocCu = TienNuocMoi = 0;
                ChiTietCu = ChiTietMoi = "";
                TieuThu_DieuChinhGia = 0;
                for (int i = 0; i < dtGiaNuoc.Rows.Count; i++)
                    if (TuNgay.Date < DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date < DenNgay.Date)
                    {
                        index = i;
                    }
                    else
                        if (TuNgay.Date >= DateTime.Parse(dtGiaNuoc.Rows[i]["NgayTangGia"].ToString()).Date)
                        {
                            index = i;
                        }
                if (index != -1)
                {
                    if (DenNgay.Date < new DateTime(2019, 11, 15))
                    {
                        //int TieuThu_DieuChinhGia;
                        List<int> lstGiaNuoc = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()) };
                        TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, 0, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
                    }
                    else
                        if (TuNgay.Date < DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date && DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date < DenNgay.Date)
                        {
                            if (ApGiaNuocCu == false)
                            {
                                //int TieuThu_DieuChinhGia;
                                int TongSoNgay = (int)((DenNgay.Date - TuNgay.Date).TotalDays);

                                int SoNgayCu = (int)((DateTime.Parse(dtGiaNuoc.Rows[index]["NgayTangGia"].ToString()).Date - TuNgay.Date).TotalDays);
                                int TieuThuCu = (int)Math.Round((double)TieuThu * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
                                int TieuThuMoi = TieuThu - TieuThuCu;
                                int TongDinhMucCu = (int)Math.Round((double)TongDinhMuc * SoNgayCu / TongSoNgay, 0, MidpointRounding.AwayFromZero);
                                int TongDinhMucMoi = TongDinhMuc - TongDinhMucCu;
                                int DinhMucHN_Cu = 0, DinhMucHN_Moi = 0;
                                if (TuNgay.Date > new DateTime(2019, 11, 15))
                                    if (TongDinhMucCu != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
                                        DinhMucHN_Cu = (int)Math.Round((double)TongDinhMucCu * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
                                if (TongDinhMucMoi != 0 && DinhMucHN != 0 && TongDinhMuc != 0)
                                    DinhMucHN_Moi = (int)Math.Round((double)TongDinhMucMoi * DinhMucHN / TongDinhMuc, 0, MidpointRounding.AwayFromZero);
                                List<int> lstGiaNuocCu = new List<int> { int.Parse(dtGiaNuoc.Rows[index - 1]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHN"].ToString()) };
                                List<int> lstGiaNuocMoi = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()) };
                                //lần đầu áp dụng giá biểu 10, tổng áp giá mới luôn
                                if (TuNgay.Date < new DateTime(2019, 11, 15) && new DateTime(2019, 11, 15) < DenNgay.Date && GiaBieu == 10)
                                    TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietCu, out TieuThu_DieuChinhGia);
                                else
                                    TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucCu, DinhMucHN_Cu, TieuThuCu, out ChiTietCu, out TieuThu_DieuChinhGia);
                                TienNuocMoi = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocMoi, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMucMoi, DinhMucHN_Moi, TieuThuMoi, out ChiTietMoi, out TieuThu_DieuChinhGia);
                            }
                            else
                            {
                                List<int> lstGiaNuocCu = new List<int> { int.Parse(dtGiaNuoc.Rows[index - 1]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index - 1]["SHN"].ToString()) };
                                TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuocCu, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
                            }
                        }
                        else
                        {
                            //int TieuThu_DieuChinhGia;
                            List<int> lstGiaNuoc = new List<int> { int.Parse(dtGiaNuoc.Rows[index]["SHTM"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM1"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHVM2"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SX"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["HCSN"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["KDDV"].ToString()), int.Parse(dtGiaNuoc.Rows[index]["SHN"].ToString()) };
                            TienNuocCu = TinhTienNuoc(DieuChinhGia, GiaDieuChinh, lstGiaNuoc, GiaBieu, TyLeSH, TyLeSX, TyLeDV, TyLeHCSN, TongDinhMuc, DinhMucHN, TieuThu, out ChiTietCu, out TieuThu_DieuChinhGia);
                        }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //giảm giá nước

        public bool checkExists_GiaNuocGiam(int Nam, int Ky, int GiaBieu, ref DataTable dt)
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

        public void updateChiTiet(ref string main_value, string update_value)
        {
            if (main_value == "")
                main_value = update_value;
            else
                main_value += "\r\n" + update_value;
        }

        public string getDanhBo_CatTam(string ID)
        {
            object result = _DAL.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatTam where MaCTCTDB=" + ID);
            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public string getDanhBo_CatHuy(string ID)
        {
            object result = _DAL.ExecuteQuery_ReturnOneValue("select DanhBo from CHDB_ChiTietCatHuy where MaCTCHDB=" + ID);
            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public bool ThemHinhDHN(string DanhBo, string CreateBy, string imageStr, string Latitude, string Longitude)
        {
            //int ID = 0;
            //if (int.Parse(_DAL.ExecuteQuery_DataTable("select COUNT(ID) from HinhDHN").Rows[0][0].ToString()) == 0)
            //    ID = 1;
            //else
            //    ID = int.Parse(_DAL.ExecuteQuery_DataTable("select MAX(ID)+1 from HinhDHN").Rows[0][0].ToString());
            //string sql = "insert into HinhDHN(ID,DanhBo,Hinh,CreateBy,CreateDate)values(" + ID + ",'" + DanhBo + "'," + image + "," + CreateBy + ",GETDATE())";
            //return _DAL.ExecuteNonQuery(sql);

            try
            {
                //dbDocSoDataContext db = new dbDocSoDataContext();
                //Byte[] image = System.Convert.FromBase64String(imageStr);

                //HinhDHN entity = new HinhDHN();
                //if (db.HinhDHNs.Count() == 0)
                //    entity.ID = 1;
                //else
                //    entity.ID = db.HinhDHNs.Max(item => item.ID) + 1;

                //entity.DanhBo = DanhBo;
                //entity.Image = image;
                //entity.Latitude = Latitude;
                //entity.Longitude = Longitude;
                //entity.CreateBy = int.Parse(CreateBy);
                //entity.CreateDate = DateTime.Now;
                //db.HinhDHNs.InsertOnSubmit(entity);
                //db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}