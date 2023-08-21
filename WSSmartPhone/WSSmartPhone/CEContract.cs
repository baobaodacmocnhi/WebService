using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Drawing;

namespace WSSmartPhone
{
    public class CEContract
    {
        private string urlApi = "https://api-econtract.cskhtanhoa.com.vn:1443/";
        CConnection _cDAL_TTKH = new CConnection(CGlobalVariable.TTKHWFH);

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

        public string createEContract(string checksum)
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
                    request.ContentType = "multipart/form-data";
                    request.ContentLength = 0;
                    request.Headers["Authorization"] = "Bearer " + getAccess_token();
                    var signFrame = new
                    {
                        x="650.95",
                        y="253.71536000000003",
                        w="120",
                        h="60",
                        page="2"
                    };
                    var customer = new
                    {
                        username = "0938040301",
                        cmnd = "",
                        email = "",
                        mst = "",
                        sdt = "0938040301",
                        signFrame=CGlobalVariable.jsSerializer.Serialize(signFrame),
                        ten = "Nguyễn Ngọc Quốc Bảo",
                        tenToChuc = "",
                        userType = "CONSUMER"
                    };
                    var contract = new
                    {
                        autoRenew="true",
                        contractValue="0",
                        creationNote="Ghi chú hợp đồng",
                        endDate="2023-08-17",
                        flowTemplateId="a7e6494a-f7bc-4306-a3b6-d0842e9c69f3",
                        templateId="64d3625c22ab89b8111134ad",
                        signFlowType="REQUIRE_FLOW",
                        sequence=1,
                        signFlow="[{\"signType\":\"DRAFT\",\"departmentId\":\"88bedc40-ccb5-4221-9e7f-c9c5aad49d67\",\"userId\":\"3440ba96-3683-49c3-9324-9e2d670d3481\",\"sequence\":1,\"limitDate\":1,\"signForm\":[\"NO_AUTHEN\",\"OTP\",\"OTP_EMAIL\",\"SMART_CA\"],\"signFrame\":[{\"x\":1069.54,\"y\":292.11536,\"w\":1095.8591999999999,\"h\":303.15536,\"page\":1}]},{\"signType\":\"APPROVAL\",\"departmentId\":\"5858ea78-7919-402a-b1bf-5786f82cb7c8\",\"userId\":\"ada7106d-ee2c-43c6-8b40-b2b3b52e6d95\",\"sequence\":2,\"limitDate\":1,\"signForm\":[\"SMART_CA\",\"USB_TOKEN\",\"NO_AUTHEN\",\"OTP\"],\"signFrame\":[{\"x\":957.82,\"y\":246.99536,\"w\":984.1192000000001,\"h\":258.03535999999997,\"page\":1},{\"signType\":\"STAMP\",\"departmentId\":\"079c68fb-1002-4b13-93bc-92d97c290096\",\"userId\":\"ea911234-c439-47e8-9d86-c123b9a6ddf8\",\"sequence\":3,\"limitDate\":1,\"signForm\":[\"USB_TOKEN\",\"SMART_CA\",\"OTP\",\"OTP_EMAIL\"],\"signFrame\":[{\"x\":899.14,\"y\":273.63536,\"w\":925.4392,\"h\":284.67535999999996,\"page\":1}]}]}]",
            signForm="[\"OTP\",\"OTP_EMAIL\",\"NO_AUTHEN\"]",
            title="Hợp đồng Hậu 21/08/2023",
            validDate="2023-08-17",
            verificationType="NONE",
            fixedPosition=true,
            callbackUrl=""
                    };
                    var data = new Dictionary<string, string>();
                    data.Add("fields", "{}");
                    data.Add("customer", CGlobalVariable.jsSerializer.Serialize(customer));
                    data.Add("contract", CGlobalVariable.jsSerializer.Serialize(contract));
                    data.Add("attachFile", Convert.ToBase64String(scanFile(@"D:\Download\giaydiduongthuduc.pdf")));
                    data.Add("EKYC_CHANDUNG", Convert.ToBase64String(scanImage(AppDomain.CurrentDomain.BaseDirectory + @"\Images\face.png")));
                    data.Add("EKYC_MATTRUOC", Convert.ToBase64String(scanImage(AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd1.png")));
                    data.Add("EKYC_MATSAU", Convert.ToBase64String(scanImage(AppDomain.CurrentDomain.BaseDirectory + @"\Images\cccd2.png")));
                    var json = CGlobalVariable.jsSerializer.Serialize(data);
                    Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                    //request.ContentLength = byteArray.Length;
                    //gắn data post
                    //Stream dataStream = request.GetRequestStream();
                    //dataStream.Write(byteArray, 0, byteArray.Length);
                    //dataStream.Close();
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

        public byte[] ImageToByte(Bitmap image)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        public byte[] scanImage(string path)
        {
            Image image = Image.FromFile(path);
            return ImageToByte((Bitmap)image);
        }

        public byte[] scanFile(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}