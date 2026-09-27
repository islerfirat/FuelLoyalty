namespace FuelLoyalty.SharedKernel
{
    /// <summary>
    /// Değer döndürmeyen bir işlemin sonucu: başarılı ya da bir hata ile başarısız.
    /// </summary>
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Başarılı sonuç hata taşıyamaz.");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Başarısız sonuç bir hata taşımalıdır.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

        /// <summary>
        /// Bir Error doğrudan Result olarak döndürülebilir: <c>return CardErrors.Inactive;</c>
        /// </summary>
        public static implicit operator Result(Error error) => Failure(error);
    }
}
