using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Kursach.Players;

public partial class PlayerWndw : UserControl
{
    private Database _database = new Database();
    private List<Players> _players = new List<Players>();
    
    private string _fullTable = "select player_id, firstname, nickname, secondname, nation, status from players " +
                                "join crm.players_status cs on cs.players_status_id = players.players_status " +
                                "order by client_id;";
    public PlayerWndw()
    {
        InitializeComponent();
    }
    public void ShowTable(string sql)
    {
        _database.openConnection();
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.HasRows && reader.Read())
        {
            var currentClient = new Players()
            {
                PlayerId = reader.GetInt32("player_id"),
                Name = reader.GetString("firstname"),
                Nickname = reader.GetString("nickname"),
                SecondName = reader.GetString("secondname"),
                Nation = reader.GetInt32("nation"),
                Status = reader.GetInt32("status")
            };
            _players.Add(currentClient);
        }
        _database.closeConnection();
        PlayersGrid.ItemsSource = _players;
    }
    private void AddPlayerBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddPlayer addClientWindow = new AddPlayer();
        addClientWindow.Show();
    }

    private void DeletePlayerBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "delete from players where player_id = @PlayerId";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@PlayerId", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private void EditPlayerBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
}