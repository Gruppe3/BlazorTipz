using BlazorTipz.Models;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.Models.AppStorage;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.Data;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggestionManager : ISuggestionManager
    {
        private readonly IDbRelay _DBR;
        private readonly IAppStorage _AS;
        private readonly IUserManager _UM;
        private readonly ITeamManager _TM;

        private List<CategoriEntity> categories;
        public List<SuggViewmodel> AssignedSuggestions;
        
        // Constructor
        public SuggestionManager(IDbRelay DBR, IAppStorage AS, IUserManager UM, ITeamManager TM)
        {
            _DBR = DBR;
            _AS = AS;
            categories = _AS.GetCategories();
            _UM = UM;
            _TM = TM;
        }

        public async Task<string?> saveSuggestion(SuggViewmodel sugg)
        {
            string? err = validateSuggestion(sugg);
            if (err != null)
            {
                return err;
            }
            
            SuggestionEntity suggEntity = new SuggestionEntity(sugg);
            suggEntity.categoryId = searchForCategoryId(sugg.Category);
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
                cat.Name = c.catName;
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
                    if (c.catName == cat.Name)
                    {
                        id = c.catId;
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
                    catOut.catName = "No category";
                    catOut.catId = "0";
                    return catOut;
                }
                foreach (CategoriEntity c in _AS.GetCategories())
                {
                    if (c.catId == catInn || c.catName == catInn)
                    {
                        catOut.catName = c.catName;
                        catOut.catId = c.catId;
                        break;
                    }
                }
                if (catOut.catName == null || catOut.catName == "")
                {
                    catOut.catName = "No category";
                    catOut.catId = "0";
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
                s.CategoryEntity = SearchForCategoryEntity(s.categoryId);
                SuggViewmodel sugg = new SuggViewmodel(s);
                await fillNameFieldsInSugg(sugg);
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
                s.CategoryEntity = SearchForCategoryEntity(s.categoryId);
                SuggViewmodel sugg = new SuggViewmodel(s);
                await fillNameFieldsInSugg(sugg);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }
        //get a suggestion from database with sugId
        public async Task<SuggViewmodel?> GetSuggestion(string sugId)
        {
            SuggestionEntity? sugg = await _DBR.GetSuggestion(sugId);
            if (sugg == null) { return null; }
            sugg.CategoryEntity = SearchForCategoryEntity(sugg.categoryId);
            SuggViewmodel suggViewmodel = new SuggViewmodel(sugg);
            await fillNameFieldsInSugg(suggViewmodel);
            return suggViewmodel;
        }

        private async Task fillNameFieldsInSugg(SuggViewmodel sugg)
        {
            UserViewmodel? UsCreSearch = await _UM.SearchActiveUsers(sugg.Creator);
            UserViewmodel? UsAnsSearch = await _UM.SearchActiveUsers(sugg.Ansvarlig);
            TeamViewmodel? TeamOwnerSearch = await _TM.SearchTeams(sugg.OwnerTeam);
            if (UsCreSearch == null) {await sugg.GetCreatorName(_UM); } else { sugg.CreatorName = UsCreSearch.name; }
            if (UsAnsSearch == null) { await sugg.GetAnsvarligName(_UM); } else { sugg.AnsvarligName = UsAnsSearch.name; }
            if (TeamOwnerSearch == null) { await sugg.GetOwnerTeamName(_TM); } else { sugg.OwnerTeamName = TeamOwnerSearch.name; }

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
            suggEntity.categoryId = searchForCategoryId(sugg.Category);
            if (suggEntity == null) { err = "Program failure"; return err; }
            await _DBR.updateSuggestion(suggEntity);

            return err;
        }
        private async Task<string?> ApproveAndUpdateSuggestion(SuggViewmodel sugg)
        {
            string? err = null;
            SuggViewmodel suggOld = await GetSuggestion(sugg.Id);
            if (suggOld == null) { err = "No suggestion found"; return err; }

            suggOld.Title = sugg.Title;
            suggOld.Description = sugg.Description;
            suggOld.Category = suggOld.Category;
            suggOld.OwnerTeam = suggOld.OwnerTeam;
            suggOld.Status = sugg.Status;
            suggOld.Ansvarlig = sugg.Ansvarlig;
            suggOld.Frist = sugg.Frist;
            suggOld.BeforeImage = sugg.BeforeImage;
            suggOld.AfterImage = sugg.AfterImage;

            err = await updateSuggestion(suggOld);

            return err;
        }
        public async Task<string?> UpdateSuggestion(SuggViewmodel sugg, UserViewmodel currentUser)
        {
            
            string? err = null;
            SuggViewmodel suggOld = await GetSuggestion(sugg.Id);
            if (suggOld == null) { err = "No suggestion found"; return err; }
            if (suggOld.Ansvarlig == null || suggOld.Ansvarlig == "") {
                err = await ApproveAndUpdateSuggestion(sugg);
                return err;
            }
            if (suggOld.Creator == currentUser.employmentId || suggOld.Ansvarlig == currentUser.employmentId || suggOld.OwnerTeam == currentUser.teamId)
            {
                suggOld.Title = sugg.Title;
                suggOld.Description = sugg.Description;
                suggOld.Category = suggOld.Category;
                suggOld.OwnerTeam = suggOld.OwnerTeam;
                suggOld.Status = sugg.Status;
                suggOld.Ansvarlig = sugg.Ansvarlig;
                suggOld.Frist = sugg.Frist;
                suggOld.BeforeImage = sugg.BeforeImage;
                suggOld.AfterImage = sugg.AfterImage;

                err = await updateSuggestion(suggOld);
            }
            else
            {
                err = "You are not the creator of this suggestion";
            }
            return err;
            
        }

        public async Task<List<SuggViewmodel>> GetAllAssignedSuggestions()
        {
            List<SuggViewmodel> suggViewmodels = new();
            List<SuggestionEntity>? entities = await _DBR.GetAssignedSuggestions(_UM.getCurrentUser().employmentId);
            if (entities == null) { return suggViewmodels; }
            foreach (SuggestionEntity e in entities)
            {
                e.CategoryEntity = SearchForCategoryEntity(e.categoryId);
                SuggViewmodel sugg = new(e);
                await fillNameFieldsInSugg(sugg);
                suggViewmodels.Add(sugg);
            }
            return suggViewmodels;
        }

        public async Task<List<SuggViewmodel>> GetPreFilteredAssignedSuggestions()
        {
            List<SuggViewmodel> suggViewmodels = await GetAllAssignedSuggestions();
            List<SuggViewmodel> filteredSuggestions = new();

            foreach (SuggViewmodel sView in suggViewmodels)
            {
                if (sView.Status == SuggStatus.Plan || 
                    sView.Status == SuggStatus.Do || 
                    sView.Status == SuggStatus.Study || 
                    sView.Status == SuggStatus.Act)
                { 
                    filteredSuggestions.Add(sView); 
                }
            }
            return filteredSuggestions;
        }
    }
}
