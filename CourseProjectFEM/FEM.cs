namespace FEM_PR2;

public class FEM
{
   private SparseMatrix _globalMatrix;
   private Vector _globalVector;
   private Vector _solution;

   private Mesh _mesh;
   private Solver _solver;
   private Vector _localVector;
   private Matrix _localStiffness;

   private bool _isNonLinear;

   public static int NodesPerElement => 4;

   public FEM(bool isNonLinear)
   {
      _localStiffness = new(NodesPerElement);
      _localVector = new(NodesPerElement);

      _solver = new BCG();
      _mesh = new Mesh();

      _globalMatrix = new SparseMatrix(0, 0);
      _globalVector = new(0);
      _solution = new(0);

      _isNonLinear = isNonLinear;
   }

   public void SetSolver(Solver solver) => _solver = solver;
   public void SetMesh(Mesh mesh) => _mesh = mesh;


   public void Compute()
   {
      if (_mesh is null)
      {
         Console.WriteLine("Set the mesh");
         return;
      }

      if (_solver is null)
      {
         Console.WriteLine("Set the solver");
         return;
      }

      BuildPortrait();

      AssemblySLAE();
      AccountSecondConditions();
      AccountFirstConditions();

      _solver.SetSLAE(_globalVector, _globalMatrix);
      _solution = _solver.Solve();
   }

   public double GetSolutionAtPoint(Point2D point)
   {
      int ielem = FindElementByPoint(point);

      if (ielem < 0)
         return 0;

      double solution = 0;

      var leftBottom = _mesh.Points[_mesh.Elements[ielem][0]];
      var rightBottom = _mesh.Points[_mesh.Elements[ielem][1]];
      var leftUpper = _mesh.Points[_mesh.Elements[ielem][2]];
      var rightUpper = _mesh.Points[_mesh.Elements[ielem][3]];

      var hx = new Interval(leftBottom.X, rightBottom.X);
      var hy = new Interval(leftBottom.Y, leftUpper.Y);

      solution += _solution[_mesh.Elements[ielem][0]] * (hx.RightBoundary - point.X) / hx.Length * (hy.RightBoundary - point.Y) / hy.Length;
      solution += _solution[_mesh.Elements[ielem][1]] * (point.X - hx.LeftBoundary) / hx.Length * (hy.RightBoundary - point.Y) / hy.Length;
      solution += _solution[_mesh.Elements[ielem][2]] * (hx.RightBoundary - point.X) / hx.Length * (point.Y - hy.LeftBoundary) / hy.Length;
      solution += _solution[_mesh.Elements[ielem][3]] * (point.X - hx.LeftBoundary) / hx.Length * (point.Y - hy.LeftBoundary) / hy.Length;

      return solution;
   }

   public void BuildPortrait()
   {
      var list = new HashSet<int>[_mesh.NodesCount].Select(_ => new HashSet<int>()).ToList();
      foreach (var element in _mesh.Elements)
         foreach (var position in element)
            foreach (var node in element)
               if (position > node)
                  list[position].Add(node);

      int offDiagonalElementsCount = list.Sum(childList => childList.Count);

      _globalMatrix = new(_mesh.NodesCount, offDiagonalElementsCount);
      _globalVector = new(_mesh.NodesCount);

      _globalMatrix._ia[0] = 0;

      for (int i = 0; i < list.Count; i++)
         _globalMatrix._ia[i + 1] = _globalMatrix._ia[i] + list[i].Count;

      int k = 0;
      foreach (var childList in list)
         foreach (var value in childList.Order())
            _globalMatrix._ja[k++] = value;
   }

   private void AssemblySLAE()
   {
      _globalVector.Fill(0);
      _globalMatrix.Clear();

      for (int ielem = 0; ielem < _mesh.ElementsCount; ielem++)
      {
         AssemblyLocalSLAE(ielem);
         AddLocalMatrixToGlobal(ielem);
         AddLocalVectorToGlobal(ielem);

         _localStiffness.Clear();
         _localVector.Clear();
      }

      Array.Copy(_globalMatrix._al, _globalMatrix._au, _globalMatrix._al.Length);
   }

