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
using ReactiveUI;
using Savvy.Events;
using Savvy.Services;
using Savvy.Services.Exceptions;
using Savvy.Services.Ynab;

namespace Savvy.ViewModels
{
    public class ShellViewModel : ReactiveScreen, IHandle<UserLoggedInEvent>
    {
        #region Fields
        private readonly IYnabService _ynabService;
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IExceptionHandler _exceptionHandler;
        #endregion

        #region Properties
        public ReactiveCommand<Unit> Login { get; set; }
        #endregion

        #region Constructors
        public ShellViewModel(IYnabService ynabService, INavigationService navigationService, IEventAggregator eventAggregator, IExceptionHandler exceptionHandler)
        {
            this._ynabService = ynabService;
            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;
            this._exceptionHandler = exceptionHandler;

            this.CreateCommands();
        }
        #endregion

        #region Private Methods
        private void CreateCommands()
        {
            this.Login = ReactiveCommand.CreateAsyncTask(_ => this._ynabService.StartLoginAsync());
            this.Login.ThrownExceptions.Subscribe(this._exceptionHandler.Handle);
        }
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
            this._navigationService.NavigateToViewModel<BudgetsViewModel>();
        }
        #endregion
    }
}
