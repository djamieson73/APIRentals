using AutoMapper;

using API_Rentals.Models;
using Entities.Models;

namespace API_Rentals.Profile
{
    public class PointOfInterestProfile : AutoMapper.Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<PointOfInterest, PointOfInterestDto>();
            //creating a map from PointOfInterest to PointOfInterestDto

            CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
            //have to add because pointofinterestcontroller.cs CreatePointofInterest method accepts a PointOfInterestForCreationDto and we must map PointOfInterestForCreationDto to a pointOfInterest entity.

            CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();

            CreateMap<PointOfInterest, PointOfInterestForUpdateDto>();

        }
    }
}

