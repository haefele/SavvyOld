using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Anotar.Custom;
using Caliburn.Micro;
using DropboxRestAPI;
using DropboxRestAPI.Models.Core;
using DropboxRestAPI.RequestsGenerators;
using LiteGuard;
using Savvy.Services.Dropbox;
using Savvy.Services.Exceptions;
using Savvy.Services.Session;
using Savvy.Services.Settings;
#if WINDOWS_PHONE_APP
using Windows.ApplicationModel.Activation;
#endif

namespace Savvy.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields
        private readonly ISettings _settings;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly ISessionService _sessionService;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="sessionService">The session service.</param>
        public AuthenticationService(ISettings settings, IEventAggregator eventAggregator, IExceptionHandler exceptionHandler, ISessionService sessionService)
        {
            Guard.AgainstNullArgument("settings", settings);
            Guard.AgainstNullArgument("eventAggregator", eventAggregator);
            Guard.AgainstNullArgument("exceptionHandler", exceptionHandler);
            Guard.AgainstNullArgument("sessionService", sessionService);

            this._settings = settings;
            this._eventAggregator = eventAggregator;
            this._exceptionHandler = exceptionHandler;
            this._sessionService = sessionService;
        }
        #endregion

        #region Implementation of IAuthenticationService
        /// <summary>
        /// Starts the login process.
        /// </summary>
        public async Task LoginAsync()
        {
            Uri uri = await this.GetClient().Core.OAuth2.AuthorizeAsync("code");
#if WINDOWS_PHONE_APP
            WebAuthenticationBroker.AuthenticateAndContinue(uri, this._settings.RedirectUrl);
#endif
#if WINDOWS_APP
            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, uri, this._settings.RedirectUrl);
            await this.FinishLoginAsync(result);
#endif
        }
        /// <summary>
        /// Starts the logout process.
        /// </summary>
        public async Task LogoutAsync()
        {
            this._sessionService.ClearState();
            await this._eventAggregator.PublishOnUIThreadAsync(new UserLoggedOutEvent());
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Continues the login process.
        /// </summary>
        /// <param name="args">The <see cref="WebAuthenticationBrokerContinuationEventArgs"/> instance containing the event data.</param>
        public Task ContinueLoginAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            return this.FinishLoginAsync(args.WebAuthenticationResult);
        }
#endif
        #endregion

        #region Private Methods
        /// <summary>
        /// Finishes the login process asynchronously.
        /// </summary>
        /// <param name="webAuthenticationResult">The web authentication result.</param>
        private async Task FinishLoginAsync(WebAuthenticationResult webAuthenticationResult)
        {
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                this._exceptionHandler.Handle(new Exception(webAuthenticationResult.ResponseData));
            }
            else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                LogTo.Debug("User cancelled the login.");
            }
            else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var responseUri = new Uri(webAuthenticationResult.ResponseData);
                var decoder = new WwwFormUrlDecoder(responseUri.Query);

                string accessToken = decoder.GetFirstValueByName("code");

                var client = this.GetClient();

                await client.Core.OAuth2.TokenAsync(accessToken);

                AccountInfo dropboxUserInfo = await client.Core.Accounts.AccountInfoAsync();
                this._sessionService.UserLoggedIn(client.UserAccessToken, dropboxUserInfo.uid);

                await this._eventAggregator.PublishOnUIThreadAsync(new UserLoggedInEvent());
            }
        }
        /// <summary>
        /// Creates a new dropbox client.
        /// </summary>
        private Client GetClient()
        {
            var options = new Options
            {
                ClientId = this._settings.DropboxClientId,
                ClientSecret = this._settings.DropboxClientSecret,
                RedirectUri = this._settings.RedirectUrl.ToString(),

            };

            return new Client(new SavvyHttpClientFactory(), new RequestGenerator(), options);
        }
        #endregion
    }
}