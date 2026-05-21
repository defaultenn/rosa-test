using System.ComponentModel.DataAnnotations;

namespace RosATest.Model.Entity {
    public class InquiryRequest
    {
        [Key]
        public int ID { get; set; }

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

        public uint Count {get; set;}

        [MaxLength(100)]
        public string Reason {get; set;}

        public int UserID {get; set;}

        public User? User {get; set;}

        public int InquiryTypeID {get; set;}

        public InquiryType? InquiryType {get; set;}

        public int StatusID {get; set;}

        public Status? Status {get; set;}
    }
}