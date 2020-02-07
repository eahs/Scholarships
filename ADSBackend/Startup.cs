using Scholarships.Configuration;
using Scholarships.Data;
using Scholarships.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scholarships.Services;
using Hangfire;
using Hangfire.Dashboard;
using Scholarships.Tasks;
using Scholarships.Util;
using System;
using System.IO;
using Smidge;
using System.Net.Mail;
using System.Data;
using System.Linq;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Scholarships.Tasks.Importer;
using Serilog;
using Smidge.Cache;
using Smidge.Options;

namespace Scholarships
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; set; }
        public string ConnString { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;

            if (Env.IsDevelopment())
                ConnString = Configuration.GetConnectionString("ScholarshipsDevelopmentContext");
            else if (Env.IsStaging())
                ConnString = Configuration.GetConnectionString("ScholarshipsStagingContext");
            else if (Env.IsProduction())
                ConnString = Configuration.GetConnectionString("ScholarshipsProductionContext");

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IMvcBuilder builder = services.AddRazorPages();

#if DEBUG
            if (Env.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // options.UseSqlServer(Configuration.GetConnectionString("ScholarshipsContext"));
                options.UseMySql(
                    ConnString,
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(10, 4, 11),
                            Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb); // replace with your Server Version and Type
                    });

            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            services.AddTransient<Services.Configuration>();

            // Add application services.
            services.AddTransient<Services.IEmailSender, Services.EmailSender>();
            services.AddTransient<Services.DataService>();
            services.AddScoped<Services.ViewRenderService, Services.ViewRenderService>();

            // services.AddSingleton<ITaskRegistry, TaskRegistry>();

            // Hangfire and related services
            var hfoptions =
                new MySqlStorageOptions
                {
                    TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                    TablePrefix = "Hangfire"
                };

            try
            {
                var storage = new MySqlStorage(ConnString, hfoptions);

                services.AddHangfire(
                    //x => x.UseSqlServerStorage(Configuration.GetConnectionString("ScholarshipsContext"))
                    config => config.UseStorage(storage)
                );
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to connect to MySQL Database");
                throw;
            }

            // register tasks
            services.AddScoped<IGenerateTranscripts, GenerateTranscripts>();
            services.AddScoped<IEmailQueue, EmailQueue>();
            services.AddScoped<IStudentDataImporter, StudentDataImporter>();

            // caching
            services.AddMemoryCache();
            services.AddTransient<Services.Cache>();


            services.AddMvc();

            // Add javascript minification library
            services.AddSmidge(Configuration.GetSection("smidge"))
                    .Configure<SmidgeOptions>(options =>
                    {
                        options.DefaultBundleOptions.DebugOptions.SetCacheBusterType<AppDomainLifetimeCacheBuster>();
                        options.DefaultBundleOptions.ProductionOptions.SetCacheBusterType<AppDomainLifetimeCacheBuster>();
                    });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                //options.RequireHeaderSymmetry = false;
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                //options.KnownNetworks.Clear();
                //options.KnownProxies.Clear();
            });
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
            }

            if (env.IsProduction())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto
                });
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            // Set up hangfire capabilities
            var options = new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            };

            app.UseHangfireDashboard("/hangfire", options);
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<IGenerateTranscripts>(
                    generator => generator.Execute(), Cron.Daily);

            BackgroundJob.Enqueue<IStudentDataImporter>(
                generator => generator.Execute());

            //BackgroundJob.Enqueue<IEmailQueue>(
            //    queue => queue.SendMail("tanczosm@eastonsd.org", "Test Message", "this is a scholarship email test.. did you get this?"));

            // If you want to run the job immediately
            /*
            BackgroundJob.Enqueue<IGenerateTranscripts>(
                generator => generator.Execute());
            */

            // Set up App_Data directory
            // set
            string baseDir = env.ContentRootPath;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(baseDir, "App_Data"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "securedownload",
                    pattern: "{controller=SecureDownload}/{id?}/{filename?}",
                    defaults: new { controller = "SecureDownload", action = "Download" }
                    );
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // seed the AspNetRoles table
                var roleSeed = new ApplicationRoleSeed(roleManager);
                roleSeed.CreateRoles();

                // seed the AspNetUsers table
                var userSeed = new ApplicationUserSeed(userManager, dbContext);
                userSeed.CreateAdminUser();

                var dbSeed = new ApplicationDbSeed(dbContext);
                dbSeed.SeedDatabase();
            }

            app.UseSmidge(bundles =>
            {
                bundles.CreateCss("scholarship-application-css",
                                    "~/lib/bootstrap/dist/css/bootstrap.css",
                                    "~/lib/font-awesome/css/font-awesome.css",
                                    "~/css/animate.css",
                                    "~/css/style.css",
                                    "~/css/site.css",
                                    "~/lib/heart/heart.css");

                // Libraries
                bundles.CreateJs("scholarship-js-libraries",                                    
                                    "~/lib/jquery/dist/jquery.js",
                                    "~/lib/jquery-ui/jquery-ui.js",
                                    "~/lib/Popper/popper.js",
                                    "~/lib/bootstrap/dist/js/bootstrap.js"
                                    );

                // Custom scripts
                bundles.CreateJs("scholarship-js-custom",
                                    "~/js/site.js");

                // Inspinia scripts
                bundles.CreateJs("scholarship-js-inspinia",
                                    "~/lib/metisMenu/dist/jquery.metisMenu.js",
                                    "~/lib/slimScroll/jquery.slimscroll.js",
                                    "~/lib/pace/pace.js",
                                    "~/js/script.js");
            });
        }

    }
}

