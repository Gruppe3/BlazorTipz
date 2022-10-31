using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using DocumentFormat.OpenXml.Wordprocessing;
using Radzen;

namespace BlazorTipz.Views
{
    public partial class TeamView
    {
        public UserViewmodel currentUser = new UserViewmodel();
        public TeamViewmodel currentTeam = new TeamViewmodel();
        UserViewmodel CUser;
        TeamViewmodel Cteam;
        private List<TeamViewmodel> teams = new List<TeamViewmodel>();
        List<SuggViewmodel> teamSug = new List<SuggViewmodel>();

        public string TeamCheck { get; set; }
        public string teamU { get; set; }


        //Get team from user
        protected override async Task OnInitializedAsync()
        {

            var token = await _localStorage.GetItemAsync<string>("token");
            if (token != null)
            {
                (UserViewmodel user, string err) = await _userManager.getCurrentUser(token);
                TeamViewmodel team = await _teamManager.getTeam(user.teamId);
                if (err != null)
                {
                    return;
                }

                currentUser = user;
                currentTeam = team;
            }

            var CurrentUser = _userManager.getCurrentUser();
            teams = await _teamManager.updateTeamsList();
            if (CurrentUser != null)
            {
                CUser = CurrentUser;
                Cteam = await _teamManager.getTeam(CUser.teamId);
                teamU = Cteam.name;
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
            //Get team suggestions
            teamSug = await _suggestionManager.GetSuggestionsOfTeam(currentTeam.id);
        }

        //Update DB
        public void UpdateDB()
        {
            NavigationManager.NavigateTo("/teamView", true);
        }

        
    }
}
