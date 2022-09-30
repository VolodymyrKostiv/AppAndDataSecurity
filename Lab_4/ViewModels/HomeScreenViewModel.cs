using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_3.Enums;
using Lab_3.Helpers;
using Lab_3.Models;
using Lab_4.Helper;
using Lab_4.Helpers.FileHelpers;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lab_4.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private const int EncipherBlockSizeRSA = 64;
        private const int DecipherBlockSizeRSA = 128;

        private bool _operationActive;
        private WordType _wordLength;
        private KeyLength _passwordLength;
        private RoundCount _numOfRounds;
        private int _bytesPerBlock = EncipherBlockSizeRSA;
        private RSACryptoServiceProvider _rsa;
        private string _timeLogs = "";

        #endregion fields

        #region properties

        public string InputFilePath { get; private set; }
        public string Password { get; set; } = "";
        public string TimeLogs
        {
            get => _timeLogs;
            set
            {
                _timeLogs = value;
                NotifyPropertyChanged(nameof(TimeLogs));
            }
        }

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

            _rsa = new RSACryptoServiceProvider();
            EncryptFileCommand = new RelayCommand(EncryptFile);
            DecryptRC5Command = new RelayCommand(DecryptRC5File);
            DecryptRSACommand = new RelayCommand(DecryptRSAFile);
        }

        #endregion constructors

        #region commands

        public ICommand EncryptFileCommand { get; set; }
        private async void EncryptFile()
        {
            if (ChooseFile())
            {
                OperationActive = true;
                try
                {
                    //----------------------------------------------------------------------------------------------------------------------------

                    RC5 rc5 = new RC5(WordLength);
                    var hashedKey = MD5Helper.GetMD5HashedKeyForRC5(Encoding.UTF8.GetBytes(Password), PasswordLength);
                    var decodedFileContent = await Task.Run(() => rc5.EncryptAsync(InputFilePath, (int)NumOfRounds, hashedKey));
                    LogTime("Encrypt RC5", rc5.GetTime());
                    //----------------------------------------------------------------------------------------------------------------------------

                    InputFileHelper inputFileHelper = new InputFileHelper(InputFilePath);
                    OutputFileHelper outputFileHelper = new OutputFileHelper(InputFilePath + "_rsa-enc");

                    byte[] bytesToEncode = new byte[_bytesPerBlock];
                    bool endOfFile = false;

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await Task.Run(() =>
                    {
                        try
                        {
                            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                            {
                                do
                                {
                                    bytesToEncode = inputFileHelper.ReadBlock(EncipherBlockSizeRSA);

                                    if (bytesToEncode.Length <= 0)
                                    {
                                        endOfFile = true;
                                        break;
                                    }

                                    var encodedBlock = _rsa.Encrypt(bytesToEncode, false);
                                    outputFileHelper.Write(encodedBlock);

                                } while (!endOfFile);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {
                            inputFileHelper.CloseFile();
                            outputFileHelper.CloseFile();
                        }
                    });

                    stopwatch.Stop();

                    var rsaTimespan = stopwatch.Elapsed - (inputFileHelper.Watch.Elapsed + outputFileHelper.Watch.Elapsed);
                    LogTime("Encrypt RSA", rsaTimespan);
                    //----------------------------------------------------------------------------------------------------------------------------
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Encryption died :(");
                }
                finally
                {
                    OperationActive = false;
                }
            }
        }

        public ICommand DecryptRC5Command { get; set; }
        private async void DecryptRC5File()
        {
            if (ChooseFile())
            {
                OperationActive = true;
                try
                {
                    RC5 rc5 = new RC5(WordLength);
                    var hashedKey = MD5Helper.GetMD5HashedKeyForRC5(Encoding.UTF8.GetBytes(Password), PasswordLength);
                    var decodedFileContent = await Task.Run(() => rc5.DecryptAsync(InputFilePath, (int)NumOfRounds, hashedKey));
                    LogTime("Decrypt RC5", rc5.GetTime());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Decryption died :(");
                }
                finally
                {
                    OperationActive = false;
                }
            }
        }

        public ICommand DecryptRSACommand { get; set; }
        private async void DecryptRSAFile()
        {
            if (ChooseFile())
            {
                OperationActive = true;
                try
                {
                    InputFileHelper inputFileHelper = new InputFileHelper(InputFilePath);
                    OutputFileHelper outputFileHelper = new OutputFileHelper(InputFilePath + "_rsa-dec");

                    byte[] bytesToDecode = new byte[_bytesPerBlock];
                    bool endOfFile = false;

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    await Task.Run(() =>
                    {
                        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                        {
                            do
                            {
                                bytesToDecode = inputFileHelper.ReadBlock(DecipherBlockSizeRSA);

                                if (bytesToDecode.Length <= 0)
                                {
                                    endOfFile = true;
                                    break;
                                }

                                var decodedBlock = _rsa.Decrypt(bytesToDecode, false);
                                outputFileHelper.Write(decodedBlock);

                            } while (!endOfFile);
                        }
                    });

                    outputFileHelper.CloseFile();
                    stopwatch.Stop();

                    var rsaTimespan = stopwatch.Elapsed - (inputFileHelper.Watch.Elapsed + outputFileHelper.Watch.Elapsed);
                    LogTime("Decrypt RSA", rsaTimespan);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Decryption died :(");
                }
                finally
                {
                    OperationActive = false;
                }
            }
        }

        #endregion commands

        #region methods

        private void LogTime(string target, TimeSpan span)
        {
            TimeLogs += string.Concat(target, ": ",span.ToString(), "\n"); 
        }

        private bool ChooseFile()
        {
            OperationActive = true;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose input file",
                InitialDirectory = @"D:\",
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
