using System.ComponentModel.DataAnnotations;

namespace RosATest.ViewModels
{
    public class ListInquiryRequestsViewModel
    {
        [Display(Name = "Статус")]
        public int? StatusID { get; set; }
    }
}