using BlazorTipz.Data;
using BlazorTipz.Models;
using BlazorTipz.ViewModels.User;
using BlazorTipz.ViewModels.Team;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggViewmodel
    {
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool JustDoIt { get; set; } = false;
        public Category category { get; set; } 
        public string OwnerTeam { get; set; }
        public string Creator { get; set; }
        public SuggStatus Status { get; set; } = SuggStatus.Waiting;
        public string StartDate { get; set; }
        public string UpdatedDate { get; set; } = string.Empty;
        public string? Ansvarlig { get; set; }
        public string Frist { get; set; }
        //picture
        public byte[]? BeforeImage { get; set; }
        public byte[]? AfterImage { get; set; }
        //for view
        public string OwnerTeamName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string AnsvarligName { get; set; } = string.Empty;

        public DateTime? FristTid { get; set; }



        public SuggViewmodel()
        { 
        }

        public SuggViewmodel(SuggestionEntity Suggestion)
        {
            this.Title = Suggestion.sugTitle;
            this.Description = Suggestion.sugDesc;
            this.JustDoIt = Suggestion.JustDoIt;           
            this.OwnerTeam = Suggestion.owner;
            this.Creator = Suggestion.creator;
            this.Status = Suggestion.status;
            this.StartDate = Suggestion.createdAt;
            this.UpdatedDate = Suggestion.lastChanged;
            this.Ansvarlig = Suggestion.assigned;
            this.Frist = Suggestion.deadline;
            this.BeforeImage = Suggestion.BeforeImage;
            this.AfterImage = Suggestion.AfterImage;
            this.Id = Suggestion.sugId;

            this.category = new Category(Suggestion.CategoryEntity);
        }

        public async Task<string> GetCreatorName(IUserManager _userManager)
        {
            if (_userManager == null) { return "Error"; }
            if(CreatorName == string.Empty)
            {
                UserViewmodel user = await _userManager.getUser(Creator);
                if (user != null) { CreatorName = user.name; }
            }
            return CreatorName;
        }
        public async Task SetCreatorName(IUserManager _userManager)
        {
            await GetCreatorName(_userManager);
        }
        public async Task<string> GetAnsvarligName(IUserManager _userManager)
        {
            if (_userManager == null) { return "Ikke Satt"; }
            if (CreatorName == string.Empty)
            {
                UserViewmodel user = await _userManager.getUser(Ansvarlig);
                if (user != null) { AnsvarligName = user.name; }
            }
            return AnsvarligName;
        }
        public async Task SetAnsvarligName(IUserManager _userManager)
        {
            await GetAnsvarligName(_userManager);
        }

        public async Task<string> GetOwnerTeamName(ITeamManager _teamManager)
        {
            if (_teamManager == null) { return "Error"; }
            if (OwnerTeamName == string.Empty)
            {
                TeamViewmodel team = await _teamManager.getTeam(OwnerTeam);
                if (team != null) { OwnerTeamName = team.name; }
            }
            return OwnerTeamName;
        }

        public async Task SetOwnerTeamName(ITeamManager _teamManager)
        {
            await GetOwnerTeamName(_teamManager);
        }

        public void SetFristTidToFrist()
        {
            if(FristTid == null) { return; }

            string? temp = FristTid.ToString();
            Frist = temp;
        }
        //from string to datetime
    }
    
}
