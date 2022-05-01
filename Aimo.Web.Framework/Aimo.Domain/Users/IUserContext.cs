using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Users
{
    public interface IUserContext
    {
        Task<User?> GetCurrentUserAsync(bool ensureNotNull = false);

        Task SetCurrentUserAsync(User? user = null);
    }
}
