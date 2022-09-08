namespace BlazorTipz.Data
{
    public class UserDto
    {
        public int? employmentId { get; set; }
        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        
        public string password { get; set; } = string.Empty;
        public string RepeatPassword { get; set; }
        public string role { get; set; } = "User";
        public string teamName { get; set; } = string.Empty;
    }
}
