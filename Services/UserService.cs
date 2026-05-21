using RosATest.Model.Entity;

public interface IUserService
{
    public Task<User> IdentifyAsync(string email);
    public void AuthenticateAsync(User user, string password);

    public Task<bool> LogoutAsync(string sessionToken);

    public Task<UserSession> CreateSessionAsync(User user, string? ipAddress, string userAgent);

    public DateTimeOffset GetExpireSessionPolicy();
}