namespace FEM_PR2;

public class Interval
{
   public double LeftBoundary { get; init; }
   public double RightBoundary { get; init; }
   public double Center => (LeftBoundary + RightBoundary) / 2;
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

   public static Interval Parse(string parseString)
   {
      var data = parseString.Split();
      return new Interval(double.Parse(data[0]), double.Parse(data[1]));
   }

   public bool Contains(double point)
    => LeftBoundary <= point && point <= RightBoundary;

   public override string ToString() => $"[{LeftBoundary}; {RightBoundary}]";
}