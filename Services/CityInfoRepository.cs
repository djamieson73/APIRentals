using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;



namespace API_Rentals.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly EntitiesDbContext _context;
	    public CityInfoRepository(EntitiesDbContext context)
	    {
            _context = context ?? throw new ArgumentNullException(nameof(context));
	    }
	    public async Task<IEnumerable<City>> GetCitiesAsync()
	    //returns all cities
	    {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();

           
	    }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
	    //returns a city, and points of interest if desired (bool)
        {
            if(includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointOfInterests)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities
            .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
	    //return a point of interest for city, which means we match both cityid and pointofinterestid
        {
           // return await _context
            return await _context.PointOfInterests
            .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
            .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>>GetPointsOfInterestForCityAsync(int cityId)
	    //get all points of interest for a city which only matches cityid
        {
            return await _context.PointOfInterests
            .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
	    {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<bool> SaveChangesAsync()
	    {
            return (await _context.SaveChangesAsync() >= 0);
            //we want it to be true when 0 or more entities have been saved, in reality we would probably have 1 instead of 0
        }


        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
	        // all we need to do to implement the delete operation is call Remove on the PointsOfInterest DbSet, passing through the pointOfInterest we want to remove
        }


        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSixe)
        {


            var collection = _context.Cities as IQueryable<City>;
            // we may want to search, filter, or both. So first thing we do is cast the city’s DbSet to an IQueryable<City>.  We can work on the collection adding where clauses for filtering when needed and for searching.

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }
            // add filter

            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) || (a.Description != null && a.Description.Contains(searchQuery)));
            }
            // add search


            var totalItemCount = await collection.CountAsync();
              // this is a database call, but we need to execute it to get the total amount of items
            
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSixe, pageNumber);

            var skipcount = pageNumber == 1 ? 0 : pageSixe * (pageNumber - 1);

            int newpageNumber = pageNumber - 1;
            var collectionToReturn = await collection.OrderBy(c => c.Name)
            .Skip(skipcount)
            .Take(pageSixe)
            .ToListAsync();

            
            return (collectionToReturn, paginationMetadata);

        }


        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointOfInterests.Add(pointOfInterest);
            }
            
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }


    }
}
