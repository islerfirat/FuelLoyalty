using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelLoyalty.SharedKernel
{
    /// <summary>
    /// Değer döndüren bir işlemin sonucu. Başarılıysa Value dolu, başarısızsa Error dolu.
    /// </summary>
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        /// <summary>
        /// Başarılı sonucun değeri. Başarısız sonuçta okunmaya çalışılırsa hata fırlatır,
        /// böylece "hatayı kontrol etmeyi unuttum" durumu sessizce geçmez.
        /// </summary>
        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Başarısız bir sonucun değeri okunamaz.");

        /// <summary>Değer doğrudan döndürülebilir: <c>return card;</c></summary>
        public static implicit operator Result<TValue>(TValue value) => new(value, true, Error.None);

        /// <summary>Hata doğrudan döndürülebilir: <c>return CardErrors.NotFound;</c></summary>
        public static implicit operator Result<TValue>(Error error) => new(default, false, error);
    }
}
