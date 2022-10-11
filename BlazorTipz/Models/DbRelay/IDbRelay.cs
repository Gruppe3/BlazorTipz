using BlazorTipz.Data;

namespace BlazorTipz.Models.DbRelay
{
    public interface IDbRelay
    {
        //methodes for getting, setting and updating the database

        //User/Users
        Task<UserDb> getLoginUser(string empId);
        Task<UserDb> lookUpUser(string empId);
        Task addUserEntries(List<UserDb> toSaveUsers);
        Task updateUserEntry(UserDb toSaveUser);
        Task<List<UserDb>> getActiveUsers();
        Task<List<UserDb>> getInactiveUsers();
        Task changeUserStateTo(string empid, bool state);
        Task changeUsersStateTo(List<UserDb> users, bool state);

        //team/teams
        Task<TeamDb> getSingleTeamDbFromDb(string teamId);
        Task addTeamEntry(TeamDb team);
        Task updateTeamEntry(TeamDb team);
        Task<List<TeamDb>> getActiveTeams();
        Task<List<TeamDb>> getInactiveTeams();
        Task changeTeamStateTo(string teamid, bool state);
        Task changeTeamsStateTo(List<TeamDb> users, bool state);

        // Suggestions
        Task saveSuggestion(SuggestionEntity suggestion);
        Task saveSuggestionList(List<SuggestionEntity> suggestions);
        Task<List<SuggestionEntity>> getAllSuggestions();

    }
}
