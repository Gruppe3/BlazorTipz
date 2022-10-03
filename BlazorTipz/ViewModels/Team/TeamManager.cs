using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.User;
using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.Team
{
    public class TeamManager : ITeamManager
    {
        private readonly IDbRelay _DBR;
        private readonly IUserManager _UM;

        public List<TeamViewmodel>? Teams { get; set; }
        public TeamManager(IDbRelay DBR, IUserManager UM)
        {
            _DBR = DBR;
            _UM = UM;
        }
        public async Task<List<TeamViewmodel>> getTeams()
        {
            if (Teams == null)
            {
                List<TeamDb> dbTeams = await _DBR.getActiveTeams();
                List<TeamViewmodel> teams = new List<TeamViewmodel>();
                foreach (TeamDb dbTeam in dbTeams)
                {
                    TeamViewmodel team = new TeamViewmodel(dbTeam);
                    teams.Add(team);
                }
                Teams = teams;
            }
            return Teams;
        }
        public async Task<List<TeamViewmodel>> updateTeamsList()
        {
            Teams = null;
            List<TeamViewmodel> teams = await getTeams();
            return teams;
        }
        public async Task<TeamViewmodel> getTeam(string teamId)
        {

            TeamViewmodel team = new TeamViewmodel();
            if (Teams == null)
            {
                await getTeams();
            }
            foreach (TeamViewmodel t in Teams)
            {
                if (t.id == teamId)
                {
                    team = t;
                    break;
                }
            }
            return team;
        }
        public async Task<List<TeamViewmodel>> getInactiveTeams()
        {
            List<TeamViewmodel> teams = new List<TeamViewmodel>();
            List<TeamDb> dbTeams = await _DBR.getInactiveTeams();
            foreach (TeamDb dbTeam in dbTeams)
            {
                TeamViewmodel team = new TeamViewmodel(dbTeam);
                teams.Add(team);
            }
            return teams;
        }
        //update team
        public async Task updateTeam(TeamViewmodel team)
        {
            TeamDb dbTeam = new TeamDb(team);
            if (updateTeams(team))
            {
                await _DBR.updateTeamEntry(dbTeam);
            }
            else
            {
                await _DBR.updateTeamEntry(dbTeam);
                await getTeams();
            }
        }
        private string getTeamId(string teamName, string teamLeader)
        {
            string teamId = string.Empty;
            foreach (TeamViewmodel team in Teams)
            {
                if (team.name == teamName)
                {
                    if (team.leader == teamLeader)
                    {
                        teamId = team.id;
                        break;
                    }
                }
            }
            return teamId;
        }
        private bool updateTeams(TeamViewmodel team)
        {
            if (Teams == null) { return false; }
            foreach (TeamViewmodel t in Teams)
            {
                if (t.id == team.id)
                {
                    t.name = team.name;
                    t.leader = team.leader;
                    return true;
                }
            }
            return false;
        }
        //add team
        public async Task<(TeamViewmodel?, string?)> createTeam(TeamViewmodel team)
        {

            if (team == null) { return (null, "Team is null"); }
            if (team.name == string.Empty) { return (null, "Team name is empty"); }
            if (team.leader == string.Empty) { return (null, "No team leader chosen"); }

            UserViewmodel teamLeader = await _UM.getUser(team.leader);
            if (teamLeader == null) { return (null, "Team leader not found"); }
            if (teamLeader.role != RoleE.TeamLeader)
            {
                await _UM.updateRole(teamLeader, RoleE.TeamLeader, true);
            }

            TeamDb dbTeam = new TeamDb(team);
            await _DBR.addTeamEntry(dbTeam);
            await updateTeamsList();
            string teamid = getTeamId(team.name, team.leader);
            if (teamid == string.Empty) { return (null, "Team creation error"); }
            TeamViewmodel rTeam = await getTeam(teamid);

            await _UM.updateUserTeam(teamLeader.employmentId, teamid);

            return (rTeam, null);
        }

    }
}
