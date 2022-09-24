using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_3.Enums;
using Lab_3.Models;
using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;

namespace Lab_4.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        //private const KeyLength LengthOfKey = KeyLength.Bytes_16;
        private const string Password = "Password";

        private readonly RC5 _rc5;
        private readonly RSACryptoServiceProvider _rsa;

        private const int EncipherBlockSizeRSA = 64;
        private const int DecipherBlockSizeRSA = 128;

        private string _filepathToBeEnciphered;

        #endregion fields

        #region properties

        public string InputFilePath { get; private set; }
        public bool OperationActive { get; private set; }

        #endregion properties

        #region constructors

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            _rc5 = new RC5(WordType.Word_32);
            _rsa = new RSACryptoServiceProvider();

            EncryptFileCommand = new RelayCommand(EncryptFile);
            DecryptFileCommand = new RelayCommand(DecryptFile);
        }

        #endregion constructors

        #region commands

        public ICommand EncryptFileCommand { get; set; }
        private void EncryptFile()
        {
            if (ChooseFile())
            {
                try
                {
                    OperationActive = true;

                    //var hashedKey = Encoding.UTF8.GetBytes(Password).GetMD5HashedKeyForRC5(PasswordLength);
                    //var decodedFileContent = await Task.Run(() => rc5.Decrypt(InputFilePath, (int)NumOfRounds, hashedKey));

                    MessageBox.Show("Encrypted", "RC5", MessageBoxButton.OK);
                    OperationActive = false;
                }
                catch (Exception ex)
                {
                    OperationActive = false;
                    MessageBox.Show(ex.Message, "Pizda Encryptiony");
                }
            }
        }

        public ICommand DecryptFileCommand { get; set; }
        private void DecryptFile()
        {
            if (ChooseFile())
            {
                try
                {
                    OperationActive = true;

                    //var hashedKey = Encoding.UTF8.GetBytes(Password).GetMD5HashedKeyForRC5(PasswordLength);
                    //var decodedFileContent = await Task.Run(() => rc5.Decrypt(InputFilePath, (int)NumOfRounds, hashedKey));

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
