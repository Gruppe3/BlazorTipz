using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using BlazorTipz.ViewModels.User;
using BlazorTipz.Data;
using Microsoft.AspNetCore.Identity;

namespace BlazorTipz.Views
{
    public partial class LoginU
    {
        bool popup; 
        private string Checker { get; set; } = string.Empty;
        private string ErrorCardState { get; set; } = "";
        private UserViewmodel UserDto { get; set; } = new();
        
        //If Submit is pressed this runs, it takes in request from the form
        //Async means it will wait for things to load in with await.
        public async Task<ActionResult<string>> LoginUs(UserViewmodel request)
        {
            string? token;
            string? err;
            //returns token or err
            (token, err) = await _userM.Login(request);
            //If error is null
            if (err == null && token != null)
            {
                await _localStorage.SetItemAsync("token", token);
                _navigationManager.NavigateTo("/", true);
                return token;
            }
            //If token is null
            else if (err != null && token == null)
            {
                Checker = err;
                ErrorCardState = "active";
                return err;
            }
            //If something else happens
            else
            {
                Checker = "something went horribly wrong";
                ErrorCardState = "active";
                return "Fatal";
            }
        }
    }
}