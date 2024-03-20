using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace Kursach.TournamentsFinished;

public partial class AddTournament : Window
{
    private Database _database = new Database();
    public AddTournament()
    {
        InitializeComponent();
        LoadDataTypeCmb();
        LoadDataTierCmb();
        Width = 400;
        Height = 300;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "insert into tournaments (Name, Organizer, Location, Type, TakiTier ) " +
                     "values (@name, @organizer, @location, @type, @tier)";
        using (MySqlCommand command = new MySqlCommand(sql, _database.getConnection()))
        {
            command.Parameters.AddWithValue("@name", NameTxt.Text);
            command.Parameters.AddWithValue("@organizer", OrgTxt.Text);
            command.Parameters.AddWithValue("@location", LocTxt.Text);
            int selectedNationId = GetSelectedTypeId(TypeCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("@type", selectedNationId);
            int selectedStatusId = GetSelectedTierId(TierCmb.SelectedItem.ToString());
            command.Parameters.AddWithValue("@tier", selectedStatusId);
            command.ExecuteNonQuery();
            this.Close();
        }
        _database.closeConnection();
    }
    private void LoadDataTypeCmb()
    {
        _database.openConnection();
        string sql = "select Name from tournamenttype;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                TypeCmb.Items.Add(reader["Name"].ToString());
            }
        }
        _database.closeConnection();
    }

    private int GetSelectedTypeId(string selectedtype)
    {
        _database.openConnection();
        string sql = "select TypeId from tournamenttype where Name = @selectedtype";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedtype", selectedtype);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
    private void LoadDataTierCmb()
    {
        _database.openConnection();
        string sql = "select Name from takitier;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                TierCmb.Items.Add(reader["Name"].ToString());
            }
        }
        _database.closeConnection();
    }

    private int GetSelectedTierId(string selectedTier)
    {
        _database.openConnection();
        string sql = "select TierId from takitier where name = @selectedTier";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedTier", selectedTier);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}