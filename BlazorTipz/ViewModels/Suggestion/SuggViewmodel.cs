using BlazorTipz.Data;
using BlazorTipz.Models;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggViewmodel
    {
        public string? Id { get; set; } = null;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool JustDoIt { get; set; } = false;
        public Category Category { get; set; }
        public string OwnerTeam { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public SuggStatus Status { get; set; } = SuggStatus.Waiting;
        public int Progression { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? Ansvarlig { get; set; } = string.Empty;
        public DateTime Frist { get; set; }
        //picture
        public string? BeforeImage { get; set; }
        public string? AfterImage { get; set; }
        //for view
        public string OwnerTeamName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string AnsvarligName { get; set; } = string.Empty;
        
        //public DateTime? FristTid { get; set; }



        public SuggViewmodel()
        {
            Category = new();
            Frist = DateTime.Now;
        }

        public SuggViewmodel(SuggestionEntity Suggestion)
        {
            this.Title = Suggestion.sugTitle;
            this.Description = Suggestion.sugDesc;
            this.JustDoIt = Suggestion.justDoIt;           
            this.OwnerTeam = Suggestion.ownerId;
            this.Creator = Suggestion.creatorId;
            this.Status = Suggestion.sugStatus;
            this.Progression = Suggestion.sugProgression;
            this.StartDate = Suggestion.createdAt;
            this.UpdatedDate = Suggestion.lastChanged;
            this.Ansvarlig = Suggestion.assignedId;
            this.Frist = Suggestion.dueDate;
            this.BeforeImage = Suggestion.beforeImage;
            this.AfterImage = Suggestion.afterImage;
            this.Id = Suggestion.sugId;
            
            this.Category = new Category(Suggestion.CatEntity);
            this.OwnerTeamName = Suggestion.teamName;
            this.CreatorName = Suggestion.creatorName;
            this.AnsvarligName = Suggestion.assignedName;
            //SetFristToFristTid();
        }

    }
}
