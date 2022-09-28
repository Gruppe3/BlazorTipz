using System.ComponentModel.DataAnnotations;

namespace BlazorTipz.Data
{
    public class User
    {
        public int? employmentId { get; set; }
        public int? teamid { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        public string iName { get; set; } = string.Empty;
        public string role { get; set; }
    }
}
