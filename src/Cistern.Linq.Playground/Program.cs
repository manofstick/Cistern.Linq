using System;
using Cistern.Linq;

namespace Playground
{
    enum Playthings
    {
        system_mikedn,
        cistern_mikedn,
        system_cartlinq,
        cistern_cartlinq,
        system_jamesqo,
        cistern_jamesqo,
    }

    class Program
    {
        static Playthings plaything = Playthings.cistern_jamesqo;

        static void Main(string[] args)
        {
            Console.WriteLine(plaything);

            switch(plaything)
            {
                case Playthings.cistern_mikedn: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.system_mikedn: mikedn.SystemLinq.Program.mikedn(); break;

                case Playthings.cistern_cartlinq: cartlinq.CisternLinq.Program.cartlinq(); break;
                case Playthings.system_cartlinq: cartlinq.SystemLinq.Program.cartlinq(); break;

                case Playthings.cistern_jamesqo: jamesko.CisternLinq.Program.jamesqo(); break;
                case Playthings.system_jamesqo: jamesko.SystemLinq.Program.jamesqo(); break;
            }
        }
    }
}
