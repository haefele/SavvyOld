using System;
using System.Threading.Tasks;
using Windows.Storage;
using LiteGuard;
using DropboxRestAPI;
using DropboxRestAPI.RequestsGenerators;
using Savvy.Services.Dropbox;
using Savvy.Services.Settings;

namespace Savvy.Services.Session
{
    public class SessionService : ISessionService
    {
        #region Constants
        private const string DropboxUserIdKey = "DropboxUserId";
        private const string DropboxAccessTokenKey = "DropboxAccessToken";
        #endregion

        #region Fields
        private readonly ISettings _settings;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the dropbox user identifier.
        /// </summary>
        public long DropboxUserId { get; private set; }
        /// <summary>
        /// Gets the dropbox access token.
        /// </summary>
        public string DropboxAccessToken { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SessionService(ISettings settings)
        {
            Guard.AgainstNullArgument("settings", settings);

            this._settings = settings;
        }
        #endregion

        public void UserLoggedIn(string accessToken, long dropboxUserId)
        {
            Guard.AgainstNullArgument("accessToken", accessToken);

            this.DropboxAccessToken = accessToken;
            this.DropboxUserId = dropboxUserId;
        }

        public void ClearState()
        {
            this.DropboxAccessToken = null;
            this.DropboxUserId = 0;
        }
        
        public Task SuspendStateAsync()
        {
            return Task.Run(() =>
            {
                var settingsContainer = this.GetSettingsContainer();

                settingsContainer.Values[DropboxAccessTokenKey] = this.DropboxAccessToken;
                settingsContainer.Values[DropboxUserIdKey] = this.DropboxUserId;
            });
        }

        public async Task<bool> ResumeStateAsync()
        {
            var settingsContainer = this.GetSettingsContainer();

            if (settingsContainer.Values.ContainsKey(DropboxAccessTokenKey) == false ||
                settingsContainer.Values.ContainsKey(DropboxUserIdKey) == false)
                return false;

            string accessToken = (string)settingsContainer.Values[DropboxAccessTokenKey];

            if (await this.TestAccessToken(accessToken) == false)
                return false;
            
            this.DropboxAccessToken = accessToken;
            this.DropboxUserId = (long)settingsContainer.Values[DropboxUserIdKey];

            return true;
        }  
        
        private async Task<bool> TestAccessToken(string accessToken)
        {
            var options = new Options
            {
                AccessToken = accessToken,
                ClientId = this._settings.DropboxClientId,
                ClientSecret = this._settings.DropboxClientSecret,
            };

            var client = new Client(new SavvyHttpClientFactory(), new RequestGenerator(),  options);

            try
            {
                await client.Core.Accounts.AccountInfoAsync();
                return true;
            }
            // ReSharper disable once CatchAllClause
            catch
            {
                return false;
            }
        }

        private ApplicationDataContainer GetSettingsContainer()
        {
            return ApplicationData.Current.LocalSettings.CreateContainer("Savvy.SessionService",
                ApplicationDataCreateDisposition.Always);
        }
    }
}
