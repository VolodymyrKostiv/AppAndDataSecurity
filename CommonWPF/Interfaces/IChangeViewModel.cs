using CommonWPF.ViewModels;

namespace CommonWPF.Interfaces
{
    public interface IChangeViewModel
    {
        void PushViewModel(BaseViewModel model);
        void PopViewModel();
    }
}
