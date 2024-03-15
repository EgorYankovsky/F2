using System.Collections.Immutable;

namespace FEM_PR2;

public class Mesh
{
   private double _timeStart;
   private double _timeEnd;
   private int _timeSplits;
   private double _timeDischarge;
   private List<double> _timeLayers;

   private double[] _linesX;
   private double[] _linesY;
   private List<double> _meshLinesX;
   private List<double> _meshLinesY;
   private int[] _splitsX;
   private int[] _splitsY;
   private double[] _dischargeX;
   private double[] _dischargeY;
   private (int, int, int, int, int)[] _areas;
   private List<int[]> _boundaryConditions;

   private List<List<int>> _allRibs;

   private List<Point2D> _points;
   private List<int[]> _elements;
   private IDictionary<int, int> _areaNodes;
   private HashSet<int> _boundaryNodes1;
   private HashSet<int> _boundaryNodes2;
   private List<List<int>> _boundaryRibs2;
   private HashSet<int> _fictiveNodes;
   private List<int> _elementMaterials;

   public int NodesCount => _points.Count;
   public int ElementsCount => _elements.Count;
   public ImmutableList<Point2D> Points => _points.ToImmutableList();
   public ImmutableList<int[]> Elements => _elements.ToImmutableList();
   public ImmutableHashSet<int> BoundaryNodes1 => _boundaryNodes1.ToImmutableHashSet();
   public ImmutableList<List<int>>  BoundaryRibs2 => _boundaryRibs2.ToImmutableList();
   public ImmutableList<int> ElementMaterials => _elementMaterials.ToImmutableList();

   public Mesh()
   {
      _allRibs = new();
      _boundaryRibs2 = new();
      _areaNodes = new Dictionary<int, int>();
      _boundaryConditions = new();
      _boundaryNodes1 = new();
      _boundaryNodes2 = new();
      _fictiveNodes = new();

      _timeLayers = new();
      _linesX = Array.Empty<double>();
      _linesY = Array.Empty<double>();
      _meshLinesX = new();
      _meshLinesY = new();
      _points = new();
      _splitsX = Array.Empty<int>();
      _splitsY = Array.Empty<int>();
      _dischargeX = Array.Empty<double>();
      _dischargeY = Array.Empty<double>();
      _areas = Array.Empty<(int, int, int, int, int)>();
      _elements = new();
      _elementMaterials = new();
   }
    
   public void ReadMesh(string elementsPath, string pointsPath, string boundariesPath)
   {
      try
      {
         using (var sr = new StreamReader(elementsPath))
         {
            int elementsCount = int.Parse(sr.ReadLine() ?? "0");

            for (int i = 0; i < elementsCount; i++)
            {
               var line = sr.ReadLine().Split();
               var element = new List<int>();

               for (int j = 0; j < line.Length - 1; j++)
                    element.Add(int.Parse(line[j]) - 1);
               
               _elements.Add(element.Order().ToArray());
               _elementMaterials.Add(int.Parse(line.Last()) - 1);
            }
         }

         using (var sr = new StreamReader(pointsPath))
         {
            int pointsCount = int.Parse(sr.ReadLine() ?? "0");

            for (int i = 0; i < pointsCount; i++)
            {
               var point = Point2D.Parse(sr.ReadLine()!);
               _points.Add(point);
            }
         }

         using (var sr = new StreamReader(boundariesPath))
         {
            int boundaryNodesCount = int.Parse(sr.ReadLine() ?? "0");

            for (int i = 0; i < boundaryNodesCount; i++)
            {
               var node1 = int.Parse(sr.ReadLine()!) - 1;
               _boundaryNodes1.Add(node1);
            }
         }
      }
      catch (Exception ex)
      {
         Console.WriteLine(ex.Message);
      }
   }
}