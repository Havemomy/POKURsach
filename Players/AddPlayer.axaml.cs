using System;
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
            command.Parameters.AddWithValue("@nickname", NickmaneTxt.Text);
            command.Parameters.AddWithValue("@secondname", ScndTxt.Text);
            int selectedNationId = GetSelectedStatusId(NationCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("nation", selectedNationId);
            int selectedStatusId = GetSelectedStatusId(StatusCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("status", selectedStatusId);
            command.ExecuteNonQuery();
            this.Close();
        }
        _database.closeConnection();
    }
    private void LoadDataStatusCmb()
    {
        _database.openConnection();
        string sql = "select status_name from status;";
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
        string sql = "select client_status_id from client_status where status_name = @id";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}