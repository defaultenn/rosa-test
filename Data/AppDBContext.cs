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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Group>().HasData(
            new Group { ID = 1, Name = "Сотрудник", Codename = Group.GroupCodename.Employee },
            new Group { ID = 2, Name = "Бухгалтер", Codename = Group.GroupCodename.Accountant }
        );
        
        modelBuilder.Entity<Status>().HasData(
            new Status { ID = 1, Name = "Готово", Codename = Status.StatusCodename.Ready },
            new Status { ID = 2, Name = "В процессе", Codename = Status.StatusCodename.WIP },
            new Status { ID = 3, Name = "Отклонено", Codename = Status.StatusCodename.Rejected }
        );
        
        modelBuilder.Entity<InquiryType>().HasData(
            new InquiryType { ID = 1, Name = "2-НДФЛ", Codename = InquiryType.InquiryTypeCodename.NDFL2 },
            new InquiryType { ID = 2, Name = "О месте работы и стаже", Codename = InquiryType.InquiryTypeCodename.AboutPlaceAndTime },
            new InquiryType { ID = 3, Name = "О среднем заработке", Codename = InquiryType.InquiryTypeCodename.AboutAverageSalary }
        );
        
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                ID = 1,
                Email = "accountant@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("accountant"),
                GroupID = 2,
            },
            new User 
            { 
                ID = 2,
                Email = "employee@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("employee"),
                GroupID = 1,
            }
        );
    }
}