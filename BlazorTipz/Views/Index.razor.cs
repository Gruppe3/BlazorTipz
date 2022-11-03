using Microsoft.AspNetCore.Components;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.Views
{
    public partial class Index
    {
        bool isLoaded;
        
        List<SuggViewmodel> UserSug = new();
        List<SuggViewmodel> Assignedsugg = new();
        UserViewmodel currentUser = new();
        TeamViewmodel currentTeam = new();
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
    }
}