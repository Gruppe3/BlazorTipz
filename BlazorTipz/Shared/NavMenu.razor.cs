using BlazorTipz.Data;
using BlazorTipz.ViewModels;
using BlazorTipz.ViewModels.Suggestion;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Radzen.Blazor;

namespace BlazorTipz.Shared
{
    public partial class NavMenu
    {
        RadzenUpload upload;
        
        private UserViewmodel CurrentUser { get; set; } = new();
        private TeamViewmodel CurrentTeam { get; set; } = new();
        private SuggViewmodel SuggDto { get; set; } = new();
        private List<UserViewmodel> ActiveUsers { get; set; } = new();
        private List<TeamViewmodel> ActiveTeams { get; set; } = new();
        private List<Category> Categories { get; set; } = new();
       


        //public string teamU { get; set; }
        private string NavMenuState { get; set; } = "";
        private string ShowUser { get; set; } = "none";
        private string ShowTeam { get; set; } = "none";
        private string Feedback { get; set; } = "";
        
        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("token");
            _navigationManager.NavigateTo("/login", true);
            _userManager.Logout();
        }


        //Get team from user
        protected override async Task OnInitializedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");
            if (token != null)
            {
                // If a token is found
                (UserViewmodel user, string err) = await _userManager.GetCurrentUser(token);
                TeamViewmodel team = await _teamManager.GetTeamById(user.TeamId);
                if (err != null)
                {
                    //If error, send to login
                    _navigationManager.NavigateTo("/login", true);
                    return;
                }

                CurrentUser = user;
                CurrentTeam = team;
            }
            else
            {
                _navigationManager.NavigateTo("/login", true);
                return;
            }

            //If all is sucessfull:

            ActiveTeams = await _teamManager.UpdateTeamsList();
            //teamU = CurrentTeam.name;
            SuggDto.OwnerTeam = CurrentTeam.TeamId;
            (Categories, string? err2) = await _suggestionManager.GetCategories();
            if (err2 != string.Empty ) { Feedback = err2; }

            var users = await _userManager.GetActiveUsers();
            foreach (UserViewmodel u in users)
            {
                await u.GetTeamName(_teamManager);
            }
            ActiveUsers = users;

        }
         
        private void ToggleNavMenu()
        {
            if (NavMenuState.Equals("active"))
            {
                NavMenuState = "";
            }
            else
            {
                NavMenuState = "active";
            }
        }

        private void CloseNavMenu()
        {
            NavMenuState = "";
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
        
        public async Task<ActionResult<string>> Submit(SuggViewmodel request)
        {
            SuggViewmodel suggToSave = new()
            {
                Title = request.Title,
                Description = request.Description,
                OwnerTeam = request.OwnerTeam,
                Creator = _userManager.GetCurrentUser().EmploymentId,
                JustDoIt = request.JustDoIt,
                Category = request.Category,
                StartDate = DateTime.Now.ToLocalTime(),
                Ansvarlig = request.Ansvarlig
            };
            if (suggToSave.JustDoIt == true)
            {
                suggToSave.Status = SuggStatus.Plan;
            }
            else
            {
                suggToSave.Status = SuggStatus.Waiting;
            }

            string? err = await _suggestionManager.SaveNewSuggestion(suggToSave);
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
        //public async Task OnUploadBeforeImage(InputFileChangeEventArgs e)
        //{
        //    //var image = await e.File.RequestImageFileAsync("image/png", 500, 500);
        //    //var buffer = new byte[image.Size];
        //    //await image.OpenReadStream().ReadAsync(buffer);
        //    //suggDto.BeforeImage = buffer;
        //}
    }
}