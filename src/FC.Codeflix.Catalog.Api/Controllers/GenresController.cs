using FC.Codeflix.Catalog.Api.ApiModels.Genre;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly IMediator _mediator;

    public GenresController(
        ILogger<GenresController> logger,
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutPut>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGenreInput(id), cancellationToken);

        return Ok(new ApiResponse<GenreModelOutPut>(result));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutPut>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteGenreInput(id), cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutPut>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateGenreInput request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { result.Id }, new ApiResponse<GenreModelOutPut>(result));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutPut>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PutById(
        [FromRoute] Guid id,
        [FromBody] UpdateGenreApiInput apiInput,
        CancellationToken cancellationToken)
    {
        var input = new UpdateGenreInput(id, apiInput.Name, apiInput.IsActive, apiInput.CategoriesIds);

        var output = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponse<GenreModelOutPut>(output));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<GenreModelOutPut>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListGenresInput();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (string.IsNullOrEmpty(search) is false) input.Search = search;
        if (string.IsNullOrEmpty(sort) is false) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var result = await _mediator.Send(input, cancellationToken);

        var response = new ApiResponseList<GenreModelOutPut>(result);

        return Ok(response);
    }
}
