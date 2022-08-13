using System.Globalization;

namespace NatuurlikBase.ViewModels
{
    public class userOrder
    {
        public int OrderID { get; set; }
        public double Amount { get; set; }
        public string? UserID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
    public class ProductOrder
    {
        public int OrderID { get; set; }
        public double Amount { get; set; }
        // public string UserID { get; set; }
    }
    public class MonthlyOrder
    {
        public int OrderID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName
        {
            get
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(this.Month);
            }
        }
        public double Amount { get; set; }
    }

    public class Productsales
    {
        public double salesAmount { get; set; }
        public double ProductAmount { get; set; }
        public int Product { get; set; }
    }
}
