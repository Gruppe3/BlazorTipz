using System.ComponentModel.DataAnnotations;

namespace BlazorTipz.Data
{
    public class User
    {
        public int? id { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
    }
}
