using System.ComponentModel.DataAnnotations;

namespace RosATest.Model.Entity {
    public class Group
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        public ICollection<User> Users { get; set; } = [];

        public GroupCodename Codename { get; set; }

        public enum GroupCodename
        {
            Accountant,
            Employee
        }
    }
}