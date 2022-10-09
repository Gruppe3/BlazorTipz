﻿using BlazorTipz.Data;
using BlazorTipz.Models;

namespace BlazorTipz.ViewModels.Suggestion
{
    public class SuggViewmodel
    {
        
        public string Title { get; set; }
        public string Description { get; set; }
        public bool JustDoIt { get; set; } = false;
        public string Category { get; set; } = string.Empty;
        public string OwnerTeam { get; set; }
        public string Creator { get; set; }
        public SuggStatus Status { get; set; } = SuggStatus.Waiting;
        public string StartDate { get; set; }
        public string UpdatedDate { get; set; } = string.Empty;



        public SuggViewmodel()
        { 
        }

        public SuggViewmodel(SuggestionEntity Suggestion)
        {
            this.Title = Suggestion.sugTitle;
            this.Description = Suggestion.sugDesc;
            this.JustDoIt = Suggestion.JustDoIt;
            this.Category = Suggestion.Category;
            this.OwnerTeam = Suggestion.owner;
            this.Creator = Suggestion.creator;
            this.Status = Suggestion.status;
            this.StartDate = Suggestion.createdAt;
            this.UpdatedDate = Suggestion.lastChanged;
        }
    }
}