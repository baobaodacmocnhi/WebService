using System;
using System.Data;
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
                    updateDS_DHN_HoaSen();
                    updateDS_DHN_Rynan();
                    updateDS_DHN_Deviwas();
                    updateDS_DHN_PhamLam();
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

        private bool updateDS_DHN_HoaSen()
        {
            try
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
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=1");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item["MaDanhbo"]) == false)
                            if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',1,1,0)");
                            else
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',1,'" + item["SeriModule"] + "',1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=1");
                    }
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

        private bool updateDS_DHN_Rynan()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:7032/api/list_swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=2");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item) == false)
                            _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item + "',2,1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1 where DanhBo='" + item + "' and IDNCC=2");
                    }
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

        private bool updateDS_DHN_Deviwas()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8039/api/all/?req=list_swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=3");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item["MaDanhbo"]) == false)
                            if (string.IsNullOrEmpty(item["SeriModule"]))
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["MaDanhbo"] + "',3,1,0)");
                            else
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["MaDanhbo"] + "',3,'" + item["SeriModule"] + "',1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["SeriModule"] + "' where DanhBo='" + item["MaDanhbo"] + "' and IDNCC=3");
                    }
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

        private bool updateDS_DHN_PhamLam()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8032/apipl/List_Swm_Id");
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=0 where IDNCC=4");
                    var obj = CGlobalVariable.jsSerializer.Deserialize<dynamic>(result);
                    foreach (var item in obj)
                    {
                        if (checkExists(item["wmid"]) == false)
                            if (string.IsNullOrEmpty(item["idlogger"]))
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,Valid,CreateBy)values('" + item["wmid"] + "',4,1,0)");
                            else
                                _cDAL_DocSo.ExecuteNonQuery("insert into sDHN(DanhBo,IDNCC,IDLogger,Valid,CreateBy)values('" + item["wmid"] + "',4,'" + item["idlogger"] + "',1,0)");
                        else
                            _cDAL_DocSo.ExecuteNonQuery("update sDHN set Valid=1,IDLogger='" + item["idlogger"] + "' where DanhBo='" + item["wmid"] + "' and IDNCC=4");
                    }
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


        [Route("get_All")]
        [HttpGet]
        public bool get_All(string Time, string checksum)
        {
            try
            {
                if (CGlobalVariable.cheksum == checksum)
                {
                    DataTable dt = _cDAL_DocSo.ExecuteQuery_DataTable("select DanhBo,IDNCC from sDHN a,CAPNUOCTANHOA.dbo.TB_DULIEUKHACHHANG where Valid=1 and a.DanhBo=b.DanhBo");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        switch (int.Parse(dt.Rows[i]["IDNCC"].ToString()))
                        {
                            case 1:
                                get_All_HoaSen(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 2:
                                get_All_Rynan(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 3:
                                get_All_Deviwas(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            case 4:
                                get_All_PhamLam(dt.Rows[i]["DanhBo"].ToString(), Time);
                                break;
                            default:
                                break;
                        }
                    }
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

        public bool get_All_HoaSen(string DanhBo, string Time)
        {
            try
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
                    //CGlobalVariable.log.Error("apiDocSo " + sql);
                    return _cDAL_DocSo.ExecuteNonQuery(sql);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool get_All_Rynan(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:7032/api/swm_hour?Id=" + DanhBo + "&Date=" + Time);
                request.Method = "GET";
                request.ContentType = "application/json; charset=utf-8";

                HttpWebResponse respuesta = (HttpWebResponse)request.GetResponse();
                if (respuesta.StatusCode == HttpStatusCode.Accepted || respuesta.StatusCode == HttpStatusCode.OK || respuesta.StatusCode == HttpStatusCode.Created)
                {
                    StreamReader read = new StreamReader(respuesta.GetResponseStream());
                    string result = read.ReadToEnd();
                    read.Close();
                    respuesta.Close();
                    if (result != "Not value return.")
                    {
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
                                       + ",NULL"
                                       + "," + obj["Interval"]
                                       + ",'" + obj["Time"] + "',N'All')";
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

        public bool get_All_Deviwas(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8039/api/all?id=" + DanhBo + "&date=" + Time);
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
                    //if (obj["Flow"] != null)
                    //    LuuLuong = obj["Flow"];
                    string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                        + ",Longitude,Latitude,Altitude,ChuKyGui"
                        + ",ThoiGianCapNhat,Loai)"
                                   + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                   + ",'" + DanhBo + "'"
                                   + "," + obj["Vol"]
                                   + ",NULL" //+ obj["Battery"]
                                   + ",'" + obj["bat_duration"] + "'"
                                   + "," + LuuLuong
                                   + ",'" //+ obj["Rssi"] + "'"
                                   + ",0" //+ flagCBPinYeu
                                   + ",0" //+ flagCBRoRi
                                   + ",0" //+ flagCBQuaDong
                                   + ",0" //+ flagCBChayNguoc
                                   + ",0" //+ flagCBNamCham
                                   + ",0" //+ flagCBKhoOng
                                   + ",0" //+ flagCBMoHop
                                   + ",NULL" //+ obj["Longitude"]
                                   + ",NULL" //+ obj["Latitude"]
                                   + ",NULL" //+ obj["Altitude"]
                                   + ",NULL" //+ obj["Interval"]
                                   + ",NULL'" + obj["TimeUpdate"] + "',N'All')";
                    return _cDAL_DocSo.ExecuteNonQuery(sql);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool get_All_PhamLam(string DanhBo, string Time)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://swm.sawaco.com.vn:8032/apipl/swm_hour/" + DanhBo + "/" + Time);
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
                    if (obj["isLowBatt"] == true)
                        flagCBPinYeu = 1;
                    if (obj["isLeakage"] == true)
                        flagCBRoRi = 1;
                    if (obj["isOverLoad"] == true)
                        flagCBQuaDong = 1;
                    if (obj["isReverse"] == true)
                        flagCBChayNguoc = 1;
                    if (obj["isTampering"] == true)
                        flagCBNamCham = 1;
                    if (obj["isDry"] == true)
                        flagCBKhoOng = 1;
                    if (obj["isOpenBox"] == true)
                        flagCBMoHop = 1;
                    string LuuLuong = "NULL";
                    if (obj["Flow"] != null)
                        LuuLuong = obj["Flow"];
                    string sql = "insert into sDHN_LichSu(ID,DanhBo,ChiSo,Pin,ThoiLuongPinConLai,LuuLuong,ChatLuongSong,CBPinYeu,CBRoRi,CBQuaDong,CBChayNguoc,CBNamCham,CBKhoOng,CBMoHop"
                        + ",Longitude,Latitude,Altitude,ChuKyGui"
                        + ",ThoiGianCapNhat,Loai)"
                                   + "values((select case when exists(select ID from sDHN_LichSu) then (select MAX(ID)+1 from sDHN_LichSu) else 1 end)"
                                   + ",'" + DanhBo + "'"
                                   + "," + obj["flow"]
                                   + ",NULL" //+ obj["Battery"]
                                   + ",NULL'" //+ obj["RemainBatt"] + "'"
                                   + ",NULL" //+ LuuLuong
                                   + ",'" + obj["rsrp"] + "'"
                                   + "," + flagCBPinYeu
                                   + "," + flagCBRoRi
                                   + "," + flagCBQuaDong
                                   + "," + flagCBChayNguoc
                                   + "," + flagCBNamCham
                                   + "," + flagCBKhoOng
                                   + "," + flagCBMoHop
                                   + "," + obj["longitude"]
                                   + "," + obj["latitude"]
                                   + "," + obj["altitude"]
                                   + "," + obj["interval"]
                                   + ",'" + obj["time"] + "',N'All')";
                    return _cDAL_DocSo.ExecuteNonQuery(sql);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}