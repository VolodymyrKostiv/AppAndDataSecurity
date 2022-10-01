using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_5.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace Lab_5.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private DSA _dsa;

        #endregion fields

        #region properties

        public bool OperationActive { get; private set; }
        public string Signature { get; private set; }

        #endregion properties

        #region constructors

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
        }

        #endregion constructors

        #region commands

        public ICommand CreateSignatureFromFileCommand { get; set; }
        private async void CreateSignatureFromFile()
        {
            var filePath = ChooseFile();
            if (filePath != string.Empty)
            {
                var text = File.ReadAllBytes(filePath);
                var result = _dsa.CreateSignature(text);
                Signature = result; 
            }
        }

        public ICommand SaveSignatureToFileCommand { get; set; }
        private async void SaveSignatureToFile()
        {
            var filePath = SaveToFile();
            if (filePath != string.Empty)
            {
                File.WriteAllText(filePath, Signature);
            }
        }

        #endregion commands

        #region methods

        private string ChooseFile()
        {
            OperationActive = true;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose input file",
                InitialDirectory = @"D:\",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            OperationActive = false;

            return string.Empty;
        }

        private string SaveToFile()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                RestoreDirectory = true,
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            else
            {
                return string.Empty;    
            }
        }

        #endregion methods
    }
}
