using System;
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
        static Playthings plaything = Playthings.cartlinqSystemLinq;

        static void Main(string[] args)
        {
            Console.WriteLine(plaything);

            switch(plaything)
            {
                case Playthings.mikednCisternLinq: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.mikednSystemLinq: mikedn.SystemLinq.Program.mikedn(); break;
                case Playthings.cartlinqCisternLinq: cartlinq.CisternLinq.Program.cartlinq(); break;
                case Playthings.cartlinqSystemLinq: cartlinq.SystemLinq.Program.cartlinq(); break;
            }
        }
    }
}
