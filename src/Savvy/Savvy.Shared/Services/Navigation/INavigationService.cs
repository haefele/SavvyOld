using CaliburnNavigationService = Caliburn.Micro.INavigationService;

namespace Savvy.Services.Navigation
{
    public interface INavigationService : CaliburnNavigationService
    {
        void ClearHistory();
    }
}
