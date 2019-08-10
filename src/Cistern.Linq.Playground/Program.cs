using System;
using Cistern.Linq;

namespace Playground
{
    enum Playthings
    {
        mikednSystemLinq,
        mikednCisternLinq,
        cartlinqSystemLinq,
        cartlinqCisternLinq,
        jamesqoSystemLinq,
        jamesqoCisternLinq,
    }

    class Program
    {
        static Playthings plaything = Playthings.jamesqoCisternLinq;

        static void Main(string[] args)
        {
            Console.WriteLine(plaything);

            switch(plaything)
            {
                case Playthings.mikednCisternLinq: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.mikednSystemLinq: mikedn.SystemLinq.Program.mikedn(); break;

                case Playthings.cartlinqCisternLinq: cartlinq.CisternLinq.Program.cartlinq(); break;
                case Playthings.cartlinqSystemLinq: cartlinq.SystemLinq.Program.cartlinq(); break;

                case Playthings.jamesqoCisternLinq: jamesko.CisternLinq.Program.jamesqo(); break;
                case Playthings.jamesqoSystemLinq: jamesko.SystemLinq.Program.jamesqo(); break;
            }
        }
    }
}
