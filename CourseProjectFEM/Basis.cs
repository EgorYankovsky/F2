namespace FEM_PR2;

public class linearBasis
{
   // Basis function Psi(i), 0 <= i <= 1
   public static double Psi(int i, double point) 
    => i switch
    {
        0 => 1 - point,
        1 => point,
        _ => 0
    };

   // Derivatives of basis function Psi(i), 0 <= i <= 1
   public static double DPsi(int i, double point) 
    => i switch
    {
        0 => -1,
        1 => 1,
        _ => 0
    };
}