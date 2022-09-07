using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_2.Business;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace Lab_2.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private MD5 _md5;
        private string _stringToHash;
        private string _hashedString;
        private bool _addOutputTextDecorations;
        private bool _makeOutputUppercase;
        private bool _displayFileInput;
        private string testFileMessage;
        private string testFileMessageHex;
        private string testFileHex;
        private string testFilesCompareResult;

        #endregion fields

        #region properties

        public string StringToHash
        {
            get => _stringToHash;
            set
            {
                _stringToHash = value;
                NotifyPropertyChanged(nameof(StringToHash));
                string hashedString = _md5.HashString(_stringToHash);
                HashedString = hashedString;
            }
        }

        public string HashedString
        {
            get => _hashedString;
            private set
            {
                _hashedString = MakeOutputUppercase ? value.ToUpper() : value;
                NotifyPropertyChanged(nameof(HashedString));
            }
        }

        public bool AddOutputTextDecorations
        {
            get => _addOutputTextDecorations;
            set
            {
                _addOutputTextDecorations = value;
                NotifyPropertyChanged(nameof(AddOutputTextDecorations));
            }
        }

        public bool MakeOutputUppercase
        {
            get => _makeOutputUppercase;
            set
            {
                _makeOutputUppercase = value;
                NotifyPropertyChanged(nameof(MakeOutputUppercase));
            }
        }

        public bool DisplayFileInput
        {
            get => _displayFileInput;
            set
            {
                _displayFileInput = value;
                NotifyPropertyChanged(nameof(DisplayFileInput));
            }
        }

        public string TestFileMessage
        {
            get => testFileMessage;
            set
            {
                testFileMessage = value;
                NotifyPropertyChanged(nameof(TestFileMessage));
            }
        }

        public string TestFileMessageHex
        {
            get => testFileMessageHex;
            set
            {
                testFileMessageHex = value;
                CompareHexes();
                NotifyPropertyChanged(nameof(TestFileMessageHex));
            }
        }

        public string TestFileHex
        {
            get => testFileHex;
            set
            {
                testFileHex = value;
                CompareHexes();
                NotifyPropertyChanged(nameof(TestFileHex));
            }
        }

        public string TestFilesCompareResult 
        {
            get => testFilesCompareResult;
            set 
            { 
                testFilesCompareResult = value;
                NotifyPropertyChanged(nameof(TestFilesCompareResult));
            }
        } 

        #endregion properties

        #region constructors

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            _md5 = new MD5();

            MakeOutputUppercase = false;
            AddOutputTextDecorations = false;

            SaveToFileCommand = new RelayCommand(SaveToFile);
            OpenFileExplorerCommand = new RelayCommand(OpenFileExplorer);
            OpenFileCommand = new RelayCommand(OpenFile);
            OpenFileToCompareCommand = new RelayCommand(OpenFileToCompare);
            OpenFileWithHexToCompareCommand = new RelayCommand(OpenFileWithHexToCompare);
        }

        #endregion constructors

        #region commands

        public ICommand SaveToFileCommand { get; set; }
        private async void SaveToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt",
                Title = "Save hashed string",
                AddExtension = true,
                InitialDirectory = @"D:\University\Term 7\Security\Labs\TestFiles\Lab_2\OutputFiles",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var result = AddOutputTextDecorations ?
                    string.Concat("Input - ", StringToHash, "\n", "Hashed - ", HashedString) :
                    HashedString;
                File.WriteAllText(saveFileDialog.FileName, result);
            }
        }

        public ICommand OpenFileExplorerCommand { get; set; }

        private async void OpenFileExplorer()
        {
            System.Diagnostics.Process.Start("explorer.exe", @"D:\University\Term 7\Security\Labs\TestFiles\Lab_2");
        }


        public ICommand OpenFileCommand { get; set; }
        private async void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose file with string to hash",
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = @"D:\University\Term 7\Security\Labs\TestFiles\Lab_2\InputFiles",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var result = File.ReadAllText(openFileDialog.FileName);

                if (DisplayFileInput)
                {
                    StringToHash = result;
                }
                else
                {
                    HashedString = _md5.HashString(result);
                }
            }
        }

        public ICommand OpenFileToCompareCommand { get; set; }
        private async void OpenFileToCompare()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose file to check it's integrity",
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = @"D:\University\Term 7\Security\Labs\TestFiles\Lab_2\ToCheck",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var result = File.ReadAllText(openFileDialog.FileName);
                TestFileMessage = result;
                TestFileMessageHex = _md5.HashString(result);
            }
        }

        public ICommand OpenFileWithHexToCompareCommand { get; set; }
        private async void OpenFileWithHexToCompare()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Choose file with hex",
                Filter = "Text file (*.txt)|*.txt",
                InitialDirectory = @"D:\University\Term 7\Security\Labs\TestFiles\Lab_2\ToCheck",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var result = File.ReadAllText(openFileDialog.FileName);
                TestFileHex = result;
            }
        }

        #endregion commands

        #region methods

        private void CompareHexes()
        {
            if (TestFileMessageHex != null && TestFileHex != null)
            {
                TestFilesCompareResult = TestFileMessageHex.ToUpper() == TestFileHex.ToUpper() ? "File is not corrupted" : "File is corrupted"; ;
            }
        }

        #endregion methods
    }
}
