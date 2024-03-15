namespace FEM_PR2;

public abstract class Function
{
   public abstract double Compute(double point, Interval interval, int i, int j);
}

public class PsiPsi : Function
{
   public override double Compute(double point, Interval interval, int i, int j)
      => linearBasis.Psi(i, point) * linearBasis.Psi(j, point)
         * interval.Length;
}

public class DPsiDPsi : Function
{
   public override double Compute(double point, Interval interval, int i, int j)
      => linearBasis.DPsi(i, point) * linearBasis.DPsi(j, point)
         * 1.0 / interval.Length;
}

public static class Gauss5
{
   private static readonly double[] _points = new double[]
   {
         0.0,
        -1.0 / 3.0 * Math.Sqrt(5.0 - 2.0 * Math.Sqrt(10.0 / 7.0)),
         1.0 / 3.0 * Math.Sqrt(5.0 - 2.0 * Math.Sqrt(10.0 / 7.0)),
        -1.0 / 3.0 * Math.Sqrt(5.0 + 2.0 * Math.Sqrt(10.0 / 7.0)),
         1.0 / 3.0 * Math.Sqrt(5.0 + 2.0 * Math.Sqrt(10.0 / 7.0))
   };

   private static readonly double[] _weights = new double[]
   {
         128.0 / 225.0,
         (322.0 + 13.0 * Math.Sqrt(70.0)) / 900.0,
         (322.0 + 13.0 * Math.Sqrt(70.0)) / 900.0,
         (322.0 - 13.0 * Math.Sqrt(70.0)) / 900.0,
         (322.0 - 13.0 * Math.Sqrt(70.0)) / 900.0
   };

   public static double Integrate(Function function, Interval interval, int i, int j)
   {
      var master_interval = new Interval(0, 1);
      double result = 0;

      for (int iweight = 0; iweight < _points.Length; iweight++)
         result += _weights[iweight] * function.Compute(master_interval.Length / 2 * _points[iweight] + master_interval.Center, interval, i, j);
      return master_interval.Length / 2 * result;
   }
}
