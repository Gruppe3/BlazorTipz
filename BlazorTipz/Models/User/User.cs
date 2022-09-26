using BlazorTipz.Data;

namespace BlazorTipz.Models.User
{
    public abstract class User
    {
        public string name { get; set; } = string.Empty;
        public string employmentId { get; set; }
        public RoleE role { get; set; } = RoleE.User;
        public string teamName { get; set; }
    }
}
