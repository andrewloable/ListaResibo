using System;

namespace ListaResibo
{
    public class Rec
    {
        public string ID { get; set; }
        public DateTime TaxableMonth { get; set; }
        public string TIN { get; set; }
        public string StoreName { get; set; }
        public string PersonName { get; set; }
        public decimal GrossPurchaseAmount { get; set; }
        public decimal ExemptPurchaseAmount { get; set; }
        public decimal TaxablePurchaseAmount { get; set; }
        public decimal ServicePurchaseAmount { get; set; }
        public string filename { get; set; }
        public string amt
        {
            get
            {
                return $"Gross: {GrossPurchaseAmount} Exempt: {ExemptPurchaseAmount} Taxable: {TaxablePurchaseAmount}";
            }
        }
        public string name
        {
            get
            {
                return $"{StoreName} {PersonName}";
            }
        }
        public string dtstring
        {
            get
            {
                return TaxableMonth.ToShortDateString();
            }
        }
        public string csvrow
        {
            get
            {
                return $"{ID}■{TaxableMonth}■{TIN}■{StoreName}■{PersonName}■{GrossPurchaseAmount}■{ExemptPurchaseAmount}■{TaxablePurchaseAmount}■{filename}{Environment.NewLine}";
            }
        }
    }
}
