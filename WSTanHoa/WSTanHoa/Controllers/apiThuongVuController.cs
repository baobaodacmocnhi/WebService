using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using System.Web.Http;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/ThuongVu")]
    public class apiThuongVuController : ApiController
    {
        private CConnection _cDAL_ThuongVu = new CConnection(CGlobalVariable.ThuongVu);
        //private CConnection _cDAL_ThuongVu = new CConnection("Data Source=server9;Initial Catalog=KTKS_DonKHtest;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        private wrThuongVu.wsThuongVu _wsThuongVu = new wrThuongVu.wsThuongVu();

        [Route("insertCCCDtoTCT")]
        [HttpGet]
        public bool insertCCCDtoTCT(string checksum)
        {
            try
            {
                if (CGlobalVariable.checksum == checksum)
                {
                    DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select * from ChungTu_ChiTiet where len(MaCT)=12 and malct=15 and Cat=0 and mact not in(select CCCD from CCCD_Temp) and DanhBo is not null and DanhBo not like ''");
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

        [Route("insertCCCDtoTCT_BoSung2023")]
        [HttpGet]
        public bool insertCCCDtoTCT_BoSung2023(string dot, string checksum)
        {
            try
            {
                if (CGlobalVariable.checksum == checksum)
                {
                    DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select * from CCCD_BoSung2023 where XuLy=0 and Dot=" + dot);
                    foreach (DataRow item in dt.Rows)
                    {
                        try
                        {
                            string result = "";
                            _wsThuongVu.them_CCCD_BoSung2023(item["DanhBo"].ToString(), item["MaCT"].ToString(), out result);
                            _cDAL_ThuongVu.ExecuteNonQuery("update CCCD_BoSung2023 set XuLy=1,Result=N'" + result + "' where MaCT='" + item["MaCT"].ToString() + "' and DanhBo='" + item["DanhBo"].ToString() + "'");
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
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select TenHTKT from KTKS_DonKH.dbo.KTXM_HienTrang where " + _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("select t.KyHieu from KTKS_DonKH.dbo.Users u,KTKS_DonKH.dbo.[To] t where u.MaTo = t.MaTo and u.MaU = " + jsonContent["IDUser"].ToString()) + "=1 order by TenHTKT asc");
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
                            MauSau = new string[] { "Vàng", "Đỏ", }
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
            public string Nam { get; set; }
            public string Ky { get; set; }
            public string Dot { get; set; }
            public string Quan { get; set; }
            public string Phuong { get; set; }
            public string Hieu { get; set; }
            public string Co { get; set; }
            public string SoThan { get; set; }
            public string NoiDung { get; set; }
            public string CreateDate { get; set; }
            public string NgayChuyen { get; set; }
            public List<BBKT> lstBBKT { get; set; }
            public List<BBBC> lstBBBC { get; set; }
            public List<HinhAnh> lstHinhAnh { get; set; }

            public DonKH()
            {
                MaDon = STT = DanhBo = MLT = HopDong = HoTen = DiaChi = GiaBieu = DinhMuc = DinhMucHN
                   = Nam = Ky = Dot = Quan = Phuong = Hieu = Co = SoThan = NoiDung = CreateDate = NgayChuyen = "";
                lstBBKT = new List<BBKT>();
                lstBBBC = new List<BBBC>();
                lstHinhAnh = new List<HinhAnh>();
            }
        }

        //hình ảnh
        class HinhAnh
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public HinhAnh()
            {
                Name = Value = Type = Url = "";
            }
        }

        //biên bản kiểm tra
        class BBKT
        {
            public string ID { get; set; }
            public string DanhBo { get; set; }
            public string HopDong { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string GiaBieu { get; set; }
            public string DinhMuc { get; set; }
            public string DinhMucHN { get; set; }
            public string Nam { get; set; }
            public string Ky { get; set; }
            public string Dot { get; set; }
            public string Quan { get; set; }
            public string Phuong { get; set; }
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
            public List<HinhAnh> lstHinhAnh { get; set; }
            public BBKT()
            {
                ID =
                DanhBo =
                HopDong =
                HoTen =
                DiaChi =
                GiaBieu =
                DinhMuc =
                DinhMucHN =
                Nam =
                Ky =
                Dot =
                Quan =
                Phuong =
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
                lstHinhAnh = new List<HinhAnh>();
            }
        }

        //biên bản bấm chì
        class BBBC
        {
            public string ID { get; set; }
            public string DanhBo { get; set; }
            public string HopDong { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string GiaBieu { get; set; }
            public string DinhMuc { get; set; }
            public string DinhMucHN { get; set; }
            public string Nam { get; set; }
            public string Ky { get; set; }
            public string Dot { get; set; }
            public string Quan { get; set; }
            public string Phuong { get; set; }
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
            public List<HinhAnh> lstHinhAnh { get; set; }
            public BBBC()
            {
                ID =
                DanhBo =
                HopDong =
                HoTen =
                DiaChi =
                GiaBieu =
                DinhMuc =
                DinhMucHN =
                Nam =
                Ky =
                Dot =
                Quan =
                Phuong =
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
                lstHinhAnh = new List<HinhAnh>();
            }
        }

        [Route("donkh_getTTKH")]
        [HttpPost]
        public MResult donkh_getTTKH()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select top 1 DANHBA, hd.HOPDONG, TENKH, SO, DUONG, GB, DM, hd.DinhMucHN, Hieu = ttkh.HIEUDH, Co = ttkh.CODH, SoThan = ttkh.SOTHANDH from[HOADON_TA].[dbo].[HOADON] hd left join[CAPNUOCTANHOA].[dbo].[TB_DULIEUKHACHHANG] ttkh on ttkh.DanhBo = hd.DanhBa"
                            + " where DanhBa = '" + jsonContent["DanhBo"].ToString() + "' order by ID_HOADON desc");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            DonKH en = new DonKH();
                            en.DanhBo = dt.Rows[0]["DANHBA"].ToString();
                            en.HopDong = dt.Rows[0]["HOPDONG"].ToString();
                            en.HoTen = dt.Rows[0]["TENKH"].ToString();
                            en.DiaChi = dt.Rows[0]["SO"].ToString() + dt.Rows[0]["DUONG"].ToString();
                            en.GiaBieu = dt.Rows[0]["GB"].ToString();
                            en.DinhMuc = dt.Rows[0]["DM"].ToString();
                            en.DinhMucHN = dt.Rows[0]["DinhMucHN"].ToString();
                            en.Hieu = dt.Rows[0]["Hieu"].ToString();
                            en.Co = dt.Rows[0]["Co"].ToString();
                            en.SoThan = dt.Rows[0]["SoThan"].ToString();
                            result.success = true;
                            result.data = CGlobalVariable.jsSerializer.Serialize(en);
                        }
                        else
                        {
                            result.success = false;
                            result.error = "Danh Bộ không tồn tại";
                        }
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
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        string[] MaDons = jsonContent["MaDon"].ToString().Split('.');
                        DataTable dtDonKH = new DataTable(), dtBBKT = new DataTable(), dtBBBC = new DataTable();
                        dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon,dtct.STT,dtct.DanhBo,dtct.MLT,dtct.HopDong,dtct.HoTen,dtct.DiaChi,dtct.GiaBieu,dtct.DinhMuc,DinhMucHN,NoiDung=dt.Name_NhomDon_PKH,dtct.CreateDate,ls.NgayChuyen"
                            + " ,dtct.Nam,dtct.Ky,dtct.Dot,dtct.Quan,dtct.Phuong"
                            + " from KTKS_DonKH.dbo.DonTu_LichSu ls, KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                            + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and dtct.MaDon=" + MaDons[0] + " and dtct.STT=" + MaDons[1]
                            + " and ls.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and ls.STT = dtct.STT order by dtct.CreateDate asc");
                        dtBBKT = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=kt.MaDonMoi,ktct.* from KTKS_DonKH.dbo.KTXM kt,KTKS_DonKH.dbo.KTXM_ChiTiet ktct,KTKS_DonKH.dbo.DonTu_LichSu ls"
                            + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and kt.MaDonMoi=" + MaDons[0] + " and ktct.STT=" + MaDons[1]
                            + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = kt.MaDonMoi and ls.STT = ktct.STT");
                        dtBBBC = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=bc.MaDonMoi,bcct.* from KTKS_DonKH.dbo.BamChi bc,KTKS_DonKH.dbo.BamChi_ChiTiet bcct,KTKS_DonKH.dbo.DonTu_LichSu ls"
                            + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and bc.MaDonMoi=" + MaDons[0] + " and bcct.STT=" + MaDons[1]
                            + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = bc.MaDonMoi and ls.STT = bcct.STT");
                        List<DonKH> lst = new List<DonKH>();
                        for (int i = 0; i < dtDonKH.Rows.Count; i++)
                        {
                            DonKH en = new DonKH();
                            en.MaDon = dtDonKH.Rows[i]["MaDon"].ToString() + "." + dtDonKH.Rows[i]["STT"].ToString();
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
                            en.Nam = dtDonKH.Rows[i]["Nam"].ToString();
                            en.Ky = dtDonKH.Rows[i]["Ky"].ToString();
                            en.Dot = dtDonKH.Rows[i]["Dot"].ToString();
                            en.Quan = dtDonKH.Rows[i]["Quan"].ToString();
                            en.Phuong = dtDonKH.Rows[i]["Phuong"].ToString();
                            if (dtBBKT.Rows.Count > 0)
                            {
                                DataRow[] dr = dtBBKT.Select("MaDon=" + dtDonKH.Rows[i]["MaDon"].ToString() + " and STT=" + dtDonKH.Rows[i]["STT"].ToString());
                                if (dr != null && dr.Length > 0)
                                {
                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        BBKT enBBKT = new BBKT();
                                        enBBKT.ID = dr[j]["MaCTKTXM"].ToString();
                                        enBBKT.DanhBo = dr[j]["DanhBo"].ToString();
                                        enBBKT.HopDong = dr[j]["HopDong"].ToString();
                                        enBBKT.HoTen = dr[j]["HoTen"].ToString();
                                        enBBKT.DiaChi = dr[j]["DiaChi"].ToString();
                                        enBBKT.GiaBieu = dr[j]["GiaBieu"].ToString();
                                        enBBKT.DinhMuc = dr[j]["DinhMuc"].ToString();
                                        enBBKT.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                        enBBKT.Nam = dr[j]["Nam"].ToString();
                                        enBBKT.Ky = dr[j]["Ky"].ToString();
                                        enBBKT.Dot = dr[j]["Dot"].ToString();
                                        enBBKT.Quan = dr[j]["Quan"].ToString();
                                        enBBKT.Phuong = dr[j]["Phuong"].ToString();
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
                                        DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select ID from KTXM_ChiTiet_Hinh where Huy=0 and IDKTXM_ChiTiet = " + enBBKT.ID + " order by CreateDate desc");
                                        foreach (DataRow item in dt.Rows)
                                        {
                                            HinhAnh enHinhAnh = new HinhAnh();
                                            enHinhAnh.Name = item["ID"].ToString();
                                            enHinhAnh.Url = "https://service.cskhtanhoa.com.vn/ThuongVu/viewFile_ChiTiet?TableName=KTXM_ChiTiet_Hinh&IDFileContent=" + enBBKT.ID + "&ID=" + item["ID"].ToString();
                                            enBBKT.lstHinhAnh.Add(enHinhAnh);
                                        }
                                        en.lstBBKT.Add(enBBKT);
                                    }
                                }
                            }
                            if (dtBBBC.Rows.Count > 0)
                            {
                                DataRow[] dr = dtBBBC.Select("MaDon=" + dtDonKH.Rows[i]["MaDon"].ToString() + " and STT=" + dtDonKH.Rows[i]["STT"].ToString());
                                if (dr != null && dr.Length > 0)
                                {
                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        BBBC enBBBC = new BBBC();
                                        enBBBC.ID = dr[j]["MaCTBC"].ToString();
                                        enBBBC.DanhBo = dr[j]["DanhBo"].ToString();
                                        enBBBC.HopDong = dr[j]["HopDong"].ToString();
                                        enBBBC.HoTen = dr[j]["HoTen"].ToString();
                                        enBBBC.DiaChi = dr[j]["DiaChi"].ToString();
                                        enBBBC.GiaBieu = dr[j]["GiaBieu"].ToString();
                                        enBBBC.DinhMuc = dr[j]["DinhMuc"].ToString();
                                        enBBBC.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                        enBBBC.Nam = dr[j]["Nam"].ToString();
                                        enBBBC.Ky = dr[j]["Ky"].ToString();
                                        enBBBC.Dot = dr[j]["Dot"].ToString();
                                        enBBBC.Quan = dr[j]["Quan"].ToString();
                                        enBBBC.Phuong = dr[j]["Phuong"].ToString();
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
                                        DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select ID from BamChi_ChiTiet_Hinh where Huy=0 and IDBamChi_ChiTiet = " + enBBBC.ID + " order by CreateDate desc");
                                        foreach (DataRow item in dt.Rows)
                                        {
                                            HinhAnh enHinhAnh = new HinhAnh();
                                            enHinhAnh.Name = item["ID"].ToString();
                                            enHinhAnh.Url = "https://service.cskhtanhoa.com.vn/ThuongVu/viewFile_ChiTiet?TableName=BamChi_ChiTiet_Hinh&IDFileContent=" + enBBBC.ID + "&ID=" + item["ID"].ToString();
                                            enBBBC.lstHinhAnh.Add(enHinhAnh);
                                        }
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

        [Route("donkh_getDSDonKH")]
        [HttpPost]
        public MResult donkh_getDSDonKH()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        string FromDate = "", ToDate = "";
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
                        DataTable dtDonKH = new DataTable(), dtBBKT = new DataTable(), dtBBBC = new DataTable();
                        switch (jsonContent["Loai"].ToString())
                        {
                            case "Giao":
                                dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon,dtct.STT,dtct.DanhBo,dtct.MLT,dtct.HopDong,dtct.HoTen,dtct.DiaChi,dtct.GiaBieu,dtct.DinhMuc,DinhMucHN,NoiDung=dt.Name_NhomDon_PKH,dtct.CreateDate,ls.NgayChuyen"
                                    + " ,dtct.Nam,dtct.Ky,dtct.Dot,dtct.Quan,dtct.Phuong"
                                    + " from KTKS_DonKH.dbo.DonTu_LichSu ls, KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                    + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and CAST(NgayChuyen as date) >= '" + FromDate + "' and CAST(NgayChuyen as date) <= '" + FromDate + "'"
                                    + " and ls.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and ls.STT = dtct.STT order by dtct.CreateDate asc");
                                dtBBKT = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=kt.MaDonMoi,ktct.* from KTKS_DonKH.dbo.KTXM kt,KTKS_DonKH.dbo.KTXM_ChiTiet ktct,KTKS_DonKH.dbo.DonTu_LichSu ls"
                                    + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and CAST(NgayChuyen as date) >= '" + FromDate + "' and CAST(NgayChuyen as date) <= '" + FromDate + "'"
                                    + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = kt.MaDonMoi and ls.STT = ktct.STT");
                                dtBBBC = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=bc.MaDonMoi,bcct.* from KTKS_DonKH.dbo.BamChi bc,KTKS_DonKH.dbo.BamChi_ChiTiet bcct,KTKS_DonKH.dbo.DonTu_LichSu ls"
                                    + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and CAST(NgayChuyen as date) >= '" + FromDate + "' and CAST(NgayChuyen as date) <= '" + FromDate + "'"
                                    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = bc.MaDonMoi and ls.STT = bcct.STT");
                                break;
                            case "XuLy":
                                //dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon, dtct.STT, dtct.DanhBo, dtct.MLT, dtct.HopDong, dtct.HoTen, dtct.DiaChi, dtct.GiaBieu, dtct.DinhMuc, DinhMucHN"
                                //    + " , NoiDung = dt.Name_NhomDon_PKH, dtct.CreateDate, NgayChuyen = (select top 1 NgayChuyen from KTKS_DonKH.dbo.DonTu_LichSu ls where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = t2.MaDon and ls.STT = t2.STT and CAST(NgayChuyen as date) <= CAST(t2.NgayXuLy as date))"
                                //    + " ,dtct.Nam,dtct.Ky,dtct.Dot,dtct.Quan,dtct.Phuong"
                                //    + " from(select distinct * from"
                                //    + " (select MaDon = kt.MaDonMoi, ktct.STT, NgayXuLy = NgayKTXM from KTKS_DonKH.dbo.KTXM kt, KTKS_DonKH.dbo.KTXM_ChiTiet ktct"
                                //    + " where CAST(NgayKTXM as date) >= '" + FromDate + "' and CAST(NgayKTXM as date) <= '" + FromDate + "'"
                                //    + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + jsonContent["IDUser"].ToString()
                                //    + " union all"
                                //    + " select MaDon = bc.MaDonMoi, bcct.STT, NgayXuLy = NgayBC from KTKS_DonKH.dbo.BamChi bc, KTKS_DonKH.dbo.BamChi_ChiTiet bcct"
                                //    + " where CAST(NgayBC as date) >= '" + FromDate + "' and CAST(NgayBC as date) <= '" + FromDate + "'"
                                //    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + jsonContent["IDUser"].ToString() + ")t1)t2,KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                //    + " where t2.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and t2.STT = dtct.STT"
                                //    + " order by dtct.CreateDate asc");
                                dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon, dtct.STT, dtct.DanhBo, dtct.MLT, dtct.HopDong, dtct.HoTen, dtct.DiaChi, dtct.GiaBieu, dtct.DinhMuc, DinhMucHN"
                                    + " , NoiDung = dt.Name_NhomDon_PKH, dtct.CreateDate, NgayChuyen = (select top 1 NgayChuyen from KTKS_DonKH.dbo.DonTu_LichSu ls where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = t2.MaDon and ls.STT = t2.STT and CAST(NgayChuyen as date) <= CAST(t2.NgayXuLy as date))"
                                    + " ,dtct.Nam,dtct.Ky,dtct.Dot,dtct.Quan,dtct.Phuong"
                                    + " from(select distinct * from"
                                    + " (select MaDon = kt.MaDonMoi, ktct.STT, NgayXuLy = NgayKTXM from KTKS_DonKH.dbo.KTXM kt, KTKS_DonKH.dbo.KTXM_ChiTiet ktct"
                                    + " where CAST(NgayKTXM as date) >= '" + FromDate + "' and CAST(NgayKTXM as date) <= '" + FromDate + "'"
                                    + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + jsonContent["IDUser"].ToString()
                                    + " union all"
                                    + " select MaDon = bc.MaDonMoi, bcct.STT, NgayXuLy = NgayBC from KTKS_DonKH.dbo.BamChi bc, KTKS_DonKH.dbo.BamChi_ChiTiet bcct"
                                    + " where CAST(NgayBC as date) >= '" + FromDate + "' and CAST(NgayBC as date) <= '" + FromDate + "'"
                                    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + jsonContent["IDUser"].ToString() + ")t1)t2,KTKS_DonKH.dbo.DonTu_LichSu ls,KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                    + " where t2.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and t2.STT = dtct.STT and ID_KTXM=" + jsonContent["IDUser"].ToString() + " and ls.MaDon=dt.MaDon and ls.STT=dtct.STT"
                                    + " order by dtct.CreateDate asc");
                                dtBBKT = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=kt.MaDonMoi,ktct.* from KTKS_DonKH.dbo.KTXM kt,KTKS_DonKH.dbo.KTXM_ChiTiet ktct"
                                    + " where CAST(NgayKTXM as date) >= '" + FromDate + "' and CAST(NgayKTXM as date) <= '" + FromDate + "'"
                                    + " and kt.MaKTXM = ktct.MaKTXM and ktct.CreateBy = " + jsonContent["IDUser"].ToString());
                                dtBBBC = _cDAL_ThuongVu.ExecuteQuery_DataTable("select MaDon=bc.MaDonMoi,bcct.* from KTKS_DonKH.dbo.BamChi bc,KTKS_DonKH.dbo.BamChi_ChiTiet bcct"
                                    + " where CAST(NgayBC as date) >= '" + FromDate + "' and CAST(NgayBC as date) <= '" + FromDate + "'"
                                    + " and bc.MaBC = bcct.MaBC and bcct.CreateBy = " + jsonContent["IDUser"].ToString());
                                break;
                            case "Ton":
                                dtDonKH = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.MaDon,dtct.STT,dtct.DanhBo,dtct.MLT,dtct.HopDong,dtct.HoTen,dtct.DiaChi,dtct.GiaBieu,dtct.DinhMuc,DinhMucHN,NoiDung=dt.Name_NhomDon_PKH,dtct.CreateDate,ls.NgayChuyen"
                                    + " ,dtct.Nam,dtct.Ky,dtct.Dot,dtct.Quan,dtct.Phuong"
                                    + " from KTKS_DonKH.dbo.DonTu_LichSu ls, KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
                                    + " where ID_KTXM = " + jsonContent["IDUser"].ToString() + " and ls.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and ls.STT = dtct.STT"
                                    + " and not exists(select top 1 MaCTKTXM from KTKS_DonKH.dbo.KTXM kt, KTKS_DonKH.dbo.KTXM_ChiTiet ktct where kt.MaKTXM = ktct.MaKTXM and kt.MaDonMoi = ls.MaDon and ktct.STT = ls.STT)"
                                    + " and not exists(select top 1 MaCTBC from KTKS_DonKH.dbo.BamChi bc, KTKS_DonKH.dbo.BamChi_ChiTiet bcct where bc.MaBC = bcct.MaBC and bc.MaDonMoi = ls.MaDon and bcct.STT = ls.STT)"
                                    + " order by dtct.CreateDate asc");
                                break;
                        }
                        List<DonKH> lst = new List<DonKH>();
                        for (int i = 0; i < dtDonKH.Rows.Count; i++)
                        {
                            DonKH en = new DonKH();
                            en.MaDon = dtDonKH.Rows[i]["MaDon"].ToString() + "." + dtDonKH.Rows[i]["STT"].ToString();
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
                            en.Nam = dtDonKH.Rows[i]["Nam"].ToString();
                            en.Ky = dtDonKH.Rows[i]["Ky"].ToString();
                            en.Dot = dtDonKH.Rows[i]["Dot"].ToString();
                            en.Quan = dtDonKH.Rows[i]["Quan"].ToString();
                            en.Phuong = dtDonKH.Rows[i]["Phuong"].ToString();
                            if (dtBBKT.Rows.Count > 0)
                            {
                                DataRow[] dr = dtBBKT.Select("MaDon=" + dtDonKH.Rows[i]["MaDon"].ToString() + " and STT=" + dtDonKH.Rows[i]["STT"].ToString());
                                if (dr != null && dr.Length > 0)
                                {
                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        BBKT enBBKT = new BBKT();
                                        enBBKT.ID = dr[j]["MaCTKTXM"].ToString();
                                        enBBKT.DanhBo = dr[j]["DanhBo"].ToString();
                                        enBBKT.HopDong = dr[j]["HopDong"].ToString();
                                        enBBKT.HoTen = dr[j]["HoTen"].ToString();
                                        enBBKT.DiaChi = dr[j]["DiaChi"].ToString();
                                        enBBKT.GiaBieu = dr[j]["GiaBieu"].ToString();
                                        enBBKT.DinhMuc = dr[j]["DinhMuc"].ToString();
                                        enBBKT.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                        enBBKT.Nam = dr[j]["Nam"].ToString();
                                        enBBKT.Ky = dr[j]["Ky"].ToString();
                                        enBBKT.Dot = dr[j]["Dot"].ToString();
                                        enBBKT.Quan = dr[j]["Quan"].ToString();
                                        enBBKT.Phuong = dr[j]["Phuong"].ToString();
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
                                        HinhAnh enHinhAnh = new HinhAnh();
                                        enHinhAnh.Url = "https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=KTXM_ChiTiet_Hinh&IDFileName=IDKTXM_ChiTiet&IDFileContent=" + enBBKT.ID;
                                        enBBKT.lstHinhAnh.Add(enHinhAnh);
                                        en.lstBBKT.Add(enBBKT);
                                    }
                                }
                            }
                            if (dtBBBC.Rows.Count > 0)
                            {
                                DataRow[] dr = dtBBBC.Select("MaDon=" + dtDonKH.Rows[i]["MaDon"].ToString() + " and STT=" + dtDonKH.Rows[i]["STT"].ToString());
                                if (dr != null && dr.Length > 0)
                                {
                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        BBBC enBBBC = new BBBC();
                                        enBBBC.ID = dr[j]["MaCTBC"].ToString();
                                        enBBBC.DanhBo = dr[j]["DanhBo"].ToString();
                                        enBBBC.HopDong = dr[j]["HopDong"].ToString();
                                        enBBBC.HoTen = dr[j]["HoTen"].ToString();
                                        enBBBC.DiaChi = dr[j]["DiaChi"].ToString();
                                        enBBBC.GiaBieu = dr[j]["GiaBieu"].ToString();
                                        enBBBC.DinhMuc = dr[j]["DinhMuc"].ToString();
                                        enBBBC.DinhMucHN = dr[j]["DinhMucHN"].ToString();
                                        enBBBC.Nam = dr[j]["Nam"].ToString();
                                        enBBBC.Ky = dr[j]["Ky"].ToString();
                                        enBBBC.Dot = dr[j]["Dot"].ToString();
                                        enBBBC.Quan = dr[j]["Quan"].ToString();
                                        enBBBC.Phuong = dr[j]["Phuong"].ToString();
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
                                        HinhAnh enHinhAnh = new HinhAnh();
                                        enHinhAnh.Url = "https://service.cskhtanhoa.com.vn/ThuongVu/viewFile?TableName=BamChi_ChiTiet_Hinh&IDFileName=IDBamChi_ChiTiet&IDFileContent=" + enBBBC.ID;
                                        enBBBC.lstHinhAnh.Add(enHinhAnh);
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
                    string NgayKTXM = "";
                    if (jsonContent.ContainsKey("NgayKTXM"))
                    {
                        string[] dates = jsonContent["NgayKTXM"].ToString().Split('/');
                        NgayKTXM = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        string[] MaDons = jsonContent["MaDon"].ToString().Split('.');
                        string MaKTXM = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("declare @Ma int"
                        + " select @Ma = MAX(SUBSTRING(CONVERT(nvarchar(50), MaKTXM), LEN(CONVERT(nvarchar(50), MaKTXM)) - 1, 2)) from KTXM"
                        + " select MAX(MaKTXM) from KTXM where SUBSTRING(CONVERT(nvarchar(50), MaKTXM), LEN(CONVERT(nvarchar(50), MaKTXM)) - 1, 2) = @Ma").ToString();
                        MaKTXM = getMaxNextIDTable(MaKTXM);
                        _cDAL_ThuongVu.ExecuteNonQuery("if not exists (select * from KTKS_DonKH.dbo.KTXM where MaDonMoi=" + MaDons[0] + ")"
                        + " insert into KTKS_DonKH.dbo.KTXM(MaKTXM, MaDonMoi, CreateBy, CreateDate)values(" + MaKTXM + ", " + MaDons[0] + ", " + jsonContent["IDUser"].ToString() + ", GETDATE())");
                        MaKTXM = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("select MaKTXM from KTKS_DonKH.dbo.KTXM where MaDonMoi=" + MaDons[0]).ToString();
                        string MaCTKTXM = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("declare @Ma int"
                        + " select @Ma = MAX(SUBSTRING(CONVERT(nvarchar(50), MaCTKTXM), LEN(CONVERT(nvarchar(50), MaCTKTXM)) - 1, 2)) from KTXM_ChiTiet"
                        + " select MAX(MaCTKTXM) from KTXM_ChiTiet where SUBSTRING(CONVERT(nvarchar(50), MaCTKTXM), LEN(CONVERT(nvarchar(50), MaCTKTXM)) - 1, 2) = @Ma").ToString();
                        MaCTKTXM = getMaxNextIDTable(MaCTKTXM).ToString();
                        string flag = "";
                        flag = string.IsNullOrEmpty(jsonContent["CanKhachHangLienHe"].ToString()) ? "0" : bool.Parse(jsonContent["CanKhachHangLienHe"].ToString()) ? "1" : "0";
                        BBKT enBBKT = new BBKT();
                        enBBKT.DanhBo = string.IsNullOrEmpty(jsonContent["DanhBo"].ToString()) ? "NULL" : "N'" + jsonContent["DanhBo"].ToString() + "'";
                        enBBKT.HopDong = string.IsNullOrEmpty(jsonContent["HopDong"].ToString()) ? "NULL" : "N'" + jsonContent["HopDong"].ToString() + "'";
                        enBBKT.HoTen = string.IsNullOrEmpty(jsonContent["HoTen"].ToString()) ? "NULL" : "N'" + jsonContent["HoTen"].ToString() + "'";
                        enBBKT.DiaChi = string.IsNullOrEmpty(jsonContent["DiaChi"].ToString()) ? "NULL" : "N'" + jsonContent["DiaChi"].ToString() + "'";
                        enBBKT.GiaBieu = string.IsNullOrEmpty(jsonContent["GiaBieu"].ToString()) ? "NULL" : "N'" + jsonContent["GiaBieu"].ToString() + "'";
                        enBBKT.DinhMuc = string.IsNullOrEmpty(jsonContent["DinhMuc"].ToString()) ? "NULL" : jsonContent["DinhMuc"].ToString();
                        enBBKT.DinhMucHN = string.IsNullOrEmpty(jsonContent["DinhMucHN"].ToString()) ? "NULL" : jsonContent["DinhMucHN"].ToString();
                        enBBKT.Dot = string.IsNullOrEmpty(jsonContent["Dot"].ToString()) ? "NULL" : "N'" + jsonContent["Dot"].ToString() + "'";
                        enBBKT.Ky = string.IsNullOrEmpty(jsonContent["Ky"].ToString()) ? "NULL" : "N'" + jsonContent["Ky"].ToString() + "'";
                        enBBKT.Nam = string.IsNullOrEmpty(jsonContent["Nam"].ToString()) ? "NULL" : "N'" + jsonContent["Nam"].ToString() + "'";
                        enBBKT.Phuong = string.IsNullOrEmpty(jsonContent["Phuong"].ToString()) ? "NULL" : "N'" + jsonContent["Phuong"].ToString() + "'";
                        enBBKT.Quan = string.IsNullOrEmpty(jsonContent["Quan"].ToString()) ? "NULL" : "N'" + jsonContent["Quan"].ToString() + "'";
                        enBBKT.HienTrangKiemTra = string.IsNullOrEmpty(jsonContent["HienTrangKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["HienTrangKiemTra"].ToString() + "'";
                        enBBKT.Hieu = string.IsNullOrEmpty(jsonContent["Hieu"].ToString()) ? "NULL" : "N'" + jsonContent["Hieu"].ToString() + "'";
                        enBBKT.Co = string.IsNullOrEmpty(jsonContent["Co"].ToString()) ? "NULL" : "N'" + jsonContent["Co"].ToString() + "'";
                        enBBKT.SoThan = string.IsNullOrEmpty(jsonContent["SoThan"].ToString()) ? "NULL" : "N'" + jsonContent["SoThan"].ToString() + "'";
                        enBBKT.ChiSo = string.IsNullOrEmpty(jsonContent["ChiSo"].ToString()) ? "NULL" : "N'" + jsonContent["ChiSo"].ToString() + "'";
                        enBBKT.ChiSoLucKiemTra = string.IsNullOrEmpty(jsonContent["ChiSoLucKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["ChiSoLucKiemTra"].ToString() + "'";
                        enBBKT.ChiMatSo = string.IsNullOrEmpty(jsonContent["ChiMatSo"].ToString()) ? "NULL" : "N'" + jsonContent["ChiMatSo"].ToString() + "'";
                        enBBKT.ChiKhoaGoc = string.IsNullOrEmpty(jsonContent["ChiKhoaGoc"].ToString()) ? "NULL" : "N'" + jsonContent["ChiKhoaGoc"].ToString() + "'";
                        enBBKT.MucDichSuDung = string.IsNullOrEmpty(jsonContent["MucDichSuDung"].ToString()) ? "NULL" : "N'" + jsonContent["MucDichSuDung"].ToString() + "'";
                        enBBKT.NoiDungKiemTra = string.IsNullOrEmpty(jsonContent["NoiDungKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["NoiDungKiemTra"].ToString() + "'";
                        enBBKT.DienThoai = string.IsNullOrEmpty(jsonContent["DienThoai"].ToString()) ? "NULL" : "N'" + jsonContent["DienThoai"].ToString() + "'";
                        enBBKT.HoTenKHKy = string.IsNullOrEmpty(jsonContent["HoTenKHKy"].ToString()) ? "NULL" : "N'" + jsonContent["HoTenKHKy"].ToString() + "'";
                        enBBKT.ThucHienTheoYeuCau = string.IsNullOrEmpty(jsonContent["ThucHienTheoYeuCau"].ToString()) ? "NULL" : "N'" + jsonContent["ThucHienTheoYeuCau"].ToString() + "'";
                        enBBKT.ViTriDHN1 = string.IsNullOrEmpty(jsonContent["ViTriDHN1"].ToString()) ? "NULL" : "N'" + jsonContent["ViTriDHN1"].ToString() + "'";
                        enBBKT.ViTriDHN2 = string.IsNullOrEmpty(jsonContent["ViTriDHN2"].ToString()) ? "NULL" : "N'" + jsonContent["ViTriDHN2"].ToString() + "'";
                        enBBKT.TTTB = string.IsNullOrEmpty(jsonContent["TTTB"].ToString()) ? "NULL" : jsonContent["TTTB"].ToString();
                        enBBKT.DinhMucMoi = string.IsNullOrEmpty(jsonContent["DinhMucMoi"].ToString()) ? "NULL" : jsonContent["DinhMucMoi"].ToString();
                        enBBKT.BaoThay = string.IsNullOrEmpty(jsonContent["BaoThay"].ToString()) ? "NULL" : "N'" + jsonContent["BaoThay"].ToString() + "'";
                        enBBKT.BaoThayGhiChu = string.IsNullOrEmpty(jsonContent["BaoThayGhiChu"].ToString()) ? "NULL" : "N'" + jsonContent["BaoThayGhiChu"].ToString() + "'";
                        enBBKT.DinhMucKhongDangKy = string.IsNullOrEmpty(jsonContent["DinhMucKhongDangKy"].ToString()) ? "NULL" : jsonContent["DinhMucKhongDangKy"].ToString();
                        var transactionOptions = new TransactionOptions();
                        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                        {
                            if (_cDAL_ThuongVu.ExecuteNonQuery("INSERT INTO [KTKS_DonKH].[dbo].[KTXM_ChiTiet]"
                           + " ([MaCTKTXM]"
                           + " ,[DanhBo]"
                           + " ,[HopDong]"
                           + " ,[HoTen]"
                           + " ,[DiaChi]"
                           + " ,[GiaBieu]"
                           + " ,[DinhMuc]"
                           + " ,[DinhMucHN]"
                           + " ,[Dot]"
                           + " ,[Ky]"
                           + " ,[Nam]"
                           + " ,[Quan]"
                           + " ,[Phuong]"
                           + " ,[NgayKTXM]"
                           + " ,[HienTrangKiemTra]"
                           + " ,[Hieu]"
                           + " ,[Co]"
                           + " ,[SoThan]"
                           + " ,[ChiSo]"
                           + " ,[TinhTrangChiSo]"
                           + " ,[ChiMatSo]"
                           + " ,[ChiKhoaGoc]"
                           + " ,[MucDichSuDung]"
                           + " ,[NoiDungKiemTra]"
                           + " ,[DienThoai]"
                           + " ,[HoTenKHKy]"
                           + " ,[TheoYeuCau]"
                           + " ,[ViTriDHN1]"
                           + " ,[ViTriDHN2]"
                           + " ,[TieuThuTrungBinh]"
                           + " ,[DinhMucMoi]"
                           + " ,[NoiDungBaoThay]"
                           + " ,[GhiChuNoiDungBaoThay]"
                           + " ,[CanKhachHangLienHe]"
                           + " ,[DinhMuc_KhongDangKy]"
                           + " ,[MaKTXM]"
                           + " ,[STT]"
                           + " ,[CreateDate]"
                           + " ,[CreateBy])"
                     + " VALUES"
                           + " (" + MaCTKTXM
                           + " ," + enBBKT.DanhBo
                           + " ," + enBBKT.HopDong
                           + " ," + enBBKT.HoTen
                           + " ," + enBBKT.DiaChi
                           + " ," + enBBKT.GiaBieu
                           + " ," + enBBKT.DinhMuc
                           + " ," + enBBKT.DinhMucHN
                           + " ," + enBBKT.Dot
                           + " ," + enBBKT.Ky
                           + " ," + enBBKT.Nam
                           + " ," + enBBKT.Quan
                           + " ," + enBBKT.Phuong
                           + " ,'" + NgayKTXM + "'"
                           + " ," + enBBKT.HienTrangKiemTra
                           + " ," + enBBKT.Hieu
                           + " ," + enBBKT.Co
                           + " ," + enBBKT.SoThan
                           + " ," + enBBKT.ChiSo
                           + " ," + enBBKT.ChiSoLucKiemTra
                           + " ," + enBBKT.ChiMatSo
                           + " ," + enBBKT.ChiKhoaGoc
                           + " ," + enBBKT.MucDichSuDung
                           + " ," + enBBKT.NoiDungKiemTra
                           + " ," + enBBKT.DienThoai
                           + " ," + enBBKT.HoTenKHKy
                           + " ," + enBBKT.ThucHienTheoYeuCau
                           + " ," + enBBKT.ViTriDHN1
                           + " ," + enBBKT.ViTriDHN2
                           + " ," + enBBKT.TTTB
                           + " ," + enBBKT.DinhMucMoi
                           + " ," + enBBKT.BaoThay
                           + " ," + enBBKT.BaoThayGhiChu
                           + " ," + flag
                           + " ," + enBBKT.DinhMucKhongDangKy
                           + " ," + MaKTXM
                           + " ," + MaDons[1]
                           + " ,getdate()"
                           + " ," + jsonContent["IDUser"].ToString() + ")"))
                            {
                                var lstHinhAnh = CGlobalVariable.jsSerializer.Deserialize<dynamic>(jsonContent["lstHinhAnh"].ToString());
                                List<HinhAnh> lstHinhAnhReturn = new List<HinhAnh>();
                                foreach (var item in lstHinhAnh)
                                {
                                    HinhAnh enHinhAnhReturn = new HinhAnh();
                                    enHinhAnhReturn.Name = DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
                                    byte[] hinh = System.Convert.FromBase64String(item["Value"]);
                                    if (_wsThuongVu.ghi_Hinh("KTXM_ChiTiet_Hinh", MaCTKTXM, enHinhAnhReturn.Name + item["Type"], hinh) == true)
                                    {
                                        string IDHinh = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("if exists (select top 1 * from KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh) select MAX(ID)+1 from KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh else select 1").ToString();
                                        _cDAL_ThuongVu.ExecuteNonQuery("insert into KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh(ID,IDKTXM_ChiTiet,Name,Loai,CreateBy,CreateDate)"
                                            + "values(" + IDHinh
                                            + "," + MaCTKTXM + ",'" + enHinhAnhReturn.Name + "','" + item["Type"] + "'," + jsonContent["IDUser"].ToString() + ",getdate())");
                                    }
                                    lstHinhAnhReturn.Add(enHinhAnhReturn);
                                }
                                string IDLichSu = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("if exists (select top 1 * from KTKS_DonKH.dbo.DonTu_LichSu) select MAX(ID)+1 from KTKS_DonKH.dbo.DonTu_LichSu else select 1").ToString();
                                result.success = _cDAL_ThuongVu.ExecuteNonQuery("insert into KTKS_DonKH.dbo.DonTu_LichSu(ID,NgayChuyen,ID_NoiChuyen,NoiChuyen,NoiDung,TableName,IDCT,MaDon,STT,CreateBy,CreateDate)"
                                    + "values(" + IDLichSu + ",'" + NgayKTXM + "',5,N'Kiểm Tra',N'Đã Kiểm Tra, "
                                    + jsonContent["NoiDungKiemTra"].ToString() + "','KTXM_ChiTiet'," + MaCTKTXM + "," + MaDons[0] + "," + MaDons[1] + "," + jsonContent["IDUser"].ToString() + ",getdate())");
                                scope.Complete();
                                scope.Dispose();
                            }
                            else
                                result.success = false;
                        }
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
                    string NgayKTXM = "";
                    if (jsonContent.ContainsKey("NgayKTXM"))
                    {
                        string[] dates = jsonContent["NgayKTXM"].ToString().Split('/');
                        NgayKTXM = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        string flag = "";
                        flag = string.IsNullOrEmpty(jsonContent["CanKhachHangLienHe"].ToString()) ? "0" : bool.Parse(jsonContent["CanKhachHangLienHe"].ToString()) ? "1" : "0";
                        BBKT enBBKT = new BBKT();
                        enBBKT.DanhBo = string.IsNullOrEmpty(jsonContent["DanhBo"].ToString()) ? "NULL" : "N'" + jsonContent["DanhBo"].ToString() + "'";
                        enBBKT.HopDong = string.IsNullOrEmpty(jsonContent["HopDong"].ToString()) ? "NULL" : "N'" + jsonContent["HopDong"].ToString() + "'";
                        enBBKT.HoTen = string.IsNullOrEmpty(jsonContent["HoTen"].ToString()) ? "NULL" : "N'" + jsonContent["HoTen"].ToString() + "'";
                        enBBKT.DiaChi = string.IsNullOrEmpty(jsonContent["DiaChi"].ToString()) ? "NULL" : "N'" + jsonContent["DiaChi"].ToString() + "'";
                        enBBKT.GiaBieu = string.IsNullOrEmpty(jsonContent["GiaBieu"].ToString()) ? "NULL" : "N'" + jsonContent["GiaBieu"].ToString() + "'";
                        enBBKT.DinhMuc = string.IsNullOrEmpty(jsonContent["DinhMuc"].ToString()) ? "NULL" : jsonContent["DinhMuc"].ToString();
                        enBBKT.DinhMucHN = string.IsNullOrEmpty(jsonContent["DinhMucHN"].ToString()) ? "NULL" : jsonContent["DinhMucHN"].ToString();
                        enBBKT.Dot = string.IsNullOrEmpty(jsonContent["Dot"].ToString()) ? "NULL" : "N'" + jsonContent["Dot"].ToString() + "'";
                        enBBKT.Ky = string.IsNullOrEmpty(jsonContent["Ky"].ToString()) ? "NULL" : "N'" + jsonContent["Ky"].ToString() + "'";
                        enBBKT.Nam = string.IsNullOrEmpty(jsonContent["Nam"].ToString()) ? "NULL" : "N'" + jsonContent["Nam"].ToString() + "'";
                        enBBKT.Phuong = string.IsNullOrEmpty(jsonContent["Phuong"].ToString()) ? "NULL" : "N'" + jsonContent["Phuong"].ToString() + "'";
                        enBBKT.Quan = string.IsNullOrEmpty(jsonContent["Quan"].ToString()) ? "NULL" : "N'" + jsonContent["Quan"].ToString() + "'";
                        enBBKT.HienTrangKiemTra = string.IsNullOrEmpty(jsonContent["HienTrangKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["HienTrangKiemTra"].ToString() + "'";
                        enBBKT.Hieu = string.IsNullOrEmpty(jsonContent["Hieu"].ToString()) ? "NULL" : "N'" + jsonContent["Hieu"].ToString() + "'";
                        enBBKT.Co = string.IsNullOrEmpty(jsonContent["Co"].ToString()) ? "NULL" : "N'" + jsonContent["Co"].ToString() + "'";
                        enBBKT.SoThan = string.IsNullOrEmpty(jsonContent["SoThan"].ToString()) ? "NULL" : "N'" + jsonContent["SoThan"].ToString() + "'";
                        enBBKT.ChiSo = string.IsNullOrEmpty(jsonContent["ChiSo"].ToString()) ? "NULL" : "N'" + jsonContent["ChiSo"].ToString() + "'";
                        enBBKT.ChiSoLucKiemTra = string.IsNullOrEmpty(jsonContent["ChiSoLucKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["ChiSoLucKiemTra"].ToString() + "'";
                        enBBKT.ChiMatSo = string.IsNullOrEmpty(jsonContent["ChiMatSo"].ToString()) ? "NULL" : "N'" + jsonContent["ChiMatSo"].ToString() + "'";
                        enBBKT.ChiKhoaGoc = string.IsNullOrEmpty(jsonContent["ChiKhoaGoc"].ToString()) ? "NULL" : "N'" + jsonContent["ChiKhoaGoc"].ToString() + "'";
                        enBBKT.MucDichSuDung = string.IsNullOrEmpty(jsonContent["MucDichSuDung"].ToString()) ? "NULL" : "N'" + jsonContent["MucDichSuDung"].ToString() + "'";
                        enBBKT.NoiDungKiemTra = string.IsNullOrEmpty(jsonContent["NoiDungKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["NoiDungKiemTra"].ToString() + "'";
                        enBBKT.DienThoai = string.IsNullOrEmpty(jsonContent["DienThoai"].ToString()) ? "NULL" : "N'" + jsonContent["DienThoai"].ToString() + "'";
                        enBBKT.HoTenKHKy = string.IsNullOrEmpty(jsonContent["HoTenKHKy"].ToString()) ? "NULL" : "N'" + jsonContent["HoTenKHKy"].ToString() + "'";
                        enBBKT.ThucHienTheoYeuCau = string.IsNullOrEmpty(jsonContent["ThucHienTheoYeuCau"].ToString()) ? "NULL" : "N'" + jsonContent["ThucHienTheoYeuCau"].ToString() + "'";
                        enBBKT.ViTriDHN1 = string.IsNullOrEmpty(jsonContent["ViTriDHN1"].ToString()) ? "NULL" : "N'" + jsonContent["ViTriDHN1"].ToString() + "'";
                        enBBKT.ViTriDHN2 = string.IsNullOrEmpty(jsonContent["ViTriDHN2"].ToString()) ? "NULL" : "N'" + jsonContent["ViTriDHN2"].ToString() + "'";
                        enBBKT.TTTB = string.IsNullOrEmpty(jsonContent["TTTB"].ToString()) ? "NULL" : jsonContent["TTTB"].ToString();
                        enBBKT.DinhMucMoi = string.IsNullOrEmpty(jsonContent["DinhMucMoi"].ToString()) ? "NULL" : jsonContent["DinhMucMoi"].ToString();
                        enBBKT.BaoThay = string.IsNullOrEmpty(jsonContent["BaoThay"].ToString()) ? "NULL" : "N'" + jsonContent["BaoThay"].ToString() + "'";
                        enBBKT.BaoThayGhiChu = string.IsNullOrEmpty(jsonContent["BaoThayGhiChu"].ToString()) ? "NULL" : "N'" + jsonContent["BaoThayGhiChu"].ToString() + "'";
                        enBBKT.DinhMucKhongDangKy = string.IsNullOrEmpty(jsonContent["DinhMucKhongDangKy"].ToString()) ? "NULL" : jsonContent["DinhMucKhongDangKy"].ToString();
                        _cDAL_ThuongVu.ExecuteNonQuery("UPDATE KTKS_DonKH.dbo.KTXM_ChiTiet SET"
                            + " [DanhBo] = " + enBBKT.DanhBo
                            + " ,[HopDong] = " + enBBKT.HopDong
                            + " ,[HoTen] = " + enBBKT.HoTen
                            + " ,[DiaChi] = " + enBBKT.DiaChi
                            + " ,[GiaBieu] = " + enBBKT.GiaBieu
                            + " ,[DinhMuc] = " + enBBKT.DinhMuc
                            + " ,[DinhMucHN] = " + enBBKT.DinhMucHN
                            + " ,[Dot] = " + enBBKT.Dot
                            + " ,[Ky] = " + enBBKT.Ky
                            + " ,[Nam] = " + enBBKT.Nam
                            + " ,[Quan] = " + enBBKT.Quan
                            + " ,[Phuong] = " + enBBKT.Phuong
                            + " ,[NgayKTXM] = '" + NgayKTXM + "'"
                            + " ,[HienTrangKiemTra] = " + enBBKT.HienTrangKiemTra
                            + " ,[Hieu] = " + enBBKT.Hieu
                            + " ,[Co] = " + enBBKT.Co
                            + " ,[SoThan] = " + enBBKT.SoThan
                            + " ,[ChiSo] = " + enBBKT.ChiSo
                            + " ,[TinhTrangChiSo] = " + enBBKT.ChiSoLucKiemTra
                            + " ,[ChiMatSo] = " + enBBKT.ChiMatSo
                            + " ,[ChiKhoaGoc] = " + enBBKT.ChiKhoaGoc
                            + " ,[MucDichSuDung] = " + enBBKT.MucDichSuDung
                            + " ,[NoiDungKiemTra] = " + enBBKT.NoiDungKiemTra
                            + " ,[DienThoai] = " + enBBKT.DienThoai
                            + " ,[HoTenKHKy] = " + enBBKT.HoTenKHKy
                            + " ,[TheoYeuCau] = " + enBBKT.ThucHienTheoYeuCau
                            + " ,[ViTriDHN1] = " + enBBKT.ViTriDHN1
                            + " ,[ViTriDHN2] = " + enBBKT.ViTriDHN2
                            + " ,[TieuThuTrungBinh] = " + enBBKT.TTTB
                            + " ,[DinhMucMoi] = " + enBBKT.DinhMucMoi
                            + " ,[NoiDungBaoThay] = " + enBBKT.BaoThay
                            + " ,[GhiChuNoiDungBaoThay] = " + enBBKT.BaoThayGhiChu
                            + " ,[CanKhachHangLienHe] = " + flag
                            + " ,[DinhMuc_KhongDangKy] = " + enBBKT.DinhMucKhongDangKy
                            + " ,[ModifyDate] = getdate()"
                            + " ,[ModifyBy] = " + jsonContent["IDUser"].ToString()
                            + " WHERE MaCTKTXM=" + jsonContent["ID"].ToString());
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

        [Route("donkh_deleteBBKT")]
        [HttpPost]
        public MResult donkh_deleteBBKT()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        var transactionOptions = new TransactionOptions();
                        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                        {
                            _cDAL_ThuongVu.ExecuteNonQuery("delete KTKS_DonKH.dbo.DonTu_LichSu where TableName='KTXM_ChiTiet' and IDCT=" + jsonContent["ID"].ToString() + " and CreateBy=" + jsonContent["IDUser"].ToString());
                            if (_cDAL_ThuongVu.ExecuteNonQuery("delete KTKS_DonKH.dbo.KTXM_ChiTiet WHERE MaCTKTXM=" + jsonContent["ID"].ToString() + " and CreateBy=" + jsonContent["IDUser"].ToString()))
                            {
                                _wsThuongVu.xoa_Folder_Hinh("KTXM_ChiTiet_Hinh", jsonContent["ID"].ToString());
                                scope.Complete();
                                scope.Dispose();
                                result.success = true;
                            }
                        }
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
                    string NgayBC = "";
                    if (jsonContent.ContainsKey("NgayBC"))
                    {
                        string[] dates = jsonContent["NgayBC"].ToString().Split('/');
                        NgayBC = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        string[] MaDons = jsonContent["MaDon"].ToString().Split('.');
                        string MaBC = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("declare @Ma int"
                        + " select @Ma = MAX(SUBSTRING(CONVERT(nvarchar(50), MaBC), LEN(CONVERT(nvarchar(50), MaBC)) - 1, 2)) from BamChi"
                        + " select MAX(MaBC) from BamChi where SUBSTRING(CONVERT(nvarchar(50), MaBC), LEN(CONVERT(nvarchar(50), MaBC)) - 1, 2) = @Ma").ToString();
                        MaBC = getMaxNextIDTable(MaBC);
                        _cDAL_ThuongVu.ExecuteNonQuery("if not exists (select * from KTKS_DonKH.dbo.BamChi where MaDonMoi=" + MaDons[0] + ")"
                        + " insert into KTKS_DonKH.dbo.BamChi(MaBC, MaDonMoi, CreateBy, CreateDate)values(" + MaBC + ", " + MaDons[0] + ", " + jsonContent["IDUser"].ToString() + ", GETDATE())");
                        MaBC = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("select MaBC from KTKS_DonKH.dbo.BamChi where MaDonMoi=" + MaDons[0] + "").ToString();
                        string MaCTBC = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("declare @Ma int"
                        + " select @Ma = MAX(SUBSTRING(CONVERT(nvarchar(50), MaCTBC), LEN(CONVERT(nvarchar(50), MaCTBC)) - 1, 2)) from BamChi_ChiTiet"
                        + " select MAX(MaCTBC) from BamChi_ChiTiet where SUBSTRING(CONVERT(nvarchar(50), MaCTBC), LEN(CONVERT(nvarchar(50), MaCTBC)) - 1, 2) = @Ma").ToString();
                        MaCTBC = getMaxNextIDTable(MaCTBC);
                        BBBC enBBBC = new BBBC();
                        enBBBC.DanhBo = string.IsNullOrEmpty(jsonContent["DanhBo"].ToString()) ? "NULL" : "N'" + jsonContent["DanhBo"].ToString() + "'";
                        enBBBC.HopDong = string.IsNullOrEmpty(jsonContent["HopDong"].ToString()) ? "NULL" : "N'" + jsonContent["HopDong"].ToString() + "'";
                        enBBBC.HoTen = string.IsNullOrEmpty(jsonContent["HoTen"].ToString()) ? "NULL" : "N'" + jsonContent["HoTen"].ToString() + "'";
                        enBBBC.DiaChi = string.IsNullOrEmpty(jsonContent["DiaChi"].ToString()) ? "NULL" : "N'" + jsonContent["DiaChi"].ToString() + "'";
                        enBBBC.GiaBieu = string.IsNullOrEmpty(jsonContent["GiaBieu"].ToString()) ? "NULL" : jsonContent["GiaBieu"].ToString();
                        enBBBC.DinhMuc = string.IsNullOrEmpty(jsonContent["DinhMuc"].ToString()) ? "NULL" : jsonContent["DinhMuc"].ToString();
                        enBBBC.DinhMucHN = string.IsNullOrEmpty(jsonContent["DinhMucHN"].ToString()) ? "NULL" : jsonContent["DinhMucHN"].ToString();
                        enBBBC.Dot = string.IsNullOrEmpty(jsonContent["Dot"].ToString()) ? "NULL" : "N'" + jsonContent["Dot"].ToString() + "'";
                        enBBBC.Ky = string.IsNullOrEmpty(jsonContent["Ky"].ToString()) ? "NULL" : "N'" + jsonContent["Ky"].ToString() + "'";
                        enBBBC.Nam = string.IsNullOrEmpty(jsonContent["Nam"].ToString()) ? "NULL" : "N'" + jsonContent["Nam"].ToString() + "'";
                        enBBBC.Phuong = string.IsNullOrEmpty(jsonContent["Phuong"].ToString()) ? "NULL" : "N'" + jsonContent["Phuong"].ToString() + "'";
                        enBBBC.Quan = string.IsNullOrEmpty(jsonContent["Quan"].ToString()) ? "NULL" : "N'" + jsonContent["Quan"].ToString() + "'";
                        enBBBC.Hieu = string.IsNullOrEmpty(jsonContent["Hieu"].ToString()) ? "NULL" : "N'" + jsonContent["Hieu"].ToString() + "'";
                        enBBBC.Co = string.IsNullOrEmpty(jsonContent["Co"].ToString()) ? "NULL" : jsonContent["Co"].ToString();
                        enBBBC.SoThan = string.IsNullOrEmpty(jsonContent["SoThan"].ToString()) ? "NULL" : "N'" + jsonContent["SoThan"].ToString() + "'";
                        enBBBC.ChiSo = string.IsNullOrEmpty(jsonContent["ChiSo"].ToString()) ? "NULL" : jsonContent["ChiSo"].ToString();
                        enBBBC.NiemChi = string.IsNullOrEmpty(jsonContent["NiemChi"].ToString()) ? "NULL" : "N'" + jsonContent["NiemChi"].ToString() + "'";
                        enBBBC.MauSac = string.IsNullOrEmpty(jsonContent["MauSac"].ToString()) ? "NULL" : "N'" + jsonContent["MauSac"].ToString() + "'";
                        enBBBC.ChiSoLucKiemTra = string.IsNullOrEmpty(jsonContent["ChiSoLucKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["ChiSoLucKiemTra"].ToString() + "'";
                        enBBBC.ChiMatSo = string.IsNullOrEmpty(jsonContent["ChiMatSo"].ToString()) ? "NULL" : "N'" + jsonContent["ChiMatSo"].ToString() + "'";
                        enBBBC.ChiKhoaGoc = string.IsNullOrEmpty(jsonContent["ChiKhoaGoc"].ToString()) ? "NULL" : "N'" + jsonContent["ChiKhoaGoc"].ToString() + "'";
                        enBBBC.MucDichSuDung = string.IsNullOrEmpty(jsonContent["MucDichSuDung"].ToString()) ? "NULL" : "N'" + jsonContent["MucDichSuDung"].ToString() + "'";
                        enBBBC.TrangThaiBamChi = string.IsNullOrEmpty(jsonContent["TrangThaiBamChi"].ToString()) ? "NULL" : "N'" + jsonContent["TrangThaiBamChi"].ToString() + "'";
                        enBBBC.MaKim = string.IsNullOrEmpty(jsonContent["MaKim"].ToString()) ? "NULL" : "N'" + jsonContent["MaKim"].ToString() + "'";
                        enBBBC.VienChi = string.IsNullOrEmpty(jsonContent["VienChi"].ToString()) ? "NULL" : jsonContent["VienChi"].ToString();
                        enBBBC.DayChi = string.IsNullOrEmpty(jsonContent["DayChi"].ToString()) ? "NULL" : jsonContent["DayChi"].ToString();
                        enBBBC.ThucHienTheoYeuCau = string.IsNullOrEmpty(jsonContent["ThucHienTheoYeuCau"].ToString()) ? "NULL" : "N'" + jsonContent["ThucHienTheoYeuCau"].ToString() + "'";
                        enBBBC.GhiChu = string.IsNullOrEmpty(jsonContent["GhiChu"].ToString()) ? "NULL" : "N'" + jsonContent["GhiChu"].ToString() + "'";
                        var transactionOptions = new TransactionOptions();
                        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                        {
                            if (_cDAL_ThuongVu.ExecuteNonQuery("INSERT INTO [KTKS_DonKH].[dbo].[BamChi_ChiTiet]"
                             + " ([MaCTBC]"
                             + " ,[DanhBo]"
                             + " ,[HopDong]"
                             + " ,[HoTen]"
                             + " ,[DiaChi]"
                             + " ,[GiaBieu]"
                             + " ,[DinhMuc]"
                             + " ,[DinhMucHN]"
                             + " ,[Dot]"
                             + " ,[Ky]"
                             + " ,[Nam]"
                             + " ,[Quan]"
                             + " ,[Phuong]"
                             + " ,[NgayBC]"
                             + " ,[Hieu]"
                             + " ,[Co]"
                             + " ,[SoThan]"
                             + " ,[ChiSo]"
                             + " ,[NiemChi]"
                             + " ,[MauSac]"
                             + " ,[TinhTrangChiSo]"
                             + " ,[ChiMatSo]"
                             + " ,[ChiKhoaGoc]"
                             + " ,[MucDichSuDung]"
                             + " ,[TrangThaiBC]"
                             + " ,[MaSoBC]"
                             + " ,[VienChi]"
                             + " ,[DayChi]"
                             + " ,[TheoYeuCau]"
                             + " ,[GhiChu]"
                             + " ,[MaBC]"
                             + " ,[STT]"
                             + " ,[CreateDate]"
                             + " ,[CreateBy])"
                       + " VALUES"
                             + " (" + MaCTBC
                             + " ," + enBBBC.DanhBo
                             + " ," + enBBBC.HopDong
                             + " ," + enBBBC.HoTen
                             + " ," + enBBBC.DiaChi
                             + " ," + enBBBC.GiaBieu
                             + " ," + enBBBC.DinhMuc
                             + " ," + enBBBC.DinhMucHN
                             + " ," + enBBBC.Dot
                             + " ," + enBBBC.Ky
                             + " ," + enBBBC.Nam
                             + " ," + enBBBC.Quan
                             + " ," + enBBBC.Phuong
                             + " ,'" + NgayBC + "'"
                             + " ," + enBBBC.Hieu
                             + " ," + enBBBC.Co
                             + " ," + enBBBC.SoThan
                             + " ," + enBBBC.ChiSo
                             + " ," + enBBBC.NiemChi
                             + " ," + enBBBC.MauSac
                             + " ," + enBBBC.ChiSoLucKiemTra
                             + " ," + enBBBC.ChiMatSo
                             + " ," + enBBBC.ChiKhoaGoc
                             + " ," + enBBBC.MucDichSuDung
                             + " ," + enBBBC.TrangThaiBamChi
                             + " ," + enBBBC.MaKim
                             + " ," + enBBBC.VienChi
                             + " ," + enBBBC.DayChi
                             + " ," + enBBBC.ThucHienTheoYeuCau
                             + " ," + enBBBC.GhiChu
                             + " ," + MaBC
                             + " ," + MaDons[1]
                             + " ,getdate()"
                             + " ," + jsonContent["IDUser"].ToString() + ")"))
                            {
                                var lstHinhAnh = CGlobalVariable.jsSerializer.Deserialize<dynamic>(jsonContent["lstHinhAnh"].ToString());
                                List<HinhAnh> lstHinhAnhReturn = new List<HinhAnh>();
                                foreach (var item in lstHinhAnh)
                                {
                                    HinhAnh enHinhAnhReturn = new HinhAnh();
                                    enHinhAnhReturn.Name = DateTime.Now.ToString("dd.MM.yyydy HH.mm.ss");
                                    byte[] hinh = System.Convert.FromBase64String(item["Value"]);
                                    if (_wsThuongVu.ghi_Hinh("BamChi_ChiTiet_Hinh", MaCTBC, enHinhAnhReturn.Name + item["Type"], hinh) == true)
                                    {
                                        string IDHinh = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("if exists (select top 1 * from KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh) select MAX(ID)+1 from KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh else select 1").ToString();
                                        _cDAL_ThuongVu.ExecuteNonQuery("insert into KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh(ID,IDBamChi_ChiTiet,Name,Loai,CreateBy,CreateDate)"
                                            + "values(" + IDHinh
                                            + "," + MaCTBC + ",'" + enHinhAnhReturn.Name + "','" + item["Type"] + "'," + jsonContent["IDUser"].ToString() + ",getdate())");
                                    }
                                    lstHinhAnhReturn.Add(enHinhAnhReturn);
                                }
                                string IDLichSu = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("if exists (select top 1 * from KTKS_DonKH.dbo.DonTu_LichSu) select MAX(ID)+1 from KTKS_DonKH.dbo.DonTu_LichSu else select 1").ToString();
                                result.success = _cDAL_ThuongVu.ExecuteNonQuery("insert into KTKS_DonKH.dbo.DonTu_LichSu(ID,NgayChuyen,ID_NoiChuyen,NoiChuyen,NoiDung,TableName,IDCT,MaDon,STT,CreateBy,CreateDate)"
                                                            + "values(" + IDLichSu + ",'" + NgayBC + "',5,N'Kiểm Tra',N'Đã Bấm Chì, "
                                                            + jsonContent["TrangThaiBamChi"].ToString() + "','BamChi_ChiTiet'," + MaCTBC + "," + MaDons[0] + "," + MaDons[1] + "," + jsonContent["IDUser"].ToString() + ",getdate())");
                                scope.Complete();
                                scope.Dispose();
                            }
                            else
                                result.success = false;
                        }
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
                    string NgayBC = "";
                    if (jsonContent.ContainsKey("NgayBC"))
                    {
                        string[] dates = jsonContent["NgayBC"].ToString().Split('/');
                        NgayBC = dates[2] + dates[1] + dates[0];
                    }
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        BBBC enBBBC = new BBBC();
                        enBBBC.DanhBo = string.IsNullOrEmpty(jsonContent["DanhBo"].ToString()) ? "NULL" : "N'" + jsonContent["DanhBo"].ToString() + "'";
                        enBBBC.HopDong = string.IsNullOrEmpty(jsonContent["HopDong"].ToString()) ? "NULL" : "N'" + jsonContent["HopDong"].ToString() + "'";
                        enBBBC.HoTen = string.IsNullOrEmpty(jsonContent["HoTen"].ToString()) ? "NULL" : "N'" + jsonContent["HoTen"].ToString() + "'";
                        enBBBC.DiaChi = string.IsNullOrEmpty(jsonContent["DiaChi"].ToString()) ? "NULL" : "N'" + jsonContent["DiaChi"].ToString() + "'";
                        enBBBC.GiaBieu = string.IsNullOrEmpty(jsonContent["GiaBieu"].ToString()) ? "NULL" : jsonContent["GiaBieu"].ToString();
                        enBBBC.DinhMuc = string.IsNullOrEmpty(jsonContent["DinhMuc"].ToString()) ? "NULL" : jsonContent["DinhMuc"].ToString();
                        enBBBC.DinhMucHN = string.IsNullOrEmpty(jsonContent["DinhMucHN"].ToString()) ? "NULL" : jsonContent["DinhMucHN"].ToString();
                        enBBBC.Dot = string.IsNullOrEmpty(jsonContent["Dot"].ToString()) ? "NULL" : "N'" + jsonContent["Dot"].ToString() + "'";
                        enBBBC.Ky = string.IsNullOrEmpty(jsonContent["Ky"].ToString()) ? "NULL" : "N'" + jsonContent["Ky"].ToString() + "'";
                        enBBBC.Nam = string.IsNullOrEmpty(jsonContent["Nam"].ToString()) ? "NULL" : "N'" + jsonContent["Nam"].ToString() + "'";
                        enBBBC.Phuong = string.IsNullOrEmpty(jsonContent["Phuong"].ToString()) ? "NULL" : "N'" + jsonContent["Phuong"].ToString() + "'";
                        enBBBC.Quan = string.IsNullOrEmpty(jsonContent["Quan"].ToString()) ? "NULL" : "N'" + jsonContent["Quan"].ToString() + "'";
                        enBBBC.Hieu = string.IsNullOrEmpty(jsonContent["Hieu"].ToString()) ? "NULL" : "N'" + jsonContent["Hieu"].ToString() + "'";
                        enBBBC.Co = string.IsNullOrEmpty(jsonContent["Co"].ToString()) ? "NULL" : jsonContent["Co"].ToString();
                        enBBBC.SoThan = string.IsNullOrEmpty(jsonContent["SoThan"].ToString()) ? "NULL" : "N'" + jsonContent["SoThan"].ToString() + "'";
                        enBBBC.ChiSo = string.IsNullOrEmpty(jsonContent["ChiSo"].ToString()) ? "NULL" : jsonContent["ChiSo"].ToString();
                        enBBBC.NiemChi = string.IsNullOrEmpty(jsonContent["NiemChi"].ToString()) ? "NULL" : "N'" + jsonContent["NiemChi"].ToString() + "'";
                        enBBBC.MauSac = string.IsNullOrEmpty(jsonContent["MauSac"].ToString()) ? "NULL" : "N'" + jsonContent["MauSac"].ToString() + "'";
                        enBBBC.ChiSoLucKiemTra = string.IsNullOrEmpty(jsonContent["ChiSoLucKiemTra"].ToString()) ? "NULL" : "N'" + jsonContent["ChiSoLucKiemTra"].ToString() + "'";
                        enBBBC.ChiMatSo = string.IsNullOrEmpty(jsonContent["ChiMatSo"].ToString()) ? "NULL" : "N'" + jsonContent["ChiMatSo"].ToString() + "'";
                        enBBBC.ChiKhoaGoc = string.IsNullOrEmpty(jsonContent["ChiKhoaGoc"].ToString()) ? "NULL" : "N'" + jsonContent["ChiKhoaGoc"].ToString() + "'";
                        enBBBC.MucDichSuDung = string.IsNullOrEmpty(jsonContent["MucDichSuDung"].ToString()) ? "NULL" : "N'" + jsonContent["MucDichSuDung"].ToString() + "'";
                        enBBBC.TrangThaiBamChi = string.IsNullOrEmpty(jsonContent["TrangThaiBamChi"].ToString()) ? "NULL" : "N'" + jsonContent["TrangThaiBamChi"].ToString() + "'";
                        enBBBC.MaKim = string.IsNullOrEmpty(jsonContent["MaKim"].ToString()) ? "NULL" : "N'" + jsonContent["MaKim"].ToString() + "'";
                        enBBBC.VienChi = string.IsNullOrEmpty(jsonContent["VienChi"].ToString()) ? "NULL" : jsonContent["VienChi"].ToString();
                        enBBBC.DayChi = string.IsNullOrEmpty(jsonContent["DayChi"].ToString()) ? "NULL" : jsonContent["DayChi"].ToString();
                        enBBBC.ThucHienTheoYeuCau = string.IsNullOrEmpty(jsonContent["ThucHienTheoYeuCau"].ToString()) ? "NULL" : "N'" + jsonContent["ThucHienTheoYeuCau"].ToString() + "'";
                        enBBBC.GhiChu = string.IsNullOrEmpty(jsonContent["GhiChu"].ToString()) ? "NULL" : "N'" + jsonContent["GhiChu"].ToString() + "'";
                        _cDAL_ThuongVu.ExecuteNonQuery("UPDATE KTKS_DonKH.dbo.BamChi_ChiTiet SET"
                          + " [DanhBo] = " + enBBBC.DanhBo
                          + " ,[HopDong] = " + enBBBC.HopDong
                          + " ,[HoTen] = " + enBBBC.HoTen
                          + " ,[DiaChi] =" + enBBBC.DiaChi
                          + " ,[GiaBieu] = " + enBBBC.GiaBieu
                          + " ,[DinhMuc] = " + enBBBC.DinhMuc
                          + " ,[DinhMucHN] = " + enBBBC.DinhMucHN
                          + " ,[Dot] = " + enBBBC.Dot
                          + " ,[Ky] = " + enBBBC.Ky
                          + " ,[Nam] = " + enBBBC.Nam
                          + " ,[Quan] = " + enBBBC.Quan
                          + " ,[Phuong] = " + enBBBC.Phuong
                          + " ,[NgayBC] = '" + NgayBC + "'"
                          + " ,[Hieu] = " + enBBBC.Hieu
                          + " ,[Co] = " + enBBBC.Co
                          + " ,[SoThan] = " + enBBBC.SoThan
                          + " ,[ChiSo] = " + enBBBC.ChiSo
                          + " ,[NiemChi] =" + enBBBC.NiemChi
                          + " ,[MauSac] = " + enBBBC.MauSac
                          + " ,[TinhTrangChiSo] =" + enBBBC.ChiSoLucKiemTra
                          + " ,[ChiMatSo] = " + enBBBC.ChiMatSo
                          + " ,[ChiKhoaGoc] =" + enBBBC.ChiKhoaGoc
                          + " ,[MucDichSuDung] = " + enBBBC.MucDichSuDung
                          + " ,[VienChi] = " + enBBBC.VienChi
                          + " ,[DayChi] = " + enBBBC.DayChi
                          + " ,[TrangThaiBC] =  " + enBBBC.TrangThaiBamChi
                          + " ,[GhiChu] =  " + enBBBC.GhiChu
                          + " ,[MaSoBC] =  " + enBBBC.MaKim
                          + " ,[TheoYeuCau] = " + enBBBC.ThucHienTheoYeuCau
                          + " ,[ModifyDate] = getdate()"
                          + " ,[ModifyBy] = " + jsonContent["IDUser"].ToString()
                          + " WHERE MaCTBC=" + jsonContent["ID"].ToString());
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

        [Route("donkh_deleteBBBC")]
        [HttpPost]
        public MResult donkh_deleteBBBC()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        var transactionOptions = new TransactionOptions();
                        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                        {
                            _cDAL_ThuongVu.ExecuteNonQuery("delete KTKS_DonKH.dbo.DonTu_LichSu where TableName='BamChi_ChiTiet' and IDCT=" + jsonContent["ID"].ToString() + " and CreateBy=" + jsonContent["IDUser"].ToString());
                            _cDAL_ThuongVu.ExecuteNonQuery("delete KTKS_DonKH.dbo.BamChi_ChiTiet WHERE MaCTBC=" + jsonContent["ID"].ToString() + " and CreateBy=" + jsonContent["IDUser"].ToString());
                            _wsThuongVu.xoa_Folder_Hinh("BamChi_ChiTiet_Hinh", jsonContent["ID"].ToString());
                            scope.Complete();
                            scope.Dispose();
                            result.success = true;
                        }
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

        [Route("donkh_insertHinhAnh")]
        [HttpPost]
        public MResult donkh_insertHinhAnh()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        var transactionOptions = new TransactionOptions();
                        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                        {
                            HinhAnh enHinhAnhReturn = new HinhAnh();
                            enHinhAnhReturn.Name = DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss");
                            byte[] hinh = System.Convert.FromBase64String(jsonContent["Value"].ToString());
                            if (jsonContent["Loai"].ToString() == "BBKT")
                            {
                                if (_wsThuongVu.ghi_Hinh("KTXM_ChiTiet_Hinh", jsonContent["ID"].ToString(), enHinhAnhReturn.Name + jsonContent["Type"].ToString(), hinh) == true)
                                {
                                    string IDHinh = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("if exists (select top 1 * from KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh) select MAX(ID)+1 from KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh else select 1").ToString();
                                    _cDAL_ThuongVu.ExecuteNonQuery("insert into KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh(ID,IDKTXM_ChiTiet,Name,Loai,CreateBy,CreateDate)"
                                        + "values((if exists (select top 1 * from KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh) select MAX(ID)+1 from KTKS_DonKH.dbo.KTXM_ChiTiet_Hinh else select 1)"
                                        + "," + jsonContent["ID"].ToString() + ",'" + enHinhAnhReturn.Name + "','" + jsonContent["Type"].ToString() + "'," + jsonContent["IDUser"].ToString() + ",getdate())");
                                }
                            }
                            else
                                if (jsonContent["Loai"].ToString() == "BBBC")
                            {
                                if (_wsThuongVu.ghi_Hinh("BamChi_ChiTiet_Hinh", jsonContent["ID"].ToString(), enHinhAnhReturn.Name + jsonContent["Type"].ToString(), hinh) == true)
                                {
                                    string IDHinh = _cDAL_ThuongVu.ExecuteQuery_ReturnOneValue("if exists (select top 1 * from KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh) select MAX(ID)+1 from KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh else select 1").ToString();
                                    _cDAL_ThuongVu.ExecuteNonQuery("insert into KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh(ID,IDBamChi_ChiTiet,Name,Loai,CreateBy,CreateDate)"
                                        + "values((if exists (select top 1 * from KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh) select MAX(ID)+1 from KTKS_DonKH.dbo.BamChi_ChiTiet_Hinh else select 1)"
                                        + "," + jsonContent["ID"].ToString() + ",'" + enHinhAnhReturn.Name + "','" + jsonContent["Type"].ToString() + "'," + jsonContent["IDUser"].ToString() + ",getdate())");
                                }
                            }
                            scope.Complete();
                            scope.Dispose();
                            result.success = true;
                        }
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

        [Route("donkh_deletetHinhAnh")]
        [HttpPost]
        public MResult donkh_deletetHinhAnh()
        {
            MResult result = new MResult();
            try
            {
                string jsonResult = Request.Content.ReadAsStringAsync().Result;
                if (jsonResult != null)
                {
                    var jsonContent = JObject.Parse(jsonResult);
                    if (jsonContent["checksum"].ToString() == CGlobalVariable.salaPass)
                    {
                        var transactionOptions = new TransactionOptions();
                        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                        {
                            string TableName = "";
                            if (jsonContent["Loai"].ToString() == "BBKT")
                            {
                                TableName = "KTXM_ChiTiet_Hinh";
                            }
                            else
                                if (jsonContent["Loai"].ToString() == "BBBC")
                            {
                                TableName = "BamChi_ChiTiet_Hinh";
                            }
                            DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select filename=Name+Loai from " + TableName + " where Huy=0 and ID=" + jsonContent["Name"].ToString());
                            _wsThuongVu.xoa_Hinh(TableName, jsonContent["ID"].ToString(), dt.Rows[0]["filename"].ToString());
                            _cDAL_ThuongVu.ExecuteNonQuery("delete " + TableName + " where ID=" + jsonContent["Name"].ToString());
                            scope.Complete();
                            scope.Dispose();
                            result.success = true;
                        }
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

        private string getMaxNextIDTable(string id)
        {
            string nam = id.Substring(id.ToString().Length - 2, 2);
            string stt = id.Substring(0, id.ToString().Length - 2);
            if (decimal.Parse(nam) == decimal.Parse(DateTime.Now.ToString("yy")))
            {
                stt = (decimal.Parse(stt) + 1).ToString();
            }
            else
            {
                stt = "1";
                nam = DateTime.Now.ToString("yy");
            }
            return (stt + nam);
        }
    }
}