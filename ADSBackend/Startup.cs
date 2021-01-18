using Hangfire;
using Hangfire.MySql.Core;
using IronPdf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Scholarships.Configuration;
using Scholarships.Data;
using Scholarships.Models.Identity;
using Scholarships.Services;
using Scholarships.Tasks;
using Scholarships.Tasks.Importer;
using Scholarships.Util;
using Serilog;
using Smidge;
using Smidge.Cache;
using Smidge.Options;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Scholarships
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            if (Env.IsDevelopment())
            {
                ConnString = Configuration.GetConnectionString("ScholarshipsDevelopmentContext");
            }
            else if (Env.IsStaging())
            {
                ConnString = Configuration.GetConnectionString("ScholarshipsStagingContext");
            }
            else if (Env.IsProduction())
            {
                ConnString = Configuration.GetConnectionString("ScholarshipsProductionContext");

                Installation.TempFolderPath = @"/tmp/scholarships";
            }
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; set; }
        public string ConnString { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddRazorPages();

#if DEBUG
            if (Env.IsDevelopment()) builder.AddRazorRuntimeCompilation();
#endif


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // options.UseSqlServer(Configuration.GetConnectionString("ScholarshipsContext"));
                options.UseMySql(
                    ConnString,
                    mySqlOptions =>
                    {
                        mySqlOptions.ServerVersion(new Version(10, 4, 11),
                            ServerType.MariaDb); // replace with your Server Version and Type
                    });
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // https://www.thinktecture.com/identity/samesite/prepare-your-identityserver/
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            var paths = Configuration.GetSection("Paths");
            if (paths.GetChildren().Any(x => x.Key == "DataProtectionKeys"))
            {
                var dppath = paths["DataProtectionKeys"];

                if (dppath != null)
                    services.AddDataProtection()
                        .PersistKeysToFileSystem(new DirectoryInfo(dppath));
            }
            else
            {
                Log.Error(
                    "Paths:DataProtectionKeys was not set in AppSettings.json - Missing data protection key path!");
            }

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    var googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                });

            services.AddTransient<Services.Configuration>();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<DataService>();
            services.AddScoped<ViewRenderService, ViewRenderService>();

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
            services.AddScoped<ICreateApplicationPackage, CreateApplicationPackage>();

            // caching
            services.AddMemoryCache();
            services.AddTransient<Cache>();


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
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            if (env.IsProduction())
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto
                });

            app.UseRouting();
            app.UseStaticFiles();
            app.UseCookiePolicy(); // https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
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
            //BackgroundJob.Enqueue<ICreateApplicationPackage>(queue => queue.Execute());

            // If you want to run the job immediately
            /*
            BackgroundJob.Enqueue<IGenerateTranscripts>(
                generator => generator.Execute());
            */

            // Set up App_Data directory
            // set
            var baseDir = env.ContentRootPath;
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(baseDir, "App_Data"));

            app.UseStatusCodePagesWithRedirects("/error/{0}");
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    "securedownload",
                    "{controller=SecureDownload}/{id?}/{filename?}",
                    new { controller = "SecureDownload", action = "Download" }
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

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent)) options.SameSite = SameSiteMode.Unspecified;
            }
        }

        /// <summary>
        ///     Checks if the UserAgent is known to interpret an unknown value as Strict.
        ///     For those the <see cref="CookieOptions.SameSite" /> property should be
        ///     set to <see cref="Unspecified" />.
        /// </summary>
        /// <remarks>
        ///     This code is taken from Microsoft:
        ///     https://devblogs.microsoft.com/aspnet/upcoming-samesite-cookie-changes-in-asp-net-and-asp-net-core/
        /// </remarks>
        /// <param name="userAgent">The user agent string to check.</param>
        /// <returns>Whether the specified user agent (browser) accepts SameSite=None or not.</returns>
        private static bool DisallowsSameSiteNone(string userAgent)
        {
            // Cover all iOS based browsers here. This includes:
            //   - Safari on iOS 12 for iPhone, iPod Touch, iPad
            //   - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            //   - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the
            // iOS networking stack.
            // Notes from Thinktecture:
            // Regarding https://caniuse.com/#search=samesite iOS versions lower
            // than 12 are not supporting SameSite at all. Starting with version 13
            // unknown values are NOT treated as strict anymore. Therefore we only
            // need to check version 12.
            if (userAgent.Contains("CPU iPhone OS 12")
                || userAgent.Contains("iPad; CPU OS 12"))
                return true;

            // Cover Mac OS X based browsers that use the Mac OS networking stack.
            // This includes:
            //   - Safari on Mac OS X.
            // This does not include:
            //   - Chrome on Mac OS X
            // because they do not use the Mac OS networking stack.
            // Notes from Thinktecture: 
            // Regarding https://caniuse.com/#search=samesite MacOS X versions lower
            // than 10.14 are not supporting SameSite at all. Starting with version
            // 10.15 unknown values are NOT treated as strict anymore. Therefore we
            // only need to check version 10.14.
            if (userAgent.Contains("Safari")
                && userAgent.Contains("Macintosh; Intel Mac OS X 10_14")
                && userAgent.Contains("Version/"))
                return true;

            // Cover Chrome 50-69, because some versions are broken by SameSite=None
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions,
            // but pre-Chromium Edge does not require SameSite=None.
            // Notes from Thinktecture:
            // We can not validate this assumption, but we trust Microsofts
            // evaluation. And overall not sending a SameSite value equals to the same
            // behavior as SameSite=None for these old versions anyways.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6")) return true;

            return false;
        }
    }
}