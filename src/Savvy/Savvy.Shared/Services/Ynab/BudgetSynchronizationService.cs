using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using DropboxRestAPI;
using DropboxRestAPI.Models.Core;
using DropboxRestAPI.RequestsGenerators;
using LiteGuard;
using Newtonsoft.Json;
using Savvy.Services.Dropbox;
using Savvy.Services.Session;
using Savvy.Services.Settings;

namespace Savvy.Services.Ynab
{
    public class BudgetSynchronizationService : IBudgetSynchronizationService
    {
        #region Fields
        private readonly ISettings _settings;
        private readonly ISessionService _sessionService;

        private readonly Timer _timer;

        private List<SynchronizedBudget> _budgets;
        private bool _loadedBudgets;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetSynchronizationService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="sessionService">The session service.</param>
        public BudgetSynchronizationService(ISettings settings, ISessionService sessionService)
        {
            Guard.AgainstNullArgument("settings", settings);
            Guard.AgainstNullArgument("sessionService", sessionService);

            this._settings = settings;
            this._sessionService = sessionService;

            this._timer = new Timer(this.TimerTick, null, -1, -1);

            this._loadedBudgets = false;
            this._budgets = new List<SynchronizedBudget>();
        }
        #endregion

        #region Implementation of IBudgetSynchronizationService
        public async Task SynchronizeBudgetInBackground(Budget budget)
        {
            var budgetToAdd = new SynchronizedBudget(budget);

            if (this._budgets.Contains(budgetToAdd) == false)
            {
                this._budgets.Add(budgetToAdd);
                await this.SaveBudgetsAsync();
            }
        }
        public async Task<IReadOnlyCollection<SynchronizedBudget>> GetSynchronizedBudgets()
        {
            if (this._loadedBudgets == false)
            {
                await this.LoadBudgetsAsync();
                this._loadedBudgets = true;
            }

            return new ReadOnlyCollection<SynchronizedBudget>(this._budgets);
        }

        public Task SuspendStateAsync()
        {
            return Task.Run(() =>
            {
                this.StopTimer();
            });
        }
        
        public Task ResumeStateAsync()
        {
            return Task.Run(() =>
            {
                this.StartTimer();
            });
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Saves the budgets asynchronously.
        /// </summary>
        private async Task SaveBudgetsAsync()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("LocalBudgets.json", CreationCollisionOption.ReplaceExisting);

            string json = JsonConvert.SerializeObject(this._budgets);

            using (var stream = await file.OpenStreamForWriteAsync())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
        }
        /// <summary>
        /// Loads the budgets asynchronously.
        /// </summary>
        private async Task LoadBudgetsAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("LocalBudgets.json");

                if (file == null)
                    return;

                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    this._budgets = JsonConvert.DeserializeObject<List<SynchronizedBudget>>(json);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        private void StopTimer()
        {
            this._timer.Change(-1, -1);
        }
        private void StartTimer()
        {
            this._timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
        private async void TimerTick(object state)
        {
            foreach (SynchronizedBudget budget in await this.GetSynchronizedBudgets())
            {
                if (budget.DropboxUserId != this._sessionService.DropboxUserId)
                    continue;

                var client = this.GetClient();

                if (client == null)
                    continue;

                Entries changes = await client.Core.Metadata.DeltaAsync(cursor: budget.DropboxCursor, path_prefix: budget.DropboxPath);
            }   
        }

        private Client GetClient()
        {
            if (string.IsNullOrWhiteSpace(this._sessionService.DropboxAccessToken))
                return null;

            var options = new Options
            {
                ClientSecret = this._settings.DropboxClientSecret,
                ClientId = this._settings.DropboxClientId,
                AccessToken = this._sessionService.DropboxAccessToken
            };

            return new Client(new SavvyHttpClientFactory(), new RequestGenerator(), options);
        }
        #endregion
    }
}