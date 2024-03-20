using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Kursach.Teams;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace Kursach.TournamentsFinished;

public partial class TournamWndw : UserControl
{
    private Database _database = new Database();
    private List<Tournaments> _tournaments = new List<Tournaments>();

    private string _fullTable =
        "select TournamentId, tournaments.Name, Organizer, Location, t.Name as Type, i.Name as TakiTier from tournaments" +
        " join мара.tournamenttype t on tournaments.Type = t.TypeId " +
        " join мара.takitier i on tournaments.TakiTier = i.TierId";
    public TournamWndw()
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
            var currentTournament = new Tournaments()
            {
                TournamentId = reader.GetInt32("TournamentId"),
                Name = reader.GetString("Name"),
                Organizer = reader.GetString("Organizer"),
                Location = reader.GetString("Location"),
                Type = reader.GetString("Type"),
                TakiTier = reader.GetString("TakiTier")
            };
            _tournaments.Add(currentTournament);
        }
        _database.closeConnection();
        TournamentGrid.ItemsSource = _tournaments;
    }

    private void AddTournamentBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddTournament addTournam = new AddTournament();
        addTournam.Show();
    }
    
    public void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from tournaments where tournamentid = @TournamentId";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@TournamentId", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteTournamenBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Tournaments selectedTournament = TournamentGrid.SelectedItem as Tournaments;
        if (selectedTournament != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить турнир?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedTournament.TournamentId);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Турнир успешно удален!", ButtonEnum.Ok);
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
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите турнир для удаления", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void EditTournamentBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Tournaments selectedTournament = TournamentGrid.SelectedItem as Tournaments;
        if (selectedTournament != null)
        {
            EditTournamentWndw ditTournamentWndw = new EditTournamentWndw(selectedTournament);
            ditTournamentWndw.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите турнир для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }
}