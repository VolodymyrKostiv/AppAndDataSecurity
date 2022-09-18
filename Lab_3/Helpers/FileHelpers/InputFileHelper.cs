using System;
using System.Collections.Generic;
using System.IO;

namespace Lab_3.Helpers.FileHelpers
{
    internal class InputFileHelper
    {
        #region fields

        private FileStream _fileStream;
        private BufferedStream _bufferedStream;
        List<byte> bytes = new List<byte>();
        private int _position;

        #endregion fields

        #region constructors

        public InputFileHelper()
        {
            //
        }

        public InputFileHelper(string fileName)
        {
            OpenFile(fileName);
            _position = 0;
        }

        #endregion constructors

        #region methods

        public byte[] ReadBlock(int blockSize)
        {

            bytes.Clear();
            int tempByte = 0;
            try
            {
                for (int i = 0; i < blockSize; i++)
                {
                    try
                    {
                        tempByte = _bufferedStream.ReadByte();
                        if (tempByte == -1)
                        {
                            break;
                        }
                        else
                        {
                            bytes.Add((byte)tempByte);
                        }
                    }
                    catch(Exception ex)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\nBlyat, ebanyi input stream");
            }

            return bytes.ToArray();
        }

        public void OpenFile(string fileName)
        {
            _fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            _bufferedStream = new BufferedStream(_fileStream);
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
