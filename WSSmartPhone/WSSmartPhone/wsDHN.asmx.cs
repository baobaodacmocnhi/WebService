﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WSSmartPhone
{
    /// <summary>
    /// Summary description for wsDHN
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class wsDHN : System.Web.Services.WebService
    {
        wsBilling _wsBilling = new wsBilling();
        CThuTien _cThuTien = new CThuTien();

        [WebMethod]
        public bool insertBilling(string DocSoID, string checksum)
        {
            return _wsBilling.insertBilling(DocSoID, checksum);
        }

        [WebMethod]
        public bool tinhCodeTieuThu(string DocSoID, string Code, int CSM, out int TieuThu, out int GiaBan, out int ThueGTGT, out int PhiBVMT, out int TongCong)
        {
           return _cThuTien.tinhCodeTieuThu(DocSoID, Code, CSM, out TieuThu, out GiaBan, out ThueGTGT, out PhiBVMT, out TongCong);
        }

    }
}
