using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_2.Business;

namespace Lab_2.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private MD5 _md5;
        private string _stringToHash;
        private string _hashedString;

        #endregion fields

        #region properties

        public string BlaBla => "Tdshhjsdfhjfdks";

        public string StringToHash
        {
            get => _stringToHash;
            set
            {
                _stringToHash = value;
                string hashedString = _md5.HashString(_stringToHash);
                HashedString = hashedString;
            }
        }

        public string HashedString
        {
            get => _hashedString;
            private set
            {
                _hashedString = value;
                NotifyPropertyChanged(nameof(HashedString));
            }
        }

        #endregion properties

        #region constructors

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            _md5 = new MD5();
        }

        #endregion constructors

        #region methods

        #endregion methods
    }
}
