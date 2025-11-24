using PaymentsApi.Models;

namespace PaymentsApi.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetPayments();
        Task<Payment> GetPayment(Guid id);
        Task<Payment> AddPayment(Payment payment);
        Task<Payment> UpdatePayment(Guid id, Payment payment);
        Task<bool> DeletePayment(Guid id);
            
    }
}
