using System.Collections.Generic;
using System.Reactive;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using Savvy.Services.Ynab;
using INavigationService = Savvy.Services.Navigation.INavigationService;

namespace Savvy.ViewModels
{
    public class BudgetsViewModel : ReactiveScreen
    {
        #region Fields
        private readonly IBudgetService _budgetService;
        private readonly INavigationService _navigationService;
        private readonly IBudgetSynchronizationService _budgetSynchronizationService;

        private readonly ObservableAsPropertyHelper<ReactiveObservableCollection<Budget>> _budgetsHelper;
        private Budget _selectedBudget;
        #endregion

        #region Properties
        public ReactiveObservableCollection<Budget> Budgets => this._budgetsHelper.Value;

        public Budget SelectedBudget
        {
            get { return this._selectedBudget; }
            set { this.RaiseAndSetIfChanged(ref this._selectedBudget, value); }
        }
        #endregion

        public ReactiveCommand<Unit> OpenBudget { get; }
        public ReactiveCommand<ReactiveObservableCollection<Budget>> ReloadBudgets { get; }

        public BudgetsViewModel(IBudgetService budgetService, INavigationService navigationService, IBudgetSynchronizationService budgetSynchronizationService)
        {
            this._budgetService = budgetService;
            this._navigationService = navigationService;
            this._budgetSynchronizationService = budgetSynchronizationService;

            var canOpenBudget = this.WhenAny(f => f.SelectedBudget, budget => budget.Value != null);
            this.OpenBudget = ReactiveCommand.CreateAsyncTask(canOpenBudget, async _ =>
            {
                await this._budgetSynchronizationService.SynchronizeBudgetInBackground(this.SelectedBudget);

                this._navigationService.NavigateToViewModel<BudgetsViewModel>();
            });

            this.ReloadBudgets = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                IReadOnlyCollection<Budget> budgets = await this._budgetService.GetBudgetsAsync();

                var result = new ReactiveObservableCollection<Budget>();
                result.AddRange(budgets);

                return result;
            });
            this.ReloadBudgets.ToProperty(this, f => f.Budgets, out this._budgetsHelper);
        }

        protected override async void OnActivate()
        {
            await this.ReloadBudgets.ExecuteAsyncTask();
        }
    }
}
