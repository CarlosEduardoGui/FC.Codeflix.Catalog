namespace FC.Codeflix.Catalog.Application.Exceptions;
public class RelatedAggregateException : Exception
{
    public RelatedAggregateException(string message) : base(message)
    {
    }
}
