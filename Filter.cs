using System;
using System.IO;
using System.Collections.Generic;

namespace WorkerServiceTest
{
    public class Filter
    {
        public void readData(List<string> items, int length)
        {
            for(int i = 0; i < length; i++)
            {
                Console.WriteLine(items[i], length);
            }
            
        }
    }
}