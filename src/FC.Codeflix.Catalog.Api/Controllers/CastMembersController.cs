using FC.Codeflix.Catalog.Api.ApiModels.CastMember;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("cast_members")]
public class CastMembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CastMembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { result.Id }, new ApiResponse<CastMemberModelOutput>(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCastMemberInput(id), cancellationToken);

        return Ok(new ApiResponse<CastMemberModelOutput>(result));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCastMemberInput(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PutById(
        [FromRoute] Guid id,
        [FromBody] UpdateCastMemberApiInput apiInput,
        CancellationToken cancellationToken)
    {
        var input = new UpdateCastMemberInput(id, apiInput.Name, apiInput.Type);

        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponse<CastMemberModelOutput>(output));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<CastMemberModelOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListCastMembersInput();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (string.IsNullOrEmpty(search) is false) input.Search = search;
        if (string.IsNullOrEmpty(sort) is false) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var result = await _mediator.Send(input, cancellationToken);

        var response = new ApiResponseList<CastMemberModelOutput>(result);

        return Ok(response);
    }
}
