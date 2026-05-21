using Microsoft.EntityFrameworkCore;
using RosATest.Model.Entity;

public interface IUserService
{
    public Task<User> IdentifyAsync(string email);
    public void AuthenticateAsync(User user, string password);

    public Task<bool> LogoutAsync(string sessionToken);

    public Task<UserSession> CreateSessionAsync(User user, string? ipAddress, string userAgent);

    public DateTimeOffset GetExpireSessionPolicy();
}

namespace RosATest.Services
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _context;

        public UserService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<User> IdentifyAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if(user == null)
            {
                throw new Exception("Неверный логин или пароль.");
            }

            return user;
        }

        public void AuthenticateAsync(User user, string password)
        {
            if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new Exception("Неверный логин или пароль.");
            }
        }

        public DateTimeOffset GetExpireSessionPolicy()
        {
            return DateTimeOffset.UtcNow.AddDays(7);
        }

        public async Task<UserSession> CreateSessionAsync(User user, string? ipAddress, string userAgent)
        {
            var sessionToken = Guid.NewGuid().ToString();

            UserSession userSession = new UserSession
            {
                UserID = user.ID,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                SessionToken = sessionToken,
                ExpiresAt = GetExpireSessionPolicy().DateTime
            };

            _context.Add(userSession);
            await _context.SaveChangesAsync();

            return userSession;
        }

        public async Task<bool> LogoutAsync(string sessionToken)
        {
            if(string.IsNullOrEmpty(sessionToken))
            {
                return false;
            }

            var userSession = await _context.UserSessions
                .FirstOrDefaultAsync(us => us.SessionToken == sessionToken);

            if(userSession == null)
                return false;

            if(userSession.IsValid)
            {
                userSession.IsValid = false;
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}