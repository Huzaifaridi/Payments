using System;

namespace PaymentsApi.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string? Reference { get; set; }
        public Guid? ClientRequestId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
