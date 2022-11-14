using Microsoft.AspNetCore.Components;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using Microsoft.IdentityModel.Tokens;
using BlazorTipz.Data;
using Category = BlazorTipz.ViewModels.Category;

namespace BlazorTipz.Views
{
    public partial class Index
    {
        bool isLoaded;
        
        List<SuggViewmodel> UserSug = new();
        List<SuggViewmodel> Assignedsugg = new();
        List<CommentViewmodel> Comments = new();
        private List<UserViewmodel> users = new List<UserViewmodel>();
        private List<TeamViewmodel> teams = new List<TeamViewmodel>();
        public List<Category>? Categories;

        UserViewmodel currentUser = new();
        TeamViewmodel currentTeam = new();
        SuggViewmodel CurrentSugg = new();
        CommentViewmodel CommentDto = new();
        public SuggViewmodel suggUpdate = new SuggViewmodel();
        private List<SuggStatus> statuses = new List<SuggStatus>();

        //CSS fields
        private string SuggCardHiddenState { get; set; } = "";
        private string Feedback { get; set; } 
        private string SuggShowMore = "show-less";

        //Everytime page loads this runs
        protected override async Task OnInitializedAsync()
        {
            //Gets token
            string token = await _localStorage.GetItemAsync<string>("token");
            //If you do not have a token it redirects you to /login
            if (token == null || token == "")
            {
                NavigationManager.NavigateTo("/login", true);
                return;
            }
            else
            //Returns UserViewmodel or an error
            {
                (UserViewmodel user, string err) = await _userManager.getCurrentUser(token);
                if (err != null || user == null)
                {
                    NavigationManager.NavigateTo("/login", true);
                    return;
                }

                if (user.firstTimeLogin == true)
                {
                    NavigationManager.NavigateTo("/userSettings");
                    return;
                }

                //Sets currentUser to user
                currentUser = user;
            }
            statuses.AddRange(new List<SuggStatus>() { SuggStatus.Plan, SuggStatus.Do, SuggStatus.Study, SuggStatus.Act, SuggStatus.Complete, SuggStatus.Rejected });
            //get active teams
            teams = await _teamManager.updateTeamsList();
            //Get team suggestions
            UserSug = await _suggestionManager.GetSuggestionsOfUser(currentUser.employmentId);
            Assignedsugg = await _suggestionManager.GetPreFilteredAssignedSuggestions();
            isLoaded = true;
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
            string respons = string.Empty;
            List<CommentViewmodel> comments = new();
            
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
            comment.EmpId = currentUser.employmentId;
            string respons = await _suggestionManager.SaveComment(comment);
            if (respons.Equals("Kommentar lagret"))
            {
                await UpdateComments(SugId);
                CommentDto = new();
            }
        }
        public async Task updateSugg()
        {
            if (suggUpdate.Id != null)
            {
                string? err;
                //suggUpdate.SetFristTidToFrist();
                err = await _suggestionManager.UpdateSuggestion(suggUpdate, currentUser);
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
            
        }
    }
}