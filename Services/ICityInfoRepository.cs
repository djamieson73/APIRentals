
using API_Rentals.Models;
using Entities.Models;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace API_Rentals.Services
{
	public interface ICityInfoRepository
	{

		Task<IEnumerable<City>> GetCitiesAsync();
		//this method gets all cities asynchronously

		Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
		//this method gets one city
		//now we can choose whether to include the points of interest when returning a single city, the bool is for letting the user say they want points of interest returned 

		Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
		//return points of interest for a city

		Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
		//return point of interest for a city

		Task<bool> CityExistsAsync(int cityId);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

		Task<bool> SaveChangesAsync();

		void DeletePointOfInterest(PointOfInterest pointOfInterest);

		Task<bool> CityNameMatchesCityId(string? cityName, int cityId);

		Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSixe);




    }
}