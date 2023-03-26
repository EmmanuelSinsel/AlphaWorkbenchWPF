using Microsoft.Data.SqlClient;

namespace AlphaWorkbenchWPF
{
    public class dbManager : sqlRequests
    {

        public SqlConnection conection()
        {
            SqlConnection cnn;
            string connetionString;
            connetionString = "Data Source=DESKTOP-3MO2286\\SQLEXPRESS;Initial Catalog=AlphaWorkbench; User id=sa;Password=1234;Encrypt=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            return cnn;
        }
    }
}
