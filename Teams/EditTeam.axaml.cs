using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Kursach.Players;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace Kursach.Teams;

public partial class EditTeam : Window
{
    private Database _database = new Database();
    private Teams _teams;
    
    public EditTeam(Teams teams)
    {
        InitializeComponent();
        _teams = teams;
        TeamNameTxt.Text = _teams.TeamName;
        CoachTxt.Text = _teams.Coach;
        CaptainCmb.SelectedItem = _teams.Captain;
        RegionCmb.SelectedItem = _teams.Region;
        LoadDataCaptainCmb();
        LoadDataRegionCmb();
        Height = 400;
        Width = 350;
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
            int id = _teams.TeamId;
            _database.openConnection();
            string sql =
                "UPDATE teams SET TeamName = @teamname, Coach = @coach, Captain = @captain, Region = @region WHERE TeamId = @id";
            MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
            command.Parameters.AddWithValue("@teamname", TeamNameTxt);
            command.Parameters.AddWithValue("@coach", CoachTxt.Text);
            
            command.Parameters.AddWithValue("@Captain", GetSelectedCaptainId(CaptainCmb.SelectedItem.ToString()));
            command.Parameters.AddWithValue("@Region", GetSelectedRegionId(RegionCmb.SelectedItem.ToString()));
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            _database.closeConnection();
            var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно изменены", ButtonEnum.Ok);
            var result = box.ShowAsync();
            this.Close();
            TeamsWndw teamsWndw = new TeamsWndw();
            teamsWndw.ShowTable(sql);
            this.Close();
        
    }
    private void LoadDataCaptainCmb()
    {
        _database.openConnection();
        string sql = "select nickname from players;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                CaptainCmb.Items.Add(reader["nickname"].ToString());
            }
        }
        _database.closeConnection();
    }
    private int GetSelectedCaptainId(string selectedCaptain)
    {
        _database.openConnection();
        string sql = "select captain from teams where PlayerId = @selectedPlayer";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedPlayer", selectedCaptain);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
    
    
    private void LoadDataRegionCmb()
    {
        _database.openConnection();
        string sql = "select Name from region;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                RegionCmb.Items.Add(reader["Name"].ToString());
            }
        }
        _database.closeConnection();
    }
    
    private int GetSelectedRegionId(string selectedRegion)
    {
        _database.openConnection();
        string sql = "SELECT RegionId FROM region WHERE name = @selectedRegion";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedRegion", selectedRegion);
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