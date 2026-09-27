namespace FuelLoyalty.SharedKernel
{
    /// <summary>
    /// Bir iş hatasını tanımlar: kodu, kullanıcıya gösterilecek mesajı ve türü.
    /// Kod sabit kalır (örnek: "Card.Inactive"), mesaj değişebilir.
    /// </summary>
    public sealed record Error(string Code, string Message, ErrorType Type)
    {
        /// <summary>"Hata yok" anlamına gelen boş değer.</summary>
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

        public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

        public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

        public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    }
}
