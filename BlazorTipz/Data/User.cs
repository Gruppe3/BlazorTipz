using System.ComponentModel.DataAnnotations;

namespace BlazorTipz.Data
{
    public class User
    {
        public int id { get; set; }
        public string f_name { get; set; }
        public string l_name { get; set; }
        public string email { get; set; }

        public string password { get; set; }
    }
}
