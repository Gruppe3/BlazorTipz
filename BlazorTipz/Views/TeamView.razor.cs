using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using Category = BlazorTipz.ViewModels.Category;

namespace BlazorTipz.Views
{
    public partial class TeamView
    {
        private UserViewmodel CurrentUser { get; set; } = new();
        private TeamViewmodel CurrentTeam { get; set; } = new();
        private SuggViewmodel SuggUpdate { get; set; } = new();
        private List<Category> Categories { get; set; } = new();

        private List<UserViewmodel> ActiveUsers = new();
        private List<TeamViewmodel> ActiveTeams = new();
        private List<SuggStatus> StatusList = new(); 

        List<SuggViewmodel> TeamSuggestions = new();

        //public string TeamCheck { get; set; }
        //public string teamU { get; set; }
        public string Feedback { get; set; }
        private bool EditDisable { get; set; } = false;


        //Get team from user
        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            if (token != null)
            {
                // If a token is found
                (UserViewmodel user, string err) = await _userManager.GetCurrentUser(token);
                TeamViewmodel team = await _teamManager.GetTeamById(user.TeamId);
                if (err != null)
                {
                    //If error, send to login
                    _navigationManager.NavigateTo("/login", true);
                    return;
                }

                CurrentUser = user;
                CurrentTeam = team;
            }
            else
            {
                _navigationManager.NavigateTo("/", true);
                return;
            }

            
            //Get team suggestions
            TeamSuggestions = await _suggestionManager.GetSuggestionsOfTeam(CurrentTeam.TeamId);
            StatusList.AddRange(new List<SuggStatus>() { SuggStatus.Plan, SuggStatus.Do, SuggStatus.Study, SuggStatus.Act, SuggStatus.Complete, SuggStatus.Rejected });
            ActiveTeams = await _teamManager.UpdateTeamsList();
            
            (Categories, string err2) = await _suggestionManager.GetCategories();
            if (err2 != string.Empty) { Feedback = err2; }
            
            var users = await _userManager.GetActiveUsers();
            foreach (UserViewmodel u in users)
            {
                await u.GetTeamName(_teamManager);
            }
            ActiveUsers = users;
            
        }

        //Update DB
        public void UpdateDB()
        {
            _navigationManager.NavigateTo("/teamView", true);
        }
        void OnChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
        }
        public async Task UpdateSugg() {
            if (SuggUpdate.Id != null)
            {
                string? err;
                //suggUpdate.SetFristTidToFrist();
                err = await _suggestionManager.UpdateSuggestion(SuggUpdate,CurrentUser);
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
