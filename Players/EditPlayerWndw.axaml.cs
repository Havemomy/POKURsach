using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace Kursach.Players;

public partial class EditPlayerWndw : Window
{
    private Database _database = new Database();
    private Players _players;
    
    public EditPlayerWndw(Players players)
    {
        InitializeComponent();
        _players = players;
        NameTxt.Text = _players.Name;
        NicknameTxt.Text = _players.Nickname;
        SecondNameTxt.Text = _players.SecondName;
        NationCmb.SelectedItem = _players.Nation;
        StatusCmb.SelectedItem = _players.Status;
        LoadDataNationCmb();
        LoadDataStatusCmb();
        Height = 400;
        Width = 350;
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
            int id = _players.PlayerId;
            _database.openConnection();
            string sql =
                "UPDATE players SET Name = @name, Nickname = @nickname, SecondName = @secondname, Nation = @nation, Status = @status WHERE PlayerId = @id";
            MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
            command.Parameters.AddWithValue("@name", NameTxt.Text);
            command.Parameters.AddWithValue("@nickname", NicknameTxt.Text);
            command.Parameters.AddWithValue("@secondname", SecondNameTxt.Text);
            command.Parameters.AddWithValue("@nation", NationCmb.SelectedValue);
            command.Parameters.AddWithValue("@status", GetSelectedStatusId(StatusCmb.SelectedItem.ToString()));
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            _database.closeConnection();
            var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно изменены", ButtonEnum.Ok);
            var result = box.ShowAsync();
            this.Close();
    }
    private void LoadDataNationCmb()
    {
        _database.openConnection();
        string sql = "select name, NationId from nation;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var nation = new Nation();
                nation.NationId = reader.GetInt32("NationId");
                nation.Name = reader.GetString("Name");
                NationCmb.Items.Add(nation);
            }
        }
        _database.closeConnection();
        NationCmb.DisplayMemberBinding = new Binding("Name");
        NationCmb.SelectedValueBinding = new Binding("NationId");
    }
    private int GetSelectedNationId(string selectedNation)
    {
        _database.openConnection();
        string sql = "select nation from players where nation = @selectedNation";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedNation", selectedNation);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
    
    
    private void LoadDataStatusCmb()
    {
        _database.openConnection();
        string sql = "select name from status;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                StatusCmb.Items.Add(reader["name"].ToString());
            }
        }
        _database.closeConnection();
    }
    
    private int GetSelectedStatusId(string selectedStatus)
    {
        _database.openConnection();
        string sql = "SELECT StatusID FROM status WHERE name = @selectedStatus";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
        object result = command.ExecuteScalar();
        
        if (result != null)
        {
            int selectedId = Convert.ToInt32(result);
            return selectedId;
        }
        else
        {
            throw new Exception("Значение не найдено в базе данных");
        }
    }
}