using Scholarships.Data;
using Scholarships.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Scholarships.Util;

namespace Scholarships.Services
{
    public class Configuration
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        
        private static bool ConfigLoaded = false;

        public Configuration(ApplicationDbContext context, IMemoryCache cache, IWebHostEnvironment env,
            IConfiguration Configuration)
        {
            _context = context;
            _cache = cache;

            IConfigurationSection section = null;

            if (!ConfigLoaded)
            {
                ConfigLoaded = true;

                // Depending on development environment, copy variables into a new configuration prefixed by "Paths"
                if (env.IsDevelopment())
                    section = Configuration.GetSection("FilePaths:Development");
                else if (env.IsStaging())
                    section = Configuration.GetSection("FilePaths:Staging");
                else if (env.IsProduction())
                    section = Configuration.GetSection("FilePaths:Production");

                if (section == null)
                {
                    Log.Error("FilePaths section missing from appsettings.json!");
                }
                else
                {
                    var pairs = section.AsEnumerable();
                    foreach (var (key, value) in pairs)
                    {
                        if (value == null)
                            continue;

                        var lastkey = "Paths:" +  key.Split(":").LastOrDefault();

                        var current = Get(lastkey);
                        
                        if (current == value)
                            continue;

                        Set(lastkey, value);
                    }

                    AsyncHelpers.RunSync(() => SaveChangesAsync());
                }
            }
        }

        public string Get(string key)
        {
            var config = _cache.Get<List<ConfigurationItem>>("Configuration");

            if(config == null)
            {
                config = InitializeConfigCache();
            }

            return config.FirstOrDefault(x => x.Key == key)?.Value;
        }

        public void Set(string key, string value)
        {
            var configItem = new ConfigurationItem
            {
                Key = key,
                Value = Get(key)
            };

            if (configItem.Value != null)
            {
                _context.Attach(configItem);
            }

            configItem.Value = value;

            _context.AddOrUpdate(configItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            InitializeConfigCache();
        }

        private List<ConfigurationItem> InitializeConfigCache()
        {
            var config = _context.ConfigurationItem.ToList();
            _cache.Set("Configuration", config);
            return config;
        }
    }
}
