﻿using System;
using Cistern.Linq;

namespace Cistern.Linq.Playground
{
    enum Playthings
    {
        mikednSystemLinq,
        mikednCisternLinq,
        cartlinqSystemLinq,
        cartlinqCisternLinq,
    }

    class Program
    {
        static Playthings plaything = Playthings.cartlinqCisternLinq;

        static void Main(string[] args)
        {
            Console.WriteLine(plaything);

            switch(plaything)
            {
                case Playthings.mikednCisternLinq: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.mikednSystemLinq: mikedn.SystemLinq.Program.mikedn(); break;
                case Playthings.cartlinqSystemLinq: cartlinq.CisternLinq.Program.cartlinq(); break;
                case Playthings.cartlinqCisternLinq: cartlinq.SystemLinq.Program.cartlinq(); break;
            }
        }
    }
}
