using BlazorTipz.Data;
using BlazorTipz.ViewModels.User;

namespace BlazorTipz.ViewModels.Suggestion
{
    public interface ISuggestionManager
    {
        // String return = err. If null = a-ok
        Task<string?> SaveNewSuggestion(SuggViewmodel sugg);

        //Get list of available categories
        //List<Category> GetCategories();
        Task<(List<Category>, string)> GetCategories();
        Task<string> UpdateCategories(List<Category> catList);


        //Get list of suggestions for a specific team
        Task<List<SuggViewmodel>> GetSuggestionsOfTeam(string teamId);
        
        //Get list of suggestions for a specific user
        Task<List<SuggViewmodel>> GetSuggestionsOfUser(string userId);
        
        //Get a suggestion from database with sugId
        Task<SuggViewmodel?> GetSuggestionById(string sugId);
        
        //When generally updating a suggestion
        Task<string?> UpdateSuggestion(SuggViewmodel sugg, UserViewmodel currentUser);

        //Returns a list of all suggestions assigned to current user 
        Task<List<SuggViewmodel>> GetAllAssignedSuggestions(string empId);
        
        //Returns a list of current-user-assigned suggestions filtered on status to Plan, Do, Study, Act 
        Task<List<SuggViewmodel>> GetFilteredSuggestions(int caseInt, string inputId, SuggStatus status);
        Task<List<SuggViewmodel>> GetFilteredSuggestions(int caseInt, string inputId);

        //Save comments 
        Task<string> SaveComment(CommentViewmodel comment);
        Task<string> UpdateComment(CommentViewmodel comment);
        Task<(List<CommentViewmodel>, string)> GetComments(string sugId);
        Task<(List<CommentViewmodel>, string)> GetAllCommentsFromUser(string empId);
    }
}