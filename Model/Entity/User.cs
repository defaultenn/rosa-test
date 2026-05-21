// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace RosATest.Model.Entity
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = "";
        
        [Required]
        public string PasswordHash { get; set; } = "";

        [Required]
        public int GroupID {get; set;}

        public Group? Group {get; set;}
        
        public ICollection<UserSession> Sessions { get; set; } = [];
    }
}