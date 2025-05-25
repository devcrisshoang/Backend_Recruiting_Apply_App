using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Backend_Recruiting_Apply_App.Data.Entities;
using SystemAPIdotnet.Data;
using Backend_Recruiting_Apply_App.Controllers;

namespace Backend_Recruiting_Apply_App.Services
{
    public interface IPaymentService
    {
        Task<(CreatePaymentResult, Payment)> CreatePaymentAsync(PaymentRequest request);
        Task<Payment> GetPaymentAsync(int id);
        Task<Payment> CancelPaymentAsync(long orderCode, string? cancellationReason);
        Task<bool> ProcessWebhookAsync(WebhookType webhookBody);
        Task<string> ConfirmWebhookAsync(string webhookUrl);
    }

    public class PaymentService : IPaymentService
    {
        private readonly RAADbContext _dbContext;
        private readonly PayOS _payOS;

        public PaymentService(RAADbContext dbContext, PayOS payOS)
        {
            _dbContext = dbContext;
            _payOS = payOS;
        }

        public async Task<(CreatePaymentResult, Payment)> CreatePaymentAsync(PaymentRequest request)
        {
            var orderCode = Random.Shared.Next(100000, 999999);
            var items = new List<ItemData> { new ItemData("Thanh toán đơn hàng", 1, request.Amount) };
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: request.Amount,
                description: request.Description,
                items: items,
                returnUrl: "recruitingapp://payment/success",
                cancelUrl: "recruitingapp://payment/cancel"
            );

            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

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

            _dbContext.Payment.Add(payment);
            await _dbContext.SaveChangesAsync();

            return (result, payment);
        }

        public async Task<Payment> GetPaymentAsync(int id)
        {
            var payment = await _dbContext.Payment.FindAsync(id);
            if (payment == null)
            {
                return null;
            }

            try
            {
                PaymentLinkInformation info = await _payOS.getPaymentLinkInformation(payment.Order_Code);
                payment.Status = info.status;
                payment.Amount = info.amount;
                _dbContext.Entry(payment).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // Log error in controller, continue with current payment state
            }

            return payment;
        }

        public async Task<Payment> CancelPaymentAsync(long orderCode, string? cancellationReason)
        {
            var payment = await _dbContext.Payment.FirstOrDefaultAsync(p => p.Order_Code == orderCode);
            if (payment == null)
            {
                return null;
            }

            PaymentLinkInformation cancelledInfo = await _payOS.cancelPaymentLink(orderCode, cancellationReason);
            payment.Status = cancelledInfo.status;
            _dbContext.Entry(payment).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return payment;
        }

        public async Task<bool> ProcessWebhookAsync(WebhookType webhookBody)
        {
            WebhookData webhookData = _payOS.verifyPaymentWebhookData(webhookBody);
            var payment = await _dbContext.Payment
                .FirstOrDefaultAsync(p => p.Order_Code == webhookData.orderCode);
            if (payment == null)
            {
                return false;
            }

            payment.Status = webhookData.code == "00" ? "PAID" : "FAILED";
            _dbContext.Entry(payment).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<string> ConfirmWebhookAsync(string webhookUrl)
        {
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                throw new ArgumentException("WebhookUrl không được để trống");
            }

            return await _payOS.confirmWebhook(webhookUrl);
        }
    }
}