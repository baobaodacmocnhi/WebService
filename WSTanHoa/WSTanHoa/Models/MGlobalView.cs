using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSTanHoa.Models
{
    public class MView
    {
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string SoLuong { get; set; }
        public string ThoiGian { get; set; }
        public string DanhBo { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string SoNha { get; set; }
        public string TenDuong { get; set; }
        public string ChiSo { get; set; }
        public string TieuThu { get; set; }
        public List<MView> lst { get; set; }
        public MView()
        {
            TieuDe = NoiDung = SoLuong = ThoiGian = DanhBo = HoTen = DiaChi = SoNha = TenDuong = ChiSo = TieuThu = "";
            lst = new List<MView>();
        }
    }

    public class MHoaDon
    {
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public int? MaHD { get; set; }
        public string SoHoaDon { get; set; }
        public string DanhBo { get; set; }
        public int? Nam { get; set; }
        public int? Ky { get; set; }
        public int TieuThu { get; set; }
        public int GiaBan { get; set; }
        public int ThueGTGT { get; set; }
        public int PhiBVMT { get; set; }
        public int PhiBVMT_Thue { get; set; }
        public int TongCong { get; set; }
        public int PhiMoNuoc { get; set; }
        public int TienDu { get; set; }
        //public DateTime? NgayGiaiTrach { get; set; }
        //public string KyHD { get; set; }
        public int CSC { get; set; }
        public int TieuThuMoi { get; set; }
        public string CodeMoi { get; set; }
        public int ChiSoMoi { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public MHoaDon()
        {
            HoTen = "";
            DiaChi = "";
            MaHD = null;
            SoHoaDon = "";
            DanhBo = "";
            Nam = null;
            Ky = null;
            GiaBan = 0;
            ThueGTGT = 0;
            PhiBVMT = 0;
            PhiBVMT_Thue = 0;
            TongCong = 0;
            PhiMoNuoc = 0;
            TienDu = 0;
            //NgayGiaiTrach = null;
            //KyHD = "";
            TieuThu = CSC = TieuThuMoi = ChiSoMoi = -1;
            CodeMoi = "";
        }
    }

    public class MDonViThiCong
    {
        [Display(Name = "STT")]
        public int STT { get; set; }

        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Đơn vị thi công")]
        public string Name { get; set; }

        [Display(Name = "Người đại diện")]
        public string DaiDien { get; set; }

        [Display(Name = "Điện thoại")]
        public string DienThoai { get; set; }

        [Display(Name = "Tài khoản")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Người tạo")]
        public string CreateBy { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Người sửa")]
        public string ModifyBy { get; set; }

        [Display(Name = "Ngày sửa")]
        public DateTime? ModifyDate { get; set; }
        public MDonViThiCong()
        {
            STT = ID = 0;
            Name = DaiDien = DienThoai = Username = Password = CreateBy = ModifyBy = "";
            ModifyDate = null;
        }
    }

    public class MThiCong
    {
        [Display(Name = "STT")]
        public int STT { get; set; }

        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Dự án/Điểm bể")]
        public string Name { get; set; }

        [Display(Name = "Đơn vị thi công")]
        public int IDDonViThiCong { get; set; }

        [Display(Name = "Đơn vị thi công")]
        public string DonViThiCong { get; set; }

        [Display(Name = "Kết cấu")]
        public int IDKetCau { get; set; }

        [Display(Name = "Kết cấu")]
        public string KetCau { get; set; }

        [Display(Name = "Danh bộ")]
        public string DanhBo { get; set; }

        [Display(Name = "Điểm đầu")]
        public string DiemDau { get; set; }

        [Display(Name = "Điểm cuối")]
        public string DiemCuoi { get; set; }

        [Display(Name = "Tên đường")]
        public string TenDuong { get; set; }

        [Display(Name = "Phường")]
        public string IDPhuong { get; set; }

        [Display(Name = "Quận")]
        public string IDQuan { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }

        [Display(Name = "Người tạo")]
        public string CreateBy { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Người sửa")]
        public string ModifyBy { get; set; }

        [Display(Name = "Ngày sửa")]
        public DateTime? ModifyDate { get; set; }

        public List<MThiCong_LichSu> lstLichSu { get; set; }

        public MThiCong()
        {
            STT = ID = 0;
            Name = DanhBo = DiemDau = DiemCuoi = TenDuong = CreateBy = ModifyBy = "";
            NgayBatDau = ModifyDate = null;
            lstLichSu = new List<MThiCong_LichSu>();
        }
    }

    public class MThiCong_LichSu
    {
        [Display(Name = "Lần thi công")]
        public string Name { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "Ngày nghiệm thu")]
        public DateTime? NgayNghiemThu { get; set; }

        [Display(Name = "Trở ngại thi công")]
        public string TroNgaiThiCong { get; set; }

        [Display(Name = "Trở ngại nghiệm thu")]
        public string TroNgaiNghiemThu { get; set; }

        public List<MThiCong_LichSu_Hinh> lstLichSu_Hinh { get; set; }

        public MThiCong_LichSu()
        {
            Name = TroNgaiThiCong = TroNgaiNghiemThu = "";
            NgayKetThuc = NgayNghiemThu = null;
            lstLichSu_Hinh = new List<MThiCong_LichSu_Hinh>();
        }
    }

    public class MThiCong_LichSu_Hinh
    {
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "Hình")]
        public string image { get; set; }

        public MThiCong_LichSu_Hinh()
        {
            ID = image = "";
        }
    }

}