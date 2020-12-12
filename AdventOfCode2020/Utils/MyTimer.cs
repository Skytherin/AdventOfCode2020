using System;
using System.Security.Cryptography;

namespace AdventOfCode2020.Utils
{
    public class MyTimer
    {
        private DateTime In = DateTime.Now;
        private DateTime LapDate = DateTime.Now;

        public void Lap()
        {
            Console.WriteLine($"{(DateTime.Now - LapDate).TotalSeconds}");
            LapDate = DateTime.Now;
        }

        public void Total()
        {
            Console.WriteLine($"{(DateTime.Now - In).TotalSeconds}");
        }
    }
}