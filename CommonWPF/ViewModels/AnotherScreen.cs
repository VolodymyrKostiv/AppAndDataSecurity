using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using System.Windows.Input;

namespace CommonWPF.ViewModels
{
    public class AnotherScreen : BaseViewModel
    {
        public AnotherScreen(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }

        public ICommand GoToMainMenu
        {
            get { return new RelayCommand(PopToMainMenu); }
        }

        private void PopToMainMenu()
        {
            PopViewModel();
        }
    }
}
