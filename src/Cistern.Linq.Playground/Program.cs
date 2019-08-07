using System;
using Cistern.Linq;

namespace Cistern.Linq.Playground
{
    enum Playthings
    {
        mikednSystemLinq,
        mikednCisternLinq,
    }

    class Program
    {
        static Playthings plaything = Playthings.mikednCisternLinq;

        static void Main(string[] args)
        {
            switch(plaything)
            {
                case Playthings.mikednCisternLinq: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.mikednSystemLinq: mikedn.SystemLinq.Program.mikedn(); break;
            }
        }
    }
}
