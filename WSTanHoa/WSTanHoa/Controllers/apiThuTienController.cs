using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using WSTanHoa.Models;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/ThuTien")]
    public class apiThuTienController : ApiController
    {
        private CConnection cDAL_ThuTien = new CConnection(CGlobalVariable.ThuTien);
        string _password = "thutien@2020";
        apiThuTien _result = new apiThuTien();

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
                if (headers.Contains("password"))
                {
                    if (headers.GetValues("password").First() == _password)
                        _result.success = true;
                    else
                        _result.message = "Sai Password";
                }
                else
                {
                    _result.message = "Không Password";
                }
                if (_result.success == true)
                {
                    return true;
                }
                else
                    return false;
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
        private apiThuTien getVersion()
        {
            try
            {
                if (checkHeader() == false)
                    return _result;

                object value = cDAL_ThuTien.ExecuteQuery_ReturnOneValue("select Version from TT_DeviceConfig");
                if (value != null)
                {
                    _result.success = true;
                    _result.message = value.ToString();
                }
            }
            catch (Exception ex)
            {
                _result.message = ex.Message;
            }
            return _result;
        }

        private apiThuTien postDangNhap(string Username, string Password, string IDMobile, string UID)
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
                    _result.success = true;
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


    }
}