using API_Flight_Altar.Data;
using API_Flight_Altar.Model;
using API_Flight_Altar_ThucTap.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace API_Flight_Altar.Services
{
    public class UserService : IUserSevice
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<User>> GetUsers() //Gọi tất cả user
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền");
            }

            var users = await _context.users.ToListAsync();
            if(users == null)
            {
                throw new NotImplementedException("Không tìm thấy người dùng nào");
            }
            return users;
        }

        public async Task<string> LoginAdmin(UserLoginDto userLoginDto)//Đăng nhập dành cho admin
        {
            ValidateEmailDomain(userLoginDto.Email); // Kiểm tra email
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null || !VerifyPassword(userLoginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
            }

            // Kiểm tra vai trò
            if (user.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập vào tài khoản Admin.");
            }

            var token = GenerateJwtToken(user);

            return token;
        }

        public async Task<string> LoginUser(UserLoginDto userLoginDto)//Đăng nhập dành cho user thường
        {
            ValidateEmailDomain(userLoginDto.Email); // Kiểm tra email
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null || !VerifyPassword(userLoginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
            }
            //Kiểm tra khóa tài khoản
            if (user.Status.ToLower().Contains("lock"))
            {
                throw new UnauthorizedAccessException("Tài khoản đã bị khóa");
            }
            var token = GenerateJwtToken(user);

            return token;
        }
        public async Task<User> RegisterAdminAsync(UserRegisterDto userRegisterDto)
        {
         
            ValidateEmailDomain(userRegisterDto.Email); // Kiểm tra email
            ValidatePassword(userRegisterDto.Password); //Kiểm tra mật khẩu
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
                Role = "Admin",
                Status = "Active"
            };

            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> RegisterAsync(UserRegisterDto userRegisterDto)//Đăng ký/ Tạo tài khoản do Admin là người tạo
        {
            ValidateEmailDomain(userRegisterDto.Email); // Kiểm tra email
            ValidatePassword(userRegisterDto.Password); //Kiểm tra mật khẩu
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền tạo tài khoản");
            }
            ValidateEmailDomain(userRegisterDto.Email); // Kiểm tra email
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
                Role = userRegisterDto.Role,
                Status = "Active"
            };

            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(string name, string phone)//Cập nhật thông tin tài khoản của người đăng nhập
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == userInfo.IdUser);
            userFind.Name = name;
            userFind.Phone = phone;
            await _context.SaveChangesAsync();
            return userFind;
        }

        public async Task<User> FindUserByEmail(string email)//Tìm kiếm user qua email
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role != "Admin" || userInfo.Role == "GO")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền");
            }
            var userFind = await _context.users.FindAsync(email);
            if(userFind == null)
            {
                throw new NotImplementedException("Không tìm thấy tài khoản người dùng");
            }
            return userFind;
        }

        public async Task<User> PermissionUser(int idUser, string role)//Phân quyền User
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền");
            }
            var userFind = await _context.users.FindAsync(idUser);
            if (userFind != null)
            {
                userFind.Role = role;
                await _context.SaveChangesAsync();
                return userFind;
            }
            throw new NotImplementedException("Không tìm thấy người dùng");
        }


        //Các phương thức ngoài

        private string GenerateJwtToken(User user)//Tạo dữ liệu token bao gồm Id,Email,Role
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

        private string HashPassword(string password)//Băm mật khẩu
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

        private bool VerifyPassword(string password, string hashedPasswordWithSalt)//Kiểm tra mật khẩu khi đăng nhập
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
        private void ValidateEmailDomain(string email)
        {
            if (!email.EndsWith("@vietjetair.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Email phải có đuôi '@vietjetair.com'.");
            }
        }
        private void ValidatePassword(string password)
        {
            var regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,16}$");
            if (!regex.IsMatch(password))
            {
                throw new InvalidOperationException("Mật khẩu không hợp lệ. Mật khẩu phải chứa ít nhất một chữ cái, một số và một ký tự đặc biệt.");
            }
        }

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()//Lấy dữ liệu thông qua token
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

                if (idClaim != null && emailClaim != null && roleClaim != null)
                {
                    return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
                }
            }
            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }

        public async Task<User> LockUser(int id)
        {
            var user = GetUserInfoFromClaims();
            // Kiểm tra vai trò
            if (user.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập vào tài khoản Admin.");
            }
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == id);
            if(userFind != null)
            {
                if (userFind.Status.ToLower().Contains("lock"))
                {
                    throw new InvalidCastException("Tài khoản đã bị khóa trước đó");
                }
                userFind.Status = "Lock";
                await _context.SaveChangesAsync();
                return userFind;
            }
            throw new NotImplementedException("Không tìm thấy người dùng");
        }

        public async Task<User> UnlockUser(int id)
        {
            var user = GetUserInfoFromClaims();
            // Kiểm tra vai trò
            if (user.Role != "Admin")
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập vào tài khoản Admin.");
            }
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == id);
            if (userFind != null)
            {
                if (userFind.Status.ToLower().Contains("active"))
                {
                    throw new InvalidCastException("Tài khoản chưa bị khóa");
                }
                userFind.Status = "Active";
                await _context.SaveChangesAsync();
                return userFind;
            }
            throw new NotImplementedException("Không tìm thấy người dùng");
        }
    }
}
