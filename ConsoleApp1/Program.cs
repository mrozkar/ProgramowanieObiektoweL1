using Microsoft.Data.SqlClient

    public class Program
{
    public static void Main()
    {
        string connectionString =
        "Data Source=10.200.2.28;" + //"(LocalDb)\\MSSQLLocalDB;" - dla lokalnej bazy
        "Initial Catalog=studenci_{numer_albumu};" + //USTAW SWÓJ NUMER!
       "Integrated Security=True;" +
        "Encrypt=True;" +
        "TrustServerCertificate=True";
        try
        {
            using SqlConnection connection = new
           SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Połączono z bazą.");
        }
        catch (Exception exc)
        {
            Console.WriteLine("Wystąpił błąd: " + exc);
        }

    }
}
