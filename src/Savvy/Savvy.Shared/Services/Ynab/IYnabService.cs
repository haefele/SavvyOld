using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savvy.Services.Ynab
{
    public interface IYnabService
    {
        bool LoggedIn { get; }
        Task StartLoginAsync();
        Task Logout();

        Task<IList<Budget>> GetBudgetsAsync();
    }

    public class Budget
    {
        public string Name { get; set; }
    }
}