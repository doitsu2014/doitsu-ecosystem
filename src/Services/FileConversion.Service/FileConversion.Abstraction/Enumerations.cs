namespace FileConversion.Abstraction
{
    public enum StreamType
    {
        NotSupported = 0,
        PlainText,
        Excel,
        Pdf
    }

    public enum InputType
    {
        NotSupported = 0,
        VendorPayment,
        VendorMaster,
        PosPay
    }

    public enum VendorStatus
    {
        Active = 1,
        Inactive,
        Temporary
    }
}
