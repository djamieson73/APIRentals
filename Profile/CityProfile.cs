
using API_Rentals.Models;
using AutoMapper;
using Entities.Models;

namespace API_Rentals.Profile
{
    public class CityProfile : AutoMapper.Profile
    {
        public CityProfile()
        //to add a mapping configuration we use constructor

        {

            CreateMap<List<CityWithoutPointsOfInterestDto>, List<City>>();
             
            CreateMap<List<City>, List<CityWithoutPointsOfInterestDto>>();
               
            CreateMap<City, CityWithoutPointsOfInterestDto>();
	        //source is city entity, destination is citywithoutpointsofinterestdto

            CreateMap<City, CityDto>();
            //create map from city entity to city dto
        }

}
}
