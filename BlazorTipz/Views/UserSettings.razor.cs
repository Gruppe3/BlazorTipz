using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using BlazorTipz.ViewModels.User;
namespace BlazorTipz.Views
{
    public partial class UserSettings
    {
        bool popup;
        public string Checker { get; set; }

        UserViewmodel userDto = new UserViewmodel();
        UserViewmodel? CUser;

        
        //checks if there is a current user
        protected override async Task OnInitializedAsync()
        {
            //checks if currentUser is null 
            var CurrentUser = _userManager.getCurrentUser();
            if (CurrentUser == null)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                CUser = CurrentUser;
            }
        }

        //changes the current user details
        public async Task<ActionResult<UserViewmodel>> ChangeSettings(UserViewmodel request)
        {
            if (request.firstTimeLogin == true)
            {
                request.firstTimeLogin = false;
            }

            string err = await _userManager.updateCurrentUser(request);
            if (err != null)
            {
                Checker = err;
                return new BadRequestObjectResult(Checker);
            }
            else
            {
                Checker = "User updated";
                return new OkObjectResult(Checker);
            }

            CUser = _userManager.getCurrentUser();
            if (CUser.password != request.RepeatPassword)
            {
                Checker = "not changed correctly, try again";
                return new BadRequestObjectResult(Checker);
            }

            return CUser;
        }

        // navigate back to home, when used
        private void back()
        {
            NavigationManager.NavigateTo("/", true);
        }

        //Check password of CUser and change settings
        public async Task<ActionResult<string>> CheckPassword(UserViewmodel request)
        {
            string token;
            string err;
            request.employmentId = CUser.employmentId;
            //returns token or err
            (token, err) = await _userManager.Login(request);
            //If error is null
            if (err == null)
            {
                await _localStorage.SetItemAsync("token", token);
                await ChangeSettings(request);
                return token;
            }
            //If token is null
            else if (token == null)
            {
                Checker = err;
                return err;
            }
            //If something else happens
            else
            {
                Checker = "something went horribly wrong";
                return "Fatal";
            }
        }
    }
}