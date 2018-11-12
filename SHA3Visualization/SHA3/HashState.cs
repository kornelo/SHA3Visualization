using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHA3Visualization.SHA3
{
    class HashState
    {
        private static string Name { get; set; }

        private static ulong Value { get; set; }

        private static List<ulong> States;



        HashState(string name, ulong value)
        {
            Name = name;
            Value = value;
        }

    }
}
