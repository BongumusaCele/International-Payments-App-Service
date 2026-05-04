namespace InternationalPaymentsAPI.Helpers
{
    public static class ValidationPatterns
    {
        public const string PersonName = @"^[A-Za-z][A-Za-z' -]{1,49}$";
        public const string Username = @"^[A-Za-z][A-Za-z0-9._-]{2,29}$";
        public const string StrongPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12,128}$";
        public const string SwiftCode = @"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$";
        public const string NumericAccountNumber = @"^\d{6,20}$";
        public const string Country = @"^[A-Za-z][A-Za-z .'-]{1,55}$";
        public const string BankName = @"^[A-Za-z0-9][A-Za-z0-9 .,'&()/-]{1,79}$";
        public const string PaymentReference = @"^[A-Za-z0-9][A-Za-z0-9 ._/#-]{2,34}$";
        public const string PaymentProvider = @"^(SWIFT|Bank Transfer|EFT)$";
        public const string IdNumber = @"^\d{13}$";
    }
}
