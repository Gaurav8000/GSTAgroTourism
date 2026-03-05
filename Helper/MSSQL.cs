using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Helper
{
    public class MSSQL
    {
        //Sqlquery

        string connectionStrings = ConfigurationManager.ConnectionStrings["AGROTOURISM"].ToString();
        public async Task<DataSet> ExecuteStoreProcedureReturnDS(string SPName, Dictionary<string,string> InPara)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(connectionStrings);
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.CommandText = SPName;
                cmd.Connection = con;
                cmd.CommandTimeout = 60;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                foreach (KeyValuePair<string, string> para in InPara)
                {
                    cmd.Parameters.AddWithValue(para.Key, para.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                await Task.Run(() => da.Fill(ds));
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }


        public async Task<SqlDataReader> ExecuteStoreProcedureReturnDR(string SPName, Dictionary<string, string> InPara)
        {
            SqlDataReader dr = null;
            try
            {
                SqlConnection con = new SqlConnection(connectionStrings);
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.CommandText = SPName;
                cmd.Connection = con;
                cmd.CommandTimeout = 60;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                foreach (KeyValuePair<string, string> para in InPara)
                {
                    cmd.Parameters.AddWithValue(para.Key, para.Value);
                }

                await con.OpenAsync();
                dr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return dr;
        }


        public async Task ExecuteNonQuery(string SPName, Dictionary<string, string> InPara)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = SPName;
                        cmd.Connection = con;
                        cmd.CommandTimeout = 60;
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (KeyValuePair<string, string> para in InPara)
                        {
                            cmd.Parameters.AddWithValue(para.Key, para.Value);
                        }
                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
