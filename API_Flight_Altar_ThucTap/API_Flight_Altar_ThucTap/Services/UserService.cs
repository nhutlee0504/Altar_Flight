using API_Flight_Altar.Data;
using API_Flight_Altar.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API_Flight_Altar.Services
{
    public class UserService : IUserSevice
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.users.ToListAsync();
        }

        public async Task<string> LoginAsync(UserLoginDto userLoginDto)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null || !VerifyPassword(userLoginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
            }

            return GenerateJwtToken(user);
        }

        public async Task<User> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            var userTim = await _context.users.FirstOrDefaultAsync(u => u.Email == userRegisterDto.Email);
            if (userTim != null)
            {
                throw new InvalidOperationException("Email đã được sử dụng.");
            }

            var user = new User
            {
                Email = userRegisterDto.Email,
                Password = HashPassword(userRegisterDto.Password),
                Name = userRegisterDto.Name,
                Phone = userRegisterDto.Phone,
                Role = "User",
                Status = "Active"
            };

            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Các thuộc tính và phương thức khác không thay đổi

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Tạo muối ngẫu nhiên
                byte[] salt = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                // Kết hợp mật khẩu và muối
                var passwordWithSalt = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();

                // Băm mật khẩu
                var hashedPassword = sha256.ComputeHash(passwordWithSalt);

                // Trả về mật khẩu băm và muối
                return Convert.ToBase64String(hashedPassword) + ":" + Convert.ToBase64String(salt);
            }
        }

        private bool VerifyPassword(string password, string hashedPasswordWithSalt)
        {
            var parts = hashedPasswordWithSalt.Split(':');
            if (parts.Length != 2) return false;

            var hashedPassword = Convert.FromBase64String(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);

            using (var sha256 = SHA256.Create())
            {
                var passwordWithSalt = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
                var computedHash = sha256.ComputeHash(passwordWithSalt);
                return hashedPassword.SequenceEqual(computedHash);
            }
        }
    }
}
