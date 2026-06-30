namespace Booking.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task<User?> GetById(int id);
        Task<User?> GetByEmail(string email);
        Task<IEnumerable<User>> GetAll();
        Task SoftDelete(int id);
        Task Restore(int id);
        Task<bool> ExistsByEmail(string email);
    }
}
