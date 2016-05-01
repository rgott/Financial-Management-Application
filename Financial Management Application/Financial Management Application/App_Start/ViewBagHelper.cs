using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Management_Application
{
    public class ViewBagHelper
    {
        public enum MessageType
        {
            WarningMsgBox,
            ErrorMsgBox,
            SuccessMsgBox
        }
        public static void setMessage(dynamic ViewBag, MessageType msg, string message)
        {
            switch (msg)
            {
                case MessageType.WarningMsgBox:
                    ViewBag.MsgBoxColor = "#F6BB42";
                    break;
                case MessageType.ErrorMsgBox:
                    ViewBag.MsgBoxColor = "#DA4453";
                    break;
                case MessageType.SuccessMsgBox:
                    ViewBag.MsgBoxColor = "#8CC152";
                    break;
            }
            ViewBag.TextForMsgBox = message;
        }
    }
}