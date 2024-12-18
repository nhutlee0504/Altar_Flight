﻿using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
	public class FlightService : IFlight
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();

        public FlightService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}
        public async Task<IEnumerable<Flight>> GetAllFlight()//Lấy danh sách tất cả chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                await UpdatePastFlightsStatus();

                var getFlight = await _context.flights.ToListAsync();
                if (getFlight != null)
                {
                    return getFlight;
                }
                throw new NotImplementedException("No flights have been created");
            }
            var userGroups = await _context.group_Users
                .Where(gu => gu.UserID == userInfo.IdUser)
                .Select(gu => gu.GroupID)
                .ToListAsync();

            var accessibleTypeDocs = await _context.group_Types
                .Where(gt => userGroups.Contains(gt.IdGroup))
                .Select(gt => gt.IdType)
                .ToListAsync();

            var accessibleFlights = await _context.documents
                .Where(df => accessibleTypeDocs.Contains(df.TypeId))
                .Select(df => df.Flight)
                .Distinct()
                .ToListAsync();
            if (accessibleFlights.Any())
            {
                return accessibleFlights;
            }

            throw new UnauthorizedAccessException("You do not have permission to view any flights");
        }
        public async Task<IEnumerable<Flight>> GetMyFlight()//Lấy danh sách các chuyến bay do người dùng tạo
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var flightFind = await _context.flights.Where(x => x.UserId == userInfo.IdUser).ToListAsync();
                if (flightFind == null)
                {
                    throw new NotImplementedException("No flight found");
                }
                return flightFind;
            }
            throw new UnauthorizedAccessException("You do not have permission to perform this action");
        }
        public async Task<Flight> AddFlight(FlightInfo flightInfo)//Tạo chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                TimeOnly timeStart;
                TimeOnly timeEnd;

                if (!TimeOnly.TryParse(flightInfo.TimeStart, out timeStart))
                {
                    throw new ArgumentException("Thời gian bắt đầu không hợp lệ.");
                }

                if (!TimeOnly.TryParse(flightInfo.TimeEnd, out timeEnd))
                {
                    throw new ArgumentException("Thời gian kết thúc không hợp lệ.");
                }
                if (flightInfo.DateTime < DateTime.Now.Date)
                {
                    throw new ArgumentException("The flight date cannot be in the past.");
                }
                var newFlight = new Flight
                {
                    FlightNo = flightInfo.FlightNo,
                    DateTime = flightInfo.DateTime,
                    PointOfLoading = flightInfo.PointOfLoading,
                    PointOfUnloading = flightInfo.PointOfUnloading,
                    TimeStart = timeStart.ToString(),
                    TimeEnd = timeEnd.ToString(),
                    Status = "Created",
                    UserId = userInfo.IdUser,
                    IsDelete = false,
                };

                await _context.flights.AddAsync(newFlight);
                await _context.SaveChangesAsync();
                return newFlight;
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng");
        }
        public async Task<Flight> RemoveFlight(int idFlight)//Xóa chuyến bay
		{
			var userInfo = GetUserInfoFromClaims();
			if(userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
			{
				var flightFind = await _context.flights.FirstOrDefaultAsync(x => x.IdFlight == idFlight);
				if(flightFind != null)
				{
                    if(userInfo.Role == "Admin")
                    {
                        flightFind.Status = "Deleted";
                        flightFind.IsDelete = true;
                        await _context.SaveChangesAsync();
                        return flightFind;
                    }
					if(flightFind.UserId == userInfo.IdUser)
					{
						flightFind.Status = "Deleted";
                        flightFind.IsDelete = true;
                        await _context.SaveChangesAsync();
						return flightFind;
					}
					throw new UnauthorizedAccessException("You do not have permission to delete this flight");
				}
				throw new NotImplementedException("Flight not found");
			}
			throw new UnauthorizedAccessException("You do not have access permission");
		}
        public async Task<Flight> UpdateFlight(int idFlight, FlightInfo flightInfo)//Cập nhật thông tin chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var flightFind = await _context.flights.FirstOrDefaultAsync(x => x.IdFlight == idFlight);
                if (flightFind != null)
                {
                    if (userInfo.Role.ToLower().Contains("admin"))
                    {
                        flightFind.FlightNo = flightInfo.FlightNo;
                        flightFind.DateTime = flightInfo.DateTime;
                        flightFind.PointOfLoading = flightInfo.PointOfLoading;
                        flightFind.PointOfUnloading = flightInfo.PointOfUnloading;

                        if (!TimeOnly.TryParse(flightInfo.TimeStart, out var timeStart))
                        {
                            throw new ArgumentException("Invalid start time");
                        }
                         
                        if (!TimeOnly.TryParse(flightInfo.TimeEnd, out var timeEnd))
                        {
                            throw new ArgumentException("Invalid end time");
                        }
                        if (flightInfo.DateTime < DateTime.Now.Date)
                        {
                            throw new ArgumentException("The flight date cannot be in the past.");
                        }
                        flightFind.TimeStart = timeStart.ToString();
                        flightFind.TimeEnd = timeEnd.ToString();

                        await _context.SaveChangesAsync();
                        return flightFind;
                    }
                    if (flightFind.UserId == userInfo.IdUser)
                    {
                        flightFind.FlightNo = flightInfo.FlightNo;
                        flightFind.DateTime = flightInfo.DateTime;
                        flightFind.PointOfLoading = flightInfo.PointOfLoading;
                        flightFind.PointOfUnloading = flightInfo.PointOfUnloading;
                        if (!TimeOnly.TryParse(flightInfo.TimeStart, out var timeStart))
                        {
                            throw new ArgumentException("Invalid start time");
                        }

                        if (!TimeOnly.TryParse(flightInfo.TimeEnd, out var timeEnd))
                        {
                            throw new ArgumentException("Invalid end time");
                        }
                        if (flightInfo.DateTime < DateTime.Now.Date)
                        {
                            throw new ArgumentException("The flight date cannot be in the past.");
                        }
                        flightFind.TimeStart = timeStart.ToString();
                        flightFind.TimeEnd = timeEnd.ToString();

                        await _context.SaveChangesAsync();
                        return flightFind;
                    }
                    throw new UnauthorizedAccessException("You do not have permission to perform this function with this flight");
                }
                throw new NotImplementedException("No flight found");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }
        private async Task UpdatePastFlightsStatus()//Tự động cập nhật trạng thái chuyến bay khi đến ngày xuất chuyến
        {
            var pastFlights = await _context.flights
                .Where(f => f.DateTime < DateTime.Now && f.Status != "Done")
                .ToListAsync();

            foreach (var flight in pastFlights)
            {
                flight.Status = "Done";
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Flight>> GetFlightByName(string name)//Tìm chuyến bay qua tên
        {
            var userInfo = GetUserInfoFromClaims();
            var flightfind = await _context.flights.Where(x => x.FlightNo.Contains(name)).ToListAsync();
            if (flightfind != null)
            {
                return flightfind;
            }
            throw new NotImplementedException("No flight found");
        }
        public async Task<Flight> GetFlightById(int id)//Tìm chuyến bay qua Id
        {
            var userInfo = GetUserInfoFromClaims();
            var flightFind = await _context.flights.FirstOrDefaultAsync(x => x.IdFlight == id);
            if (flightFind != null)
            {
                return flightFind;
            }
            throw new NotImplementedException("No flight found");
        }

        //Phương thức ngoài
        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()
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
