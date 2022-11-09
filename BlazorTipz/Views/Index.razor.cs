using Microsoft.AspNetCore.Components;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using Microsoft.IdentityModel.Tokens;

namespace BlazorTipz.Views
{
    public partial class Index
    {
        bool isLoaded;
        
        List<SuggViewmodel> UserSug = new();
        List<SuggViewmodel> Assignedsugg = new();
        List<CommentViewmodel> Comments = new();
        
        UserViewmodel currentUser = new();
        TeamViewmodel currentTeam = new();
        SuggViewmodel CurrentSugg = new();
        CommentViewmodel CommentDto = new();


        //CSS fields
        string SuggCardHiddenState = "active";
        string Mainpage = "";

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
            //Get team suggestions
            UserSug = await _suggestionManager.GetSuggestionsOfUser(currentUser.employmentId);
            Assignedsugg = await _suggestionManager.GetPreFilteredAssignedSuggestions();
            isLoaded = true;
        }

        private async Task ShowSuggWindow(SuggViewmodel sugg)
        {
            CurrentSugg = sugg;
            if (sugg.Id != null) { await UpdateComments(sugg.Id); }
            SuggCardHiddenState = "active";
        }
        private async Task CloseSuggWindow()
        {
            SuggCardHiddenState = "";
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
    }
}