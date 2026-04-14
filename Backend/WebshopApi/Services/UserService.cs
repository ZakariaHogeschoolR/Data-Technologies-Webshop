using System.Threading.Tasks;
using models;
using DataTransferObject;

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

        public async Task CreateService(UserDto user)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _userRepository.AddUser(user with { Password = hashedPassword });
        }

        public async Task UpdateService(UserDto user)
        {
            await _userRepository.UpdateUser(user);
        }

        public async Task ResetPasswordService(int id, string newPassword)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdatePassword(id, hashedPassword);
        }

        public async Task DeleteService(int id)
        {
            await _userRepository.DeleteUser(id);
        }

        public async Task RegisterService(Register data)
        {
            await CreateService(new UserDto(null, data.FirstName, data.LastName, data.Username, data.Email,
                data.Password, data.Address, data.PostCode));
        }

        public async Task<Users?> LoginService(Login data)
        {
            var user = await _userRepository.GetUserByEmail(data.Email);
            if (user == null) return null;
            
            return BCrypt.Net.BCrypt.Verify(data.Password, user.Password) ? user : null;
        }
    }
}