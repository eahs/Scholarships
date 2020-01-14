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

#if DEBUG
            ConnString = Configuration.GetConnectionString("ScholarshipsDevelopmentContext");
#else
            ConnString = Configuration.GetConnectionString("ScholarshipsProductionContext");
#endif
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

            // Add application services.
            services.AddTransient<Services.IEmailSender, Services.EmailSender>();
            services.AddTransient<Services.DataService>();
            services.AddScoped<Services.ViewRenderService, Services.ViewRenderService>();

            // services.AddSingleton<ITaskRegistry, TaskRegistry>();
            
            // Hangfire and related services
            services.AddHangfire(
                x => x.UseSqlServerStorage(Configuration.GetConnectionString("ScholarshipsContext"))
                );

            services.AddScoped<IGenerateTranscripts, GenerateTranscripts>();


            // caching
            services.AddMemoryCache();
            services.AddTransient<Services.Cache>();

            services.AddTransient<Services.Configuration>();

            services.AddMvc();

            // Add javascript minification library
            services.AddSmidge(Configuration.GetSection("smidge"));

            // https://support.google.com/a/answer/176600?hl=en
            SmtpClient client = new SmtpClient("smtp.gmail.com", 465);
            
            System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("scholarship@gmail.com", "Password");            
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicauthenticationinfo;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            services.AddFluentEmail("scholarship@eastonsd.org")
                    .AddRazorRenderer()
                    .AddSmtpSender(client);

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
                                    "~/css/site.css");

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