   private void AssemblyLocalSLAE(int ielem)
   {
      int material = _mesh.ElementMaterials[ielem];

      double hx = Math.Abs(_mesh.Points[_mesh.Elements[ielem][3]].X - _mesh.Points[_mesh.Elements[ielem][0]].X);
      double hy = Math.Abs(_mesh.Points[_mesh.Elements[ielem][3]].Y - _mesh.Points[_mesh.Elements[ielem][0]].Y);

      double coeffG1 = hy / hx / 6;
      double[,] matrixG1 =
      {
         { 2.0, -2.0, 1.0, -1.0 },
         { -2.0, 2.0, -1.0, 1.0 },
         { 1.0, -1.0, 2.0, -2.0 },
         { -1.0, 1.0, -2.0, 2.0 }
      };

      double coeffG2 = hx / hy / 6;
      double[,] matrixG2 =
      {
         { 2.0, 1.0, -2.0, -1.0 },
         { 1.0, 2.0, -1.0, -2.0 },
         { -2.0, -1.0, 2.0, 1.0 },
         { -1.0, -2.0, 1.0, 2.0 }
      };

      double coeffM = hx * hy / 36;
      double[,] matrixM =
      {
         { 4.0, 2.0, 2.0, 1.0 },
         { 2.0, 4.0, 1.0, 2.0 },
         { 2.0, 1.0, 4.0, 2.0 },
         { 1.0, 2.0, 2.0, 4.0 }
      };


      var _integrator = new Integration(Quadratures.SegmentGaussOrder5());
      var templateElement = new Rectangle(new(0.0, 0.0), new(1.0, 1.0));
      //var templateElement = new Rectangle(_mesh.Points[_mesh.Elements[ielem][0]], _mesh.Points[_mesh.Elements[ielem][3]]);
      var basis = new LinearBasis();

      var _tempMass = new Matrix(NodesPerElement);
      for (int i = 0; i < NodesPerElement; i++)
      {
         for (int j = 0; j < NodesPerElement; j++)
         {
            Func<Point2D, double> gradPsiGradPsi;
            gradPsiGradPsi = p =>
            {
               var dFi1 = basis.GetDPsi(i, 0, p);
               var dFi2 = basis.GetDPsi(j, 0, p);

               var dFi3 = basis.GetDPsi(i, 1, p);
               var dFi4 = basis.GetDPsi(j, 1, p);

               return 1.0 / Parameters.Mu(material, p, _isNonLinear) * (hy / hx * dFi1 * dFi2 + hx / hy * dFi3 * dFi4);
            };

            // В матрицу жёсткости запишу всю локальную А.
            _localStiffness[i, j] = 1.0 / Parameters.Mu(material) * (coeffG1 * matrixG1[i, j] + coeffG2 * matrixG2[i, j]);
            _tempMass[i, j] = matrixM[i, j];
         }
      }

      for (int i = 0; i < NodesPerElement; i++)
         _localVector[i] = Parameters.F(material, _mesh.Points[_mesh.Elements[ielem][i]]);

      // Вектор правой части b.
      _localVector = coeffM * _tempMass * _localVector;
   }

   private void AddLocalMatrixToGlobal(int ielem)
   {
      for (int i = 0; i < NodesPerElement; i++)
      {
         for (int j = 0; j < NodesPerElement; j++)
         {
            if (_mesh.Elements[ielem][i] == _mesh.Elements[ielem][j])
            {
               _globalMatrix._di[_mesh.Elements[ielem][i]] += _localStiffness[i, j];
               continue;
            }

            if (_mesh.Elements[ielem][i] > _mesh.Elements[ielem][j])
               for (int icol = _globalMatrix._ia[_mesh.Elements[ielem][i]]; icol < _globalMatrix._ia[_mesh.Elements[ielem][i] + 1]; icol++)
                  if (_globalMatrix._ja[icol] == _mesh.Elements[ielem][j])
                  {
                     _globalMatrix._al[icol] += _localStiffness[i, j];
                     break;
                  }
         }
      }
   }

