using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM_PR2;

public class Point2D
{
   public double X { get; init; }
   public double Y { get; init; }

   public Point2D(double x, double y) 
   {
      X = x;
      Y = y;
   }

   public override string ToString()
   {
      return $"{X:e15} {Y:e15}";
   }

   public static Point2D operator +(Point2D a, Point2D b)
      => new Point2D(a.X + b.X, a.Y + b.Y);
   public static Point2D operator -(Point2D a, Point2D b)
   => new Point2D(a.X - b.X, a.Y - b.Y);

   public static Point2D Parse(string input)
   {
      var data = input.Split().Select(double.Parse).ToList(); 
      return new Point2D(data[0], data[1]);
   }
}
