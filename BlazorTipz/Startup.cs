using DataLibrary;
using Microsoft.AspNetCore.Builder;

namespace BlazorTipz
{
    public class Startup
    {

        public Startup(IConfiguration condiguration)
        {
            Configuration = condiguration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<Data.WeatherForecastService>();
            services.AddSingleton<IDataAccess, DataAccess>();
        }

        
    }
}
