using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DropboxRestAPI.Services.Core;
using Newtonsoft.Json.Linq;

namespace Savvy.Extensions
{
    public static class DropboxExtensions
    {
        public static async Task<JObject> JsonFileAsync(this IMetadata self, string path)
        {
            using (var fileStream = new MemoryStream())
            {
                await self.FilesAsync(path, fileStream);

                byte[] jsonBytes = fileStream.ToArray();
                string jsonString = Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length);

                return JObject.Parse(jsonString);
            }
        }
    }
}
