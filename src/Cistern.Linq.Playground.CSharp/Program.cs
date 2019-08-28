using System;
using Cistern.Linq;

namespace Playground
{
    enum Playthings
    {
        system_mikedn,
        cistern_mikedn,
        system_mikedn_immutable,
        cistern_mikedn_immutable,
        system_cartlinq,
        cistern_cartlinq,
        system_jamesqo,
        cistern_jamesqo,
        system_matthewwatson,
        cistern_matthewwatson,
    }

    class Program
    {
        static Playthings plaything = Playthings.cistern_matthewwatson;

        static void Main(string[] args)
        {
            Console.WriteLine(plaything);

            switch(plaything)
            {
                case Playthings.cistern_mikedn: mikedn.CisternLinq.Program.mikedn(); break;
                case Playthings.system_mikedn: mikedn.SystemLinq.Program.mikedn(); break;

                case Playthings.cistern_matthewwatson: matthewwatson.CisternLinq.Demo.Program.matthewwatson(); break;
                case Playthings.system_matthewwatson: matthewwatson.SystemLinq.Demo.Program.matthewwatson(); break;

                case Playthings.cistern_mikedn_immutable: mikedn_immutable.CisternLinq.Program.mikedn_immutable(); break;
                case Playthings.system_mikedn_immutable: mikedn_immutable.SystemLinq.Program.mikedn_immutable(); break;

                case Playthings.cistern_cartlinq: cartlinq.CisternLinq.Program.cartlinq(); break;
                case Playthings.system_cartlinq: cartlinq.SystemLinq.Program.cartlinq(); break;

                case Playthings.cistern_jamesqo: jamesko.CisternLinq.Program.jamesqo(); break;
                case Playthings.system_jamesqo: jamesko.SystemLinq.Program.jamesqo(); break;
            }
        }
    }
}
