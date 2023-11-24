using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
public interface IListGenres : IRequestHandler<ListGenresInput, ListGenresOutPut>
{
}
