using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

namespace Savvy.Services.Navigation
{
    public class NavigationService : FrameAdapter, INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
            this._frame = frame;
        }

        public void ClearHistory()
        {
            this._frame.SetNavigationState("1,0");
        }
    }
}