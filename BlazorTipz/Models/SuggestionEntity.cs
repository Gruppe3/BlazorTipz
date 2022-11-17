using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;

namespace BlazorTipz.Models
{
    public class SuggestionEntity
    {
        //db colums
        public string? sugId { get; set; } = string.Empty;
        public string ownerId { get; set; } = string.Empty;
        public string creatorId { get; set; } = string.Empty;
        public string? assignedId { get; set; } = string.Empty;
        public string? completerId { get; set; } = string.Empty;
        public SuggStatus sugStatus { get; set; }
        public int sugProgression { get; set; }
        public string sugTitle { get; set; } = string.Empty;
        public string sugDesc { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public DateTime lastChanged { get; set; }
        public DateTime dueDate { get; set; }
        public string categoryId { get; set; } = string.Empty;
        public bool justDoIt { get; set; } = false;
        public string? beforeImage { get; set; }
        public string? afterImage { get; set; }


        //ekstra fields
        public CategoriEntity CatEntity { get; set; }
        public string teamName { get; set; } = string.Empty;
        public string creatorName { get; set; } = string.Empty;
        public string assignedName { get; set; } = string.Empty;
        public string catName { get; set; } = string.Empty;



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
            this.sugProgression = sugg.Progression;
            this.createdAt = sugg.StartDate;
            this.lastChanged = sugg.UpdatedDate;
            this.afterImage = sugg.AfterImage;
            this.beforeImage = sugg.BeforeImage;
            this.assignedId = sugg.Ansvarlig;
            this.dueDate = sugg.Frist;
            
            this.CatEntity = new CategoriEntity(sugg.Category);
        }
    }
}
