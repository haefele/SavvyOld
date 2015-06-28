using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Savvy.Services.Ynab;

namespace Savvy.ViewModels
{
    public class BudgetsViewModel : ReactiveScreen
    {
        private readonly IYnabService _ynabService;

        public BudgetsViewModel(IYnabService ynabService)
        {
            this._ynabService = ynabService;
        }

        protected override void OnDeactivate(bool close)
        {
            this._ynabService.Logout();
        }
    }
}
