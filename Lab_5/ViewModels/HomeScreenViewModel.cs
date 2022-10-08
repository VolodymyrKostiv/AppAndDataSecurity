using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_5.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Lab_5.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private DSA _dsa;
        private bool _operationActive;
        private string _inputText;
        private string _signature;
        private string _textToCheckSignatureFilePath;
        private string _signatureFromFileFilePath;
        private string _verificationResult;

        #endregion fields

        #region properties

        public bool OperationActive
        {
            get => _operationActive;
            set
            {
                _operationActive = value;
                NotifyPropertyChanged(nameof(OperationActive));
            }
        }

        public string Signature
        {
            get => _signature;
            set
            {
                _signature = value;
                NotifyPropertyChanged(nameof(Signature));
            }
        }

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                NotifyPropertyChanged(nameof(InputText));
            }
        }

        public string TextToCheckSignatureFilePath
        {
            get => _textToCheckSignatureFilePath;
            set
            {
                _textToCheckSignatureFilePath = value;
                NotifyPropertyChanged(nameof(TextToCheckSignatureFilePath));
            }
        }

        public string SignatureFromFileFilePath
        {
            get => _signatureFromFileFilePath;
            set
            {
                _signatureFromFileFilePath = value;
                NotifyPropertyChanged(nameof(SignatureFromFileFilePath));
            }
        }

        public string VerificationResult
        {
            get => _verificationResult;
            set
            {
                _verificationResult = value;
                NotifyPropertyChanged(nameof(VerificationResult));
            }
        }

        #endregion properties

        #region constructors

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            _dsa = new DSA();

            Signature = InputText = TextToCheckSignatureFilePath = SignatureFromFileFilePath = VerificationResult = string.Empty;

            ReadSignatureFromFileToVerifyCommand = new RelayCommand(ReadSignatureFromFileToVerify);
            ReadFileToVerifyCommand = new RelayCommand(ReadFileToVerify);
            CreateSignatureFromFileCommand = new RelayCommand(CreateSignatureFromFile);
            SaveSignatureToFileCommand = new RelayCommand(SaveSignatureToFile);
            CreateSignatureFromInputCommand = new RelayCommand(CreateSignatureFromInput);
            VerifyFileSignatureCommand = new RelayCommand(VerifyFileSignature);
        }

        #endregion constructors

        #region commands

        public ICommand ReadFileToVerifyCommand { get; set; }
        private async void ReadFileToVerify()
        {
            StartOperation();

            try
            {
                var filePath = ChooseFile();
                if (!string.IsNullOrEmpty(filePath))
                {
                    TextToCheckSignatureFilePath = filePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EndOperation();
        }

        public ICommand ReadSignatureFromFileToVerifyCommand { get; set; }
        private async void ReadSignatureFromFileToVerify()
        {
            StartOperation();

            try
            {
                var filePath = ChooseFile();
                if (!string.IsNullOrEmpty(filePath))
                {
                    SignatureFromFileFilePath = filePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EndOperation();
        }

        public ICommand CreateSignatureFromFileCommand { get; set; }
        private async void CreateSignatureFromFile()
        {
            StartOperation();

            try
            {
                var filePath = ChooseFile();
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    var text = File.ReadAllBytes(filePath);
                    var result = _dsa.CreateSignature(text);

                    InputText = null;
                    Signature = result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EndOperation();
        }

        public ICommand SaveSignatureToFileCommand { get; set; }
        private async void SaveSignatureToFile()
        {
            StartOperation();

            try
            {
                var filePath = SaveToFile();
                if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(Signature))
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    File.WriteAllText(filePath, Signature);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EndOperation();
        }

        public ICommand CreateSignatureFromInputCommand { get; set; }
        private async void CreateSignatureFromInput()
        {
            StartOperation();

            try
            {
                if (InputText != null)
                {
                    var result = _dsa.CreateSignature(InputText);
                    Signature = result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            EndOperation();
        }

        public ICommand VerifyFileSignatureCommand { get; set; }
        private async void VerifyFileSignature()
        {
            if (!string.IsNullOrEmpty(TextToCheckSignatureFilePath) && !string.IsNullOrEmpty(SignatureFromFileFilePath))
            {
                if (File.Exists(TextToCheckSignatureFilePath) && File.Exists(SignatureFromFileFilePath))
                {
                    byte[] message = File.ReadAllBytes(TextToCheckSignatureFilePath);
                    string sign = File.ReadAllText(SignatureFromFileFilePath);

                    bool result = _dsa.VerifySignature(message, Convert.FromBase64String(sign));
                    if (result)
                    {
                        VerificationResult = "Verified";
                    }
                    else
                    {
                        VerificationResult = "Not verified";
                    }
                }
            }
        }

        #endregion commands

        #region methods

        private void StartOperation()
        {
            OperationActive = true;
        }

        private void EndOperation()
        {
            OperationActive = false;
        }

        private string ChooseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose input file",
                InitialDirectory = @"D:\",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

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
