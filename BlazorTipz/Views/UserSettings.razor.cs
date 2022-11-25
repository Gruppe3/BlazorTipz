using BlazorTipz.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTipz.Views
{
    public partial class UserSettings
    {
        bool popup;
        public string Checker { get; set; } = string.Empty;
        UserViewmodel UserDto { get; set; } = new();
        UserViewmodel CurrentUser { get; set; } = new();
        UserViewmodel LoginCheckUser { get; set; } = new();


        //checks if there is a current user
        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            if (token != null)
            {
                // If a token is found
                (UserViewmodel user, string err) = await _userManager.GetCurrentUser(token);
                if (err != null)
                {
                    //If error, send to login
                    _navigationManager.NavigateTo("/login", true);
                    return;
                }

                CurrentUser = user;
            }
            else
            {
                _navigationManager.NavigateTo("/login", true);
                return;
            }
            UserDto.Name = CurrentUser.Name;
            UserDto.FirstTimeLogin = CurrentUser.FirstTimeLogin;
        }

        //changes the current user details
        public async Task<ActionResult<UserViewmodel>> ChangeSettings(UserViewmodel request)
        {
            bool orgFirstTimeLogin = CurrentUser.FirstTimeLogin;
            string err;
            if (request.FirstTimeLogin == true)
            {
                request.FirstTimeLogin = false;
            }
            
            err = await _userManager.UpdateCurrentUser(request);
            if (err != null)
            {
                if (orgFirstTimeLogin == true)
                {
                    request.FirstTimeLogin = true;
                }
                    Checker = err;
                return new BadRequestObjectResult(Checker);
            }
            else
            {
                Checker = "User updated";
                return new OkObjectResult(Checker);
            }

            //CurrentUser = _userManager.GetCurrentUser();
            //if (CurrentUser.Password != request.RepeatPassword)
            //{
            //    Checker = "not changed correctly, try again";
            //    return new BadRequestObjectResult(Checker);
            //}

            //return CurrentUser;
        }

        // navigate back to home, when used
        private void NavigateBack()
        {
            _navigationManager.NavigateTo("/", true);
        }

        //Check password of CUser and change settings
        public async Task<ActionResult<string>> CheckPassword(UserViewmodel request)
        {
            string? token;
            string? err;
            request.EmploymentId = CurrentUser.EmploymentId;
            //returns token or err
            (token, err) = await _userManager.Login(request);
            //If error is null
            if (err == null && token != null)
            {
                await _localStorage.SetItemAsync("token", token);
                await ChangeSettings(UserDto);
                return token;
            }
            //If token is null
            else if (err != null && token == null)
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