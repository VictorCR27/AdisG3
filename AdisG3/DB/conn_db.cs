using System;
using MySql.Data.MySqlClient;

namespace AdisG3
{
    public class conn_db
    {
        public static string GetConnectionString()
        {
            string conn = "server=192.185.77.249; port=3306; database=visualcr_dw; Uid=visualcr_dwUser; Pwd=Uni2023op&;";
            return conn;
        }
    }

}




