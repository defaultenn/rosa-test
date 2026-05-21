using System.ComponentModel.DataAnnotations;

namespace RosATest.Model.Entity {
    public class InquiryType
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public InquiryTypeCodename Codename { get; set; }

        public enum InquiryTypeCodename
        {
            NDFL2,
            AboutPlaceAndTime,
            AboutAverageSalary
        }
    }
}