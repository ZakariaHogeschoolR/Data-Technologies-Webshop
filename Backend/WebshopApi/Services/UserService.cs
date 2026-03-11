using System.Threading.Tasks;
using models;

namespace Service
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<Users>> GetAllService()
        {
            Task<List<Users?>> users = _userRepository.GetAllUsers();
            return await users;
        }

        public async Task<Users> GetByIdService(int id)
        {
            Task<Users?> user = _userRepository.GetUserById(id);
            return await user;
        }

        public void CreateService(Users user)
        {
            _userRepository.AddUser(user);
        }

        public void UpdateService(Users user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteService(int id)
        {
            _userRepository.DeleteUser(id);
        }
    }
    
}