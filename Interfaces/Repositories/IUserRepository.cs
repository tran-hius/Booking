namespace Booking.Interfaces.Repositories;
using Booking.Entities.User.User;

public interface IUserRepository
{
     Task<User> Create(User user);
     Task<User?> GetById(int id);
     Task<User?> GetByEmail(string email);
     Task Update(User user);
     Task<IEnumerable<User>> GetAll();
     Task SoftDelete(int id);
     Task Restore(int id);
     Task<bool> ExistsByEmail(string email);
}
