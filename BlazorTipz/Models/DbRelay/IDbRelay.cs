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
        Task<UserEntity> GetLoginUser(string empId);
        Task<UserEntity> LookUpUser(string empId);
        Task AddUserEntries(List<UserEntity> toSaveUsers);
        Task UpdateUserEntry(UserEntity toSaveUser);
        Task<List<UserEntity>> GetActiveUsers();
        Task<List<UserEntity>> GetInactiveUsers();
        Task<List<UserEntity>> GetAllUsers();
        Task ChangeUserStateTo(string empid, bool state);
        Task ChangeUsersStateTo(List<UserEntity> users, bool state);

        //team/teams

        Task<TeamEntity> GetSingleTeamFromDb(string teamId);
        Task AddTeamEntry(TeamEntity team);
        Task UpdateTeamEntry(TeamEntity team);
        Task<List<TeamEntity>> GetActiveTeams();
        Task<List<TeamEntity>> GetInactiveTeams();

        Task ChangeTeamStateTo(string teamid, bool state);
        Task ChangeTeamsStateTo(List<TeamEntity> teams, bool state);

        // Suggestions
        Task SaveSuggestion(SuggestionEntity suggestion);
        Task SaveSuggestionList(List<SuggestionEntity> suggestions);
        
        //get all coulums of a suggestion from database with suggestion id
        //if return = null error
        Task<SuggestionEntity> GetSuggestion(string sugId);

        //get a list of suggestion from database bound to creator id
        Task<List<SuggestionEntity>> GetSuggestionsOfCreator(string empId);
        Task<List<SuggestionEntity>> GetSuggestionsOfCreator(string empId, SuggStatus status);

        //get a list of suggestions from database bound to assigned id 
        Task<List<SuggestionEntity>> GetAssignedSuggestions(string empId);
        Task<List<SuggestionEntity>> GetAssignedSuggestions(string empId, SuggStatus status);

        //get a list of suggestion from database bound to team id
        Task<List<SuggestionEntity>> GetSuggestionOfTeam(string teamId);
        Task<List<SuggestionEntity>> GetSuggestionOfTeam(string teamId, SuggStatus status);

        //get a list suggestion from database bound to status
        //@param status = a type of SuggStatus (enum class)
        Task<List<SuggestionEntity>> GetSuggestionsByStatus(SuggStatus status);
        //update a suggestion entry
        Task UpdateSuggestion(SuggestionEntity sug);

        Task<List<CategoryEntity>> GetCategoryEntities();
        Task UpdateCategories(List<CategoryEntity> catList);


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
