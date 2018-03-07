using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ShowcaseService.DataAccess
{
    public class DatabaseUtil
    {
        public static DataTable GetCar()
        {
            DataSet ds = ExcuteQuery(CommandType.Text, "SELECT * FROM CARS", null);
            if (ds != null)
                return ds.Tables[0];
            else
                return null;
        }


        private static DataSet ExcuteQuery(CommandType type, string commandText, SqlParameter[] parameters)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["ShowcaseDB"].ConnectionString;
                using (var conn = new SqlConnection(connString))
                {
                    using (var command = new SqlCommand(commandText, conn))
                    {
                        command.CommandType = type;
                        if(parameters != null)
                            foreach (SqlParameter para in parameters)
                                command.Parameters.Add(para);

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            using (var dataTable = new DataTable())
                            {
                                DataSet ds = new DataSet();
                                adapter.Fill(ds);
                                return ds;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static int ExcuteNonQuery(CommandType type, string commandText, SqlParameter[] parameters)
        {
            try
            {
                int result = -1;
                string connString = ConfigurationManager.ConnectionStrings["ShowcaseDB"].ConnectionString;
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (var command = new SqlCommand(commandText, conn))
                    {
                        command.CommandType = type;
                        if (parameters != null)
                            foreach (SqlParameter para in parameters)
                                command.Parameters.Add(para);

                        result = command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
    }

}