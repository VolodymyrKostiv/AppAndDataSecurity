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
using System.Windows;
using System.Windows.Input;

namespace Lab_3.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region properties

        public string InputFilePath { get; set; }

        public string Password { get; set; }   

        public KeyLength PasswordLength { get; set; }
        public WordType WordLength { get; set; }
        public RoundCount NumOfRounds { get; set; }

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
                RC5 rc5 = new RC5();

                if (Password.Length != (int)PasswordLength)
                {
                    MessageBox.Show("Bad password length", "Password length must be as selected", MessageBoxButton.OK);
                    return;
                }

                var hashedKey = Encoding.UTF8
                    .GetBytes(Password)
                    .GetMD5HashedKeyForRC5(PasswordLength);

                var encodedFileContent = rc5.Encrypt(Encoding.UTF8
                    .GetBytes("test"),
                    (int)NumOfRounds,
                    hashedKey);

                MessageBox.Show("Encrypted", "RC5", MessageBoxButton.OK);
            }
        }

        public ICommand DecryptCommand { get; set; }
        private async void Decrypt()
        {
            if (ChooseFile())
            {
                try
                {
                    RC5 rc5 = new RC5();
                    var hashedKey = Encoding.UTF8
                        .GetBytes("password")
                        .GetMD5HashedKeyForRC5(Enums.KeyLength.Bytes_8);

                    var encodedFileContent = rc5.Encrypt(Encoding.UTF8
                        .GetBytes("test"),
                        8,
                        hashedKey);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "RC5");
                }

                MessageBox.Show("Decrypted", "RC5", MessageBoxButton.OK);
            }
        }

        #endregion commands

        #region methods

        private bool ChooseFile()
        {
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
            return false;
        }

        #endregion methods
    }
}
