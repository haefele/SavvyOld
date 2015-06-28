using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Anotar.Custom;
using Caliburn.Micro;
using Savvy.Services;
using Savvy.Services.Exceptions;
using Savvy.Services.Ynab;
using Savvy.ViewModels;
using Savvy.Views;

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
            this.ConfigureContainer();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            this._container.RegisterNavigationService(rootFrame);
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
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            DisplayRootView<ShellView>();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                var dropboxLoginContinuation = this._container.GetInstance<IContinueYnabLogin>();
                await dropboxLoginContinuation.ContinueAsync((WebAuthenticationBrokerContinuationEventArgs)args);
            }
        }

        #region Private Methods
        private void ConfigureContainer()
        {
            this._container = new WinRTContainer();

            //Services
            this._container.RegisterInstance(typeof(IExceptionHandler), null, new ExceptionHandler());
            this._container.RegisterInstance(typeof(IEventAggregator), null, new EventAggregator());

            var dropboxService = new YnabService("ef2iibv3k4anhvk", "5igqchrpl66ijoh", this._container.GetInstance<IExceptionHandler>(), this._container.GetInstance<IEventAggregator>());
            this._container.RegisterInstance(typeof(IYnabService), null, dropboxService);
            this._container.RegisterInstance(typeof(IContinueYnabLogin), null, dropboxService);

            //ViewModels
            this._container
                .PerRequest<ShellViewModel>()
                .PerRequest<BudgetsViewModel>();
            
        }
        #endregion
    }
}