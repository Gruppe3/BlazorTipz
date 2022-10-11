using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.Models.AppStorage;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggestionManager : ISuggestionManager
    {
        private readonly IDbRelay _DBR;
        private readonly IAppStorage _AS;
        // Constructor
        public SuggestionManager(IDbRelay DBR, IAppStorage AS)
        {
            _DBR = DBR;
            _AS = AS;
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

        //Get suggestions from database
        public async Task<List<SuggViewmodel>> getAllSuggestions()
        {
            List<SuggViewmodel> suggs = new List<SuggViewmodel>();
            List<SuggestionEntity> suggEntities = await _DBR.getAllSuggestions();
            foreach (SuggestionEntity sugg in suggEntities)
            {
                suggs.Add(new SuggViewmodel(sugg));
            }
            return suggs;
        }

        //Get suggestions from database for a specific team
        public async Task<List<SuggViewmodel>> getAllSuggestionsForTeam(string teamId)
        {
            List<SuggViewmodel> suggs = new List<SuggViewmodel>();
            List<SuggestionEntity> suggEntities = await _DBR.getAllSuggestions();
            foreach (SuggestionEntity sugg in suggEntities)
            {
                if (sugg.creator == teamId)
                {
                    suggs.Add(new SuggViewmodel(sugg));
                }
            }
            return suggs;
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
