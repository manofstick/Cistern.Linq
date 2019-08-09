using System;
using System.Collections.Generic;
using System.Text;

namespace Cistern.Linq.Maths
{
    interface IMathsOperations<T, Accumulator>
        where T : struct
        where Accumulator : struct
    {
        Accumulator Zero { get; }

        Accumulator Add(Accumulator lhs, T rhs);
        Accumulator Add(Accumulator lhs, T? rhs);

        T Cast(Accumulator a);
    }

    struct OpsDouble : IMathsOperations<Double, Double>
    {
        public double Zero => 0.0;
        public double Add(double lhs, double rhs) => lhs + rhs;
        public double Add(double lhs, double? rhs) => lhs + rhs.GetValueOrDefault();
        public double Cast(double a) => a;
    }

    struct OpsFloat : IMathsOperations<Single, Double>
    {
        public double Zero => 0.0;
        public double Add(double lhs, float rhs) => lhs + rhs;
        public double Add(double lhs, float? rhs) => lhs + rhs.GetValueOrDefault();
        public float Cast(double a) => (float)a;
    }

    struct OpsInt : IMathsOperations<int, int>
    {
        public int Zero => 0;
        public int Add(int lhs, int rhs) { checked { return lhs + rhs; } }
        public int Add(int lhs, int? rhs) { checked { return lhs + rhs.GetValueOrDefault(); } }
        public int Cast(int a) => a;
    }

    struct OpsLong : IMathsOperations<long, long>
    {
        public long Zero => 0;
        public long Add(long lhs, long rhs) { checked { return lhs + rhs; } }
        public long Add(long lhs, long? rhs) { checked { return lhs + rhs.GetValueOrDefault(); } }
        public long Cast(long a) => a;
    }


    struct OpsDecimal : IMathsOperations<Decimal, Decimal>
    {
        public Decimal Zero => 0M;
        public Decimal Add(Decimal lhs, Decimal rhs) { checked { return lhs + rhs; } }
        public Decimal Add(Decimal lhs, Decimal? rhs) { checked { return lhs + rhs.GetValueOrDefault(); } }
        public Decimal Cast(Decimal a) => a;
    }
}
