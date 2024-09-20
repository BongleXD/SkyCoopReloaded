﻿using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoop
{
    public class Logger
    {
        public static void Log(System.ConsoleColor Color, string Message)
        {
            MelonLogger.Msg(Color, Message);
        }
        public static void Log(string Message)
        {
            MelonLogger.Msg(Message);
        }

        public static void Log(object obj)
        {
            MelonLogger.Msg(obj);
        }

        public static void Log(System.ConsoleColor Color, object obj)
        {
            MelonLogger.Msg(Color, obj);
        }
    }
}
