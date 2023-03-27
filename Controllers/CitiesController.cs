using API_Rentals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using API_Rentals.Services;


using System.Text.Json;
using Entities.Models;
using ExpressMapper.Extensions;
using ExpressMapper;

namespace API_Rentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase //don't need 'Controller' here, we aren't using Views, this is an API
    {
        const int maxCitiesPageSixe = 20;

        private readonly ICityInfoRepository _cityInfoRepository;

	    public CitiesController(ICityInfoRepository cityInfoRepository)
	    {
            _cityInfoRepository = cityInfoRepository;

        }



        [HttpGet]
	    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>>GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSixe = 10)
        //returns all cities
        //its nullable?, if we don’t do that the framework will not allow not passing through a name, as that would result in a null value for the name string.
        //eg. localhost:7047/api/cities/
        {

            if(pageSixe > maxCitiesPageSixe)
            {
                pageSixe = maxCitiesPageSixe;
            }


            var cityEntities = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSixe);
                //the following code was replaced after we implemented the mapper code below
                    //var results = new List<CityWithoutPointsOfInterestDto>();
                    //foreach (var cityEntity in cityEntities)
                    //{
                    //    results.Add(new CityWithoutPointsOfInterestDto
                    //    {
                    //        Id = cityEntity.Id,
                    //        Description = cityEntity.Description,
                    //        Name = cityEntity.Name
                    //    });
                    //}
                    //return Ok(results);

            var paginationMetadata = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSixe);
            var cities = cityEntities.Item1.ToList();

            //  Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var reMapped = Mapper.Map<List<City>, List<CityWithoutPointsOfInterestDto>>(cities);
            return Ok(reMapped);

        }


        [HttpGet("{id}")] //returns a single city; id is bound to api/cities thanks to the Route
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        //IActionResult = multiple return types (CityDto and CityWithoutPointsOfInterestDto)
        //eg. localhost:7047/api/cities/2
        {

            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            //then, asynchronously call into the repository to get back the city with or without points of interest

            if (city == null)
            {
                return NotFound();
            }
            
            if(includePointsOfInterest)
            {
                return Ok(Mapper.Map<City, CityDto>(city));
            }
            //we can now use AutoMapper (since we added mapping in CityProfile.cs)
            //So, if points of interest have to be included we map to CityDto, if they don’t, we map to CithWithoutPointsOfInterestDto – see below

            return Ok(Mapper.Map<City, CityWithoutPointsOfInterestDto>(city));
 
        }

    }

}
