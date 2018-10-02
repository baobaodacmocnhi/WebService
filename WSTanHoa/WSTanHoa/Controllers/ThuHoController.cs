﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/ThuHo")]
    public class ThuHoController : ApiController
    {
        //CConnection _cDAL = new CConnection("Data Source=server9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa");
        CConnection _cDAL = new CConnection("Data Source=serverg8-01;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");
        class ErrorResponse
        {
            public string message { get; set; }
            public int code { get; set; }
            public ErrorResponse(string message, int code)
            {
                this.message = message;
                this.code = code;
            }
        }

        const int errorCodeSQL = -1;
        const int errorCodeKhongDung = 1000;
        const int errorCodeHetNo = 1001;
        const int errorCodeIDGiaoDichKhongTonTai = 1002;
        const int errorCodeIDGiaoDichTonTai = 1003;
        const int errorCodeGiaiTrach = 1004;
        const int errorCodePhiMoNuoc = 1005;
        const int errorCodePassword = 1010;

        /// <summary>
        /// Lấy Tất Cả Hóa Đơn Tồn
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getHoaDonTon")]
        public IList<HoaDon> getHoaDonTon(string DanhBo)
        {
            DataTable dt = new DataTable();
            int count = 0;
            //check exist
            try
            {
                count = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID_HOADON) from HOADON where DANHBA='" + DanhBo + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            if (count == 0)
            {
                ErrorResponse error = new ErrorResponse("Danh Bộ không đúng", errorCodeKhongDung);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            }
            //get hoadon tồn
            try
            {
                dt = _cDAL.ExecuteQuery_DataTable("select * from fnGetHoaDonTon(" + DanhBo + ")");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<HoaDon> hoadons = new List<HoaDon>();
                foreach (DataRow item in dt.Rows)
                {
                    HoaDon entity = new HoaDon();
                    entity.HoTen = item["HoTen"].ToString();
                    entity.DiaChi = item["DiaChi"].ToString();
                    entity.MaHD = int.Parse(item["MaHD"].ToString());
                    entity.SoHoaDon = item["SoHoaDon"].ToString();
                    entity.DanhBo = (string)item["DanhBo"];
                    entity.Nam = int.Parse(item["Nam"].ToString());
                    entity.Ky = int.Parse(item["Ky"].ToString());
                    entity.GiaBan = int.Parse(item["GiaBan"].ToString());
                    entity.ThueGTGT = int.Parse(item["ThueGTGT"].ToString());
                    entity.PhiBVMT = int.Parse(item["PhiBVMT"].ToString());
                    entity.TongCong = int.Parse(item["TongCong"].ToString());
                    entity.PhiMoNuoc = int.Parse(item["PhiMoNuoc"].ToString());
                    entity.TienDu = int.Parse(item["TienDu"].ToString());
                    hoadons.Add(entity);
                }
                return hoadons;
            }
            else
            {
                ErrorResponse error = new ErrorResponse("Khách Hàng hết nợ", errorCodeHetNo);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound,error ));
            }
        }

        /// <summary>
        /// Lấy Phí Mở Nước nếu có, mặc định return 0
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getPhiMoNuoc")]
        public int getPhiMoNuoc(string DanhBo)
        {
            try
            {
                return (int)_cDAL.ExecuteQuery_ReturnOneValue("select PhiMoNuoc=dbo.fnGetPhiMoNuoc(" + DanhBo + ")");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
        }

        /// <summary>
        /// Lấy Tiền Dư của khách hàng nếu có, mặc định return 0
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <returns></returns>
        [Route("getTienDu")]
        public int getTienDu(string DanhBo)
        {
            try
            {
                return (int)_cDAL.ExecuteQuery_ReturnOneValue("select TienDu=dbo.fnGetTienDu(" + DanhBo + ")");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
        }

        /// <summary>
        /// Lưu Giao Dịch thực hiện
        /// Bước 1: gọi hàm lấy hóa đơn tồn
        /// Bước 2: gọi hàm lấy phí mở nước
        /// Bước 3: gọi hàm lấy tiền dư
        /// Bước 4: gọi hàm insertThuHo
        /// </summary>
        /// <param name="DanhBo"></param>
        /// <param name="MaHDs">chuổi Mã Hóa Đơn. ví dụ new string {MaHD1,MaHD2,MaHD3}</param>
        /// <param name="SoTien">Số Tiền Tổng Tất Cả Hóa Đơn</param>
        /// <param name="PhiMoNuoc">gọi hàm getPhiMoNuoc</param>
        /// <param name="TienDu">gọi hàm getTienDu</param>
        /// <param name="TongCong">Số Tiền Tổng Cộng thu của Khách Hàng (TongCong=SoTien+PhiMoNuoc-TienDu)</param>
        /// <param name="TenDichVu">Tên Đơn Vị Thu</param>
        /// <param name="IDGiaoDich">ID Đơn Vị Thu tạo cho từng giao dịch để quản lý</param>
        /// <returns></returns>
        [Route("insertThuHo")]
        public bool insertThuHo(string DanhBo, string MaHDs, int SoTien, int PhiMoNuoc, int TienDu, int TongCong, string TenDichVu, string IDGiaoDich,string checksum)
        {
            string PasswordSQL = "";
            try
            {
                PasswordSQL = (string)_cDAL.ExecuteQuery_ReturnOneValue("select Password from NGANHANG where Username='" + TenDichVu + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            if(getSHA256(DanhBo+MaHDs+SoTien+PhiMoNuoc+TienDu+TongCong+TenDichVu+IDGiaoDich+PasswordSQL) != checksum)
            {
                ErrorResponse error = new ErrorResponse("Sai Mã kiểm tra", errorCodePassword);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, error));
            }
            //kiểm tra đã insert theo TenDichVu & IDGiaoDich
            int checkExist = 0;
            try
            {
                checkExist = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaHD) from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            if (checkExist > 0)
            {
                ErrorResponse error = new ErrorResponse("IDGiaoDich này đã tồn tại", errorCodeIDGiaoDichTonTai);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Found, error));
            }

            //kiểm tra đã insert theo TenDichVu & IDGiaoDich
            string[] arrayMaHD = MaHDs.Split(',');
            checkExist = 0;
            for (int i = 0; i < arrayMaHD.Length; i++)
            {
                try
                {
                    checkExist = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaHD) from TT_DichVuThu where MaHD=" + arrayMaHD[i]);
                }
                catch (Exception ex)
                {
                    ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
                }
                if (checkExist > 0)
                {
                    ErrorResponse error = new ErrorResponse("Khách Hàng hết nợ", errorCodeHetNo);
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Found, error));
                }
            }
            
            //insert Database
            try
            {
                _cDAL.BeginTransaction();
                int ID = (int)_cDAL.ExecuteQuery_ReturnOneValue_Transaction("select MAX(ID)+1 from TT_DichVuThuTong");
                
                string SoHoaDons = "", sql = "";
                for (int i = 0; i < arrayMaHD.Length; i++)
                {
                    DataTable dt = _cDAL.ExecuteQuery_DataTable_Transaction("select MaHD=ID_HOADON,SOHOADON,DanhBo=DANHBA,NAM,KY,GIABAN,ThueGTGT=THUE,PhiBVMT=PHI,TONGCONG from HOADON where ID_HOADON=" + arrayMaHD[i]);
                    sql = "insert into TT_DichVuThu(MaHD,SoHoaDon,DanhBo,Nam,Ky,SoTien,TenDichVu,IDDichVu,IDGiaoDich,CreateDate)"
                        + " values(" + dt.Rows[0]["MaHD"] + ",'" + dt.Rows[0]["SoHoaDon"] + "','" + dt.Rows[0]["DanhBo"] + "'," + dt.Rows[0]["Nam"] + "," + dt.Rows[0]["Ky"] + "," + dt.Rows[0]["TongCong"] + ",N'" + TenDichVu + "'," + ID + ",'" + IDGiaoDich + "',getdate())";
                    _cDAL.ExecuteNonQuery_Transaction(sql);
                    if (string.IsNullOrEmpty(SoHoaDons) == true)
                        SoHoaDons = dt.Rows[0]["SoHoaDon"].ToString();
                    else
                        SoHoaDons += "," + dt.Rows[0]["SoHoaDon"];
                }
                sql = "insert into TT_DichVuThuTong(ID,DanhBo,MaHDs,SoHoaDons,SoTien,PhiMoNuoc,TienDu,TongCong,TenDichVu,IDGiaoDich,CreateDate)"
                            + " values(" + ID + ",'" + DanhBo + "','" + MaHDs + "','" + SoHoaDons + "'," + SoTien + "," + PhiMoNuoc + "," + TienDu + "," + TongCong + ",N'" + TenDichVu + "','" + IDGiaoDich + "',getdate())";
                _cDAL.ExecuteNonQuery_Transaction(sql);
                _cDAL.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _cDAL.RollbackTransaction();
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                //return false;
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
        }

        /// <summary>
        /// Xóa Giao Dịch đã thực hiện
        /// </summary>
        /// <param name="TenDichVu"></param>
        /// <param name="IDGiaoDich"></param>
        /// <returns></returns>
        [Route("deleteThuHo")]
        [HttpPost]
        public bool deleteThuHo(string TenDichVu, string IDGiaoDich,string checksum)
        {
            string PasswordSQL = "";
            try
            {
                PasswordSQL = (string)_cDAL.ExecuteQuery_ReturnOneValue("select Password from NGANHANG where Username='" + TenDichVu + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            if (getSHA256(TenDichVu + IDGiaoDich + PasswordSQL) != checksum)
            {
                ErrorResponse error = new ErrorResponse("Sai Mã kiểm tra", errorCodePassword);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, error));
            }
            int checkExist = 0;
            try
            {
                checkExist = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(MaHD) from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            if (checkExist == 0)
            {
                ErrorResponse error = new ErrorResponse("IDGiaoDich không tồn tại", errorCodeIDGiaoDichKhongTonTai);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            }
            //
            try
            {
                DataTable dt = _cDAL.ExecuteQuery_DataTable("select MaHD from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int count = (int)_cDAL.ExecuteQuery_ReturnOneValue("select COUNT(ID_HOADON) from HOADON where ID_HOADON=" + dt.Rows[i]["MaHD"] + " and NGAYGIAITRACH is not null");
                    if (count > 0)
                    {
                        ErrorResponse error = new ErrorResponse("Hóa Đơn đã Giải Trách. Không xóa được", errorCodeGiaiTrach);
                        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, error));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            //
            int phimonuoc = 0;
            try
            {
                phimonuoc = (int)_cDAL.ExecuteQuery_ReturnOneValue("select PhiMoNuoc from TT_DichVuThuTong where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            if (phimonuoc > 0)
            {
                ErrorResponse error = new ErrorResponse("Hóa Đơn có Phí Mở Nước. Không xóa được", errorCodePhiMoNuoc);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, error));
            }
            //
            try
            {
                _cDAL.BeginTransaction();
                _cDAL.ExecuteNonQuery_Transaction("insert TT_DichVuThu_Huy select * from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                _cDAL.ExecuteNonQuery_Transaction("delete TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                _cDAL.ExecuteNonQuery_Transaction("insert TT_DichVuThuTong_Huy select * from TT_DichVuThuTong where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                _cDAL.ExecuteNonQuery_Transaction("delete TT_DichVuThuTong where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
                _cDAL.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                _cDAL.RollbackTransaction();
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                //return false;
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
        }

        /// <summary>
        /// Lấy Thông Tin Hóa Đơn đã thu trong Giao Dịch đã thực hiện
        /// </summary>
        /// <param name="TenDichVu"></param>
        /// <param name="IDGiaoDich"></param>
        /// <returns></returns>
        [Route("getThuHo")]
        public IList<ThuHoChiTiet> getThuHo(string TenDichVu, string IDGiaoDich)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = _cDAL.ExecuteQuery_DataTable("select MaHD,SoHoaDon,DanhBo,Nam,Ky,SoTien,CreateDate from TT_DichVuThu where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError,error));
            }
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<ThuHoChiTiet> thuhochitiet = new List<ThuHoChiTiet>();
                foreach (DataRow item in dt.Rows)
                {
                    ThuHoChiTiet entity = new ThuHoChiTiet();
                    entity.MaHD = int.Parse(item["MaHD"].ToString());
                    entity.SoHoaDon = item["SoHoaDon"].ToString();
                    entity.DanhBo = item["DanhBo"].ToString();
                    entity.Nam = int.Parse(item["Nam"].ToString());
                    entity.Ky = int.Parse(item["Ky"].ToString());
                    entity.SoTien = int.Parse(item["SoTien"].ToString());
                    entity.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                    thuhochitiet.Add(entity);
                }
                return thuhochitiet;
            }
            else
            {
                ErrorResponse error = new ErrorResponse("IDGiaoDich không tồn tại", errorCodeIDGiaoDichKhongTonTai);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            }
        }

        /// <summary>
        /// Lấy Thông Tin Giao Dịch đã thực hiện
        /// </summary>
        /// <param name="TenDichVu"></param>
        /// <param name="IDGiaoDich"></param>
        /// <returns></returns>
        [Route("getThuHoTong")]
        public IList<ThuHoTong> getThuHoTong(string TenDichVu, string IDGiaoDich)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = _cDAL.ExecuteQuery_DataTable("select DanhBo,MaHDs,SoHoaDons,SoTien,PhiMoNuoc,TienDu,TongCong,CreateDate from TT_DichVuThuTong where TenDichVu=N'" + TenDichVu + "' and IDGiaoDich='" + IDGiaoDich + "'");
            }
            catch (Exception ex)
            {
                ErrorResponse error = new ErrorResponse(ex.Message, errorCodeSQL);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError, error));
            }
            //
            if (dt != null && dt.Rows.Count > 0)
            {
                List<ThuHoTong> thuhotong = new List<ThuHoTong>();
                foreach (DataRow item in dt.Rows)
                {
                    ThuHoTong entity = new ThuHoTong();
                    entity.DanhBo = item["DanhBo"].ToString();
                    entity.MaHDs = item["MaHDs"].ToString();
                    entity.SoHoaDons = item["SoHoaDons"].ToString();
                    entity.SoTien = int.Parse(item["SoTien"].ToString());
                    entity.PhiMoNuoc = int.Parse(item["PhiMoNuoc"].ToString());
                    entity.TienDu = int.Parse(item["TienDu"].ToString());
                    entity.TongCong = int.Parse(item["TongCong"].ToString());
                    entity.CreateDate = DateTime.Parse(item["CreateDate"].ToString());
                    thuhotong.Add(entity);
                }
                return thuhotong;
            }
            else
            {
                ErrorResponse error = new ErrorResponse("IDGiaoDich không tồn tại", errorCodeIDGiaoDichKhongTonTai);
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound, error));
            }
        }

        private string getSHA256(string strData)
        {
            SHA256Managed crypt = new SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(strData), 0, Encoding.UTF8.GetByteCount(strData));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString().ToLower();
        }
    }
}