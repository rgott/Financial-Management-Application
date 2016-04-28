﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Financial_Management_Application.Models
{
    public class Logger
    {
        public static string LogDirectoryPath = Environment.CurrentDirectory;

        public static void Log(String lines)
        {

            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(LogDirectoryPath + "\\Error.log", true);
                file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " + lines);
                file.Close();
            }
            catch
            {

            }
        }
    }
}