﻿namespace BlazorTipz.Data
{
    public class UserDto
    {
        public int? employmentId { get; set; }
        public string iName { get; set; } = string.Empty;
        
        public string password { get; set; } = string.Empty;
        public string RepeatPassword { get; set; }
        public string role { get; set; } = "User";
        public string teamName { get; set; } = string.Empty;
        public int? teamid { get; set; }
    }
}
