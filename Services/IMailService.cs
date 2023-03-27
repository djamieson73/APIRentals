namespace API_Rentals.Services
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}