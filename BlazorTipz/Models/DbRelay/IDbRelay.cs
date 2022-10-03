using BlazorTipz.Data;

namespace BlazorTipz.Models.DbRelay
{
    public interface IDbRelay
    {
        //methodes for getting, setting and updating the database

        //User/Users
        Task<UserDb> getUser(string empId);
        Task addUserEntries(List<UserDb> toSaveUsers);
        Task updateUserEntry(UserDb toSaveUser);
        Task<List<UserDb>> getActiveUsers();
        Task<List<UserDb>> getInactiveUsers();
        Task changeUserStateTo(string empid, bool state);
        Task changeUsersStateTo(List<UserA> users, bool state);

        //team/teams
        Task<TeamA> getSingleTeamDbFromDb(string teamId);
        Task addTeamEntryToDbFromTeamDb(TeamDb team);
        Task updateTeamEntryToDbFromTeamDb(TeamDb team);
        Task<List<TeamDb>> getActiveTeams();
        Task<List<TeamDb>> getInactiveTeams();
        Task changeTeamStateTo(string teamid, bool state);
        Task changeTeamsStateTo(List<TeamA> users, bool state);


    }
}
