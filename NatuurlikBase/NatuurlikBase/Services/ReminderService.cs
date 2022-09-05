﻿using Microsoft.AspNetCore.Identity.UI.Services;
using NatuurlikBase.Data;

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
                await ConfirmationReminder();
            }
        }

        public async Task PaymentReminder()
        {
            var orderprsn = _db.Order.Where(z => z.OrderPaymentStatus == "Payment Outstanding" && z.OrderStatus != "Cancelled" && z.OrderStatus != "Rejected").ToList();

            if (orderprsn != null)
            {
                foreach (var psn in orderprsn)
                {
                    var fullTime = _db.PaymentReminder.FirstOrDefault(x => x.Id == psn.PaymentReminderId).Value;
                    var halfTime = fullTime / 2;
                    var orderDate = psn.CreatedDate.Date;
                    var threshold = orderDate.AddDays(fullTime);
                    var halfthreshold = orderDate.AddDays(halfTime);
                    var user = _db.User.Where(z => z.Id == psn.ApplicationUserId).FirstOrDefault();
                    string email = user.Email;
                    string name = user.FirstName;
                    string number = psn.Id.ToString();
                    string total = psn.OrderTotal.ToString();
                    string date = orderDate.ToString("M");
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

                    else if (threshold == DateTime.Today.Date)
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
                }

                await Task.Delay(500);
            }
        }

        public async Task ConfirmationReminder()
        {
            var orderprsn = _db.Order.Where(z => z.OrderStatus == "Delayed").ToList();

            if (orderprsn != null)
            {
                foreach (var psn in orderprsn)
                {
                    var fullTime = _db.ConfirmationReminder.FirstOrDefault(x => x.Id == psn.ConfirmationReminderId).Value;
                    var modifiedDate = psn.BackOrderDate.Date;
                    var threshold = modifiedDate.AddDays(fullTime);
                    var cancelOrderDate = threshold.AddDays(1);
                    var user = _db.User.Where(z => z.Id == psn.ApplicationUserId).FirstOrDefault();
                    string email = user.Email;
                    string name = user.FirstName;
                    string number = psn.Id.ToString();
                    string date = DateTime.Now.ToString("M");
                    string fullDate = threshold.ToString("D");
                    string wwwRootPath = _hostEnvironment.WebRootPath;


                    if (threshold == DateTime.Today.Date)
                    {
                        var template = File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\confRemTemp.html"));
                        template = template.Replace("[NAME]", name).Replace("[DUE]", fullDate)
                            .Replace("[ID]", number).Replace("[DATE]", date);
                        string message = template;
                        await _emailSender.SendEmailAsync(
                            email,
                            "Order Confirmation Reminder",
                            message);
                    }

                    if (cancelOrderDate == DateTime.Today.Date)
                    {
                        psn.OrderStatus = "Cancelled";
                        _db.SaveChanges();

                        var template = File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\canResOrderTemp.html"));
                        template = template.Replace("[NAME]", name)
                            .Replace("[ID]", number).Replace("[DATE]", date);
                        string message = template;

                        await _emailSender.SendEmailAsync(
                        email,
                        "Order Cancelled",
                        message);

                    }

                }

                await Task.Delay(500);
            }
        }

    }
}
