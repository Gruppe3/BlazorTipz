using BlazorTipz.Data;

namespace BlazorTipz.Models.DbRelay
{
    public interface IDbRelay
    {
        //a field to define a connection to use
        //default to "default" connection
        string ConnectionString { set; }
        //methodes for getting, setting and updating the database

        //User/Users
        Task<UserEntity> getLoginUser(string empId);
        Task<UserEntity> lookUpUser(string empId);
        Task addUserEntries(List<UserEntity> toSaveUsers);
        Task updateUserEntry(UserEntity toSaveUser);
        Task<List<UserEntity>> getActiveUsers();
        Task<List<UserEntity>> getInactiveUsers();
        Task changeUserStateTo(string empid, bool state);
        Task changeUsersStateTo(List<UserEntity> users, bool state);

        //team/teams

        Task<TeamEntity> getSingleTeamDbFromDb(string teamId);
        Task addTeamEntry(TeamEntity team);
        Task updateTeamEntry(TeamEntity team);
        Task<List<TeamEntity>> getActiveTeams();
        Task<List<TeamEntity>> getInactiveTeams();

        Task changeTeamStateTo(string teamid, bool state);
        Task changeTeamsStateTo(List<TeamEntity> teams, bool state);

        // Suggestions
        Task saveSuggestion(SuggestionEntity suggestion);
        Task saveSuggestionList(List<SuggestionEntity> suggestions);
        
        //get all coulums of a suggestion from database with suggestion id
        //if return = null error
        Task<SuggestionEntity?> GetSuggestion(string sugId);

        //get a list suggestion from database bound to creator id
        //if return = null error
        Task<List<SuggestionEntity>?> GetSuggestionsOfCreator(string empId);

        //get a list of suggestions assigned to spesified user
        Task<List<SuggestionEntity>?> GetAssignedSuggestions(string empId);

        //get a list suggestion from database bound to owner id
        //if return = null error
        Task<List<SuggestionEntity>?> GetSuggestionOfTeam(string teamId);
        
        //get a list suggestion from database bound to status
        //@param status = a type of SuggStatus (enum class)
        //if return null = error
        Task<List<SuggestionEntity>?> GetSuggestionsByStatus(SuggStatus status);
        //update a suggestion entry
        Task updateSuggestion(SuggestionEntity sug);

        //Save a comment to database
        Task SaveComment(CommentEntity comment);

        //Update a comment
        Task UpdateComment(CommentEntity comment);
        
        //Get a list of comments bound to suggestion id
        Task<List<CommentEntity>> GetCommentsOfSuggestion(string sugId);
        
        //For Admin to see all suggestions from a user
        Task<List<CommentEntity>> GetCommentsOfUser(string empId);



    }
}
