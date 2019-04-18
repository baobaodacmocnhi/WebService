using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using THService.LinQ;

public static class LinQConnection
{
    static HoaDonTHDataContext db = new HoaDonTHDataContext();
    public static int ExecuteCommand(string sql)
    {
        int result = 0;
        
        try
        {
            SqlConnection conn = new SqlConnection(db.Connection.ConnectionString);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            result = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            db.Connection.Close();
            db.SubmitChanges();
            return result;
        }
        catch (Exception )
        {
            
        }
        finally
        {
            db.Connection.Close();
        }
        db.SubmitChanges();
        return result;
    }

    public static int ExecuteCommand_(string sql)
    {
        int result = 0;
        try
        {
            SqlConnection conn = new SqlConnection(db.Connection.ConnectionString);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            result = Convert.ToInt32(cmd.ExecuteNonQuery());
            conn.Close();
            db.Connection.Close();
            db.SubmitChanges();
            return result;
        }
        catch (Exception ){
        }
        finally
        {
            db.Connection.Close();
        }
        db.SubmitChanges();
        return result;
    }
   
    public static DataTable getDataTable(string sql)
    {
        DataTable table = new DataTable();
        try
        {
            if (db.Connection.State == ConnectionState.Open)
            {
                db.Connection.Close();
            }
            db.Connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, db.Connection.ConnectionString);
            adapter.Fill(table);
        }
        catch (Exception) { }
        finally
        {
            db.Connection.Close();
        }
        return table;
    }

    public static DataSet getDataset(string sql)
    {
        DataSet ds = new DataSet();
        try
        {
            if (db.Connection.State == ConnectionState.Open)
            {
                db.Connection.Close();
            }
            db.Connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, db.Connection.ConnectionString);
            adapter.Fill(ds, "getBill");
        }
        catch (Exception) { }
        finally
        {
            db.Connection.Close();
        }
        return ds;
    }

    public static bool Insert(Agribank_THUTAM tb)
    {
        try
        {
            db.Agribank_THUTAMs.InsertOnSubmit(tb);
            db.SubmitChanges();
            return true;
        }
        catch (Exception ex)
        {
            
        }
        return false;
    }
}