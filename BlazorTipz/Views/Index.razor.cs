using Microsoft.AspNetCore.Components;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.Views
{
    public partial class Index
    {
        UserViewmodel currentUser = new UserViewmodel();
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
        }

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("login");
        }
    }
}