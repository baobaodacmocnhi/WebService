using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

namespace WSSmartPhone
{
    public class CEContract
    {
        private string urlApi = "https://api-econtract.cskhtanhoa.com.vn:1443/";
        CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TTKH);

        public string getAccess_token(string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "auth-service/oauth/token");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    var data = new
                    {
                        username = "admintanhoa",
                        password = "Abc@12345",
                        grant_type = "client_credentials",
                        client_id = "tanhoa.client@econtract.vnpt.vn",
                        client_secret = "C29XWd2bDhsz9jB9h8lq5WOPmw3kS2O0"
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
                        bool flag = _cDAL_TTKH.ExecuteNonQuery("update Access_token set access_token='" + obj["access_token"] + "',CreateDate=getdate() where ID='econtract'");
                        strResponse = flag.ToString();
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

        public string getAccess_token()
        {
            return _cDAL_TTKH.ExecuteQuery_ReturnOneValue("select access_token from Access_token where ID='econtract'").ToString();
        }

        public string renderEContract(string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
            , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string NgayHieuLuc, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "template-service/api/templates/64d3625c22ab89b8111134ad/render");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    //request.ContentLength = 0;
                    request.Headers["Authorization"] = "Bearer " + getAccess_token();
                    var data = new Dictionary<string, string>();
                    data.Add("${soHopDong}", HopDong);
                    data.Add("${soDanhBo}", DanhBo);
                    data.Add("${ngay}", CreateDate.Day.ToString());
                    data.Add("${thang}", CreateDate.Month.ToString());
                    data.Add("${nam}", CreateDate.Year.ToString());
                    data.Add("${tenBenB}", HoTen);
                    data.Add("${soCCCD}", CCCD);
                    data.Add("${capNgay}", NgayCap);
                    data.Add("${noiThuongTru}", DCThuongTru);
                    data.Add("${choOHienNay}", DCHienNay);
                    data.Add("${soDienThoai}", DienThoai);
                    data.Add("${fax}", Fax);
                    data.Add("${email}", Email);
                    data.Add("${taiKhoanSo}", TaiKhoan);
                    data.Add("${taiNganHang}", Bank);
                    data.Add("${maSoThue}", MST);
                    data.Add("${coDongHoNuoc}", CoDHN);
                    data.Add("${dongHoNuocDatTai}", DCLapDat);
                    data.Add("${ngayThangNamThiHanh}", NgayHieuLuc);
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
                        Stream read = respuesta.GetResponseStream();
                        MemoryStream memoryStream = new MemoryStream();
                        read.CopyTo(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        File.WriteAllBytes(@"D:\econtract.pdf", memoryStream.ToArray());
                        System.Diagnostics.Process.Start(@"D:\econtract.pdf");
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

        public string createEContract(bool GanMoi,string HopDong, string DanhBo, DateTime CreateDate, string HoTen, string CCCD, string NgayCap, string DCThuongTru, string DCHienNay
            , string DienThoai, string Fax, string Email, string TaiKhoan, string Bank, string MST, string CoDHN, string DCLapDat, string NgayHieuLuc, string checksum)
        {
            string strResponse = "";
            try
            {
                if (checksum == CGlobalVariable.checksum)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlApi + "esolution-service/contracts/create-draft-from-file-and-identification");
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    //request.ContentLength = 0;
                    request.Headers["Authorization"] = "Bearer " + getAccess_token();
                    var data = new Dictionary<string, string>();
                    data.Add("${soHopDong}", HopDong);
                    data.Add("${soDanhBo}", DanhBo);
                    data.Add("${ngay}", CreateDate.Day.ToString());
                    data.Add("${thang}", CreateDate.Month.ToString());
                    data.Add("${nam}", CreateDate.Year.ToString());
                    data.Add("${tenBenB}", HoTen);
                    data.Add("${soCCCD}", CCCD);
                    data.Add("${capNgay}", NgayCap);
                    data.Add("${noiThuongTru}", DCThuongTru);
                    data.Add("${choOHienNay}", DCHienNay);
                    data.Add("${soDienThoai}", DienThoai);
                    data.Add("${fax}", Fax);
                    data.Add("${email}", Email);
                    data.Add("${taiKhoanSo}", TaiKhoan);
                    data.Add("${taiNganHang}", Bank);
                    data.Add("${maSoThue}", MST);
                    data.Add("${coDongHoNuoc}", CoDHN);
                    data.Add("${dongHoNuocDatTai}", DCLapDat);
                    data.Add("${ngayThangNamThiHanh}", NgayHieuLuc);
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
                        Stream read = respuesta.GetResponseStream();
                        MemoryStream memoryStream = new MemoryStream();
                        read.CopyTo(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        File.WriteAllBytes(@"D:\econtract.pdf", memoryStream.ToArray());
                        System.Diagnostics.Process.Start(@"D:\econtract.pdf");
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

    }
}