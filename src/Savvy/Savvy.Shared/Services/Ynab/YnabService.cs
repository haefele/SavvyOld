using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Security.Authentication.Web;
using Caliburn.Micro;
using DropboxRestAPI;
using Savvy.Events;
using Savvy.Services.Exceptions;

namespace Savvy.Services.Ynab
{
    public class YnabService :  IContinueYnabLogin, IYnabService
    {
        const string RedirectUri = "https://www.dropbox.com/1/oauth2/redirect_receiver";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IEventAggregator _eventAggregator;

        private Client _client;
        
        public bool LoggedIn { get; private set; }

        public YnabService(string clientId, string clientSecret, IExceptionHandler exceptionHandler, IEventAggregator eventAggregator)
        {
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._exceptionHandler = exceptionHandler;
            this._eventAggregator = eventAggregator;

            this.ReCreateClient();
        }

        private void ReCreateClient()
        {
            var options = new Options
            {
                ClientId = this._clientId,
                ClientSecret = this._clientSecret,
                RedirectUri = RedirectUri
            };
            this._client = new Client(options);
        }

        public async Task StartLoginAsync()
        {
            var uri = await this._client.Core.OAuth2.AuthorizeAsync("code");
            WebAuthenticationBroker.AuthenticateAndContinue(uri, new Uri(RedirectUri));
        }

        public Task Logout()
        {
            this.ReCreateClient();
            this.LoggedIn = false;

            return Task.FromResult(new object());
        }

        async Task IContinueYnabLogin.ContinueAsync(WebAuthenticationBrokerContinuationEventArgs args)
        {
            if (args.WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                this._exceptionHandler.Handle(new Exception(args.WebAuthenticationResult.ResponseData));
            }
            else if (args.WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                //Doesn't matter for now.
            }
            else if (args.WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            { 
                Uri responseUri = new Uri(args.WebAuthenticationResult.ResponseData);
            
                var decoder = new WwwFormUrlDecoder(responseUri.Query);

                var accessToken = decoder.GetFirstValueByName("code");
                await this._client.Core.OAuth2.TokenAsync(accessToken);

                this.LoggedIn = true;
                this._eventAggregator.PublishOnUIThread(new UserLoggedInEvent());
            }
        }
    }
}
