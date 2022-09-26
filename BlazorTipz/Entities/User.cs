using BlazorTipz.Data;

namespace BlazorTipz.Entities
{
    public class Userdb : UserA
    {
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
    }
}
