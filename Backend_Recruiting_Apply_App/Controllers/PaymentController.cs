using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Backend_Recruiting_Apply_App.Data.Entities;
using Backend_Recruiting_Apply_App.Services;
using System;
using System.Threading.Tasks;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private const string WEBHOOK_URL = "https://1c1e-2405-4802-1be9-1e10-a511-e62f-fa53-9c9b.ngrok-free.app";

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment")]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                var (result, payment) = await _paymentService.CreatePaymentAsync(request);
                Console.WriteLine($"Gửi yêu cầu đến PayOS: {System.Text.Json.JsonSerializer.Serialize(new { payment.Order_Code, payment.Amount, payment.Description })}");
                Console.WriteLine($"Phản hồi từ PayOS: {System.Text.Json.JsonSerializer.Serialize(new { result.status, result.checkoutUrl, result.qrCode })}");

                return CreatedAtAction(nameof(GetPayment), new { id = payment.ID }, new
                {
                    payment.ID,
                    payment.Order_Code,
                    payment.Checkout_Url,
                    payment.Qr_Code
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            try
            {
                Console.WriteLine($"Lấy thông tin thanh toán cho ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin thanh toán: {ex.Message}");
            }

            return Ok(payment);
        }

        [HttpPost("cancel-payment/{orderCode}")]
        public async Task<ActionResult<Payment>> CancelPayment(long orderCode, [FromBody] CancelRequest? cancelRequest)
        {
            try
            {
                var payment = await _paymentService.CancelPaymentAsync(orderCode, cancelRequest?.CancellationReason);
                if (payment == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    payment.ID,
                    payment.Order_Code,
                    payment.Status,
                    CancelledAt = DateTime.Now, // Note: PayOS API may not return cancelledAt in some cases
                    CancellationReason = cancelRequest?.CancellationReason
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi hủy thanh toán: {ex.Message}");
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                var success = await _paymentService.ProcessWebhookAsync(webhookBody);
                Console.WriteLine($"Webhook Data: {System.Text.Json.JsonSerializer.Serialize(webhookBody)}");
                if (!success)
                {
                    return NotFound();
                }

                return Ok("Webhook processed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Webhook: {ex.Message}");
                return StatusCode(500, $"Lỗi Webhook: {ex.Message}");
            }
        }

        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook()
        {
            try
            {
                Console.WriteLine($"Gửi yêu cầu xác thực Webhook: {WEBHOOK_URL}");
                var result = await _paymentService.ConfirmWebhookAsync(WEBHOOK_URL);
                Console.WriteLine($"Kết quả xác thực Webhook: {result}");

                return Ok(new { WebhookUrl = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xác thực Webhook: {ex.Message}");
                return StatusCode(500, $"Lỗi xác thực Webhook: {ex.Message}");
            }
        }
    }

    public class PaymentRequest
    {
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int User_ID { get; set; }
    }

    public class CancelRequest
    {
        public string? CancellationReason { get; set; }
    }
}