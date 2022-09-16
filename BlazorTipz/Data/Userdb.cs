using System.ComponentModel.DataAnnotations;

namespace BlazorTipz.Data
{
    public class Userdb
    {
        public int? employmentId { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        public string role { get; set; }
        public int? teamId { get; set; } 
        public string teamName { get; set; } = string.Empty;
    }
}
