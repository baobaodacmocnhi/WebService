using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using System.Data;

namespace WSSmartPhone
{
    public class CDHN
    {
        CConnection _DAL_DHN = new CConnection("Data Source=hp_g7\\kd;Initial Catalog=CAPNUOCTANHOA;Persist Security Info=True;User ID=sa;Password=db8@tanhoa");
        CConnection _DAL_DocSo = new CConnection("Data Source=hp_g7\\kd;Initial Catalog=DocSoTHTest;Persist Security Info=True;User ID=sa;Password=db8@tanhoa");
        public string getPhuongQuan(string DanhBo)
        {
            string sql = "select ' P.'+p.TENPHUONG+' Q.'+q.TENQUAN from TB_DULIEUKHACHHANG dlkh,PHUONG p,QUAN q"
                    + " where DANHBO='" + DanhBo + "' and dlkh.QUAN=q.MAQUAN and dlkh.PHUONG=p.MAPHUONG"
                    + " and p.MAQUAN=q.MAQUAN";
            object result = _DAL_DHN.ExecuteQuery_ReturnOneValue(sql);
            if (result != null)
                return result.ToString();
            else
                return "";
        }


        //billing
        OdbcConnection connTongCT = new OdbcConnection();
        public void OpenConnectionTCT()
        {
            try
            {
                connTongCT = new OdbcConnection(@"Dsn=Oracle7;uid=TH_HANDHELD;pwd=TH_HANDHELD;server=center");
                if (connTongCT.State != ConnectionState.Open)
                {
                    connTongCT.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnectionTCT()
        {
            try
            {
                if (connTongCT.State != ConnectionState.Closed)
                {
                    connTongCT.Close();
                }
                connTongCT.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool insertBilling(string DocSoID, string checksum)
        {
            try
            {
                if (checksum != "tanho@2022")
                    return false;
                string sql = "SELECT DanhBa,CSCu,CASE WHEN LEFT(CodeMoi, 1) = 'F' OR LEFT(CodeMoi, 1) = '6' THEN TieuThuMoi ELSE CSMOI END AS CSMoi,TieuThuMoi,CASE WHEN LEFT(CodeMoi,1) = '4' THEN '4' ELSE CodeMoi END AS CodeMoi,MLT2,TTDHNMoi"
                        + ",DenNgay=CONVERT(varchar(10),DenNgay,103),Nam,Ky,Dot FROM DocSo WHERE DocSoID='"+DocSoID+"'";
                DataTable dt = _DAL_DocSo.ExecuteQuery_DataTable(sql);
                string DanhBo = dt.Rows[0]["DanhBa"].ToString().Trim();
                string CodeMoi = dt.Rows[0]["CodeMoi"].ToString().Trim();
                string CSC = dt.Rows[0]["CSCu"].ToString().Trim();
                string CSM = dt.Rows[0]["CSMoi"].ToString().Trim();
                string TieuThuMoi = dt.Rows[0]["TieuThuMoi"].ToString().Trim();
                string MLT = dt.Rows[0]["MLT2"].ToString().Trim();
                string May = MLT.Substring(2, 2);
                string STT = MLT.Substring(4, 3);
                string NgayDoc = dt.Rows[0]["DenNgay"].ToString().Trim();
                //double ID = Convert.ToDouble(this.getID());
                //string rST_ID = this.getRST_ID(CodeMoi);

                string cmdText = "INSERT INTO ADMIN.\"TMP$MR\" (ID, BRANCH_CODE, \"YEAR\", PERIOD, BC_CODE, CUSTOMER_NO, MR_STATUS, THIS_READING, CONSUMPTION, DATE_READING, CREATED_ON, CREATED_BY, BOOK_NO, OIB, EMP_ID, RST_ID) VALUES ("
                            + "(SELECT ADMIN.\"TMP$MR_SEQ\".NEXTVAL AS ID FROM SYS.DUAL),'TH'," + dt.Rows[0]["Nam"].ToString().Trim() + "," + dt.Rows[0]["Ky"].ToString().Trim() + ",'" + dt.Rows[0]["Dot"].ToString().Trim() + "','" + DanhBo + "','" + CodeMoi + "'," + CSM + "," + TieuThuMoi + ",'" + NgayDoc + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','TH_HANDHELD','" + May + "','" + STT + "','100000002',(SELECT ID FROM READING_STATUS WHERE STATUS_CODE='" + CodeMoi + "'))";
                if (CodeMoi.Length > 0 && (CodeMoi.Substring(0, 1) == "5" || CodeMoi.Substring(0, 1) == "8" || CodeMoi.Substring(0, 1) == "M"))
                {
                    cmdText = "INSERT INTO ADMIN.\"TMP$MR\" (ID, BRANCH_CODE, \"YEAR\", PERIOD, BC_CODE, CUSTOMER_NO, MR_STATUS, LAST_READING, THIS_READING, CONSUMPTION, DATE_READING, CREATED_ON, CREATED_BY, BOOK_NO, OIB, EMP_ID,RST_ID) VALUES ("
                            + "(SELECT ADMIN.\"TMP$MR_SEQ\".NEXTVAL AS ID FROM SYS.DUAL),'TH'," + dt.Rows[0]["Nam"].ToString().Trim() + "," + dt.Rows[0]["Ky"].ToString().Trim() + ",'" + dt.Rows[0]["Dot"].ToString().Trim() + "','" + DanhBo + "','" + CodeMoi + "'," + CSC + "," + CSM + "," + TieuThuMoi + ",'" + NgayDoc + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','TH_HANDHELD','" + May + "','" + STT + "','100000002',(SELECT ID FROM READING_STATUS WHERE STATUS_CODE='" + CodeMoi + "'))";
                }
                this.OpenConnectionTCT();
                OdbcCommand odbcCommand = new OdbcCommand(cmdText, this.connTongCT);
                int result = odbcCommand.ExecuteNonQuery();
                if (result > 0)
                    return true;
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