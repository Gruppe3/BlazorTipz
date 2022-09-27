using BlazorTipz.Data;
using DataLibrary;

namespace BlazorTipz.Models
{
    public class UserDb : UserA
    {
        //inject _data
        private readonly IDataAccess _data;
        private readonly IConfiguration _connectionString;

        public UserDb(IDataAccess data, IConfiguration connectionString)
        {
            _data = data;
            _connectionString = connectionString;
        }



        private byte[] passwordHash { get; set; }
        private byte[] passwordSalt { get; set; }

        public string token { get; }

        //inject _data
        
        

        //constructor
        public UserDb()
        {

        }
        public UserDb(UserA user) 
        {
            this.employmentId = user.employmentId;
            this.teamId = user.teamId;
            this.name = user.name;
            this.role = user.role;
        }
        public UserDb(string employmentId) 
        {
            this.employmentId = employmentId;
        }

        private async Task fillFromDb(string empId)
        {
            try 
            {
                
            }
            catch
            { 
            
            }
        }
    }
}
