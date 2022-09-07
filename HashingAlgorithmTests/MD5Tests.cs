using Lab_2.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HashingAlgorithmTests
{
    [TestClass]
    public class MD5Tests
    {
        [DataRow("", "D41D8CD98F00B204E9800998ECF8427E")]
        [DataRow("a", "0CC175B9C0F1B6A831C399E269772661")]
        [DataRow("abc", "900150983CD24FB0D6963F7D28E17F72")]
        [DataRow("message digest", "F96B697D7CB7938D525A2F31AAF161D0")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57EDF4A22BE3C955AC49DA2E2107B67A")]
        [DataTestMethod]
        public void HashString_Correct(string message, string expectedHash)
        {
            //Arrange
            var md5 = new MD5();

            //Act
            var result = md5.HashString(message);
            var resultUpperString = result.ToUpper();

            //Assert
            Assert.AreEqual(expectedHash, resultUpperString);
        }

        [DataRow("", "D41D8CD98F00B204E9800998ECF8427F")]
        [DataRow("b", "0CC175B9C0F1B6A831C399E269772661")]
        [DataRow("abd", "900150983CD24FB0D6963F7D28E17F72")]
        [DataRow("mesage digest", "F96B697D7CB7938D525A2F31AAF161D0")]
        [DataRow("abdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
        [DataRow("ABDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
        [DataRow("1245678901234567890123456789012345678901234567890123456789012345678901234567890", "57EDF4A22BE3C955AC49DA2E2107B67A")]
        [DataTestMethod]
        public void HashString_NotCorrect(string message, string expectedHash)
        {
            //Arrange
            var md5 = new MD5();

            //Act
            var result = md5.HashString(message);
            var resultUpperString = result.ToUpper();

            //Assert
            Assert.AreNotEqual(expectedHash, resultUpperString);
        }

        [DataRow("", "D41D8CD98F00B204E9800998ECF8427E")]
        [DataRow("a", "0CC175B9C0F1B6A831C399E269772661")]
        [DataRow("abc", "900150983CD24FB0D6963F7D28E17F72")]
        [DataRow("message digest", "F96B697D7CB7938D525A2F31AAF161D0")]
        [DataRow("abcdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890", "57EDF4A22BE3C955AC49DA2E2107B67A")]
        [DataTestMethod]
        public void HashString_Determinated(string message, string expectedHash)
        {
            //Arrange
            var md5_1 = new MD5();
            var md5_2 = new MD5();

            //Act
            var result_1 = md5_1.HashString(message);
            var resultUpperString_1 = result_1.ToUpper();

            var result_2 = md5_2.HashString(message);
            var resultUpperString_2 = result_2.ToUpper();

            //Assert
            Assert.AreEqual(expectedHash, resultUpperString_1);
            Assert.AreEqual(expectedHash, resultUpperString_2);
            Assert.AreEqual(resultUpperString_1, resultUpperString_2);
        }
    }
}
