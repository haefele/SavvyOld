using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using Windows.Security.Authentication.Web;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Anotar.Custom;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using DropboxRestAPI;
using LiteGuard;
using ReactiveUI;
using Savvy.Services;
using Savvy.Services.Authentication;
using Savvy.Services.Exceptions;
using Savvy.Services.Ynab;

using INavigationService = Savvy.Services.Navigation.INavigationService;

namespace Savvy.ViewModels
{
    public class ShellViewModel : ReactiveScreen, IHandle<UserLoggedInEvent>
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExceptionHandler _exceptionHandler;
        #endregion

        #region Properties
        public ReactiveCommand<Unit> Login { get; }
        #endregion

        #region Constructors
        public ShellViewModel(IAuthenticationService authenticationService, INavigationService navigationService, IEventAggregator eventAggregator, IExceptionHandler exceptionHandler)
        {
            Guard.AgainstNullArgument("authenticationService", authenticationService);
            Guard.AgainstNullArgument("navigationService", navigationService);
            Guard.AgainstNullArgument("eventAggregator", eventAggregator);
            Guard.AgainstNullArgument("exceptionHandler", exceptionHandler);

            this._authenticationService = authenticationService;
            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;
            this._exceptionHandler = exceptionHandler;

            this.Login = ReactiveCommand.CreateAsyncTask(_ => this._authenticationService.LoginAsync());
            this.Login.ThrownExceptions.Subscribe(this._exceptionHandler.Handle);
        }
        #endregion

        #region Private Methods
        protected override void OnActivate()
        {
            this._eventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            this._eventAggregator.Unsubscribe(this);
        }
        #endregion

        #region Event Handlers
        void IHandle<UserLoggedInEvent>.Handle(UserLoggedInEvent message)
        {
            this._navigationService.ClearHistory();
            this._navigationService.NavigateToViewModel<BudgetsViewModel>();
        }
        #endregion
    }
}
