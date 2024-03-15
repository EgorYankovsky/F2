namespace FEM_PR2;

public interface IBasis2D
{
    int Size { get; }

    double GetPsi(int number, Point2D point);

    double GetDPsi(int number, int varNumber, Point2D point);
}

public interface IBasis1D
{
    int Size { get; }

    double GetPsi(int number, double point, double h = 1.0);

    double GetDPsi(int number, double point, double h = 1.0);

    double GetDdPsi(int number, double point, double h = 1.0);
}

public readonly record struct LinearBasis : IBasis2D
{
    public int Size => 4;

    public double GetPsi(int number, Point2D point)
        => number switch
        {
            0 => (1.0 - point.X) * (1.0 - point.Y),
            1 => point.X * (1.0 - point.Y),
            2 => (1.0 - point.X) * point.Y,
            3 => point.X * point.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(number), number, "Not expected function number!")
        };

    public double GetDPsi(int number, int varNumber, Point2D point)
        => varNumber switch
        {
            0 => number switch
            {
                0 => point.Y - 1.0,
                1 => 1.0 - point.Y,
                2 => -point.Y,
                3 => point.Y,
                _ => throw new ArgumentOutOfRangeException(nameof(number), number, "Not expected function number!")
            },
            1 => number switch
            {
                0 => point.X - 1.0,
                1 => -point.X,
                2 => 1.0 - point.X,
                3 => point.X,
                _ => throw new ArgumentOutOfRangeException(nameof(number), number, "Not expected function number!")
            },
            _ => throw new ArgumentOutOfRangeException(nameof(varNumber), varNumber, "Not expected var number!")
        };
}