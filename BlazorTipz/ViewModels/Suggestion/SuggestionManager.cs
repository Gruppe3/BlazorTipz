using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.Models.AppStorage;
using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels.Team;
using BlazorTipz.ViewModels.User;
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

        private List<CategoryEntity> categories { get; set; }
        
        // Constructor
        public SuggestionManager(IDbRelay DBR, IAppStorage AS, IUserManager UM, ITeamManager TM)
        {
            _DBR = DBR;
            _AS = AS;
            categories = _AS.GetCategories();
            _UM = UM;
            _TM = TM;
        }

        public async Task<string?> SaveNewSuggestion(SuggViewmodel sugg)
        {
            string? err = ValidateSuggestion(sugg);
            if (err != null)
            {
                return err;
            }

            SuggestionEntity suggEntity = new(sugg);
            if (suggEntity == null) { err = "Program failure"; return err; }
            await _DBR.SaveSuggestion(suggEntity);

            return err;
        }

        //public List<Category> GetCategories()
        //{
        //    return ConvertCategoryFromEntity(_AS.GetCategories());
        //}
        //private static List<Category> ConvertCategoryFromEntity(List<CategoryEntity> categories)
        //{
        //    List<Category> cats = new();
        //    foreach (CategoryEntity c in categories)
        //    {
        //        Category cat = new()
        //        {
        //            Name = c.catName
        //        };
        //        cats.Add(cat);
        //    }
        //    return cats;
        //}

        public async Task<(List<Category>, string)> GetCategories()
        {
            string err = string.Empty;
            List<Category> catList = new();
            List<CategoryEntity> entities = await _DBR.GetCategoryEntities();
            
            if (entities.Count <= 0) { err = "Ingen kategorier ble funnet"; return (catList, err); }
            
            foreach (CategoryEntity c in entities)
            {
                Category cat = new(c);
                catList.Add(cat);
            }
            return (catList, err);
        }

        public async Task<string> UpdateCategories(List<Category> catList)
        {
            string err = string.Empty;
            List<CategoryEntity> entityList = new();
            
            foreach (Category c in catList)
            {
                CategoryEntity catE = new(c);
                entityList.Add(catE);
            }
            await _DBR.UpdateCategories(entityList);
            return err;
        }
            

        
        //private string SearchForCategoryId(Category cat)
        //{
        //    {
        //        string id = "";
        //        foreach (CategoryEntity c in _AS.GetCategories())
        //        {
        //            if (c.catName == cat.Name)
        //            {
        //                id = c.catId;
        //                break;
        //            }
        //        }
        //        return id;
        //    }
        //}
        //private CategoryEntity SearchForCategoryEntity(string catInn)
        //{
        //    {
        //        CategoryEntity catOut = new();
        //        if (catInn == null || catInn == "")
        //        {
        //            catOut.catName = "No category";
        //            catOut.catId = "0";
        //            return catOut;
        //        }
        //        foreach (CategoryEntity c in _AS.GetCategories())
        //        {
        //            if (c.catId == catInn || c.catName == catInn)
        //            {
        //                catOut.catName = c.catName;
        //                catOut.catId = c.catId;
        //                break;
        //            }
        //        }
        //        if (catOut.catName == null || catOut.catName == "")
        //        {
        //            catOut.catName = "No category";
        //            catOut.catId = "0";
        //        }
        //        return catOut;
        //    }
        //}

        //Get suggestions from database for a specific team
        public async Task<List<SuggViewmodel>> GetSuggestionsOfTeam(string teamId)
        {
            List<SuggestionEntity>? suggs = await _DBR.GetSuggestionOfTeam(teamId);
            List<SuggViewmodel> suggsViewmodel = new();
            if (suggs == null) { return suggsViewmodel; }
            foreach (SuggestionEntity s in suggs)
            {
                s.FillCatEntity();
                SuggViewmodel sugg = new(s);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }

        //Get suggestions from database for a specific user
        public async Task<List<SuggViewmodel>> GetSuggestionsOfUser(string userId)
        {
            List<SuggestionEntity>? suggs = await _DBR.GetSuggestionsOfCreator(userId);
            List<SuggViewmodel> suggsViewmodel = new();
            if (suggs == null) { return suggsViewmodel; }
            foreach (SuggestionEntity s in suggs)
            {
                s.FillCatEntity();
                SuggViewmodel sugg = new(s);
                suggsViewmodel.Add(sugg);
            }
            return suggsViewmodel;
        }
        //get a suggestion from database with sugId
        public async Task<SuggViewmodel?> GetSuggestionById(string sugId)
        {
            SuggestionEntity? sugg = await _DBR.GetSuggestion(sugId);
            if (sugg == null) { return null; }
            
            sugg.FillCatEntity();
            SuggViewmodel suggViewmodel = new(sugg);
            return suggViewmodel;
        }


        public string? ValidateSuggestion(SuggViewmodel sugg)
        {
            string? err = null;
            if (sugg == null) { err = "No supplied suggestion"; return err; }
            if (sugg.Title == null || sugg.Title == "") { err = "No supplied title"; return err; }
            if (IsValidISO(sugg.Title) != true) { err = "Title contains unrecognised characters"; return err; }
            if (sugg.Description == null || sugg.Description == "") { err = "No supplied description"; return err; }
            if (IsValidISO(sugg.Description) != true) { err = "Description contains unrecognised characters"; return err; }
            if (sugg.OwnerTeam == null || sugg.OwnerTeam == "") { err = "No supplied owner"; return err; }
            if (sugg.Creator == null || sugg.Creator == "") { err = "No supplied creator"; return err; }
            if (sugg.StartDate == DateTime.MinValue) { err = "No supplied start date"; return err; }

            return err;
        }
        private async Task<string?> UpdateSuggestion(SuggViewmodel sugg)
        {
            string? err = ValidateSuggestion(sugg);
            if (err != null)
            {
                return err;
            }
            SuggestionEntity suggEntity = new(sugg);
            if (suggEntity == null) { err = "Program failure"; return err; }
            await _DBR.UpdateSuggestion(suggEntity);

            return err;
        }
        private async Task<string?> ApproveAndUpdateSuggestion(SuggViewmodel sugg)
        {
            string? err;
            if (sugg.Id == null) { err = "No Id could be fond. Something wrong has happened with the suggestion"; return err; }

            SuggViewmodel? suggOld = await GetSuggestionById(sugg.Id);
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

            err = await UpdateSuggestion(suggOld);

            return err;
        }
        public async Task<string?> UpdateSuggestion(SuggViewmodel sugg, UserViewmodel currentUser)
        {
            
            string? err;
            if (sugg.Id == null) { err = "Kan ikke identifisere forslaget. Noe gikk galt."; return err; }
            
            SuggViewmodel? suggOld = await GetSuggestionById(sugg.Id);
            if (suggOld == null) { err = "Kan ikke finne forslaget i systemet. Noe gikk galt."; return err; }
            if (suggOld.Ansvarlig == null || suggOld.Ansvarlig == "") {
                err = await ApproveAndUpdateSuggestion(sugg);
                return err;
            }
            if (suggOld.Creator == currentUser.EmploymentId || 
                suggOld.Ansvarlig == currentUser.EmploymentId || 
                suggOld.OwnerTeam == currentUser.TeamId)
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
                suggOld.Progression = sugg.Progression;
                suggOld.ActiveStatus = sugg.ActiveStatus;

                if (sugg.Progression >= 5 || sugg.Status == SuggStatus.Complete)
                {
                    suggOld.Progression = 5;
                    suggOld.Status = SuggStatus.Complete;
                    suggOld.Completer = currentUser.EmploymentId;
                }
                
                err = await UpdateSuggestion(suggOld);
            }
            else
            {
                err = "You are not allowed to edit this suggestion";
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
                e.FillCatEntity();
                SuggViewmodel sugg = new(e);
                suggViewmodels.Add(sugg);
            }
            return suggViewmodels;
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
                e.FillCatEntity();
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
                if (e.sugStatus != SuggStatus.Waiting &&
                    e.sugStatus != SuggStatus.Complete &&
                    e.sugStatus != SuggStatus.Rejected)
                {
                    e.FillCatEntity();
                    SuggViewmodel sugg = new(e);
                    suggViewmodels.Add(sugg);
                }
            }
            return suggViewmodels;
        }
        
        

        //Sjekker om input string faller inn under karaktersettet latin1.
        private static bool IsValidISO(string input)
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

            CommentEntity commentEntity = new(comment);
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
