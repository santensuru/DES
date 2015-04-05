using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    static class Program
    {
        static byte[] initKey = {
            (byte) 0x00, (byte) 0x11, (byte) 0x22, (byte) 0x33,
            (byte) 0x44, (byte) 0x55, (byte) 0x66, (byte) 0x77
        };
        
        static int[] pc1 = {
            57, 49, 41, 33, 25, 17,  9,
             1, 58, 50, 42, 34, 26, 18,
            10,  2, 59, 51, 43, 35, 27,
            19, 11,  3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
             7, 62, 54, 46, 38, 30, 22,
            14,  6, 61, 53, 45, 37, 29,
            21, 13,  5, 28, 20, 12,  4
        };

        static int[] iter = {
            1, 1, 2, 2, 2, 2, 2, 2,
            1, 2, 2, 2, 2, 2, 2, 1
        };

        static BitArray[] CC = new BitArray[17];
        static BitArray[] DD = new BitArray[17];

        static void GenerateKey()
        {
            var bits = new BitArray(initKey);
            BitArray bits2 = new BitArray(56);
            BitArray c = new BitArray(28);
            BitArray d = new BitArray(28);

            BitArray tempC = new BitArray(28);
            BitArray tempD = new BitArray(28);

            int i;
            for (i = 0; i < 56; i++)
            {
                bits2[i] = bits[ pc1[i] ];
            }

            CC[0] = new BitArray(28);
            DD[0] = new BitArray(28);

            for (i = 0; i < 28; i++)
            {
                c[i] = bits2[i];
                d[i] = bits2[i + 28];
                CC[0][i] = bits2[i];
                DD[0][i] = bits2[i + 28];
            }
            tempC = c;
            tempD = d;

            //Console.Write(BitArrayToHexStr(c));
            //Console.Write(" ");
            //Console.WriteLine(BitArrayToHexStr(d));

            for (i = 0; i < 16; i++)
            {
                CC[i + 1] = new BitArray(28);
                DD[i + 1] = new BitArray(28);

                int j;
                for (j = 0; j < 28; j++)
                {
                    c[j] = tempC[(j + iter[i]) % 28];
                    d[j] = tempD[(j + iter[i]) % 28];
                    CC[i + 1][j] = tempC[(j + iter[i]) % 28];
                    DD[i + 1][j] = tempD[(j + iter[i]) % 28];
                }
                tempC = c;
                tempD = d;

                //Console.Write(BitArrayToHexStr(c));
                //Console.Write(" ");
                //Console.WriteLine(BitArrayToHexStr(d));
            }

        }

        static byte[] ToByteArray(this BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }

        static String BitArrayToStr(BitArray ba)
        {
            byte[] strArr = new byte[ba.Length / 8];

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            for (int i = 0; i < ba.Length / 8; i++)
            {
                for (int index = i * 8, m = 1; index < i * 8 + 8; index++, m *= 2)
                {
                    strArr[i] += ba.Get(index) ? (byte)m : (byte)0;
                }
            }

            return encoding.GetString(strArr);
        }

        static String BitArrayToHexStr(BitArray ba)
        {
            String hex = "";
            int len = ba.Length;
            int i;
            int temp;
            for (i = 0; i < len; i += 4)
            {
                temp = 0;
                if (ba[i] == true)
                {
                    temp += 1;
                }
                if (ba[i+1] == true)
                {
                    temp += 2;
                }
                if (ba[i+2] == true)
                {
                    temp += 4;
                }
                if (ba[i+3] == true)
                {
                    temp += 8;
                }

                if (temp < 10)
                {
                    hex += temp.ToString();
                }
                else
                {
                    if (temp == 10)
                        hex += "A";
                    else if (temp == 11)
                        hex += "B";
                    else if (temp == 12)
                        hex += "C";
                    else if (temp == 13)
                        hex += "D";
                    else if (temp == 14)
                        hex += "E";
                    else
                        hex += "F";
                }
            }

            return hex;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(BitArrayToHexStr(new BitArray(initKey)));
            GenerateKey();
            int i;
            for (i = 0; i < 17; i++)
            {
                Console.Write(BitArrayToHexStr(CC[i]));
                Console.Write(" ");
                Console.WriteLine(BitArrayToHexStr(DD[i]));
            }
            //string s = "Hello World";
            //byte[] bytes = Encoding.ASCII.GetBytes(s);
            //BitArray b = new BitArray(bytes);
            //string s2 = BitArrayToStr(b);
            //Console.WriteLine(s2);
            Console.ReadKey();
        }
    }
}
