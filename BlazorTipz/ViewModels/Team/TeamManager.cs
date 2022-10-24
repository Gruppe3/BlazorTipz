using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.User;
using BlazorTipz.Data;
using System.Linq.Expressions;
using System.Collections.Generic;

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
        // Returns a list of all active teams.
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
        // Updates the list of available teams.
        public async Task<List<TeamViewmodel>> updateTeamsList()
        {
            Teams = null;
            List<TeamViewmodel> teams = await getTeams();
            return teams;
        }
        // Returns the team with the given ID.
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
        // Returns a list of all inactive teams.
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
        // Update team
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
        // Gets the team id for a given team name and leader.
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
        // Updates the teams of the given viewmodel.
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

        public async Task<string?> AddTeamMembers(List<TeamMemberViewmodel> teamMemberList)
        {
            if (teamMemberList == null) { return "Team member list is null"; }
            if (teamMemberList.Count <= 0) { return "Team member list is empty"; }

            List<TeamMemberEntity> memberList = new();
            
            foreach (TeamMemberViewmodel teamMember in teamMemberList)
            {
                memberList.Add(new TeamMemberEntity(teamMember));
            }
            await _DBR.AddTeamMemberToTeam(memberList);
            return null;
        }

        public async Task<(List<TeamMemberViewmodel>, string?)> GetTeamMembersByUser(string empId)
        {
            List<TeamMemberViewmodel> TeamMemViewList = new();
            if (empId == string.Empty || empId == null) { return (TeamMemViewList, "Employee ID is empty"); }
            var resp = await _DBR.GetTeamMemberList(empId);
            if (resp == null || resp.Count <= 0) { return (TeamMemViewList, "Ingen team funnet"); }

            TeamMemViewList = ConvertToViewModel(resp);

            return (TeamMemViewList, null);
        }

        public async Task<(List<TeamMemberViewmodel>, string?)> GetTeamMembersByTeam(string teamId)
        {
            List<TeamMemberViewmodel> TeamMemViewList = new();
            if (teamId == string.Empty || teamId == null) { return (TeamMemViewList, "Team ID is empty"); }
            var resp = await _DBR.GetTeamMembersByTeam(teamId);
            if (resp == null || resp.Count <= 0) { return (TeamMemViewList, "Ingen team funnet"); }

            TeamMemViewList = ConvertToViewModel(resp);

            return (TeamMemViewList, null);
        }
        
        //For Admin to oversee all
        public async Task<(List<TeamMemberViewmodel>, string?)> GetAllTeamMembers()
        {
            List<TeamMemberViewmodel> TeamMemViewList = new();
            var resp = await _DBR.GetAllTeamMemberLists();
            if (resp == null || resp.Count <= 0) { return (TeamMemViewList, "Ingen entiteter i databasen"); }
            
            TeamMemViewList = ConvertToViewModel(resp);
            
            return (TeamMemViewList, null);
        }
        
        private List<TeamMemberViewmodel> ConvertToViewModel(List<TeamMemberEntity> memberList)
        {
            List<TeamMemberViewmodel> TeamMemViewList = new();

            foreach (TeamMemberEntity TeamMemEntity in memberList)
            {
                TeamMemberViewmodel teamMember = new(TeamMemEntity);
                TeamMemViewList.Add(teamMember);
            }
            return TeamMemViewList;
        }
    }
}
