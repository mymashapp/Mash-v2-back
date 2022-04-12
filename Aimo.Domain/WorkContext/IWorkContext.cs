using Aimo.Domain.Users;

namespace Aimo.Domain.WorkContext
{
    public interface IWorkContext
    {
        Task<User> GetCurrentUserAsync(bool ensureNotNull = false);

        Task SetCurrentUserAsync(User? user = null);
    }
}
