using Microsoft.AspNetCore.Mvc;
using PaymentsApi.Models;
using PaymentsApi.Repositories;

namespace PaymentsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var payments = await _paymentRepository.GetPayments();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // GET: api/payments/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            try
            {
                var payment = await _paymentRepository.GetPayment(id);

                if (payment == null)
                    return NotFound($"Payment with ID '{id}' not found.");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // POST: api/payments
        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdPayment = await _paymentRepository.AddPayment(payment);

                return CreatedAtAction(
                    nameof(GetPaymentById),
                    new { id = createdPayment.Id },
                    createdPayment
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // PUT: api/payments/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedPayment = await _paymentRepository.UpdatePayment(id, payment);

                if (updatedPayment == null)
                    return NotFound($"Payment with ID '{id}' not found.");

                return Ok(updatedPayment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        // DELETE: api/payments/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _paymentRepository.DeletePayment(id);

                if (!deleted)
                    return NotFound($"Payment with ID '{id}' not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }
    }
}
