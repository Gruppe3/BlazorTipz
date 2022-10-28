using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.Models.AppStorage;
using Org.BouncyCastle.Crypto.Modes.Gcm;

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
        //get a suggestion from database with sugId
        public async Task<SuggViewmodel?> GetSuggestion(string sugId)
        {
            SuggestionEntity? sugg = await _DBR.GetSuggestion(sugId);
            if (sugg == null) { return null; }
            sugg.CategoryEntity = SearchForCategoryEntity(sugg.Category);
            SuggViewmodel suggViewmodel = new SuggViewmodel(sugg);
            return suggViewmodel;
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
        private async Task<string?> updateSuggestion(SuggViewmodel sugg)
        {
            string? err = validateSuggestion(sugg);
            if (err != null)
            {
                return err;
            }
            SuggestionEntity suggEntity = new SuggestionEntity(sugg);
            suggEntity.Category = searchForCategoryId(sugg.category);
            if (suggEntity == null) { err = "Program failure"; return err; }
            await _DBR.updateSuggestion(suggEntity);

            return err;
        }
        public async Task<string?> ApproveAndUpdateSuggestion(SuggViewmodel sugg)
        {
            string? err = null;
            SuggViewmodel suggOld = await GetSuggestion(sugg.Id);
            if (suggOld == null) { err = "No suggestion found"; return err; }

            suggOld.Title = sugg.Title;
            suggOld.Description = sugg.Description;
            suggOld.category = suggOld.category;
            suggOld.OwnerTeam = suggOld.OwnerTeam;
            suggOld.Status = sugg.Status;
            suggOld.Ansvarlig = sugg.Ansvarlig;
            suggOld.Frist = sugg.Frist;
            suggOld.BeforeImage = sugg.BeforeImage;
            suggOld.AfterImage = sugg.AfterImage;

            err = await updateSuggestion(suggOld);

            return err;
        }
        public async Task<string?> UpdateSuggestion(SuggViewmodel sugg)
        {
            string? err = null;
            SuggViewmodel suggOld = await GetSuggestion(sugg.Id);
            if (suggOld == null) { err = "No suggestion found"; return err; }

            suggOld.Description = sugg.Description;
            suggOld.category = suggOld.category;
            suggOld.OwnerTeam = suggOld.OwnerTeam;
            suggOld.Status = sugg.Status;
            suggOld.Ansvarlig = sugg.Ansvarlig;
            suggOld.AfterImage = sugg.AfterImage;

            err = await updateSuggestion(suggOld);

            return err;
            
        }
    }
}
