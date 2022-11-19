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
        private List<SuggViewmodel> UserSugg { get; set; } = new();
        private List<SuggViewmodel> Assignedsugg { get; set; } = new();
        private List<SuggViewmodel> SuggList { get; set; } = new();

        //For suggestion
        private SuggViewmodel CurrentSugg { get; set; } = new();
        public SuggViewmodel SuggUpdate { get; set; } = new();
        private List<TeamViewmodel> Teams { get; set; } = new();
        public List<Category>? Categories { get; set; }
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
        private string SuggDisplayState { get; set; } = "loading";
        private string ErrString { get; set; } = string.Empty;


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
                CurrentUser = user;
            }
            StatusList.AddRange(new List<SuggStatus>() { SuggStatus.Plan, SuggStatus.Do, SuggStatus.Study, SuggStatus.Act, SuggStatus.Complete, SuggStatus.Rejected });
            //get active teams
            Teams = await _teamManager.updateTeamsList();
            //Get team suggestions
            //UserSugg = await _suggestionManager.GetSuggestionsOfUser(CurrentUser.employmentId);
            //Assignedsugg = await _suggestionManager.GetPreFilteredAssignedSuggestions(CurrentUser.employmentId);

            //SuggList = await _suggestionManager.GetPreFilteredAssignedSuggestions(CurrentUser.employmentId);
            SuggList = await _suggestionManager.GetFilteredSuggestions(FilterVisning, CurrentUser.employmentId);

            SuggDisplayState = ""; //removes loading
            isLoaded = true;
        }

        private async Task ApplyFilterToSuggList() 
        {
            ErrString = "";
            SuggDisplayState = "loading";
            string empId = CurrentUser.employmentId;
            string teamId = CurrentUser.teamId;

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
            SuggDisplayState = "";
        }

        private async Task UpdateVisning(ChangeEventArgs value)
        {
            if (value != null && value.Value != null)
            {
                string val = (string)value.Value;
                //int x = Int32.Parse(val);
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
            comment.EmpId = CurrentUser.employmentId;
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
                string? err;
                //suggUpdate.SetFristTidToFrist();
                err = await _suggestionManager.UpdateSuggestion(SuggUpdate, CurrentUser);
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