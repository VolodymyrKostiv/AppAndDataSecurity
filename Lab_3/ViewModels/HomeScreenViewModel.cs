using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_3.Enums;
using Lab_3.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lab_3.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private bool _operationActive;

        #endregion fields

        #region properties

        public string InputFilePath { get; set; }

        public string Password { get; set; } = "";

        public KeyLength PasswordLength { get; set; }
        public WordType WordLength { get; set; }
        public RoundCount NumOfRounds { get; set; }

        public bool OperationActive
        {
            get => _operationActive;
            set
            {
                _operationActive = value;
                NotifyPropertyChanged(nameof(OperationActive));
            }
        }

        #endregion properties


        #region constructors

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
        }

        #endregion constructors

        #region commands

        public ICommand EncryptCommand { get; set; }
        private async void Encrypt()
        {
            if (ChooseFile())
            {
                try
                {
                    OperationActive = true;
                    RC5 rc5 = new RC5(WordLength);
                    if (Password.Length != (int)PasswordLength)
                    {
                        MessageBox.Show("Bad password length", "Password length must be as selected", MessageBoxButton.OK);
                        OperationActive = false;
                        return;
                    }

                    var hashedKey = Encoding.UTF8.GetBytes(Password).GetMD5HashedKeyForRC5(PasswordLength);
                    var encodedFileContent = await Task.Run(() => rc5.Encrypt(InputFilePath, (int)NumOfRounds, hashedKey));

                    MessageBox.Show("Encrypted", "RC5", MessageBoxButton.OK);
                    OperationActive = false;
                }
                catch (Exception ex)
                {
                    OperationActive = false;
                    MessageBox.Show(ex.Message, "Pizda encryptiony");
                }
            }
        }

        public ICommand DecryptCommand { get; set; }
        private async void Decrypt()
        {
            if (ChooseFile())
            {
                try
                {
                    OperationActive = true;
                    RC5 rc5 = new RC5(WordLength);

                    if (Password.Length != (int)PasswordLength)
                    {
                        MessageBox.Show("Bad password length", "Password length must be as selected", MessageBoxButton.OK);
                        OperationActive = false;
                        return;
                    }

                    var hashedKey = Encoding.UTF8.GetBytes(Password).GetMD5HashedKeyForRC5(PasswordLength);
                    var decodedFileContent = await Task.Run(() => rc5.Decrypt(InputFilePath, (int)NumOfRounds, hashedKey));

                    MessageBox.Show("Decrypted", "RC5", MessageBoxButton.OK);
                    OperationActive = false;
                }
                catch (Exception ex)
                {
                    OperationActive = false;
                    MessageBox.Show(ex.Message, "Pizda decryptiony");
                }
            }
        }

        #endregion commands

        #region methods

        private bool ChooseFile()
        {
            OperationActive = true;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose input file",
                InitialDirectory = @"D:\Program Files (x86)\Origin Games\Battlefield 1",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                InputFilePath = openFileDialog.FileName;
                return true;
            }
            OperationActive = false;

            return false;
        }

        #endregion methods
    }
}
