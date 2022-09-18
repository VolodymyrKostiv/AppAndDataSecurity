using System;
using System.IO;

namespace Lab_3.Helpers.FileHelpers
{
    internal class OutputFileHelper
    {
        #region fields

        private FileStream _fileStream;

        #endregion fields

        #region constructors

        public OutputFileHelper()
        {
            //
        }

        public OutputFileHelper(string fileName)
        {
            OpenFile(fileName);
        }

        #endregion constructors

        #region methods

        public void WriteBlock(byte[] bytes)
        {
            try
            {
                _fileStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\nBlyat, ebanyi output stream");
            }
        }

        public void OpenFile(string fileName)
        {
            _fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
        }

        public void CloseFile()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream = null;
            }
        }

        #endregion methods
    }
}
