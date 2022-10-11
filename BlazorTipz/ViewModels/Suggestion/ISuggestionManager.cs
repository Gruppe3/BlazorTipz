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
    }
}