using System.Diagnostics;

namespace FEM_PR2;

public abstract class Solver
{
   protected SparseMatrix _matrix;
   protected Vector _vector;
   protected Vector _solution;

   public double Eps { get; init; }
   public int MaxIters { get; init; }
   public double SolvationTime { get; protected set; }

   public Solver(double eps = 1e-14, int maxIters = 2000)
   {
      Eps = eps;
      MaxIters = maxIters;

      _matrix = new SparseMatrix(0, 0);
      _vector = new Vector(0);
      _solution = new Vector(0);
   }

   public void SetSLAE(Vector vector, SparseMatrix matrix)
   {
      _vector = vector;
      _matrix = matrix;
   }

   public abstract Vector Solve();
}

public class BCG : Solver
{
    public BCG(double eps = 1e-14, int maxIters = 2000) : base(eps, maxIters){}

   public override Vector Solve()
   {
      _solution = new(_vector.Size);

      Vector residual = _vector - _matrix * _solution;

      Vector p = new(residual.Size);
      Vector z = new(residual.Size);
      Vector s = new(residual.Size);

      Vector.Copy(residual, p);
      Vector.Copy(residual, z);
      Vector.Copy(residual, s);


      Stopwatch sw = Stopwatch.StartNew();

      double vecNorm = _vector.Norm();
      double discrepancy = 1;
      double prPrev = p * residual;

      for (int i = 1; i <= MaxIters && discrepancy > Eps; i++)
      {
         var Az = _matrix * z;
         double alpha = prPrev / (s * Az);

         _solution = _solution + alpha * z;
         residual = residual - alpha * Az;
         p = p - alpha * SparseMatrix.TransposedMatrixMult(_matrix, s);

         double pr = p * residual;
         double beta = pr / prPrev;
         prPrev = pr;

         z = residual + beta * z;
         s = p + beta * s;

         discrepancy = residual.Norm() / vecNorm;
      }

      sw.Stop();
      SolvationTime = sw.ElapsedMilliseconds;

      return _solution;
   }
}