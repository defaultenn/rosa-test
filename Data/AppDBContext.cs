using Microsoft.EntityFrameworkCore;
using RosATest.Model.Entity;

public class AppDBContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<InquiryRequest> InquiryRequests {get; set;}
    public DbSet<Group> Groups {get; set;}
    public DbSet<InquiryType> InquiryTypes {get; set;}
    public DbSet<Status> Statuses {get; set;}
    public DbSet<UserSession> UserSessions {get; set;}

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
}