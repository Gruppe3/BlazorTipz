using Microsoft.AspNetCore.Components;
using BlazorTipz.Data;
using BlazorTipz.ViewModels;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using System;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;

namespace BlazorTipz.Shared
{
    public partial class NavMenu
    {
        RadzenUpload upload;
        
        public UserViewmodel currentUser = new UserViewmodel();
        public TeamViewmodel currentTeam = new TeamViewmodel();
        UserViewmodel CUser;
        TeamViewmodel Cteam;
        SuggViewmodel suggDto = new SuggViewmodel();
        List<SuggViewmodel> team = new List<SuggViewmodel>();
        List<TeamViewmodel> teams = new List<TeamViewmodel>();
        List<Category> Categories;
        public string TeamCheck { get; set; }
        public string teamU { get; set; }
        private string ShowUser { get; set; } = "none";
        private string ShowTeam { get; set; } = "none";
        private string Feedback { get; set; }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("token");
            NavigationManager.NavigateTo("/login", true);
            _userManager.logout();
        }

        private void Home()
        {
            NavigationManager.NavigateTo("/");
        }

        private void Settings()
        {
            NavigationManager.NavigateTo("/userSettings");
        }

        //Get team from user
        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            if (token != null)
            {
                (UserViewmodel user, string err) = await _userManager.getCurrentUser(token);
                TeamViewmodel team = await _teamManager.getTeam(user.teamId);
                if (err != null)
                {
                    return;
                }

                currentUser = user;
                currentTeam = team;
            }

            var CurrentUser = _userManager.getCurrentUser();
            teams = await _teamManager.updateTeamsList();
            if (CurrentUser != null)
            {
                CUser = CurrentUser;
                Cteam = await _teamManager.getTeam(CUser.teamId);
                teamU = Cteam.name;
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }

            Categories = _suggestionManager.GetCategories();
            suggDto.OwnerTeam = Cteam.id;
        }

        private void ShowTeamP()
        {
            if (ShowTeam != "none")
            {
                ShowTeam = "none";
            }
            else
            {
                ShowTeam = "block";
            }

            StateHasChanged();
        }

        private void ShowUserP()
        {
            if (ShowUser != "none")
            {
                ShowUser = "none";
            }
            else
            {
                ShowUser = "block";
            }

            StateHasChanged();
        }

        void OnChange(object value, string name)
        {
            var str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
        }

        public async Task<ActionResult<string>> submit(SuggViewmodel request)
        {
            SuggViewmodel suggToSave = new SuggViewmodel();
            suggToSave.Title = request.Title;
            suggToSave.Description = request.Description;
            suggToSave.OwnerTeam = request.OwnerTeam;
            suggToSave.Creator = _userManager.getCurrentUser().employmentId;
            suggToSave.JustDoIt = request.JustDoIt;
            suggToSave.category = request.category;
            suggToSave.StartDate = DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
            if (suggToSave.JustDoIt == true)
            {
                suggToSave.Status = SuggStatus.Plan;
            }
            else
            {
                suggToSave.Status = SuggStatus.Waiting;
            }

            string? err = await _suggestionManager.saveSuggestion(suggToSave);
            if (err != null)
            {
                Feedback = err;
                return err;
            }
            else
            {
                Feedback = "Forslag lagret";
                return "Success";
            }
        }
        //uplodeded image to byte[]
        public async Task OnUploadBeforeImage(InputFileChangeEventArgs e)
        {
            var image = await e.File.RequestImageFileAsync("image/png", 500, 500);
            var buffer = new byte[image.Size];
            await image.OpenReadStream().ReadAsync(buffer);
            suggDto.BeforeImage = buffer;
        }
    }
}