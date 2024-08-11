using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Markup;

namespace MessageHelperNS
{
    public static class MessageHelper
    {
        public static int BuildUpdateMessage(char memberID, int messageID, char operationID, int dataLen, string offset, string text)
        {            
            string sMsgID = MsgIDToString(messageID);
            string sDataLen = dataLenToString(dataLen);
            string formattedOffset = formatOffset(offset);

            string updateMessage = memberID.ToString() + sMsgID + operationID + sDataLen + formattedOffset + text;

            return 0;
        }

        public static string MsgIDToString(int messageID)
        {
            string msgID = "";

            if (messageID < 10)
            {
                msgID = '0' + '0' + messageID.ToString();
            }
            else if (messageID < 100)
            {
                msgID = '0' + messageID.ToString(); 
            }
            else
            {
                msgID = messageID.ToString();
            }

            return msgID;
        }

        public static string dataLenToString(int dLen)
        {
            string sDLen = "";

            if (dLen < 10)
            {
                sDLen = '0' + '0' + '0' + '0' + dLen.ToString();
            }
            else if (dLen < 100)
            {
                sDLen = '0' + '0' + '0' + dLen.ToString();
            }
            else if (dLen < 1000)
            {
                sDLen = '0' + '0' + dLen.ToString();
            }
            else if (dLen < 10000)
            {
                sDLen = '0' + dLen.ToString();
            }
            else
            {
                sDLen = dLen.ToString();
            }

            return sDLen;
        }

        public static string formatOffset(string offset)
        {
            string formatted = "";

            if (offset.Length < 10)
            {
                formatted = '0' + '0' + '0' + '0' + '0' + offset;
            }
            else if (offset.Length < 100)
            {
                formatted = '0' + '0' + '0' + '0' + offset;
            }
            else if (offset.Length < 1000)
            {
                formatted = '0' + '0' + '0' + offset;
            }
            else if (offset.Length < 10000)
            {
                formatted = '0' + '0' + offset;
            }
            else if (offset.Length < 100000)
            {
                formatted = '0' + offset;
            }
            else
            {
                formatted = offset;
            }

            return formatted;
        }
    }
}
