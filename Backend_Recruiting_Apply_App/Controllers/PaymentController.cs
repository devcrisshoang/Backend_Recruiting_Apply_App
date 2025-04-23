using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Backend_Recruiting_Apply_App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SystemAPIdotnet.Data;

namespace Backend_Recruiting_Apply_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly RAADbContext _context;
        private readonly PayOS _payOS;
        private const string WEBHOOK_URL = "https://1c1e-2405-4802-1be9-1e10-a511-e62f-fa53-9c9b.ngrok-free.app"; // Đặt URL Webhook của bạn tại đây

        public PaymentController(RAADbContext context, IConfiguration configuration)
        {
            _context = context;
            var clientId = configuration["PayOS:ClientId"] ?? throw new Exception("Thiếu ClientId PayOS");
            var apiKey = configuration["PayOS:ApiKey"] ?? throw new Exception("Thiếu ApiKey PayOS");
            var checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new Exception("Thiếu ChecksumKey PayOS");
            _payOS = new PayOS(clientId, apiKey, checksumKey);
        }

        [HttpPost("create-payment")]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                // Tạo orderCode ngẫu nhiên
                var orderCode = Random.Shared.Next(100000, 999999);

                // Tạo danh sách sản phẩm
                var items = new List<ItemData> { new ItemData("Thanh toán đơn hàng", 1, request.Amount) };

                // Tạo PaymentData
                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: request.Amount,
                    description: request.Description,
                    items: items,
                    returnUrl: "recruitingapp://payment/success",
                    cancelUrl: "recruitingapp://payment/cancel"
                );

                // Gọi PayOS để tạo link thanh toán
                Console.WriteLine($"Gửi yêu cầu đến PayOS: {System.Text.Json.JsonSerializer.Serialize(paymentData)}");
                CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);
                Console.WriteLine($"Phản hồi từ PayOS: {System.Text.Json.JsonSerializer.Serialize(result)}");

                // Lưu thông tin thanh toán vào database
                var payment = new Payment
                {
                    Order_Code = orderCode,
                    Amount = request.Amount,
                    Description = request.Description,
                    Status = result.status,
                    Checkout_Url = result.checkoutUrl,
                    Qr_Code = result.qrCode,
                    User_ID = request.User_ID
                };

                _context.Payment.Add(payment);
                await _context.SaveChangesAsync();

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
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
                return NotFound();

            // Lấy thông tin mới nhất từ PayOS
            try
            {
                PaymentLinkInformation info = await _payOS.getPaymentLinkInformation(payment.Order_Code);
                payment.Status = info.status;
                payment.Amount = info.amount;
                _context.Entry(payment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin thanh toán: {ex.Message}");
            }

            return payment;
        }

        [HttpPost("cancel-payment/{orderCode}")]
        public async Task<ActionResult<Payment>> CancelPayment(long orderCode, [FromBody] CancelRequest? cancelRequest)
        {
            try
            {
                var payment = await _context.Payment.FirstOrDefaultAsync(p => p.Order_Code == orderCode);
                if (payment == null)
                    return NotFound();

                // Hủy link thanh toán
                PaymentLinkInformation cancelledInfo = await _payOS.cancelPaymentLink(orderCode, cancelRequest?.CancellationReason);
                payment.Status = cancelledInfo.status;
                _context.Entry(payment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    payment.ID,
                    payment.Order_Code,
                    payment.Status,
                    CancelledAt = cancelledInfo.canceledAt,
                    CancellationReason = cancelledInfo.cancellationReason
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
                // Xác minh dữ liệu Webhook
                WebhookData webhookData = _payOS.verifyPaymentWebhookData(webhookBody);
                Console.WriteLine($"Webhook Data: {System.Text.Json.JsonSerializer.Serialize(webhookData)}");

                // Cập nhật trạng thái thanh toán
                var payment = await _context.Payment
                    .FirstOrDefaultAsync(p => p.Order_Code == webhookData.orderCode);
                if (payment == null)
                    return NotFound();

                payment.Status = webhookData.code == "00" ? "PAID" : "FAILED";
                _context.Entry(payment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

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
                // Kiểm tra URL Webhook
                if (string.IsNullOrWhiteSpace(WEBHOOK_URL))
                {
                    Console.WriteLine("WebhookUrl không được để trống");
                    return BadRequest("WebhookUrl không được để trống");
                }

                Console.WriteLine($"Gửi yêu cầu xác thực Webhook: {WEBHOOK_URL}");
                string result = await _payOS.confirmWebhook(WEBHOOK_URL);
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