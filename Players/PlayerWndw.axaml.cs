using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Kursach.Players;

public partial class PlayerWndw : UserControl
{
    private Database _database = new Database();
    private List<Players> _players = new List<Players>();

    private string _fullTable =
        "select PlayerId, players.Name, Nickname, SecondName, n.Name as Nation, s.Name as Status from players" +
        " join мара.nation n on players.Nation = n.NationID " +
        " join мара.status s on players.Status = s.StatusID";
    public PlayerWndw()
    {
        InitializeComponent();
        ShowTable(_fullTable);
    }
    public void ShowTable(string sql)
    {
        _database.openConnection();
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.HasRows && reader.Read())
        {
            var currentPlayer = new Players()
            {
                PlayerId = reader.GetInt32("PlayerId"),
                Name = reader.GetString("Name"),
                Nickname = reader.GetString("Nickname"),
                SecondName = reader.GetString("SecondName"),
                Nation = reader.GetString("Nation"),
                Status = reader.GetString("Status")
            };
            _players.Add(currentPlayer);
        }
        _database.closeConnection();
        PlayersGrid.ItemsSource = _players;
    }
    private void AddPlayerBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddPlayer addPlayer = new AddPlayer();
        addPlayer.Show();
    }
    public void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from players where playerId = @PlayerId";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@PlayerId", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }
    private async void DeletePlayerBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Players selectedClient = PlayersGrid.SelectedItem as Players;
        if (selectedClient != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить клиента?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedClient.PlayerId);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Клиент успешно удален!", ButtonEnum.Ok);
                var successResult = box.ShowAsync();
            }
            else
            {
                var cancelBox = MessageBoxManager.GetMessageBoxStandard("Отмена", "Операция удаления отменена", ButtonEnum.Ok);
                var cancelResult = cancelBox.ShowAsync();
            }
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите клиента для удаления", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void EditPlayerBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Players selectedPlayers = PlayersGrid.SelectedItem as Players;
        if (selectedPlayers != null)
        {
            EditPlayerWndw ditPlayerWndw = new EditPlayerWndw(selectedPlayers);
            ditPlayerWndw.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите клиента для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }
}


