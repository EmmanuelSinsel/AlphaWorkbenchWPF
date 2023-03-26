
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Shapes;


namespace AlphaWorkbenchWPF
{
    internal class tests : data_manager
    {
        dbManager dbm = new dbManager();
        SqlConnection cnn;


        public tests()
        {
            cnn = dbm.conection();
            byte[] imgdata = System.IO.File.ReadAllBytes("C:\\Users\\thega\\OneDrive\\Escritorio\\38030593_1067384420085344_6325595483855126528_n.jpg");
            SqlCommand cmd = new SqlCommand("update Usuarios set FotoPerfil = @foto Where Usuario = 'MelinaSG'", cnn);
            cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = imgdata;
            cmd.ExecuteNonQuery();
        }



    }
}
