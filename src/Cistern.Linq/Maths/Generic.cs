using System;

namespace Cistern.Linq.Maths
{
    interface IMathsOperations<T, Accumulator>
        where T : struct
        where Accumulator : struct
    {
        Accumulator Zero { get; }
        T One { get; }

        T MinValue { get; }
        T MaxValue { get; }

        Accumulator Add(Accumulator lhs, T rhs);
        Accumulator Add(Accumulator lhs, T? rhs);
        Accumulator AddInt(Accumulator lhs, int rhs);

        T Cast(Accumulator a);
        T Cast(int a);

        Accumulator Cast(T a);

        bool IsNaN(T x);
        bool GreaterThan(T lhs, T rhs);
        bool LessThan(T lhs, T rhs);

        T MaxInit { get; }
    }

    struct OpsDouble : IMathsOperations<double, double>
    {
        public double Zero => 0.0;
        public double One => 1.0;
        public double MinValue => double.MinValue;
        public double MaxValue => double.MaxValue;
        public double Add(double lhs, double rhs) => lhs + rhs;
        public double Add(double lhs, double? rhs) => lhs + rhs.GetValueOrDefault();
        public double AddInt(double lhs, int rhs) => lhs + rhs;
        public double Cast(double a) => a;
        public double Cast(int a) => a;
        public bool IsNaN(double x) => double.IsNaN(x);
        public bool GreaterThan(double lhs, double rhs) => lhs > rhs;
        public bool LessThan(double lhs, double rhs) => lhs < rhs;

        public double MaxInit => double.NaN;
    }

    struct OpsFloat : IMathsOperations<float, double>
    {
        public double Zero => 0.0;
        public float One => 1.0f;
        public float MinValue => float.MinValue;
        public float MaxValue => float.MaxValue;
        public double Add(double lhs, float rhs) => lhs + rhs;
        public double Add(double lhs, float? rhs) => lhs + rhs.GetValueOrDefault();
        public double AddInt(double lhs, int rhs) => lhs + rhs;
        public float Cast(double a) => (float)a;
        public float Cast(int a) => a;
        public double Cast(float a) => a;
        public bool IsNaN(float x) => float.IsNaN(x);
        public bool GreaterThan(float lhs, float rhs) => lhs > rhs;
        public bool LessThan(float lhs, float rhs) => lhs < rhs;
        public float MaxInit => float.NaN;
    }

    struct OpsInt : IMathsOperations<int, int>
    {
        public int Zero => 0;
        public int One => 1;
        public int MinValue => int.MinValue;
        public int MaxValue => int.MaxValue;
        public int Add(int lhs, int rhs) { checked { return lhs + rhs; } }
        public int Add(int lhs, int? rhs) { checked { return lhs + rhs.GetValueOrDefault(); } }
        public int AddInt(int lhs, int rhs) { checked { return lhs + rhs; } }
        public int Cast(int a) => a;
        public bool IsNaN(int x) => false;
        public bool GreaterThan(int lhs, int rhs) => lhs > rhs;
        public bool LessThan(int lhs, int rhs) => lhs < rhs;
        public int MaxInit => int.MinValue;
    }

    struct OpsLong : IMathsOperations<long, long>
    {
        public long Zero => 0;
        public long One => 1;
        public long MinValue => long.MinValue;
        public long MaxValue => long.MaxValue;
        public long Add(long lhs, long rhs) { checked { return lhs + rhs; } }
        public long Add(long lhs, long? rhs) { checked { return lhs + rhs.GetValueOrDefault(); } }
        public long AddInt(long lhs, int rhs) { checked { return lhs + rhs; } }
        public long Cast(long a) => a;
        public long Cast(int a) => a;
        public bool IsNaN(long x) => false;
        public bool GreaterThan(long lhs, long rhs) => lhs > rhs;
        public bool LessThan(long lhs, long rhs) => lhs < rhs;
        public long MaxInit => long.MinValue;
    }


    struct OpsDecimal : IMathsOperations<decimal, decimal>
    {
        public decimal Zero => 0M;
        public decimal One => 1M;
        public decimal MinValue => decimal.MinValue;
        public decimal MaxValue => decimal.MaxValue;
        public decimal Add(decimal lhs, decimal rhs) => lhs + rhs;
        public decimal Add(decimal lhs, decimal? rhs) => lhs + rhs.GetValueOrDefault();
        public decimal AddInt(decimal lhs, int rhs) { checked { return lhs + rhs; } }
        public decimal Cast(decimal a) => a;
        public decimal Cast(int a) => a;
        public bool IsNaN(decimal x) => false;
        public bool GreaterThan(decimal lhs, decimal rhs) => lhs > rhs;
        public bool LessThan(decimal lhs, decimal rhs) => lhs < rhs;
        public Decimal MaxInit => Decimal.MinValue;
    }
}
