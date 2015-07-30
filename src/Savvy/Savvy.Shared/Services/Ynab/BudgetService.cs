using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DropboxRestAPI;
using DropboxRestAPI.Models.Core;
using DropboxRestAPI.RequestsGenerators;
using Newtonsoft.Json.Linq;
using Savvy.Extensions;
using Savvy.Services.Dropbox;
using Savvy.Services.Session;
using Savvy.Services.Settings;

namespace Savvy.Services.Ynab
{
    public class BudgetService : IBudgetService
    {
        private readonly ISessionService _sessionService;
        private readonly ISettings _settings;

        public BudgetService(ISessionService sessionService, ISettings settings)
        {
            this._sessionService = sessionService;
            this._settings = settings;
        }

        public async Task<IReadOnlyCollection<Budget>> GetBudgetsAsync()
        {
            var client = this.GetClient();

            if (client == null)
                return new ReadOnlyCollection<Budget>(new List<Budget>());
            
            var file = await client.Core.Metadata.JsonFileAsync("/.ynabSettings.yroot");

            var budgetsFolderPath = file.Value<string>("relativeDefaultBudgetsFolder");

            var budgets = file
                .Value<JArray>("relativeKnownBudgets")
                .Values()
                .Select(f => f.Value<string>())
                .Select(f => new Budget(this._sessionService.DropboxUserId, this.ExtractBudgetName(f, budgetsFolderPath) , f))
                .ToList();

            return new ReadOnlyCollection<Budget>(budgets);
        }

        private string ExtractBudgetName(string budgetPath, string budgetsFolderPath)
        {
            var regex = new Regex(budgetsFolderPath + "/(.*)~.*");

            if (regex.IsMatch(budgetPath))
                return regex.Match(budgetPath).Groups[1].Value;

            return budgetPath;

        }

        private Client GetClient()
        {
            if (string.IsNullOrWhiteSpace(this._sessionService.DropboxAccessToken))
                return null;

            var options = new Options
            {
                ClientId = this._settings.DropboxClientId,
                ClientSecret = this._settings.DropboxClientSecret,
                AccessToken = this._sessionService.DropboxAccessToken,
            };

            return new Client(new SavvyHttpClientFactory(), new RequestGenerator(), options);
        }
    }
}
