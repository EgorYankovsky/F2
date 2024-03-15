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

   protected void DecomposeLU()
   {
      for (int i = 0; i < _matrix.Size; i++)
      {
         for (int j = _matrix._ia[i]; j < _matrix._ia[i + 1]; j++)
         {
            int jColumn = _matrix._ja[j];
            int jk = _matrix._ia[jColumn];
            int k = _matrix._ia[i];

            int shift = _matrix._ja[_matrix._ia[i]] - _matrix._ja[_matrix._ia[jColumn]];

            if (shift > 0)
               jk += shift;
            else
               k -= shift;

            double sumL = 0.0;
            double sumU = 0.0;

            for (; k < j && jk < _matrix._ia[jColumn + 1]; k++, jk++)
            {
               sumL += _matrix._al[k] * _matrix._au[jk];
               sumU += _matrix._au[k] * _matrix._al[jk];
            }

            _matrix._al[j] -= sumL;
            _matrix._au[j] -= sumU;
            _matrix._au[j] /= _matrix._di[jColumn];
         }

         double sumD = 0.0;
         for (int j = _matrix._ia[i]; j < _matrix._ia[i + 1]; j++)
            sumD += _matrix._al[j] * _matrix._au[j];

         _matrix._di[i] -= sumD;
      }
   }

   protected void ForwardElimination()
   {
      for (int i = 0; i < _matrix.Size; i++)
      {
         for (int j = _matrix._ia[i]; j < _matrix._ia[i + 1]; j++)
            _solution[i] -= _matrix._al[j] * _solution[_matrix._ja[j]];
         _solution[i] /= _matrix._di[i];
      }
   }

   protected void BackwardSubstitution()
   {
      for (int i = _matrix.Size - 1; i >= 0; i--)
         for (int j = _matrix._ia[i + 1] - 1; j >= _matrix._ia[i]; j--)
            _solution[_matrix._ja[j]] -= _matrix._au[j] * _solution[i];
   }

   public void PrintSolution(string format = "e14")
   {
      for (int i = 0; i < _solution.Size; i++)
         Console.WriteLine(_solution[i]);
   }
}

public class BCG : Solver
{
   public BCG(double eps = 1e-14, int maxIters = 2000)
   {
      Eps = eps;
      MaxIters = maxIters;
   }


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