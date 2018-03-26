using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WSSmartPhone
{
    public class CKinhDoanh
    {
        Connection _DAL = new Connection("Data Source=192.168.90.9;Initial Catalog=KTKS_DonKH;Persist Security Info=True;User ID=sa;Password=P@ssW012d9");

        public DataTable GetDSGiaNuoc()
        {
            return _DAL.ExecuteQuery_SqlDataAdapter_DataTable("select * from GiaNuoc");
        }
    }
}