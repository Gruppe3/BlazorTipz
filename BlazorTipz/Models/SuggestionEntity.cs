﻿using BlazorTipz.Data;
using BlazorTipz.ViewModels.Suggestion;

namespace BlazorTipz.Models
{
    public class SuggestionEntity
    {
        //db colums
        public string? sugId { get; set; }
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
        public byte[]? BeforeImage { get; set; }
        public byte[]? AfterImage { get; set; }
        public string? assigned { get; set; }
        public string deadline { get; set; }
        //ekstra fields
        public CategoriEntity CategoryEntity { get; set; }

        public SuggestionEntity()
        {
            
        }

        public SuggestionEntity(SuggViewmodel sugg)
        {
            this.sugId = sugg.Id;
            this.owner = sugg.OwnerTeam;
            this.sugTitle = sugg.Title;
            this.sugDesc = sugg.Description;
            this.JustDoIt = sugg.JustDoIt;
            this.creator = sugg.Creator;
            this.status = sugg.Status;
            this.createdAt = sugg.StartDate;
            this.lastChanged = sugg.UpdatedDate;
            this.AfterImage = sugg.AfterImage;
            this.BeforeImage = sugg.BeforeImage;
            this.assigned = sugg.Ansvarlig;
            this.deadline = sugg.Frist;
            
            this.CategoryEntity = new CategoriEntity(sugg.category);
        }
    }
}
