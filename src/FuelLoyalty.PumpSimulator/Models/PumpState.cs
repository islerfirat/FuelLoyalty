using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelLoyalty.PumpSimulator.Models
{
    /// <summary>Pompanın o anki durumu. Hangi düğmenin aktif olacağını bu belirler.</summary>
    public enum PumpState
    {
        Idle,        // Kart bekleniyor
        Authorized,  // Onay alındı, yakıt verilebilir
        Fueling,     // Yakıt veriliyor
        Finished     // Yakıt verme bitti, satış gönderilmeyi bekliyor
    }
}
