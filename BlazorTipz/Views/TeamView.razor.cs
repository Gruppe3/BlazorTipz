using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using DocumentFormat.OpenXml.Wordprocessing;
using Radzen;
using System.Collections.Generic;
using Category = BlazorTipz.ViewModels.Category;

namespace BlazorTipz.Views
{
    public partial class TeamView
    {
        public UserViewmodel currentUser = new UserViewmodel();
        public TeamViewmodel currentTeam = new TeamViewmodel();
        UserViewmodel CUser;
        TeamViewmodel Cteam;
        public SuggViewmodel suggUpdate = new SuggViewmodel();
        public List<Category>? Categories;
       
        private List<UserViewmodel> users = new List<UserViewmodel>();
        private List<TeamViewmodel> teams = new List<TeamViewmodel>();
        private List<SuggStatus> statuses = new List<SuggStatus>(); 

        List<SuggViewmodel> teamSug = new List<SuggViewmodel>();

        public string TeamCheck { get; set; }
        public string teamU { get; set; }
        public string Feedback { get; set; }


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
            List<Category> toset = _suggestionManager.GetCategories();
            Categories = toset;
            users = await _userManager.GetUsers();
            statuses.AddRange(new List<SuggStatus>() { SuggStatus.Plan, SuggStatus.Do, SuggStatus.Study, SuggStatus.Act, SuggStatus.Complete, SuggStatus.Rejected });
        }

        //Update DB
        public void UpdateDB()
        {
            NavigationManager.NavigateTo("/teamView", true);
        }
        void OnChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
        }
        public async Task updateSugg() {
            if (suggUpdate.Id != null)
            {
                string? err;
                //suggUpdate.SetFristTidToFrist();
                err = await _suggestionManager.UpdateSuggestion(suggUpdate,CUser);
                if (err != null)
                {
                    Feedback = err;
                }
                else
                {
                    Feedback = "Suggestion updated";
                }
            }
            else
            {
                Feedback = "Suggestion not found";
            }
            UpdateDB();
        }

    }
}
