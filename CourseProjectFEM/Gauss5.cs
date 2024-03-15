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

