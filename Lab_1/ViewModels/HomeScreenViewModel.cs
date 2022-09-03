using CommonWPF.Helpers;
using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lab_1.ViewModels
{
    public class HomeScreenViewModel : BaseViewModel
    {
        #region fields

        private FileWriter _fileWriter;
        private LinearCongruentialGenerator _generator;
        private bool _isPeriodFound;
        private ulong _numbersInPeriod;
        private int _uiNumbersCounter;
        private int _fileNumbersCounter;

        #region file data

        private int _fileCounter = 0;
        private const string _filePath = @"D:\University\Term 7\Security\Labs\TestFiles\Lab_1\";
        private const string _fileName = @"sequence_";
        private const string _fileExtension = @".txt";

        #endregion file data


        #endregion fields

        #region properties

        #region generator input data

        public ulong ComparisonModuleBase { get; set; }
        public ulong ComparisonModulePower { get; set; }
        public ulong ComparisonModuleMinus { get; set; }

        public ulong MultiplierBase { get; set; }
        public ulong MultiplierPower { get; set; }

        public ulong Increase { get; set; }

        public ulong InitialValue { get; set; }

        public ulong Count { get; set; }

        #endregion generator input data

        #region output data

        public string PeriodResult => IsPeriodFound ? 
           $"Periound was found. {_numbersInPeriod} unique values" : "Period not found";

        public bool IsPeriodFound
        {
            get => _isPeriodFound; 
            set
            {
                _isPeriodFound = value;
                NotifyPropertyChanged(nameof(IsPeriodFound));
                NotifyPropertyChanged(nameof(PeriodResult));
            }
        }

        public ObservableCollection<string> NumbersSequence { get; set; }
        
        public bool UIOutputEnabled { get; set; }

        public bool FileOutputEnabled { get; set; }

        public bool DirectlyIntoFile { get; set; }

        #endregion output data

        #endregion properties

        #region constructor

        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            GenerateNumbersCommand = new RelayCommand(GenerateNumbers);
            NumbersSequence = new ObservableCollection<string>();

            _generator = new LinearCongruentialGenerator();
            _fileWriter = new FileWriter();
        }

        #endregion constructor

        #region commands

        public ICommand GenerateNumbersCommand { get; set; }
        public async void GenerateNumbers()
        {
            _uiNumbersCounter = 0;
            _fileNumbersCounter = 0;
            NumbersSequence.Clear();
            NumbersSequence = new ObservableCollection<string>();
            NotifyPropertyChanged(nameof(NumbersSequence));

            IsPeriodFound = false;
            NotifyPropertyChanged(nameof(IsPeriodFound));

            _generator.PeriodFound += OnGeneratorPeriodFound;
            _generator.PartOfSequenceGenerated += OnGeneratorPartOfSequenceFound;
            _generator.SequenceGenerated += OnGeneratorSequenceGenerated;

            ulong multiplier = (ulong)Math.Pow(MultiplierBase, MultiplierPower);
            ulong modulus = (ulong)Math.Pow(ComparisonModuleBase, ComparisonModulePower) - ComparisonModuleMinus;

            var mode = DirectlyIntoFile ? GeneratorMode.DIRECTLY_INTO_FILE : GeneratorMode.USE_BUFFER;
            if (mode == GeneratorMode.USE_BUFFER)
            {
                _fileWriter.CreateFile(CreateFullFilePath());
                await _fileWriter.WriteIntoFile($"M = {modulus}\ta = {multiplier}\tc = {Increase}\tX0 = {InitialValue}\tN = {Count}\n\n");
            }

            await Task.Run(() => _generator.StartGenerator(mode, Count, InitialValue, multiplier, Increase, modulus));
        }

        #endregion commands

        #region methods

        private string CreateFullFilePath()
        {
            string fullFilePath =  _filePath + _fileName + _fileCounter + _fileExtension;
            _fileCounter++;
            return fullFilePath;
        }

        private async Task ProceedPartOfSequence(List<ulong> Numbers)
        {
            List<ulong> numbers = Numbers;
            if (UIOutputEnabled)
            {
                foreach (ulong number in numbers)
                {
                    string temp = _uiNumbersCounter + ". " +  number.ToString();
                    _uiNumbersCounter++;
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        NumbersSequence.Add(temp);
                    });
                }
            }
            if (FileOutputEnabled)
            {
                foreach (ulong number in numbers)
                {
                    string temp = _fileNumbersCounter + ". " +  number.ToString() + "\n";
                    _fileNumbersCounter++;
                    await _fileWriter.WriteIntoFile(temp);
                }
            }

            NotifyPropertyChanged(nameof(NumbersSequence));
        }

        private async Task ProceedFoundPeriod(ulong count)
        {
            _generator.PeriodFound -= OnGeneratorPeriodFound;

            IsPeriodFound = true;
            _numbersInPeriod = count;

            NotifyPropertyChanged(nameof(IsPeriodFound));   
            NotifyPropertyChanged(nameof(PeriodResult));   
        }

        private async Task ProceedGeneratedSequence(List<ulong> Numbers)
        {
            _generator.PartOfSequenceGenerated -= OnGeneratorPartOfSequenceFound;
            _generator.SequenceGenerated -= OnGeneratorSequenceGenerated;

            await ProceedPartOfSequence(Numbers);

            _fileWriter.CloseFile();
        }

        #endregion methods

        #region event handlers

        private async void OnGeneratorSequenceGenerated(object sender, SequenceGeneratedEventArgs e)
        {
            await ProceedGeneratedSequence(e.Numbers);
        }

        private async void OnGeneratorPartOfSequenceFound(object sender, PartOfSequenceGeneratedEventArgs e)
        {
            await ProceedPartOfSequence(e.Numbers);
        }

        private async void OnGeneratorPeriodFound(object sender, PeriodFoundEventArgs e)
        {
            await ProceedFoundPeriod(e.Period);
        }

        #endregion event handlers

        #region trash 
        public ICommand MoveToAnotherScreen
        {
            get { return new RelayCommand(LoadAnotherScreen); }
        }

        private void LoadAnotherScreen()
        {
            PushViewModel(new AnotherScreen(ViewModelChanger));
        }
        #endregion trash
    }
}
