using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Type = Google.Protobuf.WellKnownTypes.Type;

namespace Kursach.TournamentsFinished;

public partial class EditTournamentWndw : Window
{
    private Database _database = new Database();
    private Tournaments _tournaments;
    public EditTournamentWndw(Tournaments tournaments)
    {
        InitializeComponent();
        _tournaments = tournaments;
        NameTxt.Text = _tournaments.Name;
        OrganizerTxt.Text = _tournaments.Organizer;
        LocationTxt.Text = _tournaments.Location;
        TypeCmb.SelectedItem = _tournaments.Type;
        TirerCmb.SelectedItem = _tournaments.TakiTier;
        LoadDataTypeCmb();
        LoadDataTierCmb();
        Height = 400;
        Width = 350;
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        int id = _tournaments.TournamentId;
        _database.openConnection();
        string sql =
            "UPDATE tournaments SET Name = @name, Organizer = @organizer, Location = @location, Type = @type, TakiTier = @tier WHERE TournamentId = @id";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@name", NameTxt.Text);
        command.Parameters.AddWithValue("@organizer", OrganizerTxt.Text);
        command.Parameters.AddWithValue("@location", LocationTxt.Text);
        command.Parameters.AddWithValue("@type", TypeCmb.SelectedValue);
        command.Parameters.AddWithValue("@tier", GetSelectedStatusId(TirerCmb.SelectedItem.ToString()));
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно изменены", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }
    private void LoadDataTypeCmb()
    {
        _database.openConnection();
        string sql = "select name, TypeId from tournamenttype;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var tournamenttype = new TypeOfTournament();
                tournamenttype.TypeId = reader.GetInt32("TypeId");
                tournamenttype.Name = reader.GetString("Name");
                TypeCmb.Items.Add(tournamenttype);
            }
        }
        _database.closeConnection();
        TypeCmb.DisplayMemberBinding = new Binding("Name");
        TypeCmb.SelectedValueBinding = new Binding("TypeId");
    }
    private void LoadDataTierCmb()
    {
        _database.openConnection();
        string sql = "select name from takitier;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                TirerCmb.Items.Add(reader["name"].ToString());
            }
        }
        _database.closeConnection();
    }
    
    private int GetSelectedStatusId(string selectedTier)
    {
        _database.openConnection();
        string sql = "SELECT TierId FROM takitier WHERE name = @selectedTier";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedTier", selectedTier);
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