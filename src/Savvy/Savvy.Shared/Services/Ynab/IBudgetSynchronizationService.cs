using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Savvy.Services.Ynab
{
    public interface IBudgetSynchronizationService
    {
        Task SynchronizeBudgetInBackground(Budget budget);
        Task<IReadOnlyCollection<SynchronizedBudget>> GetSynchronizedBudgets();

        Task SuspendStateAsync();
        Task ResumeStateAsync();
    }
}
