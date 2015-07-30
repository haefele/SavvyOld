using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savvy.Services.Ynab
{
    public interface IBudgetService
    {
        Task<IReadOnlyCollection<Budget>> GetBudgetsAsync();
    }
}