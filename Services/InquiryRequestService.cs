using Microsoft.EntityFrameworkCore;
using RosATest.Model.Entity;
using RosATest.ViewModels;

public interface IInquiryRequestService
{
    public Task<InquiryRequest?> GetByIDAsync(User user, int id);
    public Task<List<InquiryType>> GetInquiryTypesAsync();

    public Task<List<Status>> GetStatusesAsync();
    public Task<IEnumerable<InquiryRequest>> GetInquiryRequests(
        User user,
        ListInquiryRequestsViewModel model
    );

    public Task<InquiryRequest> CreateInquiryRequestAsync(
        User user,
        CreateInquiryRequestViewModel model
    );
    public Task<InquiryRequest> UpdateInquiryRequestAsync(
        User user,
        int inquiryRequestID,
        UpdateInquiryRequestViewModel model
    );

    public User GetUser(string? userID);
}

namespace RosATest.Services
{
    public class InquiryRequestService : IInquiryRequestService
    {
        private readonly AppDBContext _context;

        public InquiryRequestService(AppDBContext context)
        {
            _context = context;
        }
        
        public Task<List<Status>> GetStatusesAsync()
        {
            return _context.Statuses.ToListAsync();
        }

        public Task<List<InquiryType>> GetInquiryTypesAsync()
        {
            return _context.InquiryTypes.ToListAsync();
        }

        public async Task<InquiryRequest> CreateInquiryRequestAsync(
            User user,
            CreateInquiryRequestViewModel model
        )
        {   
            int inquiryTypeID = 0;

            if(model.InquiryTypeID == null && !string.IsNullOrEmpty(model.InquiryType))
            {
                // TODO: поиск схожих или идентичных вариантов перед созданием
                InquiryType type = new InquiryType
                {
                    Name = model.InquiryType
                };
                _context.Add(type);
                await _context.SaveChangesAsync();

                inquiryTypeID = type.ID;
            } else if(model.InquiryTypeID.HasValue)
            {
                inquiryTypeID = model.InquiryTypeID.Value;
            } else
            {
                throw new InvalidDataException("Не указан тип справки");
            }

            var existingInquiryRequest = ScopedRequests(user)
                .Include(ir => ir.Status)
                .Where(ir => ir.Status!.Codename == Status.StatusCodename.WIP && ir.InquiryTypeID == inquiryTypeID)
                .FirstOrDefault();

            if(existingInquiryRequest != null)
                throw new InvalidOperationException("Справка такого типа уже находится в работе.");

            InquiryRequest request = new InquiryRequest
            {
                Count = model.Count,
                Reason = model.Reason,
                UserID = user.ID,
                InquiryTypeID = inquiryTypeID
            };
            
            // По умолчанию "В процессе"
            // TODO: обработка отсутствия записи
            var status = _context.Statuses.First(s => s.Codename == Status.StatusCodename.WIP);
            request.StatusID = status.ID;

            _context.Add(request);
            await _context.SaveChangesAsync();
            
            return request;
        }

        public User GetUser(string? userID)
        {   
            if(userID == null)
                throw new UnauthorizedAccessException("Требуется войти в систему.");

            return _context.Users
                .Include(u => u.Group)
                .First(u => u.ID.ToString() == userID);
        }

        public IQueryable<InquiryRequest> ScopedRequests(User user)
        {
            var query = _context.InquiryRequests.Include(ir => ir.InquiryType);
            switch (user.Group?.Codename)
            {
                case Group.GroupCodename.Accountant:
                    //выдаем все запросы
                    return query;
                case Group.GroupCodename.Employee:
                    // Выдаем собственные запросы для сотрудника
                    return query.Where(ir => ir.UserID == user.ID);
                default:
                    return query;
            }
        }

        public async Task<InquiryRequest> UpdateInquiryRequestAsync(
            User user,
            int inquiryRequestID,
            UpdateInquiryRequestViewModel model
        )
        {   
            // TODO: изменить подход: отправить запрос на изменение записи по ID,
            // вместо загрузки записи в память и последующим изменением
            var inquiryRequest = ScopedRequests(user)
                .FirstOrDefault(ir => ir.ID == inquiryRequestID);

            if(inquiryRequest == null)
                throw new InvalidDataException("Такого запроса не существует.");

            inquiryRequest.StatusID = model.StatusID;

            _context.Update(inquiryRequest);
            await _context.SaveChangesAsync();

            return inquiryRequest;
        }

        public async Task<IEnumerable<InquiryRequest>> GetInquiryRequests(
            User user, 
            ListInquiryRequestsViewModel model
        )
        {
            var query = ScopedRequests(user).Include(ir => ir.User).AsQueryable();

            if(model.StatusID != null)
            {
                query = query.Where(ir => ir.StatusID == model.StatusID);
            }

            return await query.ToListAsync();
        }

        public async Task<InquiryRequest?> GetByIDAsync(User user, int id)
        {
            var inquiryRequest = ScopedRequests(user).FirstOrDefault(ir => ir.ID == id);
            
            if(inquiryRequest == null)
                throw new InvalidDataException("Такого запроса не существует.");

            return inquiryRequest;
        }
    }
}