using System.Data;
using MySql.Data.MySqlClient;

namespace Kursach;

public class Database
{
    private MySqlConnection _connection = new MySqlConnection(@"server=localhost;database=Мара;port=3306;User Id=root;password=Password12");
    
    public void openConnection() 
    {
        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }
    }
    public void closeConnection() 
    {
        if (_connection.State == ConnectionState.Open)
        {
            _connection.Close();
        }
    }
    public MySqlConnection getConnection() 
    {
        return _connection;
    }
}