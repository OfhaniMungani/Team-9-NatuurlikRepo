using Microsoft.AspNetCore.Identity.UI.Services;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

namespace NatuurlikBase.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly DatabaseContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(15));

        public ReminderService(IWebHostEnvironment hostEnvironment, IEmailSender emailSender,
            IServiceScopeFactory factory)
        {
            _db = factory.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken)
                && !stoppingToken.IsCancellationRequested)
            {
                await PaymentReminder();
            }
        }

        public async Task PaymentReminder()
        {
            var orderprsn = _db.Order.Where(z => z.OrderPaymentStatus == "Payment Outstanding" && z.OrderStatus != "Cancelled"
            && z.OrderStatus != "Rejected" && z.OrderStatus != "Pending").ToList();

            if (orderprsn != null)
            {
                foreach (var psn in orderprsn)
                {
                    var fullTime = _db.PaymentReminder.FirstOrDefault(x => x.Id == psn.PaymentReminderId).Value;
                    var halfTime = fullTime / 2;
                    var orderProcDate = psn.ProcessedDate.Date;
                    var threshold = orderProcDate.AddDays(fullTime);
                    var halfthreshold = orderProcDate.AddDays(halfTime);
                    var user = _db.User.Where(z => z.Id == psn.ApplicationUserId).FirstOrDefault();
                    string email = user.Email;
                    string name = user.FirstName;
                    string number = psn.Id.ToString();
                    string total = psn.OrderTotal.ToString();
                    string date = orderProcDate.ToString("M");
                    string fullDate = threshold.ToString("D");
                    string status = psn.OrderPaymentStatus;
                    string wwwRootPath = _hostEnvironment.WebRootPath;

                    if (halfthreshold == DateTime.Today.Date)
                    {
                        var template = File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\payOutTemp.html"));
                        template = template.Replace("[NAME]", name).Replace("[TOTAL]", total).Replace("[STATUS]", status).Replace("[DUE]", fullDate)
                            .Replace("[ID]", number).Replace("[DATE]", date);
                        string message = template;
                        await _emailSender.SendEmailAsync(
                            email,
                            "PAYMENT REMINDER",
                            message);
                    }

                    if (threshold == DateTime.Today.Date)
                    {
                        var template = File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\payDueTemp.html"));
                        template = template.Replace("[NAME]", name).Replace("[TOTAL]", total).Replace("[STATUS]", status).Replace("[DUE]", fullDate)
                            .Replace("[ID]", number).Replace("[DATE]", date);
                        string message = template;
                        await _emailSender.SendEmailAsync(
                            email,
                            "PAYMENT DUE",
                            message);
                    }

                    else if (orderProcDate < DateTime.Today.Date)
                    {
                        psn.OrderPaymentStatus = SR.PaymentOverdue;
                        _db.SaveChanges();
                    }
                }

                await Task.Delay(500);
            }
        }        

    }
}
