using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Kursach.Teams;

public partial class AddTeam : Window
{
    private Database _database = new Database();
    public AddTeam()
    {
        InitializeComponent();
        LoadDataCaptainCmb();
        LoadDataRegionCmb();
        Width = 400;
        Height = 300;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "INSERT INTO teams (Teamname, Coach, Captain, Region) " +
                     "VALUES (@teamname, @coach, @captain, @region)";
        using (MySqlCommand command = new MySqlCommand(sql, _database.getConnection()))
        {
            command.Parameters.AddWithValue("@teamname", TeamNameTxt.Text);
            command.Parameters.AddWithValue("@coach", CoachTxt.Text);
            int selectedCaptainId = GetSelectedCaptainId(CaptainCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("@captain", selectedCaptainId);
            int selectedRegionId = GetSelectedRegionId(RegionCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("@region", selectedRegionId);
            command.ExecuteNonQuery();
            this.Close();
        }
        _database.closeConnection();
        
    }
    private void LoadDataCaptainCmb()
    {
        _database.openConnection();
        string sql = "select Nickname from players;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                CaptainCmb.Items.Add(reader["Nickname"].ToString());
            }
        }
        _database.closeConnection();
    }
    private int GetSelectedCaptainId(string selectedStatus)
    {
        _database.openConnection();
        string sql = "select PlayerId from players where Name = @selectedPlayer";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedPlayer", selectedStatus);
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
        string sql = "select RegionId from region where Name = @selectedRegion";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedRegion", selectedRegion);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}