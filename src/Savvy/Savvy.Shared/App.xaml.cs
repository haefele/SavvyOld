using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Anotar.Custom;
using Caliburn.Micro;
using Savvy.Logging;
using Savvy.Services;
using Savvy.Services.Authentication;
using Savvy.Services.Exceptions;
using Savvy.Services.Navigation;
using Savvy.Services.Session;
using Savvy.Services.Settings;
using Savvy.Services.Ynab;
using Savvy.ViewModels;
using Savvy.Views;

using INavigationService = Savvy.Services.Navigation.INavigationService;

namespace Savvy
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        private WinRTContainer _container;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void Configure()
        {
            this.ConfigureLogging();
            this.ConfigureContainer();
            this.PrepareViewFirst();
        }
        
        protected override void PrepareViewFirst(Frame rootFrame)
        {
            var navigationService = new NavigationService(rootFrame);
            this._container.Instance((INavigationService)navigationService);
        }

        protected override object GetInstance(Type service, string key)
        {
            return this._container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this._container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
        }
        
        protected override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogTo.Error(e.Exception, "Unhandled exception occured.");
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            this.Initialize();
            
            //We are already running, do nothing
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                return;
            }

            //Resume the budget synchronization
            this._container.GetInstance<IBudgetSynchronizationService>().ResumeStateAsync().Wait();
            
            //Restore state if we were terminated
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                bool resumedNavigation = this._container.GetInstance<INavigationService>().ResumeState();
                bool resumedSession = this._container.GetInstance<ISessionService>().ResumeStateAsync().Result;

                if (resumedNavigation && resumedSession)
                    return;

                this._container.GetInstance<INavigationService>().ClearHistory();
                this._container.GetInstance<ISessionService>().ClearState();
            }

            this.DisplayRootView<ShellView>();
        }

#if WINDOWS_PHONE_APP
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                var authenticationService = this._container.GetInstance<IAuthenticationService>();
                await authenticationService.ContinueLoginAsync((WebAuthenticationBrokerContinuationEventArgs)args);
            }
    }
#endif

        protected override async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            this._container.GetInstance<INavigationService>().SuspendState();
            await this._container.GetInstance<ISessionService>().SuspendStateAsync();
            await this._container.GetInstance<IBudgetSynchronizationService>().SuspendStateAsync();

            deferral.Complete();
        }

        #region Private Methods
        private void ConfigureLogging()
        {
            LogManager.GetLog = f => new CaliburnLogger(LoggerFactory.GetLogger(f).ActualLogger);
        }
        private void ConfigureContainer()
        {
            this._container = new WinRTContainer();

            //Services
            this._container
                .Singleton<IExceptionHandler, ExceptionHandler>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAuthenticationService, AuthenticationService>()
                .Singleton<IBudgetSynchronizationService, BudgetSynchronizationService>()
                .Singleton<IBudgetService, BudgetService>()
                .Singleton<ISessionService, SessionService>()
                .Instance((ISettings)new HardCodedSettings());
                                    
            //ViewModels
            this._container
                .PerRequest<ShellViewModel>()
                .PerRequest<BudgetsViewModel>();
            
        }
        #endregion
    }
}