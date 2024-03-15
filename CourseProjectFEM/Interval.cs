namespace FEM_PR2;

public class Interval
{
   public double LeftBoundary { get; init; }
   public double RightBoundary { get; init; }
   public double Length => RightBoundary - LeftBoundary;

   public Interval(double leftBoundary = 0, double rightBoundary = 1)
   {
      if (leftBoundary <= rightBoundary)
      {
         LeftBoundary = leftBoundary;
         RightBoundary = rightBoundary;
      }
      else
         throw new ArgumentException("Неверно задан интервал.");
   }
}