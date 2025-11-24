using Microsoft.EntityFrameworkCore;
using PaymentsApi.Models;
using PaymentsApi.Repositories;
using PaymentsApi.Data;

namespace PaymentsApi.Services
{
    public class PaymentsServices : IPaymentRepository
    {
        private readonly PaymentsDbContext _context;
        private readonly ILogger<PaymentsServices> _logger; // Optional logging

        public PaymentsServices(PaymentsDbContext context, ILogger<PaymentsServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Payment>> GetPayments()
        {
            try
            {
                return await _context.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment list");
                throw;
            }
        }

        public async Task<Payment> AddPayment(Payment payment)
        {
            try
            {
                if (payment == null)
                    throw new ArgumentNullException(nameof(payment), "Payment cannot be null");

                var now = DateTime.UtcNow;
                var seq = await GetNextSequenceForDate();
                var reference = $"PAY-{now:yyyyMMdd}-{seq:D4}";

                var newPayment = new Payment
                {
                    Id = Guid.NewGuid(),
                    Reference = reference,
                    ClientRequestId = payment.ClientRequestId == Guid.Empty ? null : payment.ClientRequestId,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    CreatedAt = now
                };

                _context.Payments.Add(newPayment);
                await _context.SaveChangesAsync();
                return newPayment;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while creating payment");
                throw new Exception("Database error occurred while creating payment.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error in AddPayment");
                throw;
            }
        }

        public async Task<Payment?> GetPayment(Guid id)
        {
            try
            {
                return await _context.Payments.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment with ID {id}");
                throw;
            }
        }

        public async Task<Payment?> UpdatePayment(Guid id, Payment payment)
        {
            try
            {
                var existing = await _context.Payments.FindAsync(id);
                if (existing == null)
                    throw new KeyNotFoundException($"Payment with ID {id} not found.");

                existing.Amount = payment.Amount;
                existing.Currency = payment.Currency;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Update(existing);
                await _context.SaveChangesAsync();

                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating payment with ID {id}");
                throw;
            }
        }

        public async Task<bool> DeletePayment(Guid id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                    throw new KeyNotFoundException($"Payment with ID {id} not found.");

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting payment {id}");
                throw;
            }
        }

        private async Task<int> GetNextSequenceForDate()
        {
            try
            {
                var today = DateTime.UtcNow.ToString("yyyyMMdd");

                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    SELECT ISNULL(MAX(CAST(RIGHT([Reference], 4) AS INT)), 0) + 1 
                    FROM Payments 
                    WHERE [Reference] LIKE @prefix + '%'";

                var prefixParam = cmd.CreateParameter();
                prefixParam.ParameterName = "@prefix";
                prefixParam.Value = $"PAY-{today}-";
                cmd.Parameters.Add(prefixParam);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating next sequence number");
                throw;
            }
        }
    }
}