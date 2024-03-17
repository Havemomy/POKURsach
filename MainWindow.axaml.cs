using Avalonia.Controls;
using Avalonia.Interactivity;
using Kursach.Players;
using Kursach.Teams;
using Kursach.TournamentsFinished;

namespace Kursach;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Width = 1000;
        Height = 450;
    }
    private void PlayersBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        PlayerWndw playerWndw = new PlayerWndw();
        MainPanel.Children.Add(playerWndw);
    }
    private void TeamsBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            MainPanel.Children.Clear();
            TeamsWndw teamsWndw = new TeamsWndw();
            MainPanel.Children.Add(teamsWndw);
        }

    private void TournamentsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        TournamWndw tournamWndw = new TournamWndw();
        MainPanel.Children.Add(tournamWndw);
    }
    private void ExitBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}