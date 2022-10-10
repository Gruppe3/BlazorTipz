using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTipz.Views
{
    public partial class ManageTeam
    {
        bool popup;
        bool isLoading = false;
        public string Checker { get; set; }

        public string team { get; set; }

        UserViewmodel CUser;
        TeamViewmodel Cteam;
        UserViewmodel userDto = new UserViewmodel();
        TeamViewmodel teamDto = new TeamViewmodel();
        List<UserViewmodel> people = new List<UserViewmodel>();
        List<TeamViewmodel> teams = new List<TeamViewmodel>();
        //checks if there is a current user
        protected override async Task OnInitializedAsync()
        {
            var CurrentUser = _userManager.getCurrentUser();
            people = await _userManager.updateUsersList();
            teams = await _teamManager.updateTeamsList();
            if (CurrentUser != null)
            {
                CUser = CurrentUser;
                Cteam = await _teamManager.getTeam(CUser.teamId);
                team = Cteam.name;
            }
            else
            {
                _navigationManager.NavigateTo("/");
            }
        }

        //Registeres Team and connects a user as Team Leader
        public async Task<ActionResult<TeamViewmodel>> RegisterTeam(TeamViewmodel request)
        {
            TeamViewmodel ret;
            string err;
            (ret, err) = await _teamManager.createTeam(request);
            if (err != null)
            {
                Checker = err;
                return null;
            }
            else
            {
                Checker = "Team created";
                return ret;
            }
        }

        //Registeres a member to a selected team
        public async Task AddTeamMember(UserViewmodel request)
        {
            if (request.teamId == null)
            {
                await _userManager.updateUserTeam(request.employmentId, CUser.teamId);
                Checker = "User added to team";
            }
            else
            {
                await _userManager.updateUserTeam(request.employmentId, request.teamId);
                Checker = "User added to team";
            }
        }
    }
}