using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.Views
{
    public partial class ManageTeam
    {
        bool popup;
        bool isLoading = false;
        public string Checker { get; set; } = string.Empty;
        private UserViewmodel CurrentUser { get; set; } = new();
        private TeamViewmodel CurrentTeam { get; set; } = new();
        private UserViewmodel UserDto { get; set; } = new();
        private TeamViewmodel TeamDto { get; set; } = new();
        private List<UserViewmodel> ActiveUsers { get; set; } = new();
        private List<TeamViewmodel> ActiveTeams { get; set; } = new();
        
        //checks if there is a current user
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
                    _navigationManager.NavigateTo("/");
                    return;
                }

                CurrentUser = user;
                CurrentTeam = team;
            }
            else
            {
                _navigationManager.NavigateTo("/");
                return;
            }

            ActiveUsers = await _userManager.UpdateUsersList();
            ActiveTeams = await _teamManager.UpdateTeamsList();
        }

        //Registeres Team and connects a user as Team Leader
        public async Task<TeamViewmodel?> RegisterTeam(TeamViewmodel request)
        {
            (TeamViewmodel? ret, string? err) = await _teamManager.CreateNewTeam(request);
            if (err != null)
            {
                Checker = err;
                return null;
            }
            else if (ret != null)
            {
                Checker = "Team created";
                TeamDto = new();
                return ret;
            }
            else
            {
                Checker = "Something went wrong";
                return null;
            }
        }

        //Registeres a member to a selected team
        public async Task AddTeamMember(UserViewmodel request)
        {
            if (request.TeamId == null)
            {
                await _userManager.UpdateUserTeam(request.EmploymentId, CurrentUser.TeamId);
                Checker = "User added to team";
                UserDto = new();
            }
            else
            {
                await _userManager.UpdateUserTeam(request.EmploymentId, request.TeamId);
                Checker = "User added to team";
                UserDto = new();
            }
        }
    }
}