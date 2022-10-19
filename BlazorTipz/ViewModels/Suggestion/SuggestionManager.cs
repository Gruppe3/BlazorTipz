using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.Models.AppStorage;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggestionManager : ISuggestionManager
    {
        private readonly IDbRelay _DBR;
        private readonly IAppStorage _AS;

        private List<CategoriEntity> categories; 
        // Constructor
        public SuggestionManager(IDbRelay DBR, IAppStorage AS)
        {
            _DBR = DBR;
            _AS = AS;
            categories = _AS.GetCategories();
        }

        public async Task<string?> saveSuggestion(SuggViewmodel sugg)
        {
            string? err = validateSuggestion(sugg);
            if (err != null)
            {
                return err;
            }
            
            SuggestionEntity suggEntity = new SuggestionEntity(sugg);
            suggEntity.Category = searchForCategoryId(sugg.category);
            if (suggEntity == null) { err = "Program failure"; return err; }
            await _DBR.saveSuggestion(suggEntity);

            return err;
        }

        public List<Category> GetCategories()
        {
            return convertCategoryFromEntity(_AS.GetCategories());
        }
        private List<Category> convertCategoryFromEntity(List<CategoriEntity> categoris)
        {
            List<Category> cats = new List<Category>();
            foreach (CategoriEntity c in categoris)
            {
                Category cat = new Category();
                cat.Name = c.Name;
                cats.Add(cat);
            }
            return cats;
        }
        private string searchForCategoryId(Category cat)
        {
            {
                string id = "";
                foreach (CategoriEntity c in _AS.GetCategories())
                {
                    if (c.Name == cat.Name)
                    {
                        id = c.Id;
                        break;
                    }
                }
                return id;
            }
        }
        private CategoriEntity SearchForCategoryEntity(string catInn)
        {
            {
                CategoriEntity catOut = new();
                if (catInn == null || catInn == "")
                {
                    catOut.Name = "No category";
                    catOut.Id = "0";
                    return catOut;
                }
                foreach (CategoriEntity c in _AS.GetCategories())
                {
                    if (c.Id == catInn || c.Name == catInn)
                    {
                        catOut.Name = c.Name;
                        catOut.Id = c.Id;
                        break;
                    }
                }
                if (catOut.Name == null || catOut.Name == "")
                {
                    catOut.Name = "No category";
                    catOut.Id = "0";
                }
                return catOut;
            }
        }

        //Get suggestions from database for a specific team
        public async Task<List<SuggViewmodel>> GetSuggestionsOfTeam(string teamId)
        {
            List<SuggestionEntity>? suggs = await _DBR.GetSuggestionOfTeam(teamId);
            List<SuggViewmodel> suggsViewmodel = new List<SuggViewmodel>();
            if (suggs == null) { return suggsViewmodel; }
            foreach (SuggestionEntity s in suggs)
            {
                s.CategoryEntity = SearchForCategoryEntity(s.Category);
                SuggViewmodel sugg = new SuggViewmodel(s);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }

        //Get suggestions from database for a specific user
        public async Task<List<SuggViewmodel>> GetSuggestionsOfUser(string userId)
        {
            List<SuggestionEntity>? suggs = await _DBR.GetSuggestionsOfCreator(userId);
            List<SuggViewmodel> suggsViewmodel = new List<SuggViewmodel>();
            if (suggs == null) { return suggsViewmodel; }
            foreach (SuggestionEntity s in suggs)
            {
                s.CategoryEntity = SearchForCategoryEntity(s.Category);
                SuggViewmodel sugg = new SuggViewmodel(s);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }



        public string? validateSuggestion(SuggViewmodel sugg)
        {
            string err = null;
            if (sugg == null) { err = "No supplied suggestion"; return err; }
            if (sugg.Title == null || sugg.Title == "") { err = "No supplied title"; return err; }
            if (sugg.Description == null || sugg.Description == "") { err = "No supplied description"; return err; }
            if (sugg.OwnerTeam == null || sugg.OwnerTeam == "") { err = "No supplied owner"; return err; }
            if (sugg.Creator == null || sugg.Creator == "") { err = "No supplied creator"; return err; }
            if (sugg.StartDate == null || sugg.StartDate == "") { err = "No supplied start date"; return err; }

            return err;
        }
    }
}
