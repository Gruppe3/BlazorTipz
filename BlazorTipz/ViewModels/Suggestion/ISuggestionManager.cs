﻿using BlazorTipz.ViewModels.User;

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
        
        //when generally updating a suggestion
        Task<string?> UpdateSuggestion(SuggViewmodel sugg, UserViewmodel currentUser);

        //returns a list of all suggestions assigned to current user 
        Task<List<SuggViewmodel>> GetAllAssignedSuggestions();
        
        //returns a list of current-user-assigned suggestions filtered on status to Plan, Do, Study, Act 
        Task<List<SuggViewmodel>> GetPreFilteredAssignedSuggestions();
    }
}