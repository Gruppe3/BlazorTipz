using Microsoft.AspNetCore.Components;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.Views
{
    public partial class Index
    {
        bool isLoaded;
        double value = 19;
        
        List<SuggViewmodel> UserSug = new List<SuggViewmodel>();
        UserViewmodel currentUser = new UserViewmodel();
        TeamViewmodel currentTeam = new TeamViewmodel();
        //Everytime page loads this runs
        protected override async Task OnInitializedAsync()
        {
            //Gets token
            string token = await _localStorage.GetItemAsync<string>("token");
            //If you do not have a token it redirects you to /login
            if (token == null || token == "")
            {
                NavigationManager.NavigateTo("/login", true);
            }
            else
            //Returns UserViewmodel or an error
            {
                (UserViewmodel user, string err) = await _userManager.getCurrentUser(token);
                if (err != null || user == null)
                {
                    NavigationManager.NavigateTo("/login", true);
                }

                if (user.firstTimeLogin == true)
                {
                    NavigationManager.NavigateTo("/userSettings");
                }

                //Sets currentUser to user
                currentUser = user;
            }
            

            //Get team suggestions
            UserSug = await _suggestionManager.GetSuggestionsOfUser(currentUser.employmentId);
            isLoaded = true;
        }
        

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("login");
        }
    }
}