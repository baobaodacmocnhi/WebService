using System;
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
        [HttpGet]
        public MResult donkh_login(string IDUser, string checksum)
        {
            MResult en = new MResult();
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
            en.success = true;
            en.data = CGlobalVariable.jsSerializer.Serialize(data);
            return en;
        }

        [Route("donkh_getDonKH")]
        [HttpGet]
        public MResult donkh_getDonKH(string IDUser, string checksum)
        {
            MResult en = new MResult();
            DataTable dt = _cDAL_ThuongVu.ExecuteQuery_DataTable("select dtct.DanhBo,dtct.MLT,dtct.HopDong,dtct.HoTen,dtct.DiaChi,dtct.GiaBieu,dtct.DinhMuc,DinhMucHN,dt.Name_NhomDon_PKH,dtct.CreateDate,ls.NgayChuyen"
  + " from KTKS_DonKH.dbo.DonTu_LichSu ls, KTKS_DonKH.dbo.DonTu dt, KTKS_DonKH.dbo.DonTu_ChiTiet dtct"
  + " where ID_KTXM = 5 and CAST(NgayChuyen as date) >= '20231201' and CAST(NgayChuyen as date) <= '20231210'"
  + " and ls.MaDon = dt.MaDon and dt.MaDon = dtct.MaDon and ls.STT = dtct.STT");
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
            en.success = true;
            en.data = CGlobalVariable.jsSerializer.Serialize(data);
            return en;
        }
    }
}