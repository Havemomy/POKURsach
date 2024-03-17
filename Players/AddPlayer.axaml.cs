using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Kursach.Players;

public partial class AddPlayer : Window
{
    private Database _database = new Database();
    public AddPlayer()
    {
        InitializeComponent();
        LoadDataStatusCmb();
        Width = 400;
        Height = 300;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "insert into players (name, nickname, secondname, nation, status ) " +
                     "values (@name, @nickname, @secondname, @nation, @status)";
        using (MySqlCommand command = new MySqlCommand(sql, _database.getConnection()))
        {
            command.Parameters.AddWithValue("@name", NameTxt.Text);
            command.Parameters.AddWithValue("@nickname", SurnameTxt.Text);
            command.Parameters.AddWithValue("@secondname", PhoneTxt.Text);
            command.Parameters.AddWithValue("@nation", EmailTxt.Text);
            int selectedStatusId = GetSelectedStatusId(StatusCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("status", selectedStatusId);
            command.ExecuteNonQuery();
            var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Успешно добавлен!", ButtonEnum.Ok);
            var result = box.ShowAsync();
            this.Close();
        }
        _database.closeConnection();
    }
    private void LoadDataStatusCmb()
    {
        _database.openConnection();
        string sql = "select status_name from client_status;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                StatusCmb.Items.Add(reader["status_name"].ToString());
            }
        }
        _database.closeConnection();
    }

    private int GetSelectedStatusId(string selectedStatus)
    {
        _database.openConnection();
        string sql = "select client_status_id from client_status where status_name = @selectedStatus";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}