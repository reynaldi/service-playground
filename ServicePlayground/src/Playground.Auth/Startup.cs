using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Playground.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(opt => 
                {
                    opt.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString("PlaygroundAuthDb"), sql => sql.MigrationsAssembly("Playground.Auth"));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString("PlaygroundAuthDb"), sql => sql.MigrationsAssembly("Playground.Auth"));
                    // this enables automatic token cleanup. this is optional.
                    opt.EnableTokenCleanup = true;
                    opt.TokenCleanupInterval = 30;
                });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                var persistedGrant = serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>();
                var configurationDb = serviceScope.ServiceProvider.GetService<ConfigurationDbContext>();
                if (persistedGrant.Database.GetPendingMigrations().Any())
                {
                    persistedGrant.Database.Migrate();
                }
                if (configurationDb.Database.GetPendingMigrations().Any())
                {
                    configurationDb.Database.Migrate();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
