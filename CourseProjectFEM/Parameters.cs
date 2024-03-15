namespace FEM_PR2;

public class Parameters
{
   const double Mu0 = 4.0 * Math.PI * 1e-7;

   public static double Mu(int material, Point2D? point = null, bool isNonLinear = false)
   {
      if (isNonLinear is true)
         return point!.X;

      return material switch
      {
          0 => Mu0 * 1000,
          1 => Mu0 * 1,
          2 => Mu0 * 1,
          3 => Mu0 * 1,
          _ => Mu0 * 1
      };
    }

   public static double F(int material, Point2D? point = null) => material switch
   {
       0 => 0.0,
       1 => 0.0,
       2 => 1.0e6,
       3 => -1.0e6,
       _ => 0.0
   };
     
   public static double U(Point2D point) => 0.0;

   public static double Ug1(Point2D point) => 0.0;

   public static double dU_dn(Point2D point) => 0.0;
}
