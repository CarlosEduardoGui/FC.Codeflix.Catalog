using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.Api.ApiModels.Response;

public class ApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
{
    public ApiResponseList(
        int currentPage,
        int perPage,
        int total,
        IReadOnlyList<TItemData> data) : base(data)
    {
        Meta = new(currentPage, perPage, total);
    }

    public ApiResponseList(PaginatedListOuput<TItemData> paginetedListOutput) : base(paginetedListOutput.Items)
    {
        Meta = new(paginetedListOutput.CurrentPage, paginetedListOutput.PerPage, paginetedListOutput.Total);
    }

    public ApiResponseListMeta Meta { get; private set; }
}
