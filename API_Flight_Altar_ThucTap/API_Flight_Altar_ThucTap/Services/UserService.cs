using API_Flight_Altar.Data;
using API_Flight_Altar.Model;
using API_Flight_Altar_ThucTap.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using API_Flight_Altar_ThucTap.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace API_Flight_Altar.Services
{
    public class UserService : IUserSevice
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        public UserService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<IEnumerable<User>> GetUsers() //Gọi tất cả người dùng
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var users = await _context.users.ToListAsync();
                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    if (users == null)
                    {
                        throw new NotImplementedException("No user found");
                    }
                    return users;
                }
                if (users == null)
                {
                    throw new NotImplementedException("No user found");
                }
                return users.Where(x => x.Status.Contains("Active") && x.Role != "Admin");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<string> LoginAdmin(UserLoginDto userLoginDto)//Đăng nhập dành cho admin
        {
            ValidateEmailDomain(userLoginDto.Email); // Kiểm tra email
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null || !VerifyPassword(userLoginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Incorrect email or password");
            }

            // Kiểm tra vai trò
            if (!user.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("You do not have access permission");
            }

            var token = GenerateJwtToken(user);

            return token;
        }

        public async Task<string> LoginUser(UserLoginDto userLoginDto)//Đăng nhập dành cho người dùng thường
        {
            ValidateEmailDomain(userLoginDto.Email); // Kiểm tra email
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            if (user == null || !VerifyPassword(userLoginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Incorrect email or password.");
            }
            //Kiểm tra khóa tài khoản
            if (user.Status.ToLower().Contains("lock"))
            {
                throw new UnauthorizedAccessException("The account has been locked");
            }
            var token = GenerateJwtToken(user);

            return token;
        }

        public async Task<User> RegisterAdminAsync(UserRegisterDto userRegisterDto)//tạo tài khoản admin
        {

            ValidateEmailDomain(userRegisterDto.Email);
            ValidatePassword(userRegisterDto.Password);
            var userTim = await _context.users.FirstOrDefaultAsync(u => u.Email == userRegisterDto.Email);
            if (userTim != null)
            {
                throw new InvalidOperationException("Email has already been used");
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
            ValidateEmailDomain(userRegisterDto.Email);
            ValidatePassword(userRegisterDto.Password);
            if (string.IsNullOrWhiteSpace(userRegisterDto.Name))
            {
                throw new ArgumentException("Name cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(userRegisterDto.Phone))
            {
                throw new ArgumentException("Phone cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(userRegisterDto.Role))
            {
                throw new ArgumentException("Role cannot be empty");
            }
            var phoneRegex = new Regex(@"^(?:\+84|0)([3|5|7|8|9]\d{8})$");
            if (!phoneRegex.IsMatch(userRegisterDto.Phone))
            {
                throw new ArgumentException("Phone number must be a valid Vietnamese number");
            }
            var validRoles = new[] { "Admin", "Go", "Pilot", "Crew" };
            if (!validRoles.Contains(userRegisterDto.Role))
            {
                throw new ArgumentException("Role must be one of the following: Admin, Go, Pilot, Crew");
            }
            var userInfo = GetUserInfoFromClaims();

            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("Only admin can create accounts");
            }
            ValidateEmailDomain(userRegisterDto.Email); // Kiểm tra email
            var userTim = await _context.users.FirstOrDefaultAsync(u => u.Email == userRegisterDto.Email);
            if (userTim != null)
            {
                throw new InvalidOperationException("Email has already been used");
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

        public async Task<User> UpdateUser(string name, string phone)//Chức năng cập nhật thôn tin người dùng
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.");
            }

            var phoneRegex = new Regex(@"^(0[3|5|7|8|9][0-9]{8})$"); // Đầu số: 03, 05, 07, 08, 09, theo sau là 8 chữ số
            if (!phoneRegex.IsMatch(phone))
            {
                throw new ArgumentException("Phone number is not in a valid");
            }
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == userInfo.IdUser);
            userFind.Name = name;
            userFind.Phone = phone;
            await _context.SaveChangesAsync();
            return userFind;
        }

        public async Task<IEnumerable<User>> FindUserByEmail(string email)//Chức năng tìm kiếm người dùng qua email
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var userFind = await _context.users.Where(x => x.Email.Contains(email)).ToListAsync();
                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    if (userFind == null)
                    {
                        throw new NotImplementedException("No user found");
                    }
                    return userFind;
                }
                if (userFind == null)
                {
                    throw new NotImplementedException("No user found");
                }
                return userFind.Where(x => x.Status.Contains("Active"));
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<User> PermissionUser(int idUser, string role) // Phân quyền người dùng
        {
            var userInfo = GetUserInfoFromClaims();
            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("Only admin has permission.");
            }

            var userFind = await _context.users.FindAsync(idUser);
            if (userFind != null)
            {
                if (userFind.IdUser == userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("You cannot change your own permissions.");
                }

                if (userFind.Role.ToLower().Contains("admin"))
                {
                    throw new UnauthorizedAccessException("You cannot change the role of an admin account");
                }
                if (string.IsNullOrWhiteSpace(role))
                {
                    throw new ArgumentException("Role cannot be empty.");
                }
                // Kiểm tra role phải là một trong các giá trị hợp lệ
                var validRoles = new[] { "Admin", "Go", "Pilot", "Crew" };
                if (!validRoles.Contains(role))
                {
                    throw new ArgumentException("Role must be one of the following: Admin, Go, Pilot, Crew.");
                }

                userFind.Role = role;
                await _context.SaveChangesAsync();
                return userFind;
            }

            throw new NotImplementedException("User not found.");
        }

        public async Task<User> LockUser(int id)//Chức năng khóa tài khoản
        {
            var user = GetUserInfoFromClaims();
            // Kiểm tra vai trò
            if (!user.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("Only admin has permission");
            }
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == id);
            if (userFind != null)
            {
                if (userFind.Status.ToLower().Contains("lock"))
                {
                    throw new InvalidCastException("The account has already been locked");
                }
                if (userFind.Role.ToLower().Contains("admin"))
                {
                    throw new UnauthorizedAccessException("Cannot lock an admin account");
                }
                userFind.Status = "Lock";
                await _context.SaveChangesAsync();
                return userFind;
            }
            throw new NotImplementedException("No user found");
        }

        public async Task<User> UnlockUser(int id)//Chức năng mở khóa tài khoản
        {
            var user = GetUserInfoFromClaims();
            // Kiểm tra vai trò
            if (!user.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("Only admin has permission");
            }
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == id);
            if (userFind != null)
            {
                if (userFind.Status.ToLower().Contains("active"))
                {
                    throw new InvalidCastException("The account is not locked");
                }
                userFind.Status = "Active";
                await _context.SaveChangesAsync();
                return userFind;
            }
            throw new NotImplementedException("No user found");
        }

        public async Task<User> GetMyInfo()//Chức năng lấy thông tin của bản thân người dùng
        {
            var userInfo = GetUserInfoFromClaims();
            var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == userInfo.IdUser);
            return userFind;
        }

        public async Task<User> FindUserById(int idUser) //Tìm kiếm người dùng thông qua Id
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var userFind = await _context.users.FirstOrDefaultAsync(x => x.IdUser == idUser);
                if (userFind == null)
                {
                    throw new NotImplementedException("No user found");
                }
                return userFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<User> ChangePassword(string oldPassword, string newPassword)//Chức năng thay đổi mật khẩu người dùng đang đăng nhập
        {
            var userInfo = GetUserInfoFromClaims();
            var user = await _context.users.FirstOrDefaultAsync(u => u.IdUser == userInfo.IdUser);

            if (user == null)
            {
                throw new NotImplementedException("User not found.");
            }

            if (!VerifyPassword(oldPassword, user.Password))
            {
                throw new UnauthorizedAccessException("Old password is incorrect.");
            }
            ValidatePassword(newPassword);
            user.Password = HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> ChangePasswordUser(int idUser, string newPassword)//Chức năng thay đổi mật khảu của người dùng dành cho Admin
        {
            var userInfo = GetUserInfoFromClaims();

            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("Only admin has permission to change other users' passwords.");
            }

            var user = await _context.users.FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (user == null)
            {
                throw new NotImplementedException("User not found.");
            }
            ValidatePassword(newPassword);
            user.Password = HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<string> ForgotPassword(string email) //Chức năng quên mật khẩu
        {
            ValidateEmailDomain(email);
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new InvalidOperationException("Email does not exist.");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set(email, otp, TimeSpan.FromMinutes(5));

            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your OTP code: {otp}. It is valid for 5 minutes.");

            return otp;
        }
        public async Task<User> ResetPassword(string email, string otp, string newPassword) //Chức năng đặt lại mật khẩu
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                throw new ArgumentException("Email does not exist.");
            }

            if (!_cache.TryGetValue(email, out string cachedOtp))
            {
                throw new ArgumentException("OTP has expired or is invalid.");
            }

            if (cachedOtp != otp)
            {
                throw new ArgumentException("Incorrect OTP.");
            }

            ValidatePassword(newPassword);
            user.Password = HashPassword(newPassword);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            _cache.Remove(email);

            return user;
        }
        public async Task Logout() //Chức năng đăng xuất tránh dùng lại token cũ
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _blacklistedTokens.Add(token);
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
                byte[] salt = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                var passwordWithSalt = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();

                var hashedPassword = sha256.ComputeHash(passwordWithSalt);

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
        private void ValidateEmailDomain(string email)//Kiểm tra Email khi đăng nhập
        {
            if (!email.EndsWith("@vietjetair.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Email must end with '@vietjetair.com'");
            }
        }
        private void ValidatePassword(string password)//Kiểm tra mật khẩu khi đăng nhập
        {
            var regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,16}$");
            if (!regex.IsMatch(password))
            {
                throw new InvalidOperationException("Invalid password. Password must contain at least one letter, one number, and one special character");
            }
        }
        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()//Lấy dữ liệu qua token
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;

            if (userClaim == null || !userClaim.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Please log in to the system");
            }

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_blacklistedTokens.Contains(token))
            {
                throw new UnauthorizedAccessException("Token has been invalidated.");
            }

            var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
            var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

            if (idClaim == null || emailClaim == null || roleClaim == null)
            {
                throw new InvalidOperationException("User claims are missing.");
            }

            return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
        }

    }
}
