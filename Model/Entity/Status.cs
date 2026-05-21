using System.ComponentModel.DataAnnotations;

namespace RosATest.Model.Entity {
    public class Status
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name {get; set;} = "";

        public StatusCodename Codename { get; set; }

        public enum StatusCodename
        {
            Ready,
            WIP,
            Rejected
        }
    }
}