using Application.Common.Interface.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly string _fileName = "get_post_param.json";
        private readonly string _filePath;

        public TagRepository()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), _fileName);
        }

        public async Task<List<string>> GetTagsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<string>();
            }

            var jsonContent = await File.ReadAllTextAsync(_filePath);
            Console.WriteLine($"JSON Content: {jsonContent}"); // Log the JSON content for debugging

            try
            {
                var settingsData = JsonConvert.DeserializeObject<SettingsData>(jsonContent);
                return settingsData?.Tags ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetFiltersAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<string>();
            }

            var jsonContent = await File.ReadAllTextAsync(_filePath);
            Console.WriteLine($"JSON Content: {jsonContent}"); // Log the JSON content for debugging

            try
            {
                var settingsData = JsonConvert.DeserializeObject<SettingsData>(jsonContent);
                return settingsData?.Filters ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<string>();
            }
        }
    }

    public class SettingsData
    {
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Filters { get; set; } = new List<string>();
    }
}
