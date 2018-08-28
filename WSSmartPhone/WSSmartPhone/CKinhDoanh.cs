using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WSSmartPhone
{
    public class CKinhDoanh
    {
        CConnection _DAL = new CConnection("Data Source=serverg8-01;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=db11@tanhoa");

        public DataTable GetDSGiaNuoc()
        {
            return _DAL.ExecuteQuery_DataTable("select * from GiaNuoc");
        }
    }
}