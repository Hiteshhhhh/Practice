using Models;

namespace Repository
{
    public interface IUserRepository
    {
        UserModel? ValidateUser(string username, string password);
        bool RegisterUser(UserModel user);
        bool IsUsernameExists(string username);
        bool IsEmailExists(string email);
        UserModel? GetUserById(int id);
        List<UserModel> GetAllUsers();
    }
}