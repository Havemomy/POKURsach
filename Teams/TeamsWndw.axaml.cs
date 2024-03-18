using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace Kursach.Teams;

public partial class TeamsWndw : UserControl
{
    private Database _database = new Database();
    private List<Teams> _teams = new List<Teams>();
    
    private string _fullTable =
        "select TeamId, TeamName, Coach, p.Nickname as Captain, r.Name as Region from teams" +
        " join мара.players p on teams.Captain = p.PlayerId " +
        " join мара.region r on teams.Region = r.RegionId";
    public TeamsWndw()
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
            var currentPlayer = new Teams()
            {
                TeamId = reader.GetInt32("TeamId"),
                TeamName = reader.GetString("TeamName"),
                Coach = reader.GetString("Coach"),
                Captain = reader.GetString("Captain"),
                Region = reader.GetString("Region"),
            };
            _teams.Add(currentPlayer);
        }
        _database.closeConnection();
        TeamGrid.ItemsSource = _teams;
    }

    private void AddTeamBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddTeam addTeam = new AddTeam();
        addTeam.Show();
    }
    
    public void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from teams where TeamId = @TeamId";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@TeamId", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteTeamBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Teams selectedTeam = TeamGrid.SelectedItem as Teams;
        if (selectedTeam != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить клиента?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedTeam.TeamId);
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
    
    private void EditTeamBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Teams selectedTeams = TeamGrid.SelectedItem as Teams;
        if (selectedTeams != null)
        {
            EditTeamWndw ditTeamWndwWndw = new EditTeamWndw(selectedTeams);
            ditTeamWndwWndw.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите клиента для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }
}