using Lab_2.Constants;
using Lab_2.Helpers;
using System;
using System.Linq;

namespace Lab_2.Business
{
    internal class Digest
    {
        #region properties

        public uint A { get; set; }
		public uint B { get; set; }
		public uint C { get; set; }
		public uint D { get; set; }

        #endregion properties

        #region constructor

        public Digest()
		{
			A = MD5Constants.MDBufferA;
			B = MD5Constants.MDBufferB;
			C = MD5Constants.MDBufferC;
			D = MD5Constants.MDBufferD;
		}

        #endregion constructor

        #region methods

        public void IterationSwap(uint F, uint[] X, uint i, uint k)
        {
            var tempD = D;
            D = C;
            C = B;
            B += BitsHelper.LeftRotate(A + F + X[k] + MD5Constants.T[i], MD5Constants.S[i]);
            A = tempD;
        }

        #region overrides

        public override bool Equals(object value)
        {
            return value is Digest md
                && (GetHashCode() == md.GetHashCode() || ToString() == md.ToString());
        }

        public Digest Clone()
        {
            return MemberwiseClone() as Digest;
        }

        public static Digest operator +(Digest left, Digest right)
        {
            return new Digest
            {
                A = left.A + right.A,
                B = left.B + right.B,
                C = left.C + right.C,
                D = left.D + right.D
            };
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{ToByteString(A)}{ToByteString(B)}{ToByteString(C)}{ToByteString(D)}";
        }

        private static string ToByteString(uint x)
        {
            return string.Join(string.Empty, BitConverter.GetBytes(x).Select(y => y.ToString("x2")));
        }

        #endregion overrides

        #endregion methods
    }
}
