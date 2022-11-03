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
        public string Checker { get; set; } = string.Empty;

        //Brukes denne til noe?
        public string Link { get; set; } = string.Empty; 

        UserViewmodel userDto = new UserViewmodel();
        //If Submit is pressed this runs, it takes in request from the form
        //Async means it will wait for things to load in with await.
        public async Task<ActionResult<string>> LoginUs(UserViewmodel request)
        {
            string token;
            string err;
            //returns token or err
            (token, err) = await _userM.Login(request);
            //If error is null
            if (err == null)
            {
                await _localStorage.SetItemAsync("token", token);
                NavigationManager.NavigateTo("/", true);
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