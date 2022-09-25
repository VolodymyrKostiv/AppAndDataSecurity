using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_3.Enums;
using Lab_3.Helpers;
using Lab_3.Models;
using Microsoft.Win32;
using System;
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
        private WordType _wordLength;
        private KeyLength _passwordLength;
        private RoundCount _numOfRounds;

        #endregion fields

        #region properties

        public string InputFilePath { get; set; }

        public string Password { get; set; } = "";

        public KeyLength PasswordLength
        {
            get => _passwordLength;
            set
            {
                _passwordLength = value;
                NotifyPropertyChanged(nameof(PasswordLength));
            }
        }


        public WordType WordLength
        {
            get => _wordLength;
            set
            { 
                _wordLength = value;
                NotifyPropertyChanged(nameof(WordLength));
            }
        }

        public RoundCount NumOfRounds 
        { 
            get => _numOfRounds;
            set
            { 
                _numOfRounds = value; 
                NotifyPropertyChanged(nameof(NumOfRounds));
            }
        } 

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
            PasswordLength = KeyLength.Bytes_16;
            WordLength = WordType.Word_32;
            NumOfRounds = RoundCount.Rounds_12;

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

                    var hashedKey = MD5Helper.GetMD5HashedKeyForRC5(Encoding.UTF8.GetBytes(Password), PasswordLength);
                    var encodedFileContent = await Task.Run(() => rc5.EncryptAsync(InputFilePath, (int)NumOfRounds, hashedKey));

                    MessageBox.Show("Encrypted", "RC5", MessageBoxButton.OK);
                    OperationActive = false;
                }
                catch (Exception ex)
                {
                    OperationActive = false;
                    MessageBox.Show(ex.Message, "Encryption died :(");
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

                    var hashedKey = MD5Helper.GetMD5HashedKeyForRC5(Encoding.UTF8.GetBytes(Password), PasswordLength);
                    var decodedFileContent = await Task.Run(() => rc5.DecryptAsync(InputFilePath, (int)NumOfRounds, hashedKey));

                    MessageBox.Show("Decrypted", "RC5", MessageBoxButton.OK);
                    OperationActive = false;
                }
                catch (Exception ex)
                {
                    OperationActive = false;
                    MessageBox.Show(ex.Message, "Decryption died :(");
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
                InitialDirectory = @"D:\Recordings",
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
