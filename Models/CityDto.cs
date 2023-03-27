namespace API_Rentals.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }



        public int NumberOfPointsOfInterest //Calculated field
	    {
            get
            {
            	return PointsOfInterest.Count;
            }
	    }


        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();
	    // we are extending the CityDto class so it includes a collection of points of interest.


    }
}
