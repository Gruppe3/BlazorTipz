using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.Models.AppStorage;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                s.CatEntity = SearchForCategoryEntity(s.categoryId);
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
                s.CatEntity = SearchForCategoryEntity(s.categoryId);
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
            sugg.CatEntity = SearchForCategoryEntity(sugg.categoryId);
            SuggViewmodel suggViewmodel = new SuggViewmodel(sugg);
            return suggViewmodel;
        }


        public string? validateSuggestion(SuggViewmodel sugg)
        {
            string? err = null;
            if (sugg == null) { err = "No supplied suggestion"; return err; }
            if (sugg.Title == null || sugg.Title == "") { err = "No supplied title"; return err; }
            if (sugg.Description == null || sugg.Description == "") { err = "No supplied description"; return err; }
            if (sugg.OwnerTeam == null || sugg.OwnerTeam == "") { err = "No supplied owner"; return err; }
            if (sugg.Creator == null || sugg.Creator == "") { err = "No supplied creator"; return err; }
            if (sugg.StartDate == DateTime.MinValue) { err = "No supplied start date"; return err; }

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
            string? err;
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
            
            string? err;
            SuggViewmodel suggOld = await GetSuggestion(sugg.Id);
            if (suggOld == null) { err = "No suggestion found"; return err; }
            if (suggOld.Ansvarlig == null || suggOld.Ansvarlig == "") {
                err = await ApproveAndUpdateSuggestion(sugg);
                return err;
            }
            if (suggOld.Creator == currentUser.employmentId || 
                suggOld.Ansvarlig == currentUser.employmentId || 
                suggOld.OwnerTeam == currentUser.teamId)
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

        public async Task<List<SuggViewmodel>> GetAllAssignedSuggestions(string empId)
        {
            List<SuggViewmodel> suggViewmodels = new();
            List<SuggestionEntity>? entities = await _DBR.GetAssignedSuggestions(empId);
            if (entities == null) { return suggViewmodels; }
            foreach (SuggestionEntity e in entities)
            {
                e.CatEntity = SearchForCategoryEntity(e.categoryId);
                SuggViewmodel sugg = new(e);
                suggViewmodels.Add(sugg);
            }
            return suggViewmodels;
        }

        public async Task<List<SuggViewmodel>> GetPreFilteredAssignedSuggestions(string empId)
        {
            List<SuggViewmodel> suggViewmodels = await GetAllAssignedSuggestions(empId);
            List<SuggViewmodel> filteredSuggestions = new();

            foreach (SuggViewmodel sView in suggViewmodels)
            {
                if (sView.Status != SuggStatus.Waiting ||
                    sView.Status != SuggStatus.Complete ||
                    sView.Status != SuggStatus.Rejected)
                { 
                    filteredSuggestions.Add(sView); 
                }
            }
            return filteredSuggestions;
        }

        //Get suggestions based on what list to get and filter-input
        public async Task<List<SuggViewmodel>> GetFilteredSuggestions(int caseInt, string inputId, SuggStatus status)
        {
            List<SuggViewmodel> suggViewmodels = new();
            List<SuggestionEntity>? respList = new();

            if (caseInt == 0)
            {
                respList = await _DBR.GetAssignedSuggestions(inputId, status);
            }
            else if (caseInt == 1)
            {
                respList = await _DBR.GetSuggestionsOfCreator(inputId, status);
            }
            else if (caseInt == 2)
            {
                respList = await _DBR.GetSuggestionOfTeam(inputId, status);
            }
            if (respList == null) { return suggViewmodels; }

            foreach (SuggestionEntity e in respList)
            {
                e.CatEntity = SearchForCategoryEntity(e.categoryId);
                SuggViewmodel sugg = new(e);
                suggViewmodels.Add(sugg);
            }
            return suggViewmodels;
        }

        //Get suggestions based on just what list to get
        public async Task<List<SuggViewmodel>> GetFilteredSuggestions(int caseInt, string inputId)
        {
            List<SuggViewmodel> suggViewmodels = new();
            List<SuggestionEntity>? respList = new();

            if (caseInt == 0)
            {
                respList = await _DBR.GetAssignedSuggestions(inputId);
            }
            else if (caseInt == 1)
            {
                respList = await _DBR.GetSuggestionsOfCreator(inputId);
            }
            else if (caseInt == 2)
            {
                respList = await _DBR.GetSuggestionOfTeam(inputId);
            }
            if (respList == null) { return suggViewmodels; }

            foreach (SuggestionEntity e in respList)
            {
                if (e.sugStatus != SuggStatus.Waiting ||
                    e.sugStatus != SuggStatus.Complete ||
                    e.sugStatus != SuggStatus.Rejected)
                {
                    e.CatEntity = SearchForCategoryEntity(e.categoryId);
                    SuggViewmodel sugg = new(e);
                    suggViewmodels.Add(sugg);
                }
            }
            return suggViewmodels;
        }



        //Sjekker om input string faller inn under karaktersettet latin1.
        private bool IsValidISO(string input)
        {
            byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(input);
            String result = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            return String.Equals(input, result);
        }
        

        public async Task<string> SaveComment(CommentViewmodel commentView)
        {
            if (commentView.Comment.IsNullOrEmpty()) { return "No comment"; }
            if (IsValidISO(commentView.Comment) == false) { return "Kommentar inneholder ukjente tegn"; }
            
            CommentEntity commentDto = new(commentView);
            await _DBR.SaveComment(commentDto);
            
            return "Kommentar lagret";
        }
    

        public async Task<string> UpdateComment(CommentViewmodel comment)
        {
            if (comment.Comment.IsNullOrEmpty()) { return "No comment"; }
            if (IsValidISO(comment.Comment) == false) { return "Kommentar inneholder ukjente tegn"; }

            CommentEntity commentEntity = new CommentEntity(comment);
            await _DBR.UpdateComment(commentEntity);
            
            return "Kommentar oppdatert";
        }
    

        public async Task<(List<CommentViewmodel>, string)> GetComments(string sugId)
        {
            List<CommentViewmodel> comments = new();
            
            if (sugId == string.Empty) { return (comments, "No sugId"); }
            List<CommentEntity> entities = await _DBR.GetCommentsOfSuggestion(sugId);
            foreach (CommentEntity e in entities)
            {
                CommentViewmodel comment = new(e);
                comments.Add(comment);
            }
            return (comments, "Success");
        }

        public async Task<(List<CommentViewmodel>, string)> GetAllCommentsFromUser(string empId)
        {
            List<CommentViewmodel> comments = new();

            if (empId == string.Empty) { return (comments, "No empId"); }
            List<CommentEntity> entities = await _DBR.GetCommentsOfUser(empId);
            foreach (CommentEntity e in entities)
            {
                CommentViewmodel comment = new(e);
                comments.Add(comment);
            }
            return (comments, "Success");
        }
    }
}
