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

        static byte[] moc = {
            (byte) 0x88, (byte) 0x99, (byte) 0xAA, (byte) 0xBB,
            (byte) 0xCC, (byte) 0xDD, (byte) 0xEE, (byte) 0xFF
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

        static int[] pc2 = {
            14, 17, 11, 24,  1,  5,
             3, 28, 15,  6, 21, 10,
            23, 19, 12,  4, 26,  8,
            16,  7, 27, 20, 13,  2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        //static BitArray[] CC = new BitArray[17];
        //static BitArray[] DD = new BitArray[17];

        static BitArray[] KK = new BitArray[17];

        static int[] ip = {
            58, 50, 42, 34, 26, 18, 10,  2,
            60, 52, 44, 36, 28, 20, 12,  4,
            62, 54, 46, 38, 30, 22, 14,  6,
            64, 56, 48, 40, 32, 24, 16,  8,
            57, 49, 41, 33, 25, 17,  9,  1,
            59, 51, 43, 35, 27, 19, 11,  3,
            61, 53, 45, 37, 29, 21, 13,  5,
            63, 55, 47, 39, 31, 23, 15,  7
        };

        static int[] exp = {
            32,  1,  2,  3,  4,  5,
             4,  5,  6,  7,  8,  9,
             8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,  1
        };

        // 16 * 4 ==> 1111 * 11

        static int[][] sbox = {
            // sbox 1 -> 0
            new int[]{
                14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7,
                 0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8,
                 4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0,
                15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13
            },
            new int[]{
                15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10,
                 3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5,
                 0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15,
                13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9
            },
            new int[]{
                10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8,
                13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1,
                13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7,
                 1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12
            },
            new int[]{
                 7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15,
                13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9,
                10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4,
                 3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14
            },
            // sbox 5 -> 4
            new int[]{
                 2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9,
                14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6,
                 4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14,
                11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3
            },
            new int[]{
                12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11,
                10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8,
                 9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6,
                 4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13
            },
            new int[]{
                 4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1,
                13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6,
                 1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2,
                 6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12
            },
            new int[]{
                13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7,
                 1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2,
                 7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8,
                 2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11
            }
        };

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
                bits2[i] = bits[pc1[i] - 1];
            }

            //CC[0] = new BitArray(28);
            //DD[0] = new BitArray(28);

            for (i = 0; i < 28; i++)
            {
                c[i] = bits2[i];
                d[i] = bits2[i + 28];
                //CC[0][i] = bits2[i];
                //DD[0][i] = bits2[i + 28];
            }
            tempC = c;
            tempD = d;

            //Console.Write(BitArrayToHexStr(c));
            //Console.Write(" ");
            //Console.WriteLine(BitArrayToHexStr(d));

            for (i = 0; i < 16; i++)
            {
                //CC[i + 1] = new BitArray(28);
                //DD[i + 1] = new BitArray(28);

                int j;
                for (j = 0; j < 28; j++)
                {
                    c[j] = tempC[(j + iter[i]) % 28];
                    d[j] = tempD[(j + iter[i]) % 28];
                    //CC[i + 1][j] = tempC[(j + iter[i]) % 28];
                    //DD[i + 1][j] = tempD[(j + iter[i]) % 28];
                }
                tempC = c;
                tempD = d;

                KK[i + 1] = new BitArray(48);

                for (j = 0; j < 48; j++)
                {
                    if (pc2[j] < 29)
                    {
                        KK[i + 1][j] = c[pc2[j] - 1];
                    }
                    else
                    {
                        KK[i + 1][j] = d[pc2[j] - 29];
                    }
                }

                //Console.Write(BitArrayToHexStr(c));
                //Console.Write(" ");
                //Console.WriteLine(BitArrayToHexStr(d));
            }

        }

        static void Process64bit(byte[] data)
        {
            BitArray temp = new BitArray(data);
            BitArray datas = new BitArray(data);
            int i;
            for (i = 0; i < 64; i++)
            {
                datas[i] = temp[ip[i] - 1];
            }
            Console.WriteLine(BitArrayToHexStr(datas));

            BitArray[] L = new BitArray[17];
            BitArray[] R = new BitArray[17];

            L[0] = new BitArray(32);
            R[0] = new BitArray(32);

            for (i = 0; i < 32; i++)
            {
                L[0][i] = datas[i];
                R[0][i] = datas[i + 32];
            }

            for (i = 0; i < 1; i++)
            {
                int j;
                BitArray E = new BitArray(48);
                for (j = 0; j < 48; j++)
                {
                    E[j] = R[i][exp[j] - 1];
                }
                Console.WriteLine(BitArrayToHexStr(E));
                
                for (j = 0; j < 48; j++)
                {
                    E[j] ^= KK[i + 1][j];
                }
                Console.WriteLine(BitArrayToHexStr(E));

                BitArray[] B = new BitArray[8];
                for (j = 0; j < 8; j++)
                {
                    B[j] = new BitArray(6);
                }

                for (j = 0; j < 8; j++)
                {
                    int k;
                    for (k = 0; k < 6; k++)
                    {
                        B[j][k] = E[k + j * 6];
                    }

                }

                BitArray[] BB = new BitArray[8];
                for (j = 0; j < 8; j++)
                {
                    BB[j] = new BitArray(4);
                }

                for (j = 0; j < 8; j++)
                {
                    int x = 0;
                    int y = 0;
                    if (B[j][0] == true)
                        x += 2;
                    if (B[j][5] == true)
                        x += 1;
                    int k;
                    for (k = 1; k < 5; k++)
                    {
                        if(B[j][k] == true)
                            y += (int) Math.Pow(2, 4 - k);
                    }
                    byte[] tempt = BitConverter.GetBytes(sbox[j][y + x * 16]);
                    BitArray temptt = new BitArray(tempt);
                    BitArray temptb = new BitArray(4);
                    for (k = 0; k < 4; k++)
                    {
                        temptb[k] = temptt[k];
                    }
                    BB[j] = new BitArray(temptb);

                    Console.Write(j + " ");
                    //Console.Write(sbox[j][y + x * 16] + " ");
                    //Console.Write(BB[j].Length);
                    Console.WriteLine(BitArrayToHexStr(BB[j]));
                }
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
            for (i = 1; i < 17; i++)
            {
                Console.WriteLine(BitArrayToHexStr(KK[i]));
            }
            Process64bit(moc);
            //string s = "Hello World";
            //byte[] bytes = Encoding.ASCII.GetBytes(s);
            //BitArray b = new BitArray(bytes);
            //string s2 = BitArrayToStr(b);
            //Console.WriteLine(s2);

            //Console.WriteLine(Math.Pow(3, 2));
            Console.ReadKey();
        }
    }
}
