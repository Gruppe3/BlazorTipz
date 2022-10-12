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
        private CategoriEntity searchForCategory(string catid)
        {
            {
                CategoriEntity cat = null;
                foreach (CategoriEntity c in _AS.GetCategories())
                {
                    if (c.Id == catid)
                    {
                        cat = new CategoriEntity();
                        cat.Name = c.Name;
                        cat.Id = c.Id;
                        break;
                    }
                }
                if (cat == null) { return null; }
                return cat;
            }
        }

        //Get suggestions from database for a specific team
        public async Task<List<SuggViewmodel>> GetSuggestionsOfTeam(string teamId)
        {
            List<SuggestionEntity> suggs = await _DBR.GetSuggestionOfTeam(teamId);
            List<SuggViewmodel> suggsViewmodel = new List<SuggViewmodel>();
            foreach (SuggestionEntity s in suggs)
            {
                s.CategoryEntity = searchForCategory(s.Category);
                SuggViewmodel sugg = new SuggViewmodel(s);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }

        //Get suggestions from database for a specific user
        public async Task<List<SuggViewmodel>> GetSuggestionsOfUser(string userId)
        {
            List<SuggestionEntity> suggs = await _DBR.GetSuggestionsOfCreator(userId);
            List<SuggViewmodel> suggsViewmodel = new List<SuggViewmodel>();
            foreach (SuggestionEntity s in suggs)
            {
                s.CategoryEntity = searchForCategory(s.Category);
                SuggViewmodel sugg = new SuggViewmodel(s);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }



        private string? validateSuggestion(SuggViewmodel sugg)
        {
            string err = null;
            if (sugg == null) { err = "No supplied suggestion"; return err; }
            if (sugg.Title == null) { err = "No supplied title"; return err; }
            if (sugg.Description == null) { err = "No supplied description"; return err; }
            if (sugg.OwnerTeam == null) { err = "No supplied owner"; return err; }
            if (sugg.Creator == null) { err = "No supplied creator"; return err; }
            if (sugg.StartDate == null) { err = "No supplied start date"; return err; }

            return err;
        }
    }
}
