using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Scholarships.Data;
using Scholarships.Models;
using Scholarships.Models.ConfigurationViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Services
{
    public class Configuration
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ConfigPath ConfigPath { get; set; } = new ConfigPath();

        public Configuration(ApplicationDbContext context, IMemoryCache cache, IWebHostEnvironment env, IConfiguration Configuration)
        {
            _context = context;
            _cache = cache;

            Configuration.GetSection("Paths")?.Bind(ConfigPath);

        }

        public string Get(string key)
        {
            var config = _cache.Get<List<ConfigurationItem>>("Configuration");

            if (config == null)
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
