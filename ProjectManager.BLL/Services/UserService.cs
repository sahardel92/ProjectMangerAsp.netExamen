using ProjectManager.DAL.Models;
using ProjectManager.DAL.Repositories;

namespace ProjectManager.BLL.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? Login(string email, string password)
        {
            return _userRepository.CheckPassword(email, password);
        }
    }
}
