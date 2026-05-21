using System.ComponentModel.DataAnnotations;

namespace RosATest.ViewModels
{
    public class CreateInquiryRequestViewModel
    {
        [Required]
        [Display(Name = "Количество")]
        public uint Count { get; set; }
        
        // [Required]
        [Display(Name = "Причина")]
        public string Reason { get; set; }

        [Display(Name = "Свой тип справки")]
        public string? InquiryType { get; set; }

        [Display(Name = "Тип справки")]
        public int? InquiryTypeID { get; set; }
    }
}