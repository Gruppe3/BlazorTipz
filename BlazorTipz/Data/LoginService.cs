
namespace BlazorTipz.Data
{
    public class LoginService
    {
        public Task<Login[]> GetLogins()
        {
            return Task.FromResult(new Login[0]);
        }
    }
}
