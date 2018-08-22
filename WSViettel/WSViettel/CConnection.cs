using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace WSViettel
{
    public class CConnection
    {
        private string _connectionString;  // Chuỗi kết nối
        private SqlConnection connection;         // Đối tượng kết nối
        private SqlDataAdapter adapter;           // Đối tượng adapter chứa dữ liệu
        private SqlCommand command;               // Đối tượng command thực thi truy vấn

        public CConnection()
        {
            try
            {
                _connectionString = "Data Source=192.168.90.9;Initial Catalog=HOADON_TA;Persist Security Info=True;User ID=sa;Password=db9@tanhoa";
                connection = new SqlConnection(_connectionString);
            }
            catch (Exception)
            {
            }
        }

        public void Connect()
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public void Disconnect()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public bool ExecuteNonQuery(string sql)
        {
            try
            {
                Connect();
                command = new SqlCommand(sql, connection);
                int rowsAffected = command.ExecuteNonQuery();
                Disconnect();
                if (rowsAffected >= 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Thực thi câu truy vấn SQL trả về một đối tượng DataSet chứa kết quả trả về
        /// </summary>
        /// <param name="strSelect">Câu truy vấn cần thực thi lấy dữ liệu</param>
        /// <returns>Đối tượng dataset chứa dữ liệu kết quả câu truy vấn</returns>
        public DataSet ExecuteQuery_SqlDataAdapter_DataSet(string sql)
        {
            try
            {
                Connect();
                DataSet dataset = new DataSet();
                command = new SqlCommand();
                command.Connection = this.connection;
                adapter = new SqlDataAdapter(sql, connection);
                try
                {
                    adapter.Fill(dataset);
                }
                catch (SqlException e)
                {
                    throw e;
                }
                Disconnect();
                return dataset;
            }
            catch (Exception)
            {
                Disconnect();
                return null;
            }
        }

        /// <summary>
        /// Thực thi câu truy vấn SQL trả về một đối tượng DataTable chứa kết quả trả về
        /// </summary>
        /// <param name="strSelect">Câu truy vấn cần thực thi lấy dữ liệu</param>
        /// <returns>Đối tượng datatable chứa dữ liệu kết quả câu truy vấn</returns>
        public DataTable ExecuteQuery_SqlDataAdapter_DataTable(string sql)
        {
            try
            {
                return ExecuteQuery_SqlDataAdapter_DataSet(sql).Tables[0];
            }
            catch (Exception)
            {
                Disconnect();
                return null;
            }
        }
    }
}