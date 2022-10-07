using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;

namespace BlazorTipz.Models
{
    public class SuggestionEntity
    {
        public string sugId { get; set; }
        public string owner { get; set; }
        public string creator { get; set; }
        public string completer { get; set; }
        public SuggStatus status { get; set; }
        public string sugTitle { get; set; }
        public string sugDesc { get; set; }
        public string createdAt { get; set; }
        public string lastChanged { get; set; }
        public string Category { get; set; }
        public bool JustDoIt { get; set; }
        public string BeforeImage { get; set; }
        public string AfterImage { get; set; }

        public SuggestionEntity()
        {
            
        }

        public SuggestionEntity(SuggViewmodel sugg)
        {
            this.owner = sugg.OwnerTeam;
            this.sugTitle = sugg.Title;
            this.sugDesc = sugg.Description;
            this.JustDoIt = sugg.JustDoIt;
            this.Category = sugg.Category;
            this.creator = sugg.Creator;
            this.status = sugg.Status;
            this.createdAt = sugg.StartDate;
            this.lastChanged = sugg.UpdatedDate;
        }
    }
}
