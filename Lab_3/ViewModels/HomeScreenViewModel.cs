using CommonWPF.Interfaces;
using CommonWPF.ViewModels;
using Lab_3.Enums;
using Lab_3.Models;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Lab_3.ViewModels
{
    internal class HomeScreenViewModel : BaseViewModel
    {
        public HomeScreenViewModel(IChangeViewModel viewModelChanger) : base(viewModelChanger)
        {
            //FileHelper fileHelper = new FileHelper();
            //fileHelper.ReadFile(@"D:\Programs\WindowISO\Windows.iso");
            //RC5 rc5 = new RC5(10, GetType());
            try
            {
                RC5 rc5 = new RC5();
                var hashedKey = Encoding.UTF8
                    .GetBytes("i wanna foxy")
                    .GetMD5HashedKeyForRC5(Enums.KeyLength.Bytes_8);

                var encodedFileContent = rc5.Encrypt(Encoding.UTF8
                    .GetBytes("I wanna be the foxy"), 
                    8,
                    hashedKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "RC5");
            }
        }
    }
}
