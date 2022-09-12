using System;
using System.Collections.Generic;
using System.IO;

namespace Lab_3.Helpers
{
    internal class FileHelper
    {
        public FileHelper()
        {

        }

        public byte[] ReadFile(string fileName)
        {
            //using (Stream stream = File.OpenRead(fileName))
            //{
            //    using (BufferedStream memoryStream = new BufferedStream(stream))
            //    {
            //        using (StreamReader reader = new StreamReader(memoryStream))
            //        {
            //            reader.R
            //        }
            //    }
            //}

            byte[] bytes = null;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (BufferedStream buff =  new BufferedStream(fs))
                    {
                        List<byte> x = new List<byte>((int)fs.Length);
                        
                        using (BinaryReader b = new BinaryReader(buff))
                        {
                            long count = 0;
                            long mmsLength = buff.Length;
                            while (count < mmsLength)
                            {
                                //bytes[count] = b.ReadByte();
                                x.Add(b.ReadByte());
                                count++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return bytes;
        }
    }
}
