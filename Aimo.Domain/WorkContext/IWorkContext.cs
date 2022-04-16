using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.WorkContext
{
    public interface IWorkContext
    {
        Task<User?> GetCurrentUserAsync(bool ensureNotNull = false);

        Task SetCurrentUserAsync(User? user = null);
    }
}
