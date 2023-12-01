using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/TrungTamKhachHang")]
    public class apiTrungTamKhachHangController : ApiController
    {
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);
        //private CConnection _cDAL_DocSo12 = new CConnection(CGlobalVariable.DocSo12);
        private CConnection _cDAL_GanMoi = new CConnection(CGlobalVariable.GanMoi);
        private CConnection _cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        private CConnection _cDAL_KinhDoanh = new CConnection(CGlobalVariable.ThuongVu);
        private CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TrungTamKhachHang);
        private string _urlApi = "https://cskhapi.sawaco.com.vn";

        private bool checkExists_HoSoGoc(string DanhBo)
        {
            try
            {
                List<CSKH_TCT> lst = new List<CSKH_TCT>();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.1.5:84/api/thuongvu/checkhosogoc/" + DanhBo);
                request.Method = "GET";
                request.ContentType = "application/json";
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    return bool.Parse(result);
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy thông tin khách hàng
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getThongTinKhachHang")]
        public ThongTinKhachHang getThongTinKhachHang(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                DataTable dt = new DataTable();

                //lấy thông tin khách hàng
                string sql = "select DanhBo"
                             + ",HoTen"
                             + ",DiaChi=SoNha+' '+TenDuong+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                             + ",DiaChiHoaDon=(select top 1 SO+' '+DUONG from HOADON_TA.dbo.HOADON where HOADON_TA.dbo.HOADON.DanhBa=TB_DULIEUKHACHHANG.DanhBo order by HOADON_TA.dbo.HOADON.ID_HOADON desc)+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                             + ",HopDong"
                             //+ ",DienThoai"
                             + ",MLT=LoTrinh"
                             + ",DinhMuc"
                             + ",DinhMucHN"
                             + ",GiaBieu"
                             + ",HieuDH"
                             + ",CoDH"
                             + ",Cap"
                             + ",SoThanDH"
                             + ",ViTriDHN"
                             + ",ViTriDHN_Ngoai"
                             + ",ViTriDHN_Hop"
                             + ",NgayThay"
                             + ",NgayKiemDinh"
                             + ",HieuLuc=convert(varchar(2),Ky)+'/'+convert(char(4),Nam)"
                             + ",Gieng,DienThoai=(select top 1 DienThoai from SDT_DHN where SDT_DHN.DanhBo=TB_DULIEUKHACHHANG.DanhBo order by CreateDate desc)"
                             + " from TB_DULIEUKHACHHANG where DanhBo='" + DanhBo + "'";
                dt = _cDAL_DHN.ExecuteQuery_DataTable(sql);
                //lấy thông tin khách hàng đã hủy
                if (dt == null || dt.Rows.Count == 0)
                {
                    sql = "select DanhBo"
                                 + ",HoTen"
                                 + ",DiaChi=SoNha+' '+TenDuong+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                                 + ",DiaChiHoaDon=(select top 1 SO+' '+DUONG from HOADON_TA.dbo.HOADON where HOADON_TA.dbo.HOADON.DanhBa=TB_DULIEUKHACHHANG_HUYDB.DanhBo order by HOADON_TA.dbo.HOADON.ID_HOADON desc)+', P.'+(select TenPhuong from Phuong where MaPhuong=Phuong and MaQuan=Quan)+', Q.'+(select TenQuan from Quan where MaQuan=Quan)"
                                 + ",HopDong"
                                 //+ ",DienThoai=''"
                                 + ",MLT=LoTrinh"
                                 + ",DinhMuc"
                                 + ",DinhMucHN"
                                 + ",GiaBieu"
                                 + ",HieuDH"
                                 + ",CoDH"
                                 + ",Cap"
                                 + ",SoThanDH"
                                 + ",ViTriDHN"
                                 + ",ViTriDHN_Ngoai='false'"
                                 + ",ViTriDHN_Hop='false'"
                                 + ",NgayThay"
                                 + ",NgayKiemDinh"
                                 + ",HieuLuc=N'Het '+HieuLucHuy"
                                 + ",Gieng='false',DienThoai=''"
                                 + " from TB_DULIEUKHACHHANG_HUYDB where DanhBo='" + DanhBo + "'";
                    dt = _cDAL_DHN.ExecuteQuery_DataTable(sql);
                }
                //
                if (dt != null && dt.Rows.Count > 0)
                {
                    ThongTinKhachHang en = new ThongTinKhachHang();
                    en.DanhBo = dt.Rows[0]["DanhBo"].ToString();
                    en.HoTen = dt.Rows[0]["HoTen"].ToString();
                    en.DiaChi = dt.Rows[0]["DiaChi"].ToString();
                    if (dt.Rows[0]["DiaChi"].ToString() != dt.Rows[0]["DiaChiHoaDon"].ToString())
                        en.DiaChiHoaDon = dt.Rows[0]["DiaChiHoaDon"].ToString();
                    en.HopDong = dt.Rows[0]["HopDong"].ToString();
                    object DienThoais = _cDAL_DHN.ExecuteQuery_ReturnOneValue("select DienThoai=stuff((select ';' + DienThoai from CAPNUOCTANHOA.dbo.SDT_DHN"
                                    + " where DanhBo = '" + dt.Rows[0]["DanhBo"].ToString() + "' order by CreateDate desc for xml path('')),1,1,'')");
                    if (DienThoais != null)
                        en.DienThoai = DienThoais.ToString();
                    en.MLT = dt.Rows[0]["MLT"].ToString();
                    en.DinhMuc = dt.Rows[0]["DinhMuc"].ToString();
                    en.DinhMucHN = dt.Rows[0]["DinhMucHN"].ToString();
                    en.GiaBieu = dt.Rows[0]["GiaBieu"].ToString();
                    en.HieuDH = dt.Rows[0]["HieuDH"].ToString();
                    en.CoDH = dt.Rows[0]["CoDH"].ToString();
                    en.Cap = dt.Rows[0]["Cap"].ToString();
                    en.SoThanDH = dt.Rows[0]["SoThanDH"].ToString();
                    string str = "";
                    if (bool.Parse(dt.Rows[0]["ViTriDHN_Ngoai"].ToString()))
                        str += "Ngoài";
                    if (bool.Parse(dt.Rows[0]["ViTriDHN_Hop"].ToString()))
                        if (str != "")
                            str += " - Hộp";
                        else
                            str += "Hộp";
                    if (str != "")
                        str += " - " + dt.Rows[0]["ViTriDHN"].ToString();
                    else
                        str += dt.Rows[0]["ViTriDHN"].ToString();
                    en.ViTriDHN = str;
                    if (dt.Rows[0]["NgayThay"].ToString() != "")
                        en.NgayThay = DateTime.Parse(dt.Rows[0]["NgayThay"].ToString());
                    if (dt.Rows[0]["NgayKiemDinh"].ToString() != "")
                        en.NgayKiemDinh = DateTime.Parse(dt.Rows[0]["NgayKiemDinh"].ToString());
                    en.HieuLuc = dt.Rows[0]["HieuLuc"].ToString();
                    en.ThongTin = "";
                    //lấy tọa độ
                    object toado = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 Latitude+','+Longitude from DocSo where DanhBa='" + en.DanhBo + "' and Latitude is not null order by DocSoID desc");
                    if (toado != null)
                    {
                        if (en.ThongTin != "")
                            en.ThongTin += " - ";
                        en.ThongTin += "<a style='color: blue;' target='_blank' href='https://www.google.com/maps/search/?api=1&query=" + toado.ToString() + "'>Định vị GPS</a>";
                    }
                    if ((int)_cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select count(DanhBo) from DonTu_ChiTiet where DanhBo='" + dt.Rows[0]["DanhBo"].ToString() + "' and CAST(CreateDate as date)>=CAST(DATEADD(DAY, -14, GETDATE()) as date) ") == 1)
                    {
                        if (en.ThongTin != "")
                            en.ThongTin += " - ";
                        en.ThongTin = "Có Đơn trong 14 ngày gần nhất";
                    }
                    if (bool.Parse(dt.Rows[0]["Gieng"].ToString()))
                    {
                        if (en.ThongTin != "")
                            en.ThongTin += " - ";
                        en.ThongTin += "Có sử dụng Giếng";
                    }
                    if (checkExists_HoSoGoc(en.DanhBo))
                        en.HoSoGoc = "<a style='color: blue;' target='_blank' href='https://old.cskhtanhoa.com.vn:1803/api/thuongvu/hosogoc/" + en.DanhBo + "'>Xem File</a>";
                    else
                        en.HoSoGoc = "Chưa scan";
                    //if (en.ThongTin != "")
                    //    en.ThongTin += " - ";
                    //en.ThongTin += "<a style='color: blue;' target='_blank' href='https://old.cskhtanhoa.com.vn:1803/api/thuongvu/hosogoc/" + en.DanhBo + "'>Hồ sơ gốc</a>";
                    DataTable dtNiemChi = getDS_NiemChi(dt.Rows[0]["DanhBo"].ToString());
                    if (dtNiemChi != null && dtNiemChi.Rows.Count > 0)
                    {
                        //string NoiDung = "";
                        //foreach (DataRow item in dtNiemChi.Rows)
                        //    if (bool.Parse(item["KhoaTu"].ToString()))
                        //        NoiDung += item["NoiDung"].ToString() + ", Khóa Từ\n";
                        //    else
                        //        NoiDung += item["NoiDung"].ToString() + ", Khóa Chì: " + item["NiemChi"].ToString() + " " + item["MauSac"].ToString() + "\n";
                        if (en.ThongTin != "")
                            en.ThongTin += " - ";
                        if (bool.Parse(dtNiemChi.Rows[0]["KhoaTu"].ToString()))
                        {
                            //en.ThongTin += "<a style='color: red;' target='_blank' href='https://service.cskhtanhoa.com.vn/khachhang/lichsubamchi?danhbo=" + dt.Rows[0]["DanhBo"].ToString() + "'>" + dtNiemChi.Rows[0]["NoiDung"].ToString() + ", Khóa Từ</a>";
                            en.BamChi = "<a style='color: red;' target='_blank' href='https://service.cskhtanhoa.com.vn/khachhang/lichsubamchi?danhbo=" + dt.Rows[0]["DanhBo"].ToString() + "'>" + dtNiemChi.Rows[0]["NoiDung"].ToString() + ", Khóa Từ</a>";
                        }
                        else
                        {
                            //en.ThongTin += "<a style='color: red;' target='_blank' href='https://service.cskhtanhoa.com.vn/khachhang/lichsubamchi?danhbo=" + dt.Rows[0]["DanhBo"].ToString() + "'>" + dtNiemChi.Rows[0]["NoiDung"].ToString() + ", Khóa Chì: " + dtNiemChi.Rows[0]["NiemChi"].ToString() + " " + dtNiemChi.Rows[0]["MauSac"].ToString() + "</a>";
                            en.BamChi = "<a style='color: red;' target='_blank' href='https://service.cskhtanhoa.com.vn/khachhang/lichsubamchi?danhbo=" + dt.Rows[0]["DanhBo"].ToString() + "'>" + dtNiemChi.Rows[0]["NoiDung"].ToString() + ", Khóa Chì: " + dtNiemChi.Rows[0]["NiemChi"].ToString() + " " + dtNiemChi.Rows[0]["MauSac"].ToString() + "</a>";
                        }
                    }
                    return en;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy thông tin nhân viên đọc số, lịch ghi chỉ số nước, ghi chú
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getDocSo")]
        public DocSo getDocSo(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                DocSo en = new DocSo();
                DataTable dt = new DataTable();

                //lấy lịch ghi chỉ số
                string sql = "select top(12) KyHD=CONVERT(char(2),Ky)+'/'+CONVERT(char(4),Nam),Ky,Nam"
                               //+ ",NgayDoc=CONVERT(char(10),DenNgay,103)"
                               + ",NgayDoc=DenNgay"
                               + ",CodeMoi"
                               + ",ChiSoCu=CSCu"
                               + ",ChiSoMoi=CSMoi"
                               + ",TieuThu=TieuThuMoi"
                               + ",MLT=MLT2"
                               + " from DocSo"
                               + " where DanhBa=" + DanhBo
                               + " order by Nam desc,CAST(Ky as int) desc";
                dt = _cDAL_DocSo.ExecuteQuery_DataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    object result = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 N'Nhân viên ghi chỉ số: '+HoTen+' : '+DienThoai from NguoiDung where May=" + dt.Rows[0]["MLT"].ToString().Substring(2, 2));
                    if (result != null)
                    {
                        en.NhanVien = result.ToString();
                        en.NhanVien += " ; " + getLichDocSo_Func_String(DanhBo, dt.Rows[0]["MLT"].ToString());
                    }
                    foreach (DataRow item in dt.Rows)
                    {
                        GhiChiSo enCT = new GhiChiSo();
                        //enCT.Ky = item["KyHD"].ToString();
                        enCT.Ky = "<a target='_blank' href='https://old.cskhtanhoa.com.vn:1803/api/docso/imagehtml/" + item["Nam"].ToString() + item["Ky"].ToString() + DanhBo + "'>" + item["KyHD"].ToString() + "</a>";
                        if (item["NgayDoc"].ToString() != "")
                            enCT.NgayDoc = DateTime.Parse(item["NgayDoc"].ToString());
                        enCT.CodeMoi = item["CodeMoi"].ToString();
                        enCT.ChiSoCu = item["ChiSoCu"].ToString();
                        enCT.ChiSoMoi = item["ChiSoMoi"].ToString();
                        enCT.TieuThu = item["TieuThu"].ToString();
                        en.lstGhiChiSo.Add(enCT);
                    }
                }
                //lấy ghi chú
                sql = "select NoiDung,CreateDate from TB_GHICHU where DanhBo='" + DanhBo + "' order by CreateDate desc";
                dt = _cDAL_DHN.ExecuteQuery_DataTable(sql);
                //
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        GhiChu enCT = new GhiChu();
                        enCT.NoiDung = item["NoiDung"].ToString();
                        if (item["CreateDate"].ToString() != "")
                            enCT.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                        en.lstGhiChu.Add(enCT);
                    }
                }
                return en;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy lịch đọc số
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getLichDocSo")]
        public ThongTinKhachHang getLichDocSo(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
                if (dt_ThongTin != null && dt_ThongTin.Rows.Count > 0)
                {
                    ThongTinKhachHang en = new ThongTinKhachHang();
                    en.DanhBo = dt_ThongTin.Rows[0]["DanhBo"].ToString();
                    en.HoTen = dt_ThongTin.Rows[0]["HoTen"].ToString();
                    en.DiaChi = dt_ThongTin.Rows[0]["DiaChi"].ToString();
                    en.DinhMuc = dt_ThongTin.Rows[0]["DinhMuc"].ToString();
                    en.GiaBieu = dt_ThongTin.Rows[0]["GiaBieu"].ToString();

                    string result_Lich = getLichDocSo_Func_String(DanhBo, dt_ThongTin.Rows[0]["MLT"].ToString());

                    string result_NhanVien = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select top 1 NhanVien=N'Nhân viên ghi chỉ số: '+HoTen+' : '+DienThoai from NguoiDung where May=" + dt_ThongTin.Rows[0]["MLT"].ToString().Substring(2, 2)).ToString();

                    en.ThongTin = result_NhanVien + " ; " + result_Lich;
                    return en;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        //hàm gốc
        [ApiExplorerSettings(IgnoreApi = true)]
        public DataTable getLichDocSo_Func_SQL(string DanhBo, string MLT)
        {
            string sql_Lich = "WITH temp AS(select top 2 KY, NAM from DocSo where DANHBA = '" + DanhBo + "' order by DocSoID desc)"
                                   + " select distinct NoiDung=N'Kỳ '+CONVERT(varchar(2),a.Ky)+'/'+CONVERT(varchar(4),a.Nam)+N' dự kiến sẽ được ghi chỉ số vào ngày '+CONVERT(varchar(10),b.NgayDoc,103)"
                                   + " ,NgayDoc,NgayChuyenListing,NgayThuTien,a.Ky,a.Nam,Dot=b.IDDot"
                                   + " from Lich_DocSo a,Lich_DocSo_ChiTiet b,Lich_Dot c,temp where a.ID=b.IDDocSo and c.ID=b.IDDot and ((a.Nam>temp.Nam) or (a.Nam=temp.Nam and a.Ky>=temp.Ky))"
                                   + " and((c.TB1_From <= " + MLT + " and c.TB1_To >= " + MLT + ")"
                                   + " or (c.TB2_From <= " + MLT + " and c.TB2_To >= " + MLT + ")"
                                   + " or (c.TP1_From <= " + MLT + " and c.TP1_To >= " + MLT + ")"
                                   + " or (c.TP2_From <= " + MLT + " and c.TP2_To >= " + MLT + "))"
                                   + " order by NgayDoc desc";
            return _cDAL_DocSo.ExecuteQuery_DataTable(sql_Lich);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string getLichDocSo_Func_String(string DanhBo, string MLT)
        {
            DataTable dt = getLichDocSo_Func_SQL(DanhBo, MLT);
            string result = "";
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                    if (DateTime.Parse(item["NgayDoc"].ToString()).Date <= DateTime.Now.Date && DateTime.Parse(item["NgayChuyenListing"].ToString()).Date >= DateTime.Now.Date)
                    {
                        result = item["NoiDung"].ToString();
                        break;
                    }
                if (result == "")
                    result = dt.Rows[0]["NoiDung"].ToString();
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public DataRow getLichDocSo_Func_DataRow(string DanhBo, string MLT)
        {
            DataTable dt = getLichDocSo_Func_SQL(DanhBo, MLT);
            DataRow result = null;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                    if (DateTime.Parse(item["NgayDoc"].ToString()).Date <= DateTime.Now.Date && DateTime.Parse(item["NgayChuyenListing"].ToString()).Date >= DateTime.Now.Date)
                    {
                        result = item;
                        break;
                    }
                if (result == null)
                    result = dt.Rows[0];
            }
            return result;
        }

        /// <summary>
        /// Lấy thông tin hóa đơn
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getThuTien")]
        public ThuTien getThuTien(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                ThuTien en = new ThuTien();
                DataTable dt = new DataTable();
                int TongNo = 0;
                bool CNKD = false;

                string sql = "select * from fnTimKiem('" + DanhBo + "','') order by MaHD desc";
                dt = _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                //
                if (dt != null && dt.Rows.Count > 0)
                {
                    en.NhanVien = "Nhân viên thu tiền: " + dt.Rows[0]["NhanVien"].ToString();
                    en.NhanVien += " ; " + getLichThuTien_Func_String(DanhBo, dt.Rows[0]["MLT"].ToString());

                    foreach (DataRow item in dt.Rows)
                    {
                        HoaDonThuTien enCT = new HoaDonThuTien();
                        enCT.GiaBieu = item["GiaBieu"].ToString();
                        enCT.DinhMuc = item["DinhMuc"].ToString();
                        enCT.DinhMucHN = item["DinhMucHN"].ToString();
                        enCT.SoHoaDon = item["SoHoaDon"].ToString();
                        enCT.Ky = item["Ky"].ToString();
                        enCT.TieuThu = item["TieuThu"].ToString();
                        DataTable dtHDDC = _cDAL_ThuTien.ExecuteQuery_DataTable("select [GIABAN_DC],[THUE_DC],[PHI_DC],[PHI_Thue_DC],[TONGCONG_DC] from DIEUCHINH_HD where FK_HOADON=" + item["MaHD"].ToString());
                        if (dtHDDC != null && dtHDDC.Rows.Count > 0)
                        {
                            if (dtHDDC.Rows[0]["GIABAN_DC"].ToString() != "")
                                enCT.GiaBan = (int.Parse(item["GiaBan"].ToString()) - int.Parse(dtHDDC.Rows[0]["GIABAN_DC"].ToString())).ToString();
                            if (dtHDDC.Rows[0]["THUE_DC"].ToString() != "")
                                enCT.ThueGTGT = (int.Parse(item["ThueGTGT"].ToString()) - int.Parse(dtHDDC.Rows[0]["THUE_DC"].ToString())).ToString();
                            if (dtHDDC.Rows[0]["PHI_DC"].ToString() != "")
                                enCT.PhiBVMT = (int.Parse(item["PhiBVMT"].ToString()) - int.Parse(dtHDDC.Rows[0]["PHI_DC"].ToString())).ToString();
                            if (dtHDDC.Rows[0]["PHI_Thue_DC"].ToString() != "")
                                enCT.PhiBVMT_Thue = (int.Parse(item["PhiBVMT_Thue"].ToString()) - int.Parse(dtHDDC.Rows[0]["PHI_Thue_DC"].ToString())).ToString();
                            else
                                enCT.PhiBVMT_Thue = item["PhiBVMT_Thue"].ToString();
                            if (dtHDDC.Rows[0]["TONGCONG_DC"].ToString() != "")
                                enCT.TongCong = (int.Parse(item["TongCong"].ToString()) - int.Parse(dtHDDC.Rows[0]["TONGCONG_DC"].ToString())).ToString();
                        }
                        else
                        {
                            enCT.GiaBan = item["GiaBan"].ToString();
                            enCT.ThueGTGT = item["ThueGTGT"].ToString();
                            enCT.PhiBVMT = item["PhiBVMT"].ToString();
                            enCT.PhiBVMT_Thue = item["PhiBVMT_Thue"].ToString();
                            enCT.TongCong = item["TongCong"].ToString();
                        }
                        if (item["NgayGiaiTrach"].ToString() != "")
                        {
                            if (item["DangNgan"].ToString() != "CNKĐ")
                                enCT.NgayGiaiTrach = DateTime.Parse(item["NgayGiaiTrach"].ToString());
                            else
                            {
                                TongNo += int.Parse(item["TongCong"].ToString());
                                CNKD = true;
                            }
                        }
                        else
                            TongNo += int.Parse(item["TongCong"].ToString());
                        enCT.DangNgan = item["DangNgan"].ToString();
                        enCT.HanhThu = item["HanhThu"].ToString();
                        enCT.MaDN = item["MaDN"].ToString();
                        if (item["NgayDN"].ToString() != "")
                            enCT.NgayDN = DateTime.Parse(item["NgayDN"].ToString());
                        if (item["NgayMN"].ToString() != "")
                            enCT.NgayMN = DateTime.Parse(item["NgayMN"].ToString());
                        enCT.DongNuoc2 = bool.Parse(item["DongNuoc2"].ToString());
                        enCT.LenhHuy = bool.Parse(item["LenhHuy"].ToString());
                        enCT.ToTrinh = bool.Parse(item["ToTrinh"].ToString());
                        en.lstHoaDon.Add(enCT);
                    }
                }
                en.ThongTin = "Hết nợ";

                if (TongNo > 0)
                    if (CNKD == false)
                        en.ThongTin = "Hiện còn nợ: " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", TongNo);
                    else
                        en.ThongTin = "Hiện còn nợ (CNKĐ): " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", TongNo);

                int PhiMoNuoc = (int)_cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select PhiMoNuoc=dbo.fnGetPhiMoNuoc(" + DanhBo + ")");
                if (PhiMoNuoc > 0)
                    en.ThongTin += " ; Phí mở nước: " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", PhiMoNuoc);

                dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnThuHoChuaDangNgan(" + DanhBo + ")");
                if (dt != null && dt.Rows.Count > 0)
                {
                    en.ThongTin += " ; Đã Thu Hộ " + String.Format(CultureInfo.CreateSpecificCulture("vi-VN"), "{0:#,##đ}", int.Parse(dt.Rows[0]["TongCong"].ToString())) + " - Kỳ " + dt.Rows[0]["Kys"].ToString() + " - ngày " + dt.Rows[0]["CreateDate"].ToString() + " - qua " + dt.Rows[0]["TenDichVu"].ToString();
                }
                return en;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy lịch thu tiền
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getLichThuTien")]
        public ThongTinKhachHang getLichThuTien(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                DataTable dt_ThongTin = _cDAL_ThuTien.ExecuteQuery_DataTable("select top 1 DanhBo=DANHBA,HoTen=TENKH,DiaChi=(SO+' '+DUONG),GiaBieu=GB,DinhMuc=DM,MLT=MALOTRINH from HOADON where DANHBA='" + DanhBo + "' order by ID_HOADON desc");
                if (dt_ThongTin != null && dt_ThongTin.Rows.Count > 0)
                {
                    ThongTinKhachHang en = new ThongTinKhachHang();
                    en.DanhBo = dt_ThongTin.Rows[0]["DanhBo"].ToString();
                    en.HoTen = dt_ThongTin.Rows[0]["HoTen"].ToString();
                    en.DiaChi = dt_ThongTin.Rows[0]["DiaChi"].ToString();
                    en.DinhMuc = dt_ThongTin.Rows[0]["DinhMuc"].ToString();
                    en.GiaBieu = dt_ThongTin.Rows[0]["GiaBieu"].ToString();

                    string result_Lich = getLichThuTien_Func_String(DanhBo, dt_ThongTin.Rows[0]["MLT"].ToString());

                    string result_NhanVien = _cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select top 1 NhanVien=N'Nhân viên thu tiền: '+HoTen+' : '+DienThoai from HOADON a,TT_NguoiDung b where DANHBA='" + dt_ThongTin.Rows[0]["DanhBo"].ToString() + "' and a.MaNV_HanhThu=b.MaND order by ID_HOADON desc").ToString();

                    en.ThongTin = result_NhanVien + " ; " + result_Lich;
                    return en;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        //hàm gốc
        [ApiExplorerSettings(IgnoreApi = true)]
        public DataTable getLichThuTien_Func_SQL(string DanhBo, string MLT)
        {
            string sql_Lich = "WITH temp AS (select top 2 KY,NAM from [HOADON_TA].[dbo].[HOADON] where DANHBA='" + DanhBo + "' order by ID_HOADON desc)"
                                   + " select distinct NoiDung=N'Kỳ '+CONVERT(varchar(2),a.Ky)+'/'+CONVERT(varchar(4),a.Nam)+N' dự kiến sẽ được thu tiền từ ngày '+CONVERT(varchar(10),b.NgayThuTien_From,103)+N' đến ngày '+CONVERT(varchar(10),b.NgayThuTien_To,103)"
                                   + " ,NgayThuTien_From,NgayThuTien_To"
                                   + " from Lich_ThuTien a,Lich_ThuTien_ChiTiet b,Lich_Dot c,temp where a.ID=b.IDThuTien and c.ID=b.IDDot and ((a.Nam>temp.Nam) or (a.Nam=temp.Nam and a.Ky>=temp.Ky))"
                                   + " and((c.TB1_From <= " + MLT + " and c.TB1_To >= " + MLT + ")"
                                   + " or (c.TB2_From <= " + MLT + " and c.TB2_To >= " + MLT + ")"
                                   + " or (c.TP1_From <= " + MLT + " and c.TP1_To >= " + MLT + ")"
                                   + " or (c.TP2_From <= " + MLT + " and c.TP2_To >= " + MLT + "))";
            //+ " order by a.CreateDate desc";
            return _cDAL_DocSo.ExecuteQuery_DataTable(sql_Lich);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string getLichThuTien_Func_String(string DanhBo, string MLT)
        {
            //DataTable dt = getLichThuTien_Func_SQL(DanhBo, MLT);
            //string result = "";
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow item in dt.Rows)
            //        if (DateTime.Parse(item["NgayThuTien_To"].ToString()).Date >= DateTime.Now.Date)
            //        {
            //            result = item["NoiDung"].ToString();
            //            break;
            //        }
            //    if (result == "")
            //        result = dt.Rows[dt.Rows.Count - 1]["NoiDung"].ToString();
            //}
            //return result;

            //không áp dụng hành thu nên lấy ngày phát hành hóa đơn bên lịch đọc số
            DataTable dt = getLichDocSo_Func_SQL(DanhBo, MLT);
            string result = "";
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                    if (DateTime.Parse(item["NgayDoc"].ToString()).Date <= DateTime.Now.Date && DateTime.Parse(item["NgayChuyenListing"].ToString()).Date >= DateTime.Now.Date)
                    {
                        DateTime date = DateTime.Parse(item["NgayThuTien"].ToString()).AddDays(1);
                        if (date.DayOfWeek == DayOfWeek.Saturday)
                            date = date.AddDays(2);
                        result = "Kỳ " + item["Ky"].ToString() + "/" + item["Nam"].ToString() + " dự kiến sẽ được phát hành hóa đơn vào ngày " + date.ToString("dd/MM/yyyy");
                        break;
                    }
                if (result == "")
                {
                    DateTime date = DateTime.Parse(dt.Rows[0]["NgayThuTien"].ToString()).AddDays(1);
                    if (date.DayOfWeek == DayOfWeek.Saturday)
                        date = date.AddDays(2);
                    result = "Kỳ " + dt.Rows[0]["Ky"].ToString() + "/" + dt.Rows[0]["Nam"].ToString() + " dự kiến sẽ được phát hành hóa đơn vào ngày " + date.ToString("dd/MM/yyyy");
                }
            }
            return result;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        //public DataRow getLichThuTien_Func_DataRow(string DanhBo, string MLT)
        //{
        //    DataTable dt = getLichThuTien_Func_SQL(DanhBo, MLT);
        //    DataRow result = null;
        //    if (dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow item in dt.Rows)
        //            if (DateTime.Parse(item["NgayThuTien_To"].ToString()).Date >= DateTime.Now.Date)
        //            {
        //                result = item;
        //                break;
        //            }
        //        if (result == null)
        //            result = dt.Rows[dt.Rows.Count - 1];
        //    }
        //    return result;
        //}

        //sẽ update getdontumoi

        /// <summary>
        /// Lấy thông tin đơn phòng kinh doanh
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getDonKinhDoanh")]
        public DonKinhDoanh getDonKinhDoanh(string DanhBo, string checksum)
        {
            if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
            {
                ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("exec spTimKiemByBanhBo_DonTu '" + DanhBo + "'");
                ds = _cDAL_KinhDoanh.ExecuteQuery_DataSet("exec spTimKiemByBanhBo_DonTuChiTiet '" + DanhBo + "'");
                //
                if (dt != null && dt.Rows.Count > 0)
                {
                    DonKinhDoanh enKD = new DonKinhDoanh();
                    enKD.ThongTin = _cDAL_KinhDoanh.ExecuteQuery_ReturnOneValue("select dbo.fnCheckTinhTrangCanKhachHangLienHe('" + DanhBo + "')").ToString();

                    foreach (DataRow item in dt.Rows)
                        if (enKD.lstDonTu.Any(itemA => itemA.MaDon == item["MaDon"].ToString() + " - " + item["Phong"].ToString()) == false)
                        {
                            DonTu en = new DonTu();
                            en.MaDon = item["MaDon"].ToString() + " - " + item["Phong"].ToString();
                            en.TenLD = item["TenLD"].ToString();
                            if (item["CreateDate"].ToString() != "")
                                en.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                            en.DanhBo = item["DanhBo"].ToString();
                            en.HoTen = item["HoTen"].ToString();
                            en.DiaChi = item["DiaChi"].ToString();
                            en.GiaBieu = item["GiaBieu"].ToString();
                            en.DinhMuc = item["DinhMuc"].ToString();
                            en.DinhMucHN = item["DinhMucHN"].ToString();
                            en.NoiDung = item["NoiDung"].ToString();

                            //thêm chi tiết
                            for (int i = 0; i < ds.Tables.Count; i++)
                                if (ds.Tables[i].Rows.Count > 0)
                                {
                                    switch (ds.Tables[i].Rows[0][0].ToString())
                                    {
                                        case "KTXM":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    KTXM enCT = new KTXM();
                                                    if (dr[j]["Phong"].ToString() == "TV")
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=KTXM_ChiTiet_Hinh&IDFileName=IDKTXM_ChiTiet&IDFileContent=" + dr[j]["MaCTKTXM"].ToString() + "'>File Scan</a>";
                                                    else
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/QLDHN/viewFile?TableName=MaHoa_KTXM_Hinh&ID=" + dr[j]["MaCTKTXM"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["NgayKTXM"].ToString() != "")
                                                        enCT.NgayKTXM = DateTime.Parse(dr[j]["NgayKTXM"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.NoiDungKiemTra = dr[j]["NoiDungKiemTra"].ToString();
                                                    enCT.NoiDungDongTien = dr[j]["NoiDungDongTien"].ToString();
                                                    if (dr[j]["NgayDongTien"].ToString() != "")
                                                        enCT.NgayDongTien = DateTime.Parse(dr[j]["NgayDongTien"].ToString());
                                                    enCT.SoTienDongTien = dr[j]["SoTienDongTien"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstKTXM.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "BamChi":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    BamChi enCT = new BamChi();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=BamChi_ChiTiet_Hinh&IDFileName=IDBamChi_ChiTiet&IDFileContent=" + dr[j]["MaCTBC"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["NgayBC"].ToString() != "")
                                                        enCT.NgayBC = DateTime.Parse(dr[j]["NgayBC"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.TrangThaiBC = dr[j]["TrangThaiBC"].ToString();
                                                    enCT.TheoYeuCau = dr[j]["TheoYeuCau"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstBamChi.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "DongNuoc":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    DongNuoc enCT = new DongNuoc();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=DongNuoc_ChiTiet_Hinh&IDFileName=IDDongNuoc_ChiTiet&IDFileContent=" + dr[j]["MaCTDN"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["NgayDN"].ToString() != "")
                                                        enCT.NgayDN = DateTime.Parse(dr[j]["NgayDN"].ToString());
                                                    if (dr[j]["NgayMN"].ToString() != "")
                                                        enCT.NgayMN = DateTime.Parse(dr[j]["NgayMN"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstDongNuoc.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "DCBD":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    DCBD enCT = new DCBD();
                                                    if (dr[j]["Phong"].ToString() == "TV")
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=DCBD_ChiTietBienDong_Hinh&IDFileName=IDDCBD_ChiTietBienDong&IDFileContent=" + dr[j]["MaDC"].ToString() + "'>File Scan</a>";
                                                    else
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/QLDHN/viewFile?TableName=MaHoa_DCBD_Hinh&ID=" + dr[j]["MaDC"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.GiaBieu = dr[j]["GiaBieu"].ToString();
                                                    enCT.GiaBieu_BD = dr[j]["GiaBieu_BD"].ToString();
                                                    enCT.DinhMuc = dr[j]["DinhMuc"].ToString();
                                                    enCT.DinhMuc_BD = dr[j]["DinhMuc_BD"].ToString();
                                                    enCT.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                                    enCT.DinhMucHN_BD = dr[j]["DinhMucHN_BD"].ToString();
                                                    enCT.HoTen_BD = dr[j]["HoTen_BD"].ToString();
                                                    enCT.DiaChi_BD = dr[j]["DiaChi_BD"].ToString();
                                                    enCT.ThongTin = dr[j]["ThongTin"].ToString() + "; Hiệu lực: " + dr[j]["HieuLucKy"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstDCBD.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "DCHD":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    DCHD enCT = new DCHD();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=DCBD_ChiTietHoaDon_Hinh&IDFileName=IDDCBD_ChiTietHoaDon&IDFileContent=" + dr[j]["MaDC"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.GiaBieu = dr[j]["GiaBieu"].ToString();
                                                    enCT.GiaBieu_BD = dr[j]["GiaBieu_BD"].ToString();
                                                    enCT.DinhMuc = dr[j]["DinhMuc"].ToString();
                                                    enCT.DinhMuc_BD = dr[j]["DinhMuc_BD"].ToString();
                                                    enCT.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                                    enCT.DinhMucHN_BD = dr[j]["DinhMucHN_BD"].ToString();
                                                    enCT.TieuThu = dr[j]["TieuThu"].ToString();
                                                    enCT.TieuThu_BD = dr[j]["TieuThu_BD"].ToString();
                                                    enCT.TongCong_Start = dr[j]["TongCong_Start"].ToString();
                                                    enCT.TongCong_End = dr[j]["TongCong_End"].ToString();
                                                    enCT.TongCong_BD = dr[j]["TongCong_BD"].ToString();
                                                    enCT.TangGiam = dr[j]["TangGiam"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstDCHD.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "CHDB":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    CHDB enCT = new CHDB();
                                                    if (dr[j]["LoaiCat"].ToString() == "Cắt Tạm")
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=CHDB_ChiTietCatTam_Hinh&IDFileName=IDCHDB_ChiTietCatTam&IDFileContent=" + dr[j]["MaCH"].ToString() + "'>File Scan</a>";
                                                    else
                                                        if (dr[j]["LoaiCat"].ToString() == "Cắt Hủy")
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=CHDB_ChiTietCatHuy_Hinh&IDFileName=IDCHDB_ChiTietCatHuy&IDFileContent=" + dr[j]["MaCH"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.LoaiCat = dr[j]["LoaiCat"].ToString();
                                                    enCT.LyDo = dr[j]["LyDo"].ToString();
                                                    enCT.GhiChuLyDo = dr[j]["GhiChuLyDo"].ToString();
                                                    enCT.DaLapPhieu = bool.Parse(dr[j]["DaLapPhieu"].ToString());
                                                    enCT.SoPhieu = dr[j]["SoPhieu"].ToString();
                                                    if (dr[j]["NgayLapPhieu"].ToString() != "")
                                                        enCT.NgayLapPhieu = DateTime.Parse(dr[j]["NgayLapPhieu"].ToString());
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstCHDB.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "PhieuCHDB":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    PhieuCHDB enCT = new PhieuCHDB();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=CHDB_Phieu_Hinh&IDFileName=IDCHDB_Phieu&IDFileContent=" + dr[j]["MaYCCHDB"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.LyDo = dr[j]["LyDo"].ToString();
                                                    enCT.GhiChuLyDo = dr[j]["GhiChuLyDo"].ToString();
                                                    enCT.HieuLucKy = dr[j]["HieuLucKy"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstPhieuCHDB.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "TTTL":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    TTL enCT = new TTL();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=ThuTraLoi_ChiTiet_Hinh&IDFileName=IDTTTL_ChiTiet&IDFileContent=" + dr[j]["MaCTTTTL"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.VeViec = dr[j]["VeViec"].ToString();
                                                    //enCT.NoiDung = dr[j]["NoiDung"].ToString();
                                                    enCT.NoiNhan = dr[j]["NoiNhan"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstTTL.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "GianLan":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    GianLan enCT = new GianLan();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=GianLan_ChiTiet_Hinh&IDFileName=IDGianLan_ChiTiet&IDFileContent=" + dr[j]["ID"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.NoiDungViPham = dr[j]["NoiDungViPham"].ToString();
                                                    enCT.TinhTrang = dr[j]["TinhTrang"].ToString();
                                                    enCT.ThanhToan1 = bool.Parse(dr[j]["ThanhToan1"].ToString());
                                                    enCT.ThanhToan2 = bool.Parse(dr[j]["ThanhToan2"].ToString());
                                                    enCT.ThanhToan3 = bool.Parse(dr[j]["ThanhToan3"].ToString());
                                                    enCT.XepDon = bool.Parse(dr[j]["XepDon"].ToString());
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstGianLan.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "TruyThu":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    TruyThu enCT = new TruyThu();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=TruyThuTienNuoc_ChiTiet_Hinh&IDFileName=IDTruyThuTienNuoc_ChiTiet&IDFileContent=" + dr[j]["IDCT"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.NoiDung = dr[j]["NoiDung"].ToString();
                                                    enCT.TongTien = dr[j]["TongTien"].ToString();
                                                    enCT.Tongm3BinhQuan = dr[j]["Tongm3BinhQuan"].ToString();
                                                    enCT.TinhTrang = dr[j]["TinhTrang"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstTruyThu.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "ToTrinh":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    ToTrinh enCT = new ToTrinh();
                                                    if (dr[j]["Phong"].ToString() == "TV")
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=ToTrinh_ChiTiet_Hinh&IDFileName=IDToTrinh_ChiTiet&IDFileContent=" + dr[j]["IDCT"].ToString() + "'>File Scan</a>";
                                                    else
                                                        enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/QLDHN/viewFile?TableName=MaHoa_ToTrinh_Hinh&ID=" + dr[j]["IDCT"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.VeViec = dr[j]["VeViec"].ToString();
                                                    //enCT.NoiDung = dr[j]["NoiDung"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstToTrinh.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "ThuMoi":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    ThuMoi enCT = new ThuMoi();
                                                    enCT.MaDon = "<a target='_blank' href='https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=ThuMoi_ChiTiet_Hinh&IDFileName=IDThuMoi_ChiTiet&IDFileContent=" + dr[j]["IDCT"].ToString() + "'>File Scan</a>";
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["CreateDate"].ToString() != "")
                                                        enCT.CreateDate = DateTime.Parse(dr[j]["CreateDate"].ToString());
                                                    enCT.DanhBo = dr[j]["DanhBo"].ToString();
                                                    enCT.HoTen = dr[j]["HoTen"].ToString();
                                                    enCT.DiaChi = dr[j]["DiaChi"].ToString();
                                                    enCT.Lan = dr[j]["Lan"].ToString();
                                                    enCT.VeViec = dr[j]["VeViec"].ToString();
                                                    enCT.CreateBy = dr[j]["CreateBy"].ToString();

                                                    en.lstThuMoi.Add(enCT);
                                                }
                                            }
                                            break;
                                        case "TienTrinh":
                                            if (ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'").Count() > 0)
                                            {
                                                DataRow[] dr = ds.Tables[i].Select("MaDon = '" + item["MaDon"].ToString() + "'");

                                                for (int j = 0; j < dr.Count(); j++)
                                                {
                                                    TienTrinh enCT = new TienTrinh();
                                                    enCT.MaDon = dr[j]["MaDon"].ToString();
                                                    enCT.TabSTT = dr[j]["TabSTT"].ToString();
                                                    enCT.TabName = dr[j]["TabName"].ToString();
                                                    if (dr[j]["NgayChuyen"].ToString() != "")
                                                        enCT.NgayChuyen = DateTime.Parse(dr[j]["NgayChuyen"].ToString());
                                                    enCT.NoiChuyen = dr[j]["NoiChuyen"].ToString();
                                                    enCT.NoiNhan = dr[j]["NoiNhan"].ToString();
                                                    enCT.KTXM = dr[j]["KTXM"].ToString();
                                                    enCT.NoiDung = dr[j]["NoiDung"].ToString();

                                                    en.lstTienTrinh.Add(enCT);
                                                }
                                            }
                                            break;
                                    }
                                }
                            enKD.lstDonTu.Add(en);
                        }
                    return enKD;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy danh sách file đã scan
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getHoSoScan")]
        public IList<HoSoScan> getHoSoScan(string DanhBo, string checksum)
        {
            try
            {
                //if (CConstantVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                //{
                //    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                //}
                DataTable dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select * from fnGetHoSoScan('" + DanhBo + "') order by CreateDate desc");
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<HoSoScan> lst = new List<HoSoScan>();
                    foreach (DataRow item in dt.Rows)
                        if (lst.Any(itemA => itemA.ID == int.Parse(item["ID"].ToString())) == false)
                        {
                            HoSoScan en = new HoSoScan();
                            en.ID = int.Parse(item["ID"].ToString());
                            if (item["CreateDate"].ToString() != "")
                                en.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                            en.LoaiVanBan = item["LoaiVanBan"].ToString();
                            HoSoScan_File enF = new HoSoScan_File();
                            enF.File = (byte[])item["Hinh"];
                            en.lstFile.Add(enF);
                            lst.Add(en);
                        }
                        else
                        {
                            HoSoScan_File enF = new HoSoScan_File();
                            enF.File = (byte[])item["Hinh"];
                            lst.SingleOrDefault(itemA => itemA.ID == int.Parse(item["ID"].ToString())).lstFile.Add(enF);
                        }
                    return lst;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Tìm kiếm thông tin khách hàng bằng họ tên, số nhà, tên đường
        /// </summary>
        /// <param name="HoTen"></param>
        /// <param name="SoNha"></param>
        /// <param name="TenDuong"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("searchThongTinKhachHang")]
        [HttpGet]
        public IList<ThongTinKhachHang> searchThongTinKhachHang(string SoThan, string HoTen, string SoNha, string TenDuong, string checksum)
        {
            try
            {
                string checkSoThan = "", checkHoTen = "", checkSoNha = "", checkTenDuong = "";
                if (SoThan != null)
                    checkSoThan = SoThan;
                if (HoTen != null)
                    checkHoTen = HoTen;
                if (SoNha != null)
                    checkSoNha = SoNha;
                if (TenDuong != null)
                    checkTenDuong = TenDuong;
                if (CGlobalVariable.getSHA256(checkSoThan + checkHoTen + checkSoNha + checkTenDuong + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                DataTable dt = new DataTable();
                List<ThongTinKhachHang> lst = new List<ThongTinKhachHang>();
                //kiếm số thân ĐHN
                if (checkSoThan != "")
                {
                    dt = _cDAL_DHN.ExecuteQuery_DataTable("select DANHBO,HOTEN,DiaChi=SONHA+' '+TENDUONG from TB_DULIEUKHACHHANG where SoThanDH like N'%" + checkSoThan + "%'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            ThongTinKhachHang en = new ThongTinKhachHang();
                            en.DanhBo = item["DanhBo"].ToString();
                            en.HoTen = item["HoTen"].ToString();
                            en.DiaChi = item["DiaChi"].ToString();
                            lst.Add(en);
                        }
                    }
                }
                else
                if (checkHoTen != "" || checkSoNha != "" || checkTenDuong != "")
                {
                    dt = _cDAL_ThuTien.ExecuteQuery_DataTable("select * from fnTimKiemTTKH('" + checkHoTen + "','" + checkSoNha + "','" + checkTenDuong + "')");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            ThongTinKhachHang en = new ThongTinKhachHang();
                            en.DanhBo = item["DanhBo"].ToString();
                            en.HoTen = item["HoTen"].ToString();
                            en.DiaChi = item["DiaChi"].ToString();
                            if (item["DiaChi"].ToString() != item["DiaChiHoaDon"].ToString())
                                en.DiaChiHoaDon = item["DiaChiHoaDon"].ToString();
                            lst.Add(en);
                        }
                    }
                }
                //kiếm danh bộ hủy
                //dt = CConstantVariable.cDAL_DHN.ExecuteQuery_DataTable("select DANHBO,HOTEN,DiaChi=SONHA+' '+TENDUONG from TB_DULIEUKHACHHANG_HUYDB where HOTEN like N'%" + checkHoTen + "%' and ((SONHA like N'%" + checkSoNha + "%' and TENDUONG like N'%" + checkTenDuong + "%') or (SONHA+' '+TENDUONG like N'%" + checkSoNha + " " + checkTenDuong + "%'))");
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        ThongTinKhachHang en = new ThongTinKhachHang();
                //        en.DanhBo = item["DanhBo"].ToString();
                //        en.HoTen = item["HoTen"].ToString();
                //        en.DiaChi = item["DiaChi"].ToString();

                //        lst.Add(en);
                //    }
                //}
                if (lst.Count > 0)
                    return lst;
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Tìm kiếm danh bộ bằng số điện thoại
        /// </summary>
        /// <param name="DienThoai"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("searchDanhBoByDienThoai")]
        [HttpGet]
        public IList<ThongTinKhachHang> searchDanhBoByDienThoai(string DienThoai, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DienThoai + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi + "api/DongHoNuoc/CapNhatDongHoNuoc");
                request.Method = "POST";
                request.ContentType = "application/json";
                //request.Headers["Authorization"] = "Bearer " + _cDAL_sDHN.ExecuteQuery_ReturnOneValue("select access_token from Configure").ToString();
                DataTable dt = new DataTable();

                dt = _cDAL_KinhDoanh.ExecuteQuery_DataTable("select distinct DanhBo from DonDienThoai where DanhBo!='' and DienThoai like '%" + DienThoai + "%'");
                dt.Merge(_cDAL_DocSo.ExecuteQuery_DataTable("select DanhBo=DanhBa from KhachHang where SDT like '%" + DienThoai + "%'"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<ThongTinKhachHang> lst = new List<ThongTinKhachHang>();
                    foreach (DataRow item in dt.Rows)
                    {
                        ThongTinKhachHang en = new ThongTinKhachHang();
                        en.DanhBo = item["DanhBo"].ToString();

                        lst.Add(en);
                    }

                    return lst;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy thông tin tiến trình xử lý hồ sơ gắn mới
        /// </summary>
        /// <param name="SoHoSo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getHoSoGanMoi")]
        public HoSoGanMoi getHoSoGanMoi(string SoHoSo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(SoHoSo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                HoSoGanMoi en = new HoSoGanMoi();
                DataTable dt = new DataTable();

                string sql = "SELECT  biennhan.SHS, biennhan.HOTEN,(SONHA + '  ' + DUONG + ',  P.' + p.TENPHUONG + ',  Q.' + q.TENQUAN) as 'DIACHI',"
                            + " biennhan.NGAYNHAN AS 'CreateDate',lhs.TENLOAI as 'LOAIHS'"
                            + " FROM QUAN q,PHUONG p, BIENNHANDON biennhan, LOAI_HOSO lhs"
                            + " WHERE biennhan.QUAN = q.MAQUAN AND q.MAQUAN = p.MAQUAN  AND biennhan.PHUONG = p.MAPHUONG AND lhs.MALOAI = biennhan.LOAIDON"
                            + " AND biennhan.SHS = '" + SoHoSo + "'";

                dt = _cDAL_GanMoi.ExecuteQuery_DataTable(sql);
                //
                if (dt != null && dt.Rows.Count > 0)
                {
                    en.SoHoSo = dt.Rows[0]["SHS"].ToString();
                    en.LoaiHoSo = dt.Rows[0]["LOAIHS"].ToString();
                    en.HoTen = dt.Rows[0]["HOTEN"].ToString();
                    en.DiaChi = dt.Rows[0]["DIACHI"].ToString();
                    if (dt.Rows[0]["CreateDate"].ToString() != "")
                        en.CreateDate = DateTime.Parse(dt.Rows[0]["CreateDate"].ToString());

                    string sqlCT = "select NgayChuyenThietKe=NGAYCHUYEN_HOSO,TRONGAITHIETKE,NOIDUNGTRONGAI,NGAYHOANTATTK=(select NGAYHOANTATTK from TOTHIETKE where SOHOSO=donkh.SOHOSO),"
                                  + " TaiXet=(select GHICHUTR from TMP_TAIXET where MAHOSO=donkh.SOHOSO),"
                                  + " NgayXinPhepDaoDuong = (select NGAYLAP from KH_XINPHEPDAODUONG where MADOT = (select MADOTDD from KH_HOSOKHACHHANG where SHS = donkh.SHS)),"
                                  + " NgayCoPhepDaoDuong = (select NGAYCOPHEP from KH_XINPHEPDAODUONG where MADOT = (select MADOTDD from KH_HOSOKHACHHANG where SHS = donkh.SHS)),"
                                  + " NgayThiCong = (select NGAYTHICONG from KH_HOSOKHACHHANG where SHS = donkh.SHS)"
                                  + " from DON_KHACHHANG donkh where SHS = '" + SoHoSo + "'";
                    DataTable dtCT = _cDAL_GanMoi.ExecuteQuery_DataTable(sqlCT);

                    GhiChu enCT = new GhiChu();
                    enCT.NoiDung = "Ngày Chuyển Thiết Kế";
                    if (dtCT.Rows[0]["NgayChuyenThietKe"].ToString() != "")
                        enCT.CreateDate = DateTime.Parse(dtCT.Rows[0]["NgayChuyenThietKe"].ToString());
                    en.lstGhiChu.Add(enCT);

                    if (dtCT.Rows[0]["TRONGAITHIETKE"].ToString() != "" && bool.Parse(dtCT.Rows[0]["TRONGAITHIETKE"].ToString()) == true)
                    {
                        enCT = new GhiChu();
                        enCT.NoiDung = "Hồ sơ trở ngại thiết kế; " + dtCT.Rows[0]["NOIDUNGTRONGAI"].ToString() + "; Tái xét: " + dtCT.Rows[0]["TaiXet"].ToString();
                        if (dtCT.Rows[0]["NGAYHOANTATTK"].ToString() != "")
                            enCT.CreateDate = DateTime.Parse(dtCT.Rows[0]["NGAYHOANTATTK"].ToString());
                        en.lstGhiChu.Add(enCT);
                    }

                    enCT = new GhiChu();
                    enCT.NoiDung = "Ngày Xin Phép Đào Đường";
                    if (dtCT.Rows[0]["NgayXinPhepDaoDuong"].ToString() != "")
                        enCT.CreateDate = DateTime.Parse(dtCT.Rows[0]["NgayXinPhepDaoDuong"].ToString());
                    en.lstGhiChu.Add(enCT);

                    enCT = new GhiChu();
                    enCT.NoiDung = "Ngày Có Phép Đào Đường";
                    if (dtCT.Rows[0]["NgayCoPhepDaoDuong"].ToString() != "")
                        enCT.CreateDate = DateTime.Parse(dtCT.Rows[0]["NgayCoPhepDaoDuong"].ToString());
                    en.lstGhiChu.Add(enCT);

                    enCT = new GhiChu();
                    enCT.NoiDung = "Ngày Thi Công";
                    if (dtCT.Rows[0]["NgayThiCong"].ToString() != "")
                        enCT.CreateDate = DateTime.Parse(dtCT.Rows[0]["NgayThiCong"].ToString());
                    en.lstGhiChu.Add(enCT);

                    //enCT.SoHoaDon = item["SoHoaDon"].ToString();
                    //enCT.Ky = item["Ky"].ToString();
                    //enCT.TieuThu = item["TieuThu"].ToString();
                    //enCT.GiaBan = item["GiaBan"].ToString();
                    //enCT.ThueGTGT = item["ThueGTGT"].ToString();
                    //enCT.PhiBVMT = item["PhiBVMT"].ToString();
                    //enCT.TongCong = item["TongCong"].ToString();
                    //if (item["NgayGiaiTrach"].ToString() != "")
                    //    enCT.NgayGiaiTrach = DateTime.Parse(item["NgayGiaiTrach"].ToString());
                    //enCT.DangNgan = item["DangNgan"].ToString();
                    //enCT.HanhThu = item["HanhThu"].ToString();
                    //enCT.MaDN = item["MaDN"].ToString();
                    //if (item["NgayDN"].ToString() != "")
                    //    enCT.NgayDN = DateTime.Parse(item["NgayDN"].ToString());
                    //if (item["NgayMN"].ToString() != "")
                    //    enCT.NgayMN = DateTime.Parse(item["NgayMN"].ToString());
                    //enCT.DongNuoc2 = bool.Parse(item["DongNuoc2"].ToString());
                    //enCT.LenhHuy = bool.Parse(item["LenhHuy"].ToString());
                    //enCT.ToTrinh = bool.Parse(item["ToTrinh"].ToString());
                    //en.lstHoaDon.Add(enCT);
                }
                return en;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public DataTable getDS_NiemChi(string DanhBo)
        {
            string sql = "select NoiDung=N'Thu Tiền đóng nước: '+CONVERT(varchar(10),NgayDN,103)+' '+CONVERT(varchar(10),NgayDN,108)"
                        + " , NiemChi,MauSac,KhoaTu=case when KhoaTu=1 then 'true' else 'false' end,KhoaKhac=case when KhoaKhac=1 then 'true' else 'false' end"
                        + " ,NgayLap=NgayDN from TT_KQDongNuoc where DanhBo = '" + DanhBo + "' and DongNuoc = 1"
                        + " union all"
                        + " select NoiDung = N'Thu Tiền mở nước: ' + CONVERT(varchar(10), NgayMN, 103) + ' ' + CONVERT(varchar(10), NgayMN, 108)"
                        + " , NiemChiMN,MauSacMN,KhoaTu=case when KhoaTu=1 then 'true' else 'false' end,KhoaKhac=case when KhoaKhac=1 then 'true' else 'false' end"
                        + " ,NgayLap=NgayMN from TT_KQDongNuoc where DanhBo = '" + DanhBo + "' and MoNuoc = 1"
                        + " union all"
                        + " select NoiDung=N'Thương Vụ bấm chì: '+CONVERT(varchar(10),NgayBC,103)+' '+CONVERT(varchar(10),NgayBC,108)"
                        + " ,NiemChi,MauSac,KhoaTu='false',KhoaKhac='false',NgayLap=NgayBC from KTKS_DonKH.dbo.BamChi_ChiTiet where DanhBo = '" + DanhBo + "'"
                        + " union all"
                        + " SELECT NoiDung = N'Thay ĐHN định kỳ: ' + CONVERT(varchar(10),[HCT_NGAYGAN], 103) + ' ' + CONVERT(varchar(10),[HCT_NGAYGAN], 108)"
                        + " ,NiemChi =[HCT_CHIGOC],[HCT_MAUCHIGOC],KhoaTu='false',KhoaKhac='false',NgayLap=[HCT_NGAYGAN]"
                        + " FROM[CAPNUOCTANHOA].[dbo].[TB_THAYDHN]"
                        + " where DHN_DANHBO = '" + DanhBo + "'"
                        + " order by NgayLap desc";
            return _cDAL_ThuTien.ExecuteQuery_DataTable(sql);
        }

        /// <summary>
        /// Lấy List thông tin mở rộng
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getThongTinExtra")]
        [HttpGet]
        public IList<ThongTinExtra> getThongTinExtra(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                List<ThongTinExtra> lst = new List<ThongTinExtra>();
                DataTable dtThayDHN = _cDAL_DHN.ExecuteQuery_DataTable("SELECT isnull(DHN_LOAIBANGKE,'')+'-' + isnull(convert(varchar, DHN_SOBANGKE), '') MASO,DHN_NGAYBAOTHAY NGAYBAO, 'THAY '+DHN_LYDOTHAY AS LYDO,HCT_NGAYGAN HOANTHANH, case when HCT_NHOMTRONGAI = 0 then '' else LyDo + ', ' end + isnull(HCT_LYDOTRONGAI, '') TRONGAI"
                + " FROM TB_THAYDHN t1"
                + " join TB_THAYDHN_TRONGAI t2 on t1.HCT_NHOMTRONGAI = t2.ID"
                + " where DHN_DANHBO = '" + DanhBo + "'"
                + " union"
                + " select t1.SHS MASO, t1.NGAYNHAN NGAYBAO, TENLOAI+'; '+t1.GHICHU AS LYDO,NGAYHOANCONG HOANTHANH, isnull(t1.NOIDUNGTRONGAI + ', ', '')+isnull(t1.NOIDUNGTNCHUYEN + ', ', '') + isnull(t3.NOIDUNGTRONGAI + ', ', '') + ', ' + isnull(t4.NOIDUNGTN + ', ', '') TRONGAI"
                + " from TANHOA_WATER.dbo.DON_KHACHHANG t1"
                + " left join TANHOA_WATER.dbo.LOAI_HOSO t2 on t1.LOAIHOSO = t2.MALOAI"
                + " left join TANHOA_WATER.dbo.TOTHIETKE t3 on t1.SHS = t3.SHS"
                + " left join TANHOA_WATER.dbo.KH_HOSOKHACHHANG t4 on t1.SHS = t4.SHS"
                + " where REPLACE(DANHBO,'-', '')= '" + DanhBo + "'"
                + " order by NGAYBAO desc");
                ThongTinExtra en = new ThongTinExtra();
                en.Title = "Công tác Thi Công";
                en.totalColumn = 5;
                en.lstColumn = new List<string> { "Mã", "Ngày Báo", "Lý Do", "Hoàn Thành", "Trở Ngại" };
                en.totalRow = dtThayDHN.Rows.Count;
                for (int i = 0; i < en.totalRow; i++)
                {
                    List<string> lstC = new List<string>();
                    for (int j = 0; j < en.totalColumn; j++)
                    {
                        lstC.Add(dtThayDHN.Rows[i][j].ToString());
                    }
                    en.lstRow.Add(lstC);
                }
                lst.Add(en);
                return lst;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy thông tin danh bộ khi quét QR Code
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getDanhBo_QRCode")]
        [HttpGet]
        public string getDanhBo_QRCode(string DanhBo, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(DanhBo + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                if (DanhBo.ToUpper().Contains("THW"))
                {
                    object result = null;
                    result = _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select DanhBo from QR_Dong where KyHieu='THW' and ID=" + DanhBo.Substring(3, DanhBo.Length - 3));
                    if (result != null && result.ToString() != "")
                        DanhBo = result.ToString();
                }
                return DanhBo;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        #region api TCT
        private string getAccess_token_TCT()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='cskhapi'").ToString();
        }

        /// <summary>
        /// Lấy thông tin báo chỉ số nước
        /// </summary>
        /// <param name="Nam">Năm</param>
        /// <param name="Thang">Tháng</param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getCSN_CSKH_TCT")]
        [HttpGet]
        public IList<CSKH_TCT> getCSN_CSKH_TCT(string Nam, string Thang, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(Nam + Thang + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                List<CSKH_TCT> lst = new List<CSKH_TCT>();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi + "/api/LichDocSo/LayChiSoNuocBao?branch_code=TH&nam=" + Nam + "&thang=" + Thang);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + getAccess_token_TCT();
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    if (obj["ketQua"] == 1)
                    {
                        var lst1 = obj["duLieu"];
                        foreach (var itemC1 in lst1)
                        {
                            CSKH_TCT en = new CSKH_TCT();
                            en.ID = itemC1["id"];
                            en.Nam = itemC1["nam"];
                            en.Ky = itemC1["ky"];
                            en.DanhBo = itemC1["danhbo"];
                            en.HoTen = itemC1["tenkh"];
                            en.CreateDate = DateTime.Parse(itemC1["createdDate"]);
                            en.CSN = itemC1["chiSoNuoc"];
                            if (itemC1["hinhAnhChiSo"] != "")
                                en.urlImage = _urlApi + itemC1["hinhAnhChiSo"];
                            lst.Add(en);
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy thông tin đơn khiếu nại
        /// </summary>
        /// <param name="Nam">Năm</param>
        /// <param name="Thang">Tháng</param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("getDonTu_CSKH_TCT")]
        [HttpGet]
        public IList<CSKH_TCT> getDonTu_CSKH_TCT(string Nam, string Thang, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(Nam + Thang + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                List<CSKH_TCT> lst = new List<CSKH_TCT>();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi + "/api/KhachHangPhanHoi/ThongTinPhanHoi?branch_code=TH&nam=" + Nam + "&thang=" + Thang);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + getAccess_token_TCT();
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    if (obj["ketQua"] == 1)
                    {
                        var lst1 = obj["duLieu"];
                        foreach (var itemC1 in lst1)
                        {
                            CSKH_TCT en = new CSKH_TCT();
                            en.ID = itemC1["id"];
                            en.Loai = itemC1["nameLoaiPhanHoi"];
                            en.DanhBo = itemC1["danhbo"];
                            en.DienThoai = itemC1["soDT"];
                            en.HoTen = itemC1["tenkh"];
                            en.CreateDate = DateTime.Parse(itemC1["createdDate"]);
                            en.TieuDe = itemC1["tieuDePhanHoi"];
                            en.NoiDung = itemC1["noiDungPhanHoi"];
                            en.GhiChu = itemC1["ghiChuThem"];
                            if (itemC1["hinhAnhPhanHoi"] != "")
                                en.urlImage = _urlApi + itemC1["hinhAnhPhanHoi"];
                            lst.Add(en);
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// cập nhật phản hồi đơn khiếu nại
        /// </summary>
        /// <param name="ID">ID đơn khiếu nại</param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("insertPhanHoi_DonTu_CSKH_TCT")]
        [HttpGet]
        public bool insertPhanHoi_DonTu_CSKH_TCT(string ID, string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(ID + CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                List<CSKH_TCT> lst = new List<CSKH_TCT>();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_urlApi + "/api/KhachHangPhanHoi/TraLoiPhanHoi");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + getAccess_token_TCT();
                var data = new
                {
                    IDPhanHoi = ID,
                    NoiDungTraLoi = "Công ty Cổ phần Cấp nước Tân Hòa đã tiếp nhận thông tin của Quý khách. Công ty sẽ phản hồi thông tin trong thời gian sớm nhất. Khi cần hỗ trợ thêm thông tin vui lòng liên hệ tổng đài 19006489. Xin cảm ơn!",
                    NguoiPhanHoi = "cskh.cnth"
                };
                var json = CGlobalVariable.jsSerializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                request.ContentLength = byteArray.Length;
                //gắn data post
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    if (obj["ketQua"] == 1)
                        return true;
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        #endregion

        #region api chất lượng nước

        private string getAccess_token_CLN()
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.capnuoctanhoa.com.vn/index.php?m=api&username=api&password=tanhoa@123&s=dang-nhap");
                request.Method = "POST";
                request.ContentLength = 0;
                request.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    if (obj["success"] == true)
                    {
                        return obj["data"]["token"];
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        /// <summary>
        /// Lấy danh sách chất lượng nước
        /// </summary>
        /// <returns></returns>
        [Route("getChatLuongNuoc")]
        [HttpGet]
        public IList<CSKH_TCT> getDonTu_CSKH_TCT(string checksum)
        {
            try
            {
                if (CGlobalVariable.getSHA256(CGlobalVariable.salaPass) != checksum)
                {
                    ErrorResponse error = new ErrorResponse(ErrorResponse.ErrorPassword, ErrorResponse.ErrorCodePassword);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
                }
                string token = getAccess_token_CLN();
                List<CSKH_TCT> lst = new List<CSKH_TCT>();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Ssl3;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.capnuoctanhoa.com.vn/index.php?m=api&s=chat-luong-nuoc");
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers["token"] = token;
                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    if (obj["success"] == true)
                    {
                        var lst1 = obj["data"];
                        foreach (var itemC1 in lst1)
                        {
                            CSKH_TCT en = new CSKH_TCT();
                            en.ID = int.Parse(itemC1["id"]);
                            string[] dates = ((string)itemC1["date"]).ToString().Split('-');
                            en.CreateDate = new DateTime(int.Parse(dates[2]), int.Parse(dates[1]), int.Parse(dates[0]));
                            en.TieuDe = itemC1["title"];
                            if (itemC1["file_download"] != "")
                                en.urlFile = itemC1["file_download"];
                            lst.Add(en);
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, ErrorResponse.ErrorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.OK, error));
            }
        }

        #endregion

        #region api sala

        string _tokensala = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyX2lkIjoiMjI0NDE2ODY3NDcwMTEyNjAyIiwidXNlcm5hbWUiOiJvcGVuYXBpIiwicGFzc3dvcmQiOiJkM2JkYmMxZGYyZTZlZGUyN2Y5MTVkNDg1ZDgzZDllMiJ9.QODFPm_-JwkPTam_sZoYUNSvYhG79ljbtNRaC0bB1JQ";

        /// <summary>
        /// Cập nhật số điện thoại từ crm
        /// </summary>
        /// <param name="FromCreateDate">yyyy-MM-dd</param>
        /// <param name="ToCreateDate">yyyy-MM-dd</param>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [Route("sala_updateDienThoai")]
        [HttpGet]
        public string sala_updateDienThoai(string FromCreateDate, string ToCreateDate, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Ssl3;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://crm.cskhtanhoa.com.vn:8443/openapi/v2/customers/get?from_date=" + FromCreateDate + "&to_date=" + ToCreateDate);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Bearer " + _tokensala;

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();
                        CGlobalVariable.jsSerializer.MaxJsonLength = Int32.MaxValue;
                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        if (obj["total"] > 0)
                        {
                            var lst1 = obj["data"];
                            string sql = "";
                            foreach (var itemC1 in lst1)
                                if (itemC1["danhbo"] != null && ((string)itemC1["phone"]).Length == 10)
                                {
                                    //if (itemC1["danhbo"] == "")
                                    //    strResponse = itemC1["danhbo"] + " " + itemC1["phone"] + " " + itemC1["name"] + " " + itemC1["created_at"];
                                    string name = "";
                                    if (itemC1["name"] != null && ((string)itemC1["name"]).Trim().Length > 0)
                                    {
                                        name = itemC1["name"];
                                        name = name.Replace("'", "");
                                    }
                                    if (((string)itemC1["danhbo"]).Length == 11)
                                        sql += " if not exists(select DanhBo from SDT_DHN where DanhBo='" + itemC1["danhbo"] + "' and DienThoai='" + itemC1["phone"] + "')"
                                            + " insert into SDT_DHN(DanhBo,DienThoai,HoTen,SoChinh,GhiChu,CreateBy,CreateDate)values('" + itemC1["danhbo"] + "','" + itemC1["phone"] + "',N'" + name + "',0,N'P. KH',0,GETDATE())";
                                    else
                                        if (((string)itemC1["danhbo"]).Length > 11)
                                    {
                                        string[] danhbos = ((string)itemC1["danhbo"]).Split(',');
                                        foreach (string danhbo in danhbos)
                                            if (danhbo.Length == 11)
                                            {
                                                sql += " if not exists(select DanhBo from SDT_DHN where DanhBo='" + danhbo + "' and DienThoai='" + itemC1["phone"] + "')"
                                                + " insert into SDT_DHN(DanhBo,DienThoai,HoTen,SoChinh,GhiChu,CreateBy,CreateDate)values('" + danhbo + "','" + itemC1["phone"] + "',N'" + name + "',0,N'P. KH',0,GETDATE())";
                                            }
                                    }
                                }
                            if (sql != "")
                                _cDAL_DHN.ExecuteNonQuery(sql);
                            strResponse = "Đã xử lý";
                        }
                    }
                    else
                    {
                        strResponse = "Error: " + respuesta.StatusCode;
                    }
                }
                else
                    strResponse = "Sai checksum";
            }
            catch (Exception ex)
            {
                strResponse = ex.Message;
            }
            return strResponse;
        }


        #endregion

    }
}