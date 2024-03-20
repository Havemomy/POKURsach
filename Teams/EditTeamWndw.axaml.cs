using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kursach.TournamentsFinished;

namespace Kursach.Teams;

public partial class EditTeamWndw : Window
{
    public EditTeamWndw(Teams selectedTeams)
    {
        InitializeComponent();
    }
}