using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;

namespace BlazorTipz.Models
{
    public class SuggestionEntity
    {
        //db colums
        public string? sugId { get; set; }
        public string ownerId { get; set; }
        public string creatorId { get; set; }
        public string? assignedId { get; set; }
        public string? completerId { get; set; }
        public SuggStatus sugStatus { get; set; }
        public string? sugProgression { get; set; }
        public string sugTitle { get; set; }
        public string sugDesc { get; set; }
        public string createdAt { get; set; }
        public string lastChanged { get; set; }
        public DateTime dueDate { get; set; }
        public string categoryId { get; set; }
        public bool justDoIt { get; set; }
        public string? beforeImage { get; set; }
        public string? afterImage { get; set; }
        
        
        //ekstra fields
        public CategoriEntity CategoryEntity { get; set; }

        public SuggestionEntity()
        {
            
        }

        public SuggestionEntity(SuggViewmodel sugg)
        {
            this.sugId = sugg.Id;
            this.ownerId = sugg.OwnerTeam;
            this.sugTitle = sugg.Title;
            this.sugDesc = sugg.Description;
            this.justDoIt = sugg.JustDoIt;
            this.creatorId = sugg.Creator;
            this.sugStatus = sugg.Status;
            this.createdAt = sugg.StartDate;
            this.lastChanged = sugg.UpdatedDate;
            this.afterImage = sugg.AfterImage;
            this.beforeImage = sugg.BeforeImage;
            this.assignedId = sugg.Ansvarlig;
            this.dueDate = sugg.Frist;
            
            this.CategoryEntity = new CategoriEntity(sugg.Category);
        }
    }
}