   private void AddLocalVectorToGlobal(int ielem)
   {
      for (int i = 0; i < NodesPerElement; i++)
         _globalVector[_mesh.Elements[ielem][i]] += _localVector[i];
   }

   private void AccountSecondConditions()
   {
      for (int i = 0; i < _mesh.BoundaryRibs2.Count; i++)
         for (int j = 0; j < _mesh.BoundaryRibs2[i].Count; j++)
         {
            double h = Math.Max
               (
               Math.Abs(_mesh.Points[i].X - _mesh.Points[_mesh.BoundaryRibs2[i][j]].X),
               Math.Abs(_mesh.Points[i].Y - _mesh.Points[_mesh.BoundaryRibs2[i][j]].Y)
               );

            double Theta1 = Parameters.dU_dn(_mesh.Points[i]);
            double Theta2 = Parameters.dU_dn(_mesh.Points[_mesh.BoundaryRibs2[i][j]]);

            _globalVector[i] += h / 6.0 * (2.0 * Theta1 + Theta2);
            _globalVector[_mesh.BoundaryRibs2[i][j]] += h / 6.0 * (Theta1 + 2.0 * Theta2);
         }
   }

   private void AccountFirstConditions()
   {
      foreach (var node in _mesh.BoundaryNodes1)
      {
         int row = node;

         // На диагонали единица.
         _globalMatrix._di[row] = 1;
         
         // В векторе правой части значение краевого.
         _globalVector[row] = Parameters.Ug1(_mesh.Points[node]);

         // Вся остальная строка 0. 
         for (int i = _globalMatrix._ia[row]; i < _globalMatrix._ia[row + 1]; i++)
            _globalMatrix._al[i] = 0;

         for (int col = row + 1; col < _globalMatrix.Size; col++)
            for (int j = _globalMatrix._ia[col]; j < _globalMatrix._ia[col + 1]; j++)
               if (_globalMatrix._ja[j] == row)
               {
                  _globalMatrix._au[j] = 0;
                  break;
               }
      }
   }

   private int FindElementByPoint(Point2D point)
   {
      for (int i = 0; i < _mesh.Elements.Count; i++)
      {
         var leftBottom = _mesh.Points[_mesh.Elements[i][0]];
         var rightUpper = _mesh.Points[_mesh.Elements[i][3]];

         if (leftBottom.X <= point.X && point.X <= rightUpper.X)
            if (leftBottom.Y <= point.Y && point.Y <= rightUpper.Y)
               return i;
      }
      return -1;
   }


   public void ToPythonMeshVisualization(string elementsPath, string pointsPath, string solutionPath)
   {
      try
      {
         using (var sw = new StreamWriter(pointsPath))
         {
            foreach (var point in _mesh.Points)
               sw.WriteLine(@"{0:e14} {1:e14}", point.X, point.Y);
         }

         using (var sw = new StreamWriter(elementsPath))
         {
            for (int i = 0; i < _mesh.Elements.Count; i++)
            {
               for (int j = 0; j < NodesPerElement; j++)
                  sw.Write($"{_mesh.Elements[i][j]} ");
               sw.WriteLine($"{_mesh.ElementMaterials[i]}");
            }

         }

         using (var sw = new StreamWriter(solutionPath))
         {
            for (int i = 0; i < _solution.Size; i++)
               sw.WriteLine(@"{0:e14}", _solution[i]);
         }
      }
      catch (Exception ex)
      {
         Console.WriteLine(ex.Message);
      }
   }
}
