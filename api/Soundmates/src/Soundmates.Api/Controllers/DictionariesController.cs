using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Application.Dictionaries.Queries.GetBandRoles;
using Soundmates.Application.Dictionaries.Queries.GetCities;
using Soundmates.Application.Dictionaries.Queries.GetCountries;
using Soundmates.Application.Dictionaries.Queries.GetGenders;
using Soundmates.Application.Dictionaries.Queries.GetTagCategories;
using Soundmates.Application.Dictionaries.Queries.GetTags;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Api.Controllers;

[Route("dictionaries")]
[ApiController]
public class DictionariesController(
    IMediator _mediator
) : ControllerBase
{
    // GET /dictionaries/countries
    [HttpGet("countries")]
    public async Task<ActionResult<List<CountryDto>>> GetCountries()
    {
        var query = new GetCountriesQuery();

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /dictionaries/cities/{countryId}
    [HttpGet("cities/{countryId}")]
    public async Task<ActionResult<List<CityDto>>> GetCities(Guid countryID)
    {
        var query = new GetCitiesQuery(
            CountryId: countryID);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /dictionaries/band-roles
    [HttpGet("band-roles")]
    public async Task<ActionResult<List<BandRoleDto>>> GetBandRoles()
    {
        var query = new GetBandRolesQuery();

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /dictionaries/tags
    [HttpGet("tags")]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        var query = new GetTagsQuery();

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /dictionaries/tag-categories
    [HttpGet("tag-categories")]
    public async Task<ActionResult<List<TagCategoryDto>>> GetTagCategories()
    {
        var query = new GetTagCategoriesQuery();

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /dictionaries/genders
    [HttpGet("genders")]
    public async Task<ActionResult<List<GenderDto>>> GetGenders()
    {
        var query = new GetGendersQuery();

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }
}
