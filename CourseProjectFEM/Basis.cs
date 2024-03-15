namespace FEM_PR2;

public class linearBasis
{
   // Basis function Psi(i), 0 <= i <= 1
   public static double Psi(int i, double point) => i switch
                                                      {
                                                         0 => 1 - point,
                                                         1 => point,
                                                         _ => 0
                                                      };

   // Derivatives of basis function Psi(i), 0 <= i <= 1
   public static double DPsi(int i, double point) => i switch
                                                      {
                                                         0 => -1,
                                                         1 => 1,
                                                         _ => 0
                                                      };


   public static double MasterElement(double point, Interval interval)
      => (point - interval.LeftBoundary) / interval.Length;

   public static (int, int) BilinearToLinear(int iterator) => iterator switch
                                                              {
                                                                 0 => (0, 0),
                                                                 1 => (1, 0),
                                                                 2 => (0, 1),
                                                                 3 => (1, 1),
                                                                 _ => (0, 0)
                                                              };
}

public static class CubicLagrangianBasis
{
   // Basis function Psi(i), 0 <= i <= 3
   public static double Psi(int i, double point) => i switch
      {
         0 => -4.5 * (point - 1.0 / 3.0) * (point - 2.0 / 3.0) * (point - 1.0),
         1 => 13.5 * point * (point - 2.0 / 3.0) * (point - 1.0),
         2 => -13.5 * point * (point - 1.0 / 3.0) * (point - 1.0),
         3 => 4.5 * point * (point - 1.0 / 3.0) * (point - 2.0 / 3.0),
         _ => 0
      };

   // Derivatives of basis function Psi(i), 0 <= i <= 3
   public static double DPsi(int i, double point) => i switch
      {
         0 => -13.5 * point * point + 18 * point - 5.5,
         1 => 40.5 * point * point - 45 * point + 9,
         2 => -4.5 * (9 * point * point - 8 * point + 1),
         3 => 13.5 * point * point - 9 * point + 1,
         _ => 0
      };

   /*
      Master element: section with four nodes like this
      ._____._____._____.
      <-h/3->
      <--------h-------->
      where h = 1
   */
   public static double MasterElement(double point, Interval interval)
      => (point - interval.LeftBoundary) / interval.Length;
}