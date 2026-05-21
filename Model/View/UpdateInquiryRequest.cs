using System.ComponentModel.DataAnnotations;

namespace RosATest.ViewModels
{
    public class UpdateInquiryRequestViewModel
    {
        [Required]
        [Display(Name = "Статус")]
        public int StatusID { get; set; }
    }
}