namespace API_Rentals.Services
{
    public class PaginationMetadata
    {
public int TotalItemCount { get; set; }
	public int TotalPageCount { get; set; }
	public int PageSixe { get; set; }
	public int CurrentPage { get; set; }
	public PaginationMetadata(int totalItemCount, int pageSixe, int currentPage)
	{
		TotalItemCount = totalItemCount;
		PageSixe = pageSixe;
		CurrentPage = currentPage;
		TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSixe);
	}

    }
}
