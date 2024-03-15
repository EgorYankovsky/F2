namespace FEM_PR2;

public class Matrix
{
   private double[,] _container;
   public int Size { get; }

   public double this[int i, int j]
   {
      get => _container[i, j];
      set => _container[i, j] = value;
   }

   public Matrix(int size)
   {
      _container = new double[size, size];
      Size = size;
   }

   public void Clear() => Array.Clear(_container, 0, _container.Length);

   public static Matrix operator *(double coef, Matrix Matrix)
   {
      Matrix resultMatrix = new(Matrix.Size);

      for (int i = 0; i < resultMatrix.Size; i++)
         for (int j = 0; j < resultMatrix.Size; j++)
            resultMatrix[i, j] = coef * Matrix[i, j];

      return resultMatrix;
   }
}
