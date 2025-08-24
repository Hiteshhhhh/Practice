using Models;

namespace Repository
{
    public interface IUserRepository
    {
        UserModel? ValidateUser(string username, string password);
        bool RegisterUser(UserModel user);
    }
}