namespace FileConversion.Abstraction.Model.StandardV2
{
    public class VendorMaster : IStandardModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public VendorStatus Status { get; set; }
        public bool Is1099 { get; set; }
        public string DefaultTermsId { get; set; }
        public string DefaultCashGLAccountId { get; set; }
        public string DefaultAccountsPayableGLAccountId { get; set; }
        public string DefaultTermsDiscountsAvailableGLAccountId { get; set; }
        public string DefaultTermsDiscountTakenGLAccountId { get; set; }
        public string DefaultDiscountGLAccountId { get; set; }
        public string DefaultFreightGLAccountId { get; set; }
        public string DefaultOtherGLAccountId { get; set; }
        public string DefaultTaxGLAccountId { get; set; }
        public string DefaultSubTotalGLAccountIds { get; set; }
        public string VendorClassId { get; set; }
        public string Currency { get; set; }
        public string TaxIdNumber { get; set; }
        public int HoldStatus { get; set; }
        public int EnabledCheckLocalPrint { get; set; }
        public Address Address { get; set; }
        public string Contact { get; set; }
        public string BusinessTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string EstimatedAnnualSpend { get; set; }
        public string Comments { get; set; }
        public string AccountNumber { get; set; }
        public string EnrollmentCategoryId { get; set; }
        public string OwnerId { get; set; }
        public string IsRestricted { get; set; }
        public string UdfFieldName { get; set; }
        public string StandardIndustrialClassificationCode { get; set; }
        public string VendorEnrollmentContactStatusId { get; set; }
        public string VendorEnrollmentCallBackDate { get; set; }
        public string IsTargetedForEnrollment { get; set; }
    }
}
