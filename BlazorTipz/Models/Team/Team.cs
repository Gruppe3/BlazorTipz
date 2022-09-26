using System;

namespace BlazorTipz.Models.Team
{
    public abstract class Team
    {
        public string teamName { get; set; } = string.Empty;
        public string teamLeader { get; set; } = string.Empty;
        public string teamid { get; set; } = string.Empty;
    }
}

