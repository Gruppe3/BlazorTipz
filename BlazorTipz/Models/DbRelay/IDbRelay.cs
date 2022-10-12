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
        Task changeTeamsStateTo(List<TeamDb> teams, bool state);

        // Suggestions
        Task saveSuggestion(SuggestionEntity suggestion);
        Task saveSuggestionList(List<SuggestionEntity> suggestions);
        
        //get all coulums of a suggestion from database with suggestion id
        //if return = null error
        Task<SuggestionEntity?> GetSuggestion(string sugId);

        //get a list suggestion from database bound to creator id
        //if return = null error
        Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId);

        //get a list suggestion from database bound to owner id
        //if return = null error
        Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId);

        //get a list suggestion from database bound to status
        //@param status = a type of SuggStatus (enum class)
        //if return null = error
        Task<List<SuggestionEntity>?> GetSuggestionsByStatus(SuggStatus status);

    }
}
