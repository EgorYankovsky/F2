namespace FEM_PR2;

public class SparseMatrix
{
   public int[] _ia { get; set; }
   public int[] _ja { get; set; }
   public double[] _di { get; set; }
   public double[] _al { get; set; }
   public double[] _au { get; set; }
   public int Size { get; init; }

   public SparseMatrix(int size, int elemsCount)
   {
      Size = size;
      _ia = new int[size + 1];
      _ja = new int[elemsCount];
      _al = new double[elemsCount];
      _au = new double[elemsCount];
      _di = new double[size];
   }

   public static Vector operator *(SparseMatrix matrix, Vector vector)
   {
      Vector product = new(vector.Size);

      for (int i = 0; i < vector.Size; i++)
      {
         product[i] += matrix._di[i] * vector[i];

         for (int j = matrix._ia[i]; j < matrix._ia[i + 1]; j++)
         {
            product[i] += matrix._al[j] * vector[matrix._ja[j]];
            product[matrix._ja[j]] += matrix._au[j] * vector[i];
         }
      }

      return product;
   }

   public static Vector TransposedMatrixMult(SparseMatrix matrix, Vector vector)
   {
      Vector product = new(vector.Size);

      for (int i = 0; i < vector.Size; i++)
      {
         product[i] += matrix._di[i] * vector[i];

         for (int j = matrix._ia[i]; j < matrix._ia[i + 1]; j++)
         {
            product[i] += matrix._au[j] * vector[matrix._ja[j]];
            product[matrix._ja[j]] += matrix._al[j] * vector[i];
         }
      }

      return product;
   }

   public void Clear()
   {
      for (int i = 0; i < Size; i++)
      {
         _di[i] = 0.0;

         for (int k = _ia[i]; k < _ia[i + 1]; k++)
         {
            _al[k] = 0.0;
            _au[k] = 0.0;
         }
      }
   }
}