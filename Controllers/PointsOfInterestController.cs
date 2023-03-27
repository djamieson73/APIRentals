using API_Rentals.Models;
using API_Rentals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.JsonPatch;
using API_Rentals.Services;
using AutoMapper;

using System.Drawing;
using Entities.Models;

namespace API_Rentals.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {


        private readonly IMailService _mailService;
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        //use constructor injection to inject an ILogger of T, where T is PointsOfInterestController
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //add a null check

            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId, ICityInfoRepository _cityInfoRepository)
        //we want this to return a list points of interest for a specific city;
        //e.g. localhost:portnumer/api/cities/1/pointsofinterest
        {

            var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
             //call into the repository to get the points of interest for the city

             return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));

        }


        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        //Giving it a name so we can easily refer to it when calling into CreatedAtRoute below
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        //return a single point of interest
        //localhost:portnumer/api/cities/1/pointsofinterest/2
        {

            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
            if(!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
                // results in a 403 status code, means that the user is authenticated but doesn’t have access.
            }

                return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterestId));

        }


        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        //CREATES point of interest
        ////e.g. localhost:portnumer/api/cities/1/pointsofinterest
        //this request body will contain the pointofinterest data which we want to deserialixe to PointOfInterestForCreationDto
        //this is a complex type, so as we’re using the APIController attribute, we do not need to add the [FromBody] annotation
        {

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);
            // to get finalPointOfInterest, we map the incoming PointOfInterestForCreationDto to a pointOfInterest entity
            //we can longer add the point of interest to the city because we dono’t have a city entity at this level anymore, we replaced getting the city with a CityExistsAsync call. So, we now have to add an additional method to our repository – see below

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            //add the point of interest for a city

            await _cityInfoRepository.SaveChangesAsync();
            // then we save the changes

            var createdPointOfInterestToReturn =
            _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
            // the nice thing is that from the moment on SaveChangesAsync() has been called, our entity will have its ID filled out, auto-generated at database level.
            // So all that’s left to do is map the entity back to a DTO, and return it.
            //So, the pointOfInterestId is the createdPointOfInterestToReturn’s ID, and we include that in the response body. see here

            return CreatedAtRoute("GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = createdPointOfInterestToReturn.Id
            },
            createdPointOfInterestToReturn);
            //we also pass in the newly created point of interest, which will end up in the response body

        }







        [HttpPut("{pointOfInterestId}")]     //this is a ‘full update’ so that’s why it’s HttpPut attribute
        ////e.g. localhost:portnumer/api/cities/1/pointsofinterest/1
        //According to HTTP standard, PUT should fully update a resource - that means the consumer of the API must provide values for all fields of the resource, except ID, as that one is already coming from the URI.
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        //the content of the request will be deserialied into a PointOfInterestForUpdateDto, and we dono’t have to mark it with the FromBody attribute because it’s a complex type, and thanks to the ApiController attribute, it will automatically be deserialied from the request body.
        //as our update method returns no content eventually, it’s a Task<ActionResult> and not a task of type ActionResult<T>
        {

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            // if we pass the source object (pointOfInterest) that’s been passed in via the request body as the first parameter,
            // and we pass in the Entity (pointOfInterestEntity) which is the destination object, as the second Parameter,
            // AutoMapper will override the values in the destination object with those from the source object.
            // So, after this statement we have an up to date pointOfInterestEntity. As that pointOfInterestEntity is an entity,
            // it’s tracked by the DbContext. So, once we call SaveChangesAsync, the changes will effectively be persisted to the database.

            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();


        }
        //* the above works.But 2 things
        //If they don’t provide anything for a field, that field is updated with null
        //If they only update one field, all fields still get updated, either with the new data or the data they already had 


        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }
            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            _mailService.Send("Point of interest delete.", $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");
            return NoContent();



        }



        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        //we are using “PointOfInterestForUpdateDto” because it already has validation, and it doesn’t have an ID because we don’t ever want to update ID
        //this accepts a patchDocument. It’s on the DTO and not directly on the entity, as we shouldn’t expose entity implementation details to the outer-facing layer.
        //So, we will have to find our entity first, and then map it to a DTO before applying the patchDocument. This results in async calls into our database, so we add async above
        {


            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
            // we have to map the entity to a DTO (PointOfInterestForUpdateDto), because that’s what the patchDocument works on
            //we pass through the entity, and we map it to a PointOfInterestForUpdateDto
            //we haven’t created a map for that yet, so we need to add a CreateMap statement to our profile

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }
            //check if PointOfInterestForUpdateDto is still valid after applying the patchDocument

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            //  now we can map the changes into the entity. For that we can use the _mapper.Map statement in which we passed in the source object, which is the patch DTO and the destination object, which is the entity.
            //this results in the PointOfInterest entity to contain the changes that were applied to the PointOfInterestToPatchDto
            // save changes on  repository
            // pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            // pointOfInterestFromStore.Description =pointOfInterestToPatch.Description;
            //no longer need the above mapping code because of the .Map line above
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
    }




}

}





