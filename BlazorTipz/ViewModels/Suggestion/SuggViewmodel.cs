using BlazorTipz.Data;
using BlazorTipz.Models;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggViewmodel
    {
        
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
        public string OwnerTeamName { get; set; }
        public string CreatorName { get; set; }
        public string AnsvarligName { get; set; }
       


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
            this.Ansvarlig = Suggestion.Assigned;
            this.Frist = Suggestion.Deadline;
            this.BeforeImage = Suggestion.BeforeImage;
            this.AfterImage = Suggestion.AfterImage;

            this.category = new Category(Suggestion.CategoryEntity);
        }
    }
    
}
