using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBCI_GUI
{
    class Convert
    {
        public static int Bit16ToInt32(byte[] byteArray)
        {
            int result = (
              ((0xFF & byteArray[0]) << 8) |
               (0xFF & byteArray[1])
              );
            if ((result & 0x00008000) > 0)
            {
                result = (int)((uint)result | (uint)0xFFFF0000);

            }
            else
            {
                result = (int)((uint)result & (uint)0x0000FFFF);
            }
            return result;
        }
        public static int Bit24ToInt32(byte[] byteArray)
        {
            int result = (
                 ((0xFF & byteArray[0]) << 16) |
                 ((0xFF & byteArray[1]) << 8) |
                 (0xFF & byteArray[2])
               );
            if ((result & 0x00800000) > 0)
            {
                result = (int)((uint)result | (uint)0xFF000000);
            }
            else
            {
                result = (int)((uint)result & (uint)0x00FFFFFF);
            }
            return result;
        }
        static double[] ConvertedData = new double[12];
        private static int localByteCounter = 0;
        private static int localChannelCounter = 0;
        private static int PACKET_readstate = 0;
        private static byte[] localAdsByteBuffer = { 0, 0, 0 };
        private static byte[] localAccelByteBuffer = { 0, 0 };

        public static double[] interpretBinaryStream(byte actbyte)
        {
            bool flag_copyRawDataToFullData = false;

            switch (PACKET_readstate)
            {
                case 0:
                    if (actbyte == 0xC0)
                    {          // poszukiwanie poczatku pakietu
                        PACKET_readstate++;
                    }
                    break;
                case 1:
                    if (actbyte == 0xA0)
                    {          // poszukiwanie poczatku pakietu
                        PACKET_readstate++;
                    }
                    else
                    {
                        PACKET_readstate = 0;
                    }
                    break;
                case 2:
                    localByteCounter = 0;
                    localChannelCounter = 0;
                    ConvertedData[localChannelCounter] = actbyte;
                    localChannelCounter++;
                    PACKET_readstate++;
                    break;
                case 3:
                    localAdsByteBuffer[localByteCounter] = actbyte;
                    localByteCounter++;
                    if (localByteCounter == 3)
                    {
                        ConvertedData[localChannelCounter] = Bit24ToInt32(localAdsByteBuffer);
                        localChannelCounter++;
                        if (localChannelCounter == 9)
                        {
                            PACKET_readstate++;
                            localByteCounter = 0;
                        }
                        else
                        {
                            localByteCounter = 0;
                        }
                    }
                    break;
                case 4:
                    localAccelByteBuffer[localByteCounter] = actbyte;
                    localByteCounter++;
                    if (localByteCounter == 2)
                    {
                        ConvertedData[localChannelCounter] = Bit16ToInt32(localAccelByteBuffer);
                        localChannelCounter++;
                        if (localChannelCounter == 12)
                        {
                            PACKET_readstate++;
                            localByteCounter = 0;
                        }
                        else
                        {
                            localByteCounter = 0;
                        }
                    }
                    break;
                case 5:
                    if (actbyte == 0xC0)
                    {
                        flag_copyRawDataToFullData = true;
                        PACKET_readstate = 1;
                    }
                    else
                    {
                        PACKET_readstate = 0;
                    }

                    break;
                default:
                    PACKET_readstate = 0;
                    break;
            }

            if (flag_copyRawDataToFullData)
            {
                flag_copyRawDataToFullData = false;
                return ConvertedData;
            }
            else
            {
                return null;
            }
        }
    }
}
