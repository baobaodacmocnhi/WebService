using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/ThuTien")]
    public class apiThuTienController : ApiController
    {
        private CConnection cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTienWFH);
        private CConnection cDAL_KinhDoanh = new CConnection(CGlobalVariable.ThuongVuWFH);
        MResult _result = new MResult();

        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        private string DataTableToJSON(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        private bool checkHeader()
        {
            try
            {
                var headers = Request.Headers;
                if (headers.Contains("checksum"))
                {
                    if (headers.GetValues("checksum").First() != CGlobalVariable.cheksum)
                    {
                        _result.alert = "ERR:11";
                        _result.message = "Sai Mã Xác Nhận";
                        return false;
                    }
                }
                else
                {
                    _result.alert = "ERR:10";
                    _result.message = "Không Mã Xác Nhận";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool checkActiveMobile(string MaNV)
        {
            try
            {
                return (bool)cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select ActiveMobile from TT_NguoiDung where MaND=" + MaNV);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool checkChotDangNgan(string NgayGiaiTrach)
        {
            try
            {
                if ((int)cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(*) from TT_ChotDangNgan where CAST(NgayChot as date)='" + NgayGiaiTrach + "' and Chot=1") > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("getVersion")]
        private MResult getVersion()
        {
            //try
            //{
            //    if (checkHeader() == false)
            //        return _result;

            //    object value = cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select Version from TT_DeviceConfig");
            //    if (value != null)
            //    {
            //        _result.success = true;
            //        _result.message = value.ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _result.message = ex.Message;
            //}
            return _result;
        }

        private MResult postDangNhap(string Username, string Password, string IDMobile, string UID)
        {
            try
            {
                object MaNV = null;
                MaNV = cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and An=0");
                if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                    MaNV = cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select MaND from TT_NguoiDung where TaiKhoan='" + Username + "' and MatKhau='" + Password + "' and IDMobile='" + IDMobile + "' and An=0");

                if (MaNV == null || MaNV.ToString() == "")
                {
                    _result.message = "Sai mật khẩu hoặc IDMobile";
                    return _result;
                }

                //xóa máy đăng nhập MaNV khác
                object MaNV_UID_Old = cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");
                if (MaNV_UID_Old != null && (int)MaNV_UID_Old > 0)
                    cDAL_ThuTien.ExecuteNonQuery("delete TT_DeviceSigned where MaNV!=" + MaNV + " and UID='" + UID + "'");

                //if (MaNV.ToString() != "0" && MaNV.ToString() != "1")
                //{
                //    DataTable dt = _cDAL.ExecuteQuery_DataTable("select UID from TT_DeviceSigned where MaNV=" + MaNV);
                //    foreach (DataRow item in dt.Rows)
                //    {
                //        SendNotificationToClient("Thông Báo Đăng Xuất", "Hệ thống server gửi đăng xuất đến thiết bị", item["UID"].ToString(), "DangXuat", "DangXuat", "false", "");
                //        _cDAL.ExecuteNonQuery("delete TT_DeviceSigned where UID='" + item["UID"].ToString() + "'");
                //    }
                //}

                object MaNV_UID = cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select COUNT(MaNV) from TT_DeviceSigned where MaNV='" + MaNV + "' and UID='" + UID + "'");
                if (MaNV_UID != null)
                    if ((int)MaNV_UID == 0)
                        cDAL_ThuTien.ExecuteNonQuery("insert TT_DeviceSigned(MaNV,UID,CreateDate)values(" + MaNV + ",'" + UID + "',getDate())");
                    else
                        cDAL_ThuTien.ExecuteNonQuery("update TT_DeviceSigned set ModifyDate=getdate() where MaNV=" + MaNV + " and UID='" + UID + "'");

                //_cDAL.ExecuteNonQuery("update TT_NguoiDung set UID='" + UID + "',UIDDate=getdate() where MaND=" + MaNV);


                DataTable dt = cDAL_ThuTien.ExecuteQuery_DataTable("select TaiKhoan,MatKhau,MaND,HoTen,Admin,HanhThu,DongNuoc,Doi,ToTruong,MaTo,DienThoai,InPhieuBao,TestApp,SyncNopTien from TT_NguoiDung where MaND=" + MaNV);
                if (dt != null && dt.Rows.Count > 0)
                {
                    
                    _result.message = DataTableToJSON(dt);
                }
                else
                    _result.message = "Không Có Giá Trị";
            }
            catch (Exception ex)
            {
                _result.message = ex.Message;
            }
            return _result;
        }

        [HttpGet]
        [Route("exportExcelHDDCBCT")]
        public MResult exportExcelHDDCBCT(string NgayGiaiTrach)
        {
            HttpResponseMessage response = response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                //if (checkHeader() == true)
                {
                    MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    byte[] excelFile;
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                        string sql = "select 'Đợt'=ctdchd.Dot,'Kỳ'=ctdchd.Ky,'Năm'=ctdchd.Nam,'Danh Bộ'=DanhBo,'Số Phát Hành'=SoPhatHanh,'Số Hóa Đơn Cũ'=ctdchd.SoHoaDon"
                                    + " ,'Giá Biểu Cũ'=ctdchd.GiaBieu,'Định Mức Cũ'=ctdchd.DinhMuc,'Tiêu Thụ Cũ'=ctdchd.TieuThu"
                                    + " ,'Tiền Nước Cũ' = TienNuoc_Start,'Thuế GTGT Cũ' = ThueGTGT_Start,'Phí BVMT Cũ' = PhiBVMT_Start,'Tổng Cộng Cũ' = TongCong_Start"
                                    + " ,'Giá Biểu Mới' = GiaBieu_BD,'Định Mức Mới' = DinhMuc_BD,'Tiêu Thụ Mới' = TieuThu_BD"
                                    + " ,'Tiền Nước Mới' = TienNuoc_End,'Thuế GTGT Mới' = ThueGTGT_End,'Phí BVMT Mới' = PhiBVMT_End,'Tổng Cộng Mới' = TongCong_End"
                                    + " from HOADON hd,[SERVER11].[KTKS_DonKH].[dbo].[DCBD_ChiTietHoaDon] ctdchd"
                                    + " where hd.BaoCaoThue=1 and CAST(NGAYGIAITRACH as date)='" + DateTime.ParseExact(NgayGiaiTrach, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd") + "' and MaNV_DangNgan is not null"
                                    + " and hd.DANHBA=ctdchd.DanhBo and hd.NAM= ctdchd.Nam and hd.Ky= ctdchd.Ky";
                        DataTable dt = cDAL_ThuTien.ExecuteQuery_DataTable(sql);
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                        byte[] bytes = package.GetAsByteArray();
                        excelFile = bytes;
                    }
                    string fileName = "TanHoa.HDDCBCT." + NgayGiaiTrach.Replace("/", ".") + ".xlsx";
                    MemoryStream memoryStream = new MemoryStream(excelFile);
                    response.Content = new StreamContent(memoryStream);
                    response.Content.Headers.ContentType = mediaType;
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("fileName") { FileName = fileName };
                }
            }
            catch (Exception ex)
            {
                _result.alert = "ERR:0";
                _result.message = ex.Message;
            }
            return _result;
        }


    }
}