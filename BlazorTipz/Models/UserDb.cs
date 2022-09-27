using BlazorTipz.Data;

namespace BlazorTipz.Models
{
    public class UserDb : UserA
    {
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
    }
}
