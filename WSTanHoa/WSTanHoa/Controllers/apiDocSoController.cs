using System;
using System.IO;
using System.Net;
using System.Web.Http;
using WSTanHoa.Providers;

namespace WSTanHoa.Controllers
{
    [RoutePrefix("api/DocSo")]
    public class apiDocSoController : ApiController
    {
        private CConnection _cDAL_DHN = new CConnection(CGlobalVariable.DHN);
        private CConnection _cDAL_DocSo = new CConnection(CGlobalVariable.DocSo);

        #region Hoa Sen

        private bool checkExists(string DanhBo)
        {
            try
            {
                object result = _cDAL_DocSo.ExecuteQuery_ReturnOneValue("select * from sDHN where DanhBo='" + DanhBo + "'");
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("updateDS_DHN")]
        [HttpGet]
        public bool updateDS_DHN(string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/all/?req=list_swm_Id");
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        foreach (var item in obj)
                        {
                            if (checkExists(item) == false)
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid)values('" + item + "',1,1)");
                        }
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("get_ChiSoNuoc")]
        [HttpGet]
        public bool get_ChiSoNuoc(string DanhBo, string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/volume/?id=" + DanhBo + "&date=" + Time);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        string sql = "insert into sDHN_LichSu(ID,ChiSo,ThoiGianCapNhat,Loai)"
                            + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                            + "," + obj["Vol"] + ",'" + obj["TimeUpdate"] + "',N'ChiSoNuoc')";
                        return _cDAL_DocSo.ExecuteNonQuery(sql);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("get_ChatLuongSong")]
        [HttpGet]
        public bool get_ChatLuongSong(string DanhBo, string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/signal_quality/?id=" + DanhBo + "&date=" + Time);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        string sql = "insert into sDHN_LichSu(ID,ChatLuongSong,Loai)"
                            + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                            + ",'" + obj["ChatLuongSong"] + "',N'ChatLuongSong')";
                        return _cDAL_DocSo.ExecuteNonQuery(sql);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("get_CanhBao")]
        [HttpGet]
        public bool get_CanhBao(string DanhBo, string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/warnings/?id=" + DanhBo + "&date=" + Time);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        string sql = "insert into sDHN_LichSu(ID,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop,ThoiGianCapNhat,Loai)"
                            + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                            + "," + obj["IsLowBatt"] == "False" ? 0 : 1
                            + "," + obj["IsLeakage"] == "False" ? 0 : 1
                            + "," + obj["IsOverLoad"] == "False" ? 0 : 1
                            + "," + obj["IsReverse"] == "False" ? 0 : 1
                            + "," + obj["IsTampering"] == "False" ? 0 : 1
                            + "," + obj["IsDry"] == "False" ? 0 : 1
                            + "," + obj["IsOpenBox"] == "False" ? 0 : 1
                                + ",'" + obj["TimeUpdate"] + "',N'CanhBao')";
                        return _cDAL_DocSo.ExecuteNonQuery(sql);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("get_Pin")]
        [HttpGet]
        public bool get_Pin(string DanhBo, string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/battery/?id=" + DanhBo + "&date=" + Time);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        string sql = "insert into sDHN_LichSu(ID,Pin,ThoiLuongPinConLai,ThoiGianCapNhat,Loai)"
                            + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                            + "," + obj["batt_percent"]
                            + ",'" + obj["batt_duration"] + "'"
                            + ",'" + obj["TimeUpdate"] + "',N'Pin')";
                        return _cDAL_DocSo.ExecuteNonQuery(sql);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("get_All")]
        [HttpGet]
        public bool get_All(string DanhBo, string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8033/api/all/?id=" + DanhBo + "&date=" + Time);
                    request.Method = "GET";
                    request.ContentType = "application/json; charset=utf-8";

                    HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                    if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                    {
                        StreamReader read = new StreamReader(respuesta.GetResponseStream());
                        string result = read.ReadToEnd();
                        read.Close();
                        respuesta.Close();

                        var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                        int flagCBPinYeu = 0, flagCBRoRi = 0, flagCBQuaDong = 0, flagCBChayNguoc = 0, flagCBNamCham = 0, flagCBKhoOng = 0, flagCBMoHop = 0;
                        if (obj["IsLowBatt"] == true)
                            flagCBPinYeu = 1;
                        if (obj["IsLeakage"] == true)
                            flagCBRoRi = 1;
                        if (obj["IsOverLoad"] == true)
                            flagCBQuaDong = 1;
                        if (obj["IsReverse"] == true)
                            flagCBChayNguoc = 1;
                        if (obj["IsTampering"] == true)
                            flagCBNamCham = 1;
                        if (obj["IsDry"] == true)
                            flagCBKhoOng = 1;
                        if (obj["IsOpenBox"] == true)
                            flagCBMoHop = 1;
                        string LuuLuong = "NULL";
                        if (obj["Flow"] != null)
                            LuuLuong = obj["Flow"];
                        string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                            + ",Longitude,Latitude,Altitude,ChuKyGui"
                            + ",ThoiGianCapNhat,Loai)"
                                       + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                       + ",'" + DanhBo + "'"
                                       + "," + obj["Volume"]
                                       + "," + obj["Battery"]
                                       + ",'" + obj["RemainBatt"] + "'"
                                       + "," + LuuLuong
                                       + ",'" + obj["Rssi"] + "'"
                                       + "," + flagCBPinYeu
                                       + "," + flagCBRoRi
                                       + "," + flagCBQuaDong
                                       + "," + flagCBChayNguoc
                                       + "," + flagCBNamCham
                                       + "," + flagCBKhoOng
                                       + "," + flagCBMoHop
                                       + "," + obj["Longitude"]
                                       + "," + obj["Latitude"]
                                       + "," + obj["Altitude"]
                                       + "," + obj["Interval"]
                                       + ",'" + obj["Time"] + "',N'All')";
                        CGlobalVariable.log.Error("apiDocSo " + sql);
                        return _cDAL_DocSo.ExecuteNonQuery(sql);
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}