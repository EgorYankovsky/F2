using System.Globalization;

using FEM_PR2;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

const string ELEMENTS = "BinaryConvert/elemsPr.txt";
const string POINTS = "BinaryConvert/coords.txt";
const string BOUNDARY_CONDITIONS = "BinaryConvert/firstConds.txt";


const string ELEMS_OUT = "Mesh/elements.txt";
const string POINTS_OUT = "Mesh/points.txt";
const string SOLUTION_OUT = "Mesh/q.txt";


var mesh = new Mesh();
mesh.ReadMesh(ELEMENTS, POINTS, BOUNDARY_CONDITIONS);

var fem = new FEM(false);
fem.SetMesh(mesh);
fem.SetSolver(new BCG(1e-14, 3000));
//fem.SetSolver(new LU());

fem.Compute();


Point2D[] points = 
{
   new(0.04, 0.0011),
   new(0.0474, 0.0016),
   new(0.05, 0.0006),
   new(0.0524, 0.0016),
   new(0.0589, 0.0015),
};

List<double> Bx = new();
List<double> By = new();

Console.WriteLine("Az:");

for (int i = 0; i < points.Length; i++)
{
   Console.WriteLine(@"{0:e8}" ,fem.GetSolutionAtPoint(points[i]));
}


double step = 1e-10; // Хороший для Bx
Point2D pointStepX = new(step, 0);
Point2D pointStepY = new(0, step);

Console.WriteLine("Bx:");
for (int i = 0; i < points.Length; i++)
{
   var retert = fem.GetSolutionAtPoint(points[i] + pointStepY);
   var opopop = fem.GetSolutionAtPoint(points[i]);

   var asfasfdsf = (retert - opopop);
   Bx.Add((retert - opopop) / (step));
   Console.WriteLine(@"{0:e8}", Bx.Last());
}


Console.WriteLine("By:");
for (int i = 0; i < points.Length; i++)
{
   var safasfgas = fem.GetSolutionAtPoint(points[i]);
   var kjljkllkj = fem.GetSolutionAtPoint(points[i] - pointStepX);

   By.Add(-((safasfgas - kjljkllkj) / (step)));
   Console.WriteLine(@"{0:e8}", By.Last());
}


Console.WriteLine("|B|:");
for (int i = 0; i < points.Length; i++)
{
   Console.WriteLine(@"{0:e8}", Math.Sqrt(Bx[i] * Bx[i] + By[i] * By[i]));
}

fem.ToPythonMeshVisualization(ELEMS_OUT, POINTS_OUT, SOLUTION_OUT);