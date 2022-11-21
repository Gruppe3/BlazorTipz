using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Category = BlazorTipz.ViewModels.Category;

namespace BlazorTipz.Views
{
    public partial class Index
    {
        bool isLoaded;

        private UserViewmodel CurrentUser { get; set; } = new();
        private List<SuggViewmodel> SuggList { get; set; } = new();

        //For suggestion
        private SuggViewmodel CurrentSugg { get; set; } = new();
        public SuggViewmodel SuggUpdate { get; set; } = new();
        private List<TeamViewmodel> ActiveTeams { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        private List<SuggStatus> StatusList { get; set; } = new();
        private List<UserViewmodel> ActiveUsers { get; set; } = new();
        
        //Comments
        private List<CommentViewmodel> Comments { get; set; } = new();
        private CommentViewmodel CommentDto { get; set; } = new();

        //Filter fields
        private int FilterVisning = 0;
        private int FilterState = 7; //Between 0-7 with 7 being these(plan, do, study, act)

        
        //CSS fields
        private string SuggCardHiddenState { get; set; } = "";
        private string Feedback { get; set; } = string.Empty;
        private string SuggShowMore { get; set; } = "show-less";
        private string ErrString { get; set; } = string.Empty;
        private string ErrorCardState { get; set; } = "active";


        //Everytime page loads this runs
        protected override async Task OnInitializedAsync()
        {
            //Gets token
            string token = await _localStorage.GetItemAsync<string>("token");
            //If you do not have a token it redirects you to /login
            if (token == null || token == "")
            {
                _navigationManager.NavigateTo("/login", true);
                return;
            }
            else
            //Returns UserViewmodel or an error
            {
                (UserViewmodel user, string err) = await _userManager.GetCurrentUser(token);
                if (err != null || user == null)
                {
                    _navigationManager.NavigateTo("/login", true);
                    return;
                }

                if (user.FirstTimeLogin == true)
                {
                    _navigationManager.NavigateTo("/userSettings");
                    return;
                }

                //Sets currentUser to user
                CurrentUser = user;
            }
            SuggList = await _suggestionManager.GetFilteredSuggestions(FilterVisning, CurrentUser.EmploymentId);
            

            // ==== Fill for the update-form ====
            StatusList.AddRange(new List<SuggStatus>() { SuggStatus.Plan, SuggStatus.Do, SuggStatus.Study, SuggStatus.Act, SuggStatus.Complete, SuggStatus.Rejected });
            
            //Get active teams
            ActiveTeams = await _teamManager.UpdateTeamsList();
            
            //Fill inn active users along with corresponding teamname
            var users = await _userManager.GetActiveUsers();
            foreach (UserViewmodel u in users)
            {
                await u.GetTeamName(_teamManager);
            }
            ActiveUsers = users;

            (Categories, string? err2) = await _suggestionManager.GetCategories();
            if (err2 != string.Empty) { Feedback = err2; }
            // ==== End of form-filling ====

            //Removes loading
            isLoaded = true;
        }

        private async Task ApplyFilterToSuggList() 
        {
            ErrString = "";
            string empId = CurrentUser.EmploymentId;
            string teamId = CurrentUser.TeamId;

            if (FilterVisning == 2) //Team suggestions
            { 
                if (FilterState == 7) //No chosen state to filter by
                {
                    SuggList = await _suggestionManager.GetFilteredSuggestions(FilterVisning, teamId);
                }
                else //One chosen state to filter by
                {
                    SuggStatus stat = (SuggStatus)FilterState;
                    SuggList = await _suggestionManager.GetFilteredSuggestions(FilterVisning, teamId, stat);
                }
            }
            else //Assigned suggestions, or User's own suggestions
            {
                if (FilterState == 7) //No chosen state to filter by
                {
                    SuggList = await _suggestionManager.GetFilteredSuggestions(FilterVisning, empId);
                }
                else //One chosen state to filter by
                {
                    SuggStatus stat = (SuggStatus)FilterState;
                    SuggList = await _suggestionManager.GetFilteredSuggestions(FilterVisning, empId, stat);
                }
            }

            if (SuggList.Count <= 0)
            {
                //No suggestions found
                ErrString = "Ingen forslag funnet.";
            }
        }

        private async Task UpdateVisning(ChangeEventArgs value)
        {
            if (value != null && value.Value != null)
            {
                string val = (string)value.Value;
                FilterVisning = Int32.Parse(val);
            }
            await ApplyFilterToSuggList();
        }

        private async Task UpdateStatus(ChangeEventArgs value)
        {
            if (value != null && value.Value != null)
            {
                string val = (string)value.Value;
                FilterState = Int32.Parse(val);
            }
            await ApplyFilterToSuggList();
        }

        


        void OnChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
        }

        private async Task ShowSuggWindow(SuggViewmodel sugg)
        {
            CurrentSugg = sugg;
            if (sugg.Id != null) { await UpdateComments(sugg.Id); }

            //SuggProgress = ConvertProgres(sugg.Progression);
            SuggCardHiddenState = "active";
        }
        private void CloseSuggWindow()
        {
            SuggCardHiddenState = "";
            SuggShowMore = "show-less";
        }
		private void ShowMoreToggle()
        {
            if (SuggShowMore == "show-more")
            {
                SuggShowMore = "show-less";
            }
            else
            {
                SuggShowMore = "show-more";
            }
        }
        
        private async Task UpdateComments(string suggId)
        {
            string respons;
            List<CommentViewmodel> comments;
            
            (comments, respons) = await _suggestionManager.GetComments(suggId);
            if (respons.Equals("Success"))
            {
                Comments = comments;
            }
        }
        private async Task SaveComment(string SugId, CommentViewmodel comment)
        {
            if (comment.Comment.IsNullOrEmpty()) { return; }

            comment.SugId = SugId;
            comment.EmpId = CurrentUser.EmploymentId;
            string respons = await _suggestionManager.SaveComment(comment);
            if (respons.Equals("Kommentar lagret"))
            {
                await UpdateComments(SugId);
                CommentDto = new();
            }
        }
        public async Task UpdateSugg()
        {
            if (SuggUpdate.Id != null)
            {
                string? err = await _suggestionManager.UpdateSuggestion(SuggUpdate, CurrentUser);
                if (err != null)
                {
                    ErrorCardState = "active";
                    Feedback = err;
                }
                else
                {
                    await ApplyFilterToSuggList();
                    Feedback = "Forslag er oppdatert";
                }
            }
            else
            {
                ErrorCardState = "active";
                Feedback = "Noe er galt. Forslag kan ikke identifiseres.";
            }
        }
    }
}