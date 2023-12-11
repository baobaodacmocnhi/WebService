using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/ThuongVu")]
    public class apiThuongVuController : ApiController
    {
        private CConnection _cDAL_ThuongVu = new CConnection(CGlobalVariable.ThuongVuWFH);
        private wrThuongVu.wsThuongVu _wsThuongVu = new wrThuongVu.wsThuongVu();

        [Route("insertCCCDtoTCT")]
        [HttpGet]
        public bool insertCCCDtoTCT(string checksum)
        {
            try
            {
                if (CGlobalVariable.checksum == checksum)
                {
                    DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select * from ChungTu_ChiTiet where len(MaCT)=12 and malct=15 and Cat=0 and mact not in(select CCCD from CCCD_Temp) and DanhBo not like ''");
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            string result = "";
                            _wsThuongVu.them_CCCD(item["DanhBo"].ToString(), item["MaCT"].ToString(), out result);
                            _cDAL_ThuongVu.ExecuteNonQuery("insert into CCCD_Temp(CCCD,DanhBo,Result,ModifyDate)values('" + item["MaCT"].ToString() + "','" + item["DanhBo"].ToString() + "',N'" + result + "',getdate())");
                        }
                        catch
                        {
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        [Route("donkh_login")]
        [HttpPost]
        public MResult donkh_login()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string IDUser = jsonContent["IDUser"].ToString(), checksum = jsonContent["checksum"].ToString();
                    if (checksum == CGlobalVariable.salaPass)
                    {
                        DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select TenHTKT from KTKS_DonKH.dbo.KTXM_HienTrang where " + _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("select t.KyHieu from KTKS_DonKH.dbo.Users u,KTKS_DonKH.dbo.[To] t where u.MaTo = t.MaTo and u.MaU = " + IDUser) + "=1 order by TenHTKT asc");
                        string[] HTKT = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            HTKT[i] = dt.Rows[i][0].ToString();
                        };
                        dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select TenTTBC from KTKS_DonKH.dbo.BamChi_TrangThai order by TenTTBC asc");
                        string[] TTBC = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            TTBC[i] = dt.Rows[i][0].ToString();
                        };
                        var data = new
                        {
                            HienTrangKiemTra = HTKT,
                            ViTriDHN1 = new string[] { "Bên Trái", "Bên Phải", "Ở Giữa" },
                            ViTriDHN2 = new string[] { "Trong Sân", "Trong Nhà", "Vỉa Hè" },
                            ChiSoLucKiemTra = new string[] { "Chạy", "NCN", "Chạy lết", "Chạy ngược", "Kẹt số", "Tuôn số", "Không nước", "Kiếng mờ", "Mất ĐHN"
                            , "Không ĐHN", "Lấp mất", "Bể kiếng", "Mất mặt số", "Chủ gỡ", "Hầm sâu", "Chất đồ", "Gắn ngược", "Đóng nước", "Cắt ống bên ngoài", "Kẹt khóa" },
                            ChiMatSo = new string[] { "Còn", "Không", "Lấp", "Mục đứt", "Đứt" },
                            ChiKhoaGoc = new string[] { "Còn", "Không", "Lấp", "Mục đứt", "Đứt" },
                            BaoThay = new string[] { "Chì kẽm", "Nghi ngờ gian lận", "Tờ trình miễn phí", "Hộp bảo vệ", "Khác" },
                            TrangThaiBamChi = TTBC,
                        };
                        result.success = true;
                        result.data = CGlobalVariable.jsSerializer.Serialize(data);
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
                else
                {
                    result.success = false;
                    result.error = "Thiếu parameter";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }

        //đơn từ
        class DonKH
        {
            public string MaDon { get; set; }
            public string STT { get; set; }
            public string DanhBo { get; set; }
            public string MLT { get; set; }
            public string HopDong { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string GiaBieu { get; set; }
            public string DinhMuc { get; set; }
            public string DinhMucHN { get; set; }
            public string NoiDung { get; set; }
            public string CreateDate { get; set; }
            public string NgayChuyen { get; set; }
            public List<BBKT> lstBBKT { get; set; }
            public List<BBBC> lstBBBC { get; set; }
            public DonKH()
            {
                MaDon = STT = DanhBo = MLT = HopDong = HoTen = DiaChi = GiaBieu = DinhMuc = DinhMucHN = NoiDung = CreateDate = NgayChuyen = "";
                lstBBKT = new List<BBKT>();
                lstBBBC = new List<BBBC>();
            }
        }

        //biên bản kiểm tra
        class BBKT
        {
            public string DanhBo { get; set; }
            public string HopDong { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string GiaBieu { get; set; }
            public string DinhMuc { get; set; }
            public string DinhMucHN { get; set; }
            public string NgayKTXM { get; set; }
            public string HienTrangKiemTra { get; set; }
            public string ViTriDHN1 { get; set; }
            public string ViTriDHN2 { get; set; }
            public string ChiSo { get; set; }
            public string ChiSoLucKiemTra { get; set; }
            public string Hieu { get; set; }
            public string Co { get; set; }
            public string SoThan { get; set; }
            public string ChiMatSo { get; set; }
            public string ChiKhoaGoc { get; set; }
            public string NoiDungKiemTra { get; set; }
            public string MucDichSuDung { get; set; }
            public string DienThoai { get; set; }
            public string HoTenKHKy { get; set; }
            public string BaoThay { get; set; }
            public string BaoThayGhiChu { get; set; }
            public string ThucHienTheoYeuCau { get; set; }
            public string TTTB { get; set; }
            public string TongSoTien { get; set; }
            public string DinhMucMoi { get; set; }
            public string DinhMucKhongDangKy { get; set; }
            public bool CanKhachHangLienHe { get; set; }
            public BBKT()
            {
                DanhBo =
                HopDong =
                HoTen =
                DiaChi =
                GiaBieu =
                DinhMuc =
                DinhMucHN =
                NgayKTXM =
                HienTrangKiemTra =
                ViTriDHN1 =
                ViTriDHN2 =
                ChiSo =
                ChiSoLucKiemTra =
                Hieu =
                Co =
                SoThan =
                ChiMatSo =
                ChiKhoaGoc =
                NoiDungKiemTra =
                MucDichSuDung =
                DienThoai =
                HoTenKHKy =
                BaoThay =
                BaoThayGhiChu =
                ThucHienTheoYeuCau =
                TTTB =
                TongSoTien =
                DinhMucMoi =
                DinhMucKhongDangKy = "";
                CanKhachHangLienHe = false;
            }
        }

        //biên bản bấm chì
        class BBBC
        {
            public string DanhBo { get; set; }
            public string HopDong { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string GiaBieu { get; set; }
            public string DinhMuc { get; set; }
            public string DinhMucHN { get; set; }
            public string NgayBC { get; set; }
            public string ChiSo { get; set; }
            public string ChiSoLucKiemTra { get; set; }
            public string Hieu { get; set; }
            public string Co { get; set; }
            public string SoThan { get; set; }
            public string ChiMatSo { get; set; }
            public string ChiKhoaGoc { get; set; }
            public string NiemChi { get; set; }
            public string MauSac { get; set; }
            public string MucDichSuDung { get; set; }
            public string TrangThaiBamChi { get; set; }
            public string MaKim { get; set; }
            public string VienChi { get; set; }
            public string DayChi { get; set; }
            public string ThucHienTheoYeuCau { get; set; }
            public string GhiChu { get; set; }
            public BBBC()
            {
                DanhBo =
                HopDong =
                HoTen =
                DiaChi =
                GiaBieu =
                DinhMuc =
                DinhMucHN =
                NgayBC =
                ChiSo =
                ChiSoLucKiemTra =
                Hieu =
                Co =
                SoThan =
                ChiMatSo =
                ChiKhoaGoc =
                NiemChi =
                MauSac =
                MucDichSuDung =
                TrangThaiBamChi =
                MaKim =
                VienChi =
                DayChi =
                ThucHienTheoYeuCau =
                GhiChu = "";
            }
        }

        [Route("donkh_getDonKH")]
        [HttpPost]
        public MResult donkh_getDonKH()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string IDUser = jsonContent["IDUser"].ToString(), checksum = jsonContent["checksum"].ToString()
                        , Loai = jsonContent["Loai"].ToString(), FromDate = "", ToDate = "";
                    if (jsonContent.ContainsKey("FromDate"))
                    {
                        string[] dates = jsonContent["FromDate"].ToString().Split('/');
                        FromDate = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent.ContainsKey("ToDate"))
                    {
                        string[] dates = jsonContent["ToDate"].ToString().Split('/');
                        ToDate = dates[2] + dates[1] + dates[0];
                    }
                    if (checksum == CGlobalVariable.salaPass)
                    {
                        DataTable dtDonKH = new DataTable(), dtBBKT = new DataTable(), dtBBBC = new DataTable();
                        switch (Loai)
                        {
                            case "Giao":
                                dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon,dtct.STT,dtct.DanhBo,dtct.MLT,dtct.HopDong,dtct.HoTen,dtct.DiaChi,dtct.GiaBieu,dtct.DinhMuc,DinhMucHN,NoiDung=dt.Name_NhomDon_PKH,dtct.CreateDate,ls.NgayChuyen"
                                   + " from KTKS_DonKH.dbo.DonTu_LichSu ls, KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                   + " where ID_KTXM = " + IDUser + " and CAST(NgayChuyen as date) >= '" + FromDate + "' and CAST(NgayChuyen as date) <= '" + FromDate + "'"
                                   + " and ls.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and ls.STT = dtct.STT order by dtct.CreateDate asc");
                                dtBBKT = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=kt.MaDonMoi,ktct.* from KTKS_DonKH.dbo.KTXM kt,KTKS_DonKH.dbo.KTXM_ChiTiet ktct,KTKS_DonKH.dbo.DonTu_LichSu ls"
                                   + " where ID_KTXM = " + IDUser + " and CAST(NgayChuyen as date) >= '" + FromDate + "' and CAST(NgayChuyen as date) <= '" + FromDate + "'"
                                   + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + IDUser + " and ls.MaDon = kt.MaDonMoi and ls.STT = ktct.STT");
                                dtBBBC = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=bc.MaDonMoi,bcct.* from KTKS_DonKH.dbo.BamChi bc,KTKS_DonKH.dbo.BamChi_ChiTiet bcct,KTKS_DonKH.dbo.DonTu_LichSu ls"
                                    + " where ID_KTXM = " + IDUser + " and CAST(NgayChuyen as date) >= '" + FromDate + "' and CAST(NgayChuyen as date) <= '" + FromDate + "'"
                                    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + IDUser + " and ls.MaDon = bc.MaDonMoi and ls.STT = bcct.STT");
                                break;
                            case "XuLy":
                                dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon, dtct.STT, dtct.DanhBo, dtct.MLT, dtct.HopDong, dtct.HoTen, dtct.DiaChi, dtct.GiaBieu, dtct.DinhMuc, DinhMucHN"
                                    + " , NoiDung = dt.Name_NhomDon_PKH, dtct.CreateDate, NgayChuyen = (select top 1 NgayChuyen from KTKS_DonKH.dbo.DonTu_LichSu ls where ID_KTXM = 71 and ls.MaDon = t2.MaDon and ls.STT = t2.STT and CAST(NgayChuyen as date) <= CAST(t2.NgayXuLy as date))"
                                    + " from(select distinct * from"
                                    + " (select MaDon = kt.MaDonMoi, ktct.STT, NgayXuLy = NgayKTXM from KTKS_DonKH.dbo.KTXM kt, KTKS_DonKH.dbo.KTXM_ChiTiet ktct"
                                    + " where CAST(NgayKTXM as date) >= '" + FromDate + "' and CAST(NgayKTXM as date) <= '" + FromDate + "'"
                                    + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + IDUser + ""
                                    + " union all"
                                    + " select MaDon = bc.MaDonMoi, bcct.STT, NgayXuLy = NgayBC from KTKS_DonKH.dbo.BamChi bc, KTKS_DonKH.dbo.BamChi_ChiTiet bcct"
                                    + " where CAST(NgayBC as date) >= '" + FromDate + "' and CAST(NgayBC as date) <= '" + FromDate + "'"
                                    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + IDUser + ")t1)t2,KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                    + " where t2.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and t2.STT = dtct.STT"
                                    + " order by dtct.CreateDate asc");
                                dtBBKT = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=kt.MaDonMoi,ktct.* from KTKS_DonKH.dbo.KTXM kt,KTKS_DonKH.dbo.KTXM_ChiTiet ktct"
                                    + " where CAST(NgayKTXM as date) >= '" + FromDate + "' and CAST(NgayKTXM as date) <= '" + FromDate + "'"
                                    + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + IDUser);
                                dtBBBC = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=bc.MaDonMoi,bcct.* from KTKS_DonKH.dbo.BamChi bc,KTKS_DonKH.dbo.BamChi_ChiTiet bcct"
                                    + " where CAST(NgayBC as date) >= '" + FromDate + "' and CAST(NgayBC as date) <= '" + FromDate + "'"
                                    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + IDUser);
                                break;
                            case "Ton":
                                dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon,dtct.STT,dtct.DanhBo,dtct.MLT,dtct.HopDong,dtct.HoTen,dtct.DiaChi,dtct.GiaBieu,dtct.DinhMuc,DinhMucHN,NoiDung=dt.Name_NhomDon_PKH,dtct.CreateDate,ls.NgayChuyen"
                                    + " from KTKS_DonKH.dbo.DonTu_LichSu ls, KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                    + " where ID_KTXM = " + IDUser + " and ls.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and ls.STT = dtct.STT"
                                    + " and not exists(select top 1 MaCTKTXM from KTKS_DonKH.dbo.KTXM kt, KTKS_DonKH.dbo.KTXM_ChiTiet ktct where kt.MaKTXM = ktct.MaKTXM and kt.MaDonMoi = ls.MaDon and ktct.STT = ls.STT)"
                                    + " and not exists(select top 1 MaCTBC from KTKS_DonKH.dbo.BamChi bc, KTKS_DonKH.dbo.BamChi_ChiTiet bcct where bc.MaBC = bcct.MaBC and bc.MaDonMoi = ls.MaDon and bcct.STT = ls.STT)"
                                    + " order by dtct.CreateDate asc");
                                break;
                        }
                        List<DonKH> lst = new List<DonKH>();
                        for (int i = 0; i < dtDonKH.Rows.Count; i++)
                        {
                            DonKH en = new DonKH();
                            en.MaDon = dtDonKH.Rows[i]["MaDon"].ToString();
                            en.STT = dtDonKH.Rows[i]["STT"].ToString();
                            en.DanhBo = dtDonKH.Rows[i]["DanhBo"].ToString();
                            en.MLT = dtDonKH.Rows[i]["MLT"].ToString();
                            en.HopDong = dtDonKH.Rows[i]["HopDong"].ToString();
                            en.HoTen = dtDonKH.Rows[i]["HoTen"].ToString();
                            en.DiaChi = dtDonKH.Rows[i]["DiaChi"].ToString();
                            en.GiaBieu = dtDonKH.Rows[i]["GiaBieu"].ToString();
                            en.DinhMuc = dtDonKH.Rows[i]["DinhMuc"].ToString();
                            en.DinhMucHN = dtDonKH.Rows[i]["DinhMucHN"].ToString();
                            en.NoiDung = dtDonKH.Rows[i]["NoiDung"].ToString();
                            en.CreateDate = dtDonKH.Rows[i]["CreateDate"].ToString();
                            en.NgayChuyen = dtDonKH.Rows[i]["NgayChuyen"].ToString();
                            if (dtBBKT.Rows.Count > 0)
                            {
                                DataRow[] dr = dtBBKT.Select("MaDon=" + en.MaDon + " and STT=" + en.STT);
                                if (dr != null && dr.Length > 0)
                                {
                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        BBKT enBBKT = new BBKT();
                                        enBBKT.DanhBo = dr[j]["DanhBo"].ToString();
                                        enBBKT.HopDong = dr[j]["HopDong"].ToString();
                                        enBBKT.HoTen = dr[j]["HoTen"].ToString();
                                        enBBKT.DiaChi = dr[j]["DiaChi"].ToString();
                                        enBBKT.GiaBieu = dr[j]["GiaBieu"].ToString();
                                        enBBKT.DinhMuc = dr[j]["DinhMuc"].ToString();
                                        enBBKT.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                        enBBKT.DinhMucMoi = dr[j]["DinhMucMoi"].ToString();
                                        enBBKT.DinhMucKhongDangKy = dr[j]["DinhMuc_KhongDangKy"].ToString();
                                        enBBKT.CanKhachHangLienHe = bool.Parse(dr[j]["CanKhachHangLienHe"].ToString());
                                        enBBKT.NgayKTXM = dr[j]["NgayKTXM"].ToString();
                                        enBBKT.HienTrangKiemTra = dr[j]["HienTrangKiemTra"].ToString();
                                        enBBKT.ViTriDHN1 = dr[j]["ViTriDHN1"].ToString();
                                        enBBKT.ViTriDHN2 = dr[j]["ViTriDHN2"].ToString();
                                        enBBKT.ChiSo = dr[j]["ChiSo"].ToString();
                                        enBBKT.ChiSoLucKiemTra = dr[j]["TinhTrangChiSo"].ToString();
                                        enBBKT.Hieu = dr[j]["Hieu"].ToString();
                                        enBBKT.Co = dr[j]["Co"].ToString();
                                        enBBKT.SoThan = dr[j]["SoThan"].ToString();
                                        enBBKT.ChiMatSo = dr[j]["ChiMatSo"].ToString();
                                        enBBKT.ChiKhoaGoc = dr[j]["ChiKhoaGoc"].ToString();
                                        enBBKT.NoiDungKiemTra = dr[j]["NoiDungKiemTra"].ToString();
                                        enBBKT.DienThoai = dr[j]["DienThoai"].ToString();
                                        enBBKT.HoTenKHKy = dr[j]["HoTenKHKy"].ToString();
                                        enBBKT.MucDichSuDung = dr[j]["MucDichSuDung"].ToString();
                                        enBBKT.BaoThay = dr[j]["NoiDungBaoThay"].ToString();
                                        enBBKT.BaoThayGhiChu = dr[j]["GhiChuNoiDungBaoThay"].ToString();
                                        enBBKT.ThucHienTheoYeuCau = dr[j]["TheoYeuCau"].ToString();
                                        enBBKT.TTTB = dr[j]["TieuThuTrungBinh"].ToString();
                                        enBBKT.TongSoTien = dr[j]["SoTienDongTien"].ToString();
                                        en.lstBBKT.Add(enBBKT);
                                    }
                                }
                            }
                            if (dtBBBC.Rows.Count > 0)
                            {
                                DataRow[] dr = dtBBBC.Select("MaDon=" + en.MaDon + " and STT=" + en.STT);
                                if (dr != null && dr.Length > 0)
                                {
                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        BBBC enBBBC = new BBBC();
                                        enBBBC.DanhBo = dr[j]["DanhBo"].ToString();
                                        enBBBC.HopDong = dr[j]["HopDong"].ToString();
                                        enBBBC.HoTen = dr[j]["HoTen"].ToString();
                                        enBBBC.DiaChi = dr[j]["DiaChi"].ToString();
                                        enBBBC.GiaBieu = dr[j]["GiaBieu"].ToString();
                                        enBBBC.DinhMuc = dr[j]["DinhMuc"].ToString();
                                        enBBBC.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                        enBBBC.NgayBC = dr[j]["NgayBC"].ToString();
                                        enBBBC.ChiSo = dr[j]["ChiSo"].ToString();
                                        enBBBC.ChiSoLucKiemTra = dr[j]["TinhTrangChiSo"].ToString();
                                        enBBBC.Hieu = dr[j]["Hieu"].ToString();
                                        enBBBC.Co = dr[j]["Co"].ToString();
                                        enBBBC.SoThan = dr[j]["SoThan"].ToString();
                                        enBBBC.ChiMatSo = dr[j]["ChiMatSo"].ToString();
                                        enBBBC.ChiKhoaGoc = dr[j]["ChiKhoaGoc"].ToString();
                                        enBBBC.NiemChi = dr[j]["NiemChi"].ToString();
                                        enBBBC.MauSac = dr[j]["MauSac"].ToString();
                                        enBBBC.MucDichSuDung = dr[j]["MucDichSuDung"].ToString();
                                        enBBBC.TrangThaiBamChi = dr[j]["TrangThaiBC"].ToString();
                                        enBBBC.MaKim = dr[j]["MaSoBC"].ToString();
                                        enBBBC.VienChi = dr[j]["VienChi"].ToString();
                                        enBBBC.DayChi = dr[j]["DayChi"].ToString();
                                        enBBBC.ThucHienTheoYeuCau = dr[j]["TheoYeuCau"].ToString();
                                        enBBBC.GhiChu = dr[j]["GhiChu"].ToString();
                                        en.lstBBBC.Add(enBBBC);
                                    }
                                }
                            }
                            lst.Add(en);
                        };
                        result.success = true;
                        result.data = CGlobalVariable.jsSerializer.Serialize(lst);
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
                else
                {
                    result.success = false;
                    result.error = "Thiếu parameter";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }

        [Route("donkh_insertBBKT")]
        [HttpPost]
        public MResult donkh_insertBBKT()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string IDUser = jsonContent["IDUser"].ToString(), checksum = jsonContent["checksum"].ToString()
                        , Loai = jsonContent["Loai"].ToString(), FromDate = "", ToDate = "";
                    if (jsonContent.ContainsKey("FromDate"))
                    {
                        string[] dates = jsonContent["FromDate"].ToString().Split('/');
                        FromDate = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent.ContainsKey("ToDate"))
                    {
                        string[] dates = jsonContent["ToDate"].ToString().Split('/');
                        ToDate = dates[2] + dates[1] + dates[0];
                    }
                    if (checksum == CGlobalVariable.salaPass)
                    {


                        result.success = true;
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
                else
                {
                    result.success = false;
                    result.error = "Thiếu parameter";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }

        [Route("donkh_updateBBKT")]
        [HttpPost]
        public MResult donkh_updateBBKT()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string IDUser = jsonContent["IDUser"].ToString(), checksum = jsonContent["checksum"].ToString()
                        , Loai = jsonContent["Loai"].ToString(), FromDate = "", ToDate = "";
                    if (jsonContent.ContainsKey("FromDate"))
                    {
                        string[] dates = jsonContent["FromDate"].ToString().Split('/');
                        FromDate = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent.ContainsKey("ToDate"))
                    {
                        string[] dates = jsonContent["ToDate"].ToString().Split('/');
                        ToDate = dates[2] + dates[1] + dates[0];
                    }
                    if (checksum == CGlobalVariable.salaPass)
                    {


                        result.success = true;
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
                else
                {
                    result.success = false;
                    result.error = "Thiếu parameter";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }

        [Route("donkh_insertBBBC")]
        [HttpPost]
        public MResult donkh_insertBBBC()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string IDUser = jsonContent["IDUser"].ToString(), checksum = jsonContent["checksum"].ToString()
                        , Loai = jsonContent["Loai"].ToString(), FromDate = "", ToDate = "";
                    if (jsonContent.ContainsKey("FromDate"))
                    {
                        string[] dates = jsonContent["FromDate"].ToString().Split('/');
                        FromDate = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent.ContainsKey("ToDate"))
                    {
                        string[] dates = jsonContent["ToDate"].ToString().Split('/');
                        ToDate = dates[2] + dates[1] + dates[0];
                    }
                    if (checksum == CGlobalVariable.salaPass)
                    {


                        result.success = true;
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
                else
                {
                    result.success = false;
                    result.error = "Thiếu parameter";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }

        [Route("donkh_updateBBBC")]
        [HttpPost]
        public MResult donkh_updateBBBC()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    string IDUser = jsonContent["IDUser"].ToString(), checksum = jsonContent["checksum"].ToString()
                        , Loai = jsonContent["Loai"].ToString(), FromDate = "", ToDate = "";
                    if (jsonContent.ContainsKey("FromDate"))
                    {
                        string[] dates = jsonContent["FromDate"].ToString().Split('/');
                        FromDate = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent.ContainsKey("ToDate"))
                    {
                        string[] dates = jsonContent["ToDate"].ToString().Split('/');
                        ToDate = dates[2] + dates[1] + dates[0];
                    }
                    if (checksum == CGlobalVariable.salaPass)
                    {


                        result.success = true;
                    }
                    else
                    {
                        result.success = false;
                        result.error = "Sai checksum";
                    }
                }
                else
                {
                    result.success = false;
                    result.error = "Thiếu parameter";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.error = ex.Message;
            }
            return result;
        }
    }
}