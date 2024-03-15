namespace FEM_PR2;

public class Integration
{
    private readonly IEnumerable<QuadratureNode<double>> _quadratures;

    public Integration(IEnumerable<QuadratureNode<double>> quadratures) => _quadratures = quadratures;
}