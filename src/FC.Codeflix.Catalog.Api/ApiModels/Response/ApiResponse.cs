namespace FC.Codeflix.Catalog.Api.ApiModels.Response;

public class ApiResponse<TData>
{
    public ApiResponse(TData data)
    {
        Data = data;
    }

    public TData Data { get; private set; }
}
