using Albumify.Domain;
using Albumify.Domain.Spotify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Albumify.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1#basic-usage
            // Enable basic usage of IHttpClientFactory
            // By default, all clients created using IHttpClientFactory will record log messages for all requests
            services.AddHttpClient<ISpotifyAuthorization, SpotifyClientCredentialsFlow>();
            services.AddHttpClient<ISpotifyService, SpotifyWebApi>();
            services.AddHttpClient<I3rdPartyMusicService, SpotifyWebApi>();

            // Register as a singleton since it takes a direct dependency on MongoClient, which suggests
            // MongoClient be registered in DI with a singleton service lifetime.
            // https://mongodb.github.io/mongo-csharp-driver/2.8/reference/driver/connecting/#re-use
            services.AddSingleton<IMyCollectionRepository, MongoDbAlbumRepository>();

            services.AddScoped<AlbumifyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
