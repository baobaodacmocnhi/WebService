using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    public class QLDHNTCTController : Controller
    {
        private CConnection cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        private CConnection cDAL_sDHN = new CConnection(CGlobalVariable.sDHN);
        private apiTrungTamKhachHangController apiTTKH = new apiTrungTamKhachHangController();
        private wrDHN.wsDHN wsDHN = new wrDHN.wsDHN();
        private wrThuongVu.wsThuongVu wsThuongVu = new wrThuongVu.wsThuongVu();

        // GET: QLDHNTCT
        public ActionResult sDHN(FormCollection collection, string function, string TuNgay, string DenNgay, string NgayXem)
        {
            DataTable dtNCC = cDAL_sDHN.ExecuteQuery_DataTable("select ID,Name from sDHN_NCC");
            ViewBag.NCC = ToSelectList(dtNCC, "ID", "Name");
            dtNCC = cDAL_sDHN.ExecuteQuery_DataTable("select DMA=MADMA from sDHN_TCT sdhn,[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh"
                 + " where Valid = 1 and sdhn.DanhBo = ttkh.DANHBO"
                 + " group by MADMA"
                 + " order by MADMA");
            ViewBag.DMA = ToSelectList(dtNCC, "DMA", "DMA");

            object soluong = cDAL_sDHN.ExecuteQuery_ReturnOneValue("select SoLuong=COUNT(*) from sDHN_NCC a,sDHN_TCT b,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG c"
                                    + " where a.ID = b.IDNCC and b.DanhBo = c.DANHBO and Valid = 1");
            ViewBag.SoLuong = soluong;
            DataTable dt = cDAL_sDHN.ExecuteQuery_DataTable("select b.IDNCC,Name,SoLuong=COUNT(*),SoLuongLD=(SELECT count(*) FROM CAPNUOCTANHOA.dbo.TB_THAYDHN WHERE DHN_LOAIBANGKE='DHTM' AND HCT_NGAYGAN IS NOT NULL"
                                    + " and HCT_HIEUDHNGAN = (select a1.HIEU_DHTM from sDHN.dbo.DHTM_THONGTIN a1, sDHN.dbo.sDHN_NCC b1 where a1.ID = b.IDNCC and a1.ID = b1.ID))"
                                    + " from sDHN_NCC a,sDHN_TCT b, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG c"
                                    + " where a.ID = b.IDNCC and b.DanhBo = c.DANHBO and Valid = 1"
                                    + " group by IDNCC,Name order by IDNCC");
            List<MView> vTong = new List<MView>();
            foreach (DataRow item in dt.Rows)
            {
                MView en = new MView();
                en.TieuDe = item["Name"].ToString();
                en.SoLuong = item["SoLuong"].ToString() + " (" + item["SoLuongLD"].ToString() + ")";
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
                            sql += ",'" + fromdate.ToString("dd/MM/yyyy") + "'=(select count(DanhBo) from sDHN_LichSu_TCT where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)='" + fromdate.ToString("yyyyMMdd") + "')";
                            fromdate = fromdate.AddDays(1);
                        }
                    }
                    else
                    {
                        while (fromdate.Date <= todate.Date)
                        {
                            sql += ",ChiSo=(select top 1 ChiSo from sDHN_LichSu_TCT where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)='" + fromdate.ToString("yyyyMMdd") + "' and DATEPART(HOUR, ThoiGianCapNhat)=" + collection["Hour"].ToString() + ")"
                            + ",ThoiGian=(select top 1 ThoiGianCapNhat from sDHN_LichSu_TCT where DanhBo=ttkh.DanhBo and cast(ThoiGianCapNhat as date)='" + fromdate.ToString("yyyyMMdd") + "' and DATEPART(HOUR, ThoiGianCapNhat)=" + collection["Hour"].ToString() + ")";
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
                            + ",CSsDHN=(select ChiSo from"
                            + " (select top 1 diff =case when datediff(SECOND, (select convert(varchar(20),GioGhi,120) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "), ThoiGianCapNhat) < 0 then datediff(SECOND, (select convert(varchar(20),GioGhi,120) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "), ThoiGianCapNhat) * -1 else datediff(SECOND, (select convert(varchar(20),GioGhi,120) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "), ThoiGianCapNhat) end"
                            + " , ChiSo, ThoiGianCapNhat from sDHN_LichSu_TCT where CAST(ThoiGianCapNhat as date) = (select cast(GioGhi as date) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + ") and DanhBo=ttkh.DanhBo order by diff)t1)"
                            + ",ThoiGiansDHN=(select ThoiGianCapNhat from"
                            + " (select top 1 diff =case when datediff(SECOND, (select convert(varchar(20),GioGhi,120) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "), ThoiGianCapNhat) < 0 then datediff(SECOND, (select convert(varchar(20),GioGhi,120) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "), ThoiGianCapNhat) * -1 else datediff(SECOND, (select convert(varchar(20),GioGhi,120) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + "), ThoiGianCapNhat) end"
                            + " , ChiSo, ThoiGianCapNhat from sDHN_LichSu_TCT where CAST(ThoiGianCapNhat as date) = (select cast(GioGhi as date) from DocSoTH.dbo.DocSo where danhba=sdhn.DanhBo and nam=" + KyHD[1] + " and ky=" + KyHD[0] + ") and DanhBo=ttkh.DanhBo order by diff)t1)";
                    }
                }
                sql += " from sDHN_TCT sdhn,[DHTM_THONGTIN] ttdhn,[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh"
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
                else
                    if (collection["radLoai"].ToString() == "radAll")
                {
                    filename = "TatCa";
                }
                if (collection["radLoai"].ToString() == "radDanhBo")
                {
                    sql += " and sdhn.DanhBo=" + collection["DanhBo"].ToString();
                    filename = "DanhBo." + collection["DanhBo"].ToString();
                }
                sql += " order by IDNCC";
                if (collection["radLoai"].ToString() == "radDanhBo")
                {
                    string[] fromdatestr = collection["TuNgay"].ToString().Split('/');
                    DateTime fromdate = new DateTime(int.Parse(fromdatestr[2]), int.Parse(fromdatestr[1]), int.Parse(fromdatestr[0]));
                    string[] todatestr = collection["DenNgay"].ToString().Split('/');
                    DateTime todate = new DateTime(int.Parse(todatestr[2]), int.Parse(todatestr[1]), int.Parse(todatestr[0]));
                    sql = "select DanhBo=ttkh.DANHBO,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG"
                + " ,SoThanDH,Hieu_DHTM,ThoiGianCapNhat,ChiSo,TieuThu = 0.0"
                + " from sDHN_TCT sdhn,sDHN_LichSu_TCT ls,[DHTM_THONGTIN] ttdhn,[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh"
                + " where Valid=1 and sdhn.IDNCC=ttdhn.ID and sdhn.DanhBo= ttkh.DANHBO and sdhn.DanhBo= ls.DanhBo"
                + " and sdhn.DanhBo= '" + collection["DanhBo"].ToString() + "' and cast(ThoiGianCapNhat as date)>='" + fromdate.ToString("yyyyMMdd") + "' and cast(ThoiGianCapNhat as date)<='" + todate.ToString("yyyyMMdd") + "' order by ThoiGianCapNhat";
                    dt = cDAL_sDHN.ExecuteQuery_DataTable(sql);
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["TieuThu"] = (double.Parse(dt.Rows[i]["ChiSo"].ToString()) - double.Parse(dt.Rows[i - 1]["ChiSo"].ToString())).ToString();
                    }
                    filename = "DanhBo." + collection["DanhBo"].ToString();
                }
                else
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

        //-------------------------

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

        public string getKhongTinHieu_sDHN(string SoNgay, string Ngay, string KhongDu24records)
        {
            try
            {
                List<MView> vKhongTinHieu = new List<MView>();
                DataTable dt;
                if (!bool.Parse(KhongDu24records))
                    dt = getDS_KhongTinHieu_sDHN("", "", SoNgay, Ngay);
                else
                    dt = getDS_BatThuong_sDHN("", "", SoNgay, Ngay);
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
        public DataTable getDS_KhongTinHieu_sDHN(string function, string NCC, string SoNgay, string Ngay)
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
                    sql += " select IDNCC,DanhBo,SoLuong=(select COUNT(*) from sDHN_LichSu_TCT where DanhBo=sDHN_TCT.DanhBo and CAST(ThoiGianCapNhat as date)='" + date.ToString("yyyyMMdd") + "')"
                        + " from sDHN_TCT where Valid = 1";
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
                    sql += " select IDNCC,DanhBo,SoLuong=(select COUNT(*) from sDHN_LichSu_TCT where DanhBo=sDHN_TCT.DanhBo and CAST(ThoiGianCapNhat as date)='" + date.ToString("yyyyMMdd") + "')"
                        + " from sDHN_TCT where Valid = 1";
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

        [NonAction]
        public DataTable getDS_BatThuong_sDHN(string function, string NCC, string SoNgay, string Ngay)
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
                    sql += " select IDNCC,DanhBo,SoLuong=(select COUNT(*) from sDHN_LichSu_TCT where DanhBo=sDHN_TCT.DanhBo and CAST(ThoiGianCapNhat as date)='" + date.ToString("yyyyMMdd") + "')"
                        + " from sDHN_TCT where Valid = 1";
                    if (count != 1)
                        sql += " union all";
                    count--;
                    date = date.AddDays(-1);
                }
                sql += ")t1,sDHN_NCC ncc"
                        + " where t1.SoLuong > 0 and t1.SoLuong < 24 and t1.IDNCC = ncc.ID and t1.IDNCC like '%" + NCC + "%'"
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
                    sql += " select IDNCC,DanhBo,SoLuong=(select COUNT(*) from sDHN_LichSu_TCT where DanhBo=sDHN_TCT.DanhBo and CAST(ThoiGianCapNhat as date)='" + date.ToString("yyyyMMdd") + "')"
                        + " from sDHN_TCT where Valid = 1";
                    if (count != 1)
                        sql += " union all";
                    count--;
                    date = date.AddDays(-1);
                }
                sql += ")t1,sDHN_NCC ncc"
                        + " where t1.SoLuong > 0 and t1.SoLuong < 24 and t1.IDNCC = ncc.ID and t1.IDNCC like '%" + NCC + "%'"
                        + " group by IDNCC,ncc.Name,t1.DanhBo"
                        + " having COUNT(t1.DanhBo) = " + SoNgay
                        + " order by IDNCC";
            }
            return cDAL_sDHN.ExecuteQuery_DataTable(sql);
        }

        //-------------------------

        public string getCanhBao_sDHN(string Ngay)
        {
            try
            {
                string[] datestr = Ngay.Split('/');
                DateTime date = new DateTime(int.Parse(datestr[2]), int.Parse(datestr[1]), int.Parse(datestr[0]));
                List<MView> vCanhBao = new List<MView>();
                DataTable dt = getDS_CanhBao_PinYeu_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                dt = getDS_CanhBao_RoRi_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                dt = getDS_CanhBao_QuaDong_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                dt = getDS_CanhBao_ChayNguoc_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                dt = getDS_CanhBao_NamCham_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                dt = getDS_CanhBao_KhoOng_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                dt = getDS_CanhBao_MoHop_sDHN(date);
                if (dt != null && dt.Rows.Count > 0)
                {
                    MView en = new MView();
                    en.TieuDe = dt.Rows[0]["Loai"].ToString();
                    en.SoLuong = dt.Rows.Count.ToString();
                    en.NoiDung = dt.Rows[0]["Loai2"].ToString();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MView enCT = new MView();
                        enCT.DanhBo = dt.Rows[i]["DanhBo"].ToString();
                        enCT.HoTen = dt.Rows[i]["NCC"].ToString();
                        enCT.DiaChi = dt.Rows[i]["DiaChi"].ToString();
                        en.lst.Add(enCT);
                    }
                    vCanhBao.Add(en);
                }
                return CGlobalVariable.jsSerializer.Serialize(vCanhBao);
            }
            catch
            {
                return null;
            }
        }

        [NonAction]
        public DataTable getDS_CanhBao_PinYeu_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Pin Yếu',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='PinYeu' from"
                                                    + " (select distinct DanhBo from sDHN_LichSu_TCT"
                                                    + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and CBPinYeu = 1)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1"
                                                    + " order by NCC");
        }

        [NonAction]
        public DataTable getDS_CanhBao_RoRi_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Rò Rỉ',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='RoRi' from"
                                                     + " (select DanhBo, SL = COUNT(*) from sDHN_LichSu_TCT"
                                                     + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and Diff - 0.5 >= 0.5 group by DanhBo)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                     + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1 and t1.SL = (select COUNT(DanhBo) from sDHN_LichSu_TCT"
                                                     + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and DanhBo = t1.DanhBo)"
                                                     + " order by NCC");
        }

        [NonAction]
        public DataTable getDS_CanhBao_QuaDong_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Quá Dòng',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='QuaDong' from"
                                                    + " (select distinct DanhBo from sDHN_LichSu_TCT"
                                                    + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and CBQuaDong = 1)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1"
                                                    + " order by NCC");
        }

        [NonAction]
        public DataTable getDS_CanhBao_ChayNguoc_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Chạy Ngược',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='ChayNguoc' from"
                                                    + " (select distinct DanhBo from sDHN_LichSu_TCT"
                                                    + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and Diff<=-0.5)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1"
                                                    + " order by NCC");
        }

        [NonAction]
        public DataTable getDS_CanhBao_NamCham_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Nam Châm',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='NamCham' from"
                                                    + " (select distinct DanhBo from sDHN_LichSu_TCT"
                                                    + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and CBNamCham = 1)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1"
                                                    + " order by NCC");
        }

        [NonAction]
        public DataTable getDS_CanhBao_KhoOng_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Khô Ống',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='KhoOng' from"
                                                    + " (select distinct DanhBo from sDHN_LichSu_TCT"
                                                    + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and CBKhoOng = 1)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1"
                                                    + " order by NCC");
        }

        [NonAction]
        public DataTable getDS_CanhBao_MoHop_sDHN(DateTime date)
        {
            return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Mở Hộp',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA,IDNCC,NCC=ncc.Name,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='MoHop' from"
                                                    + " (select distinct DanhBo from sDHN_LichSu_TCT"
                                                    + " where CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyyMMdd") + "' and CBMoHop = 1)t1, sDHN_NCC ncc, sDHN_TCT dhn, DHTM_THONGTIN ttsdhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                    + " where ncc.ID = dhn.IDNCC and ncc.ID = ttsdhn.ID and dhn.DanhBo = t1.DanhBo and dhn.DanhBo = ttkh.DANHBO and Valid = 1"
                                                    + " order by NCC");
        }

        //-------------------------

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
                    DataTable dtBinhThuong = getDS_BinhThuong_sDHN("", date);
                    DataTable dtBatThuong = getDS_BatThuong_sDHN("", date);
                    DataTable dtKhongTinHieu = getDS_KhongTinHieu_sDHN("", date);

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
        public DataTable getDS_KhongTinHieu_sDHN(string function, DateTime date)
        {
            if (function == "export")
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Không Tín Hiệu',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='0' from"
                                                        + " (select DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu_TCT ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN_TCT dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                        + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong = 0 and t1.IDNCC=ttsdhn.ID order by NCC");
            else
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Không Tín Hiệu',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='0' from"
                                                        + " (select DanhBo=dhn.DanhBo, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu_TCT ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN_TCT dhn"
                                                        + " where ncc.ID=dhn.IDNCC and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong = 0 and t1.IDNCC=ttsdhn.ID order by NCC");
        }

        [NonAction]
        public DataTable getDS_BinhThuong_sDHN(string function, DateTime date)
        {
            if (function == "export")
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bình Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='2' from"
                                                        + " (select DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu_TCT ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN_TCT dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                        + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong >= 24 and t1.IDNCC=ttsdhn.ID order by NCC");
            else
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bình Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='2' from"
                                              + " (select DanhBo=dhn.DanhBo, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu_TCT ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                              + " from sDHN_NCC ncc,sDHN_TCT dhn"
                                              + " where ncc.ID=dhn.IDNCC and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                              + " where t1.SoLuong >= 24 and t1.IDNCC=ttsdhn.ID order by NCC");
        }

        [NonAction]
        public DataTable getDS_BatThuong_sDHN(string function, DateTime date)
        {
            if (function == "export")
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bất Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='1' from"
                                                        + " (select DanhBo=dhn.DanhBo,MLT=ttkh.LOTRINH,HoTen=ttkh.HOTEN,DiaChi=ttkh.SONHA+' '+ttkh.TENDUONG,DMA=ttkh.MADMA, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu_TCT ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                        + " from sDHN_NCC ncc,sDHN_TCT dhn, CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG ttkh"
                                                        + " where ncc.ID=dhn.IDNCC and dhn.DanhBo = ttkh.DANHBO and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                        + " where t1.SoLuong > 0 and t1.SoLuong < 24 and t1.IDNCC=ttsdhn.ID order by NCC");
            else
                return cDAL_sDHN.ExecuteQuery_DataTable("select Loai=N'Bất Thường',ThoiGian='" + date.ToString("dd/MM/yyyy") + "',t1.*,HieuDHN=ttsdhn.HIEU_DHTM,Loai2='1' from"
                                                      + " (select DanhBo=dhn.DanhBo, SoLuong = (select COUNT(DanhBo) from sDHN_LichSu_TCT ls where ls.DanhBo = dhn.DanhBo and CAST(ThoiGianCapNhat as date) = '" + date.ToString("yyyy-MM-dd") + "'),IDNCC,NCC=ncc.Name"
                                                      + " from sDHN_NCC ncc,sDHN_TCT dhn"
                                                      + " where ncc.ID=dhn.IDNCC and Valid = 1)t1,DHTM_THONGTIN ttsdhn"
                                                      + " where t1.SoLuong > 0 and t1.SoLuong < 24 and t1.IDNCC=ttsdhn.ID order by NCC");
        }

        //-------------------------

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
                    dt = getDS_KhongTinHieu_sDHN("export", function2, SoNgay, Ngay);
                    dt.TableName = "Không Tín Hiệu";
                    filename = dt.TableName + "." + Ngay.Replace("/", ".");
                }
                else
                if (function1 == "CanhBao")
                {
                    switch (function2)
                    {
                        case "PinYeu":
                            dt = getDS_CanhBao_PinYeu_sDHN(date);
                            dt.TableName = "Pin Yếu";
                            break;
                        case "RoRi":
                            dt = getDS_CanhBao_RoRi_sDHN(date);
                            dt.TableName = "Rò Rỉ";
                            break;
                        case "QuaDong":
                            dt = getDS_CanhBao_QuaDong_sDHN(date);
                            dt.TableName = "Quá Dòng";
                            break;
                        case "ChayNguoc":
                            dt = getDS_CanhBao_ChayNguoc_sDHN(date);
                            dt.TableName = "Chạy Ngược";
                            break;
                        case "NamCham":
                            dt = getDS_CanhBao_NamCham_sDHN(date);
                            dt.TableName = "Nam Châm";
                            break;
                        case "KhoOng":
                            dt = getDS_CanhBao_KhoOng_sDHN(date);
                            dt.TableName = "Khô Ống";
                            break;
                        case "MoHop":
                            dt = getDS_CanhBao_MoHop_sDHN(date);
                            dt.TableName = "Mở Hộp";
                            break;
                        default:
                            break;
                    }
                    filename = dt.TableName + "." + Ngay.Replace("/", ".");
                }
                else
                    if (function1 == "LichSu")
                {
                    if (function2 == "0")
                    {
                        dt = getDS_KhongTinHieu_sDHN("export", date);
                    }
                    else
                    if (function2 == "1")
                    {
                        dt = getDS_BatThuong_sDHN("export", date);
                    }
                    else
                    if (function2 == "2")
                    {
                        dt = getDS_BinhThuong_sDHN("export", date);
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

    }
}