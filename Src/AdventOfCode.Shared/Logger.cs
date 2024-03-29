﻿using System;

namespace AdventOfCode.Shared
{
    public static class Logger
    {
        public static bool ShowDebug { get; set; }

        public static void Debug(object message)
        {
            if (!ShowDebug)
            {
                return;
            }

            Console.WriteLine(message);
        }

        public static void Info(object message)
        {
            Console.WriteLine(message);
        }
    }
}
