using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lab_1.Models
{
    internal class FileWriter
    {
        #region fields

        private string _filePath;
        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        #endregion fields

        #region contructors

        public FileWriter()
        {

        }

        #endregion contructors

        #region methods

        public void CreateFile(string filePath)
        {
            _filePath = filePath;
            _fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            _streamWriter = new StreamWriter(_fileStream);
        }

        public async Task WriteIntoFile(string data)
        {
            _streamWriter.Write(data);
        }

        public async Task WriteIntoFile(string[] data)
        {
            foreach (var item in data)
            {
                await _streamWriter.WriteAsync(item);
            }
        }

        public async Task WriteIntoFile(IEnumerable<ulong> data)
        {
            foreach (var item in data)
            {
                await _streamWriter.WriteAsync(item.ToString() + "\n");
            }
        }

        public void CloseFile()
        {
            _streamWriter.Close();
            _fileStream.Close();
        }

        #endregion methods
    }
}
