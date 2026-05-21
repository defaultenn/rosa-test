
using System.ComponentModel.DataAnnotations;

namespace RosATest.Model.Entity
{
    public class UserSession
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public int UserID { get; set; }

        public User? User { get; set; }
        
        [Required]
        public string SessionToken { get; set; } = "";

        public string? IPAddress { get; set; } = "";
        
        [Required]
        public string UserAgent { get; set; } = "";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiresAt { get; set; }
        
        public bool IsValid { get; set; } = true;
    }
}