using System;

namespace FileConversion.Abstraction.Model.StandardV2
{
    public class PosPay : IStandardModel
    {
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public decimal CheckAmount { get; set; }
        public DateTime CheckDate { get; set; }
        public string Filter { get; set; }
        public string Payee { get; set; }
    }
}
