﻿namespace NatuurlikBase.Models
{
    public static class SR
    {
        //static class that holds roles within the application
        public const string Role_Customer = "Customer";
        public const string Role_Reseller = "Reseller";
        public const string Role_IM = "Inventory Manager";
        public const string Role_MD = "Marketing Director";
        public const string Role_SA = "Sales Assistant";
        public const string Role_Admin = "Admin";

        //Order Management

        //Reseller placed order
        public const string OrderPending = "Pending";
        public const string OrderApproved = "Approved";
        public const string OrderRejected = "Rejected";
        //public const string OrderApproved = "Placed";
        public const string ProcessingOrder = "Processing";
        public const string OrderDispatched = "Dispatched";
        public const string OrderCancelled = "Cancelled";
        public const string OrderRefundPending = "Pending Return";
        public const string OrderRefunded = "Refunded";
        public const string OrderDelayed = "Delayed";
        public const string RejectDelayedOrder = "Rejected Delayed Order";

        //Order Payment Status
        public const string OrderPaymentApproved = "Paid";
        public const string ResellerDelayedPayment = "Payment Outstanding";

        //Query Status Management
        public const string QueryLogged = "Pending Review";
        public const string QueryReview = "Reviewed";

        //Returned Product
        public const string ReturnedProduct = "Includes Returned Product(s)";



    }
}
