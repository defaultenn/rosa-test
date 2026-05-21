using RosATest.Model.Entity;

public interface IInquiryRequestService
{
    public Task<InquiryRequest?> GetByIDAsync(User user, int id);
    public Task<List<InquiryType>> GetInquiryTypesAsync();

    public Task<List<Status>> GetStatusesAsync();
    public Task<IEnumerable<InquiryRequest>> GetInquiryRequests(
        User user
    );

    public Task<InquiryRequest> CreateInquiryRequestAsync(
        User user
    );
    public Task<InquiryRequest> UpdateInquiryRequestAsync(
        User user,
        int inquiryRequestID
    );

    public User GetUser(string? userID);
}