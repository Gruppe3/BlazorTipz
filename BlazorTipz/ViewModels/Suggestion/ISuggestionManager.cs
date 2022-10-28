namespace BlazorTipz.ViewModels.Suggestion
{
    public interface ISuggestionManager
    {
        // String return = err. If null = a-ok
        Task<string?> saveSuggestion(SuggViewmodel sugg);
        
        //get list of available categories
        List<Category> GetCategories();

        //Get list of suggestions for a specific team
        Task<List<SuggViewmodel>> GetSuggestionsOfTeam(string teamId);
        
        //Get list of suggestions for a specific user
        Task<List<SuggViewmodel>> GetSuggestionsOfUser(string userId);
        
        //get a suggestion from database with sugId
        Task<SuggViewmodel> GetSuggestion(string sugId);
        
        //when approving or rejaecting a suggestion
        Task<string?> ApproveAndUpdateSuggestion(SuggViewmodel sugg);
        
        //when generally updating a suggestion
        Task<string?> UpdateSuggestion(SuggViewmodel sugg);
    }
}