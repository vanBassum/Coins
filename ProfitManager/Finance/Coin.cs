using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using System.Linq;

namespace ProfitManager.Finance
{
    public class Coin
    {
        public string Symbol { get; set; } = "";
        public decimal Price { get; set; }
        public Coin(string symbol)
        {
            Symbol = symbol;
            //Price = BinanceAPI.Instance.GetPriceUSDT(Symbol);
            
        }



        public static bool operator ==(Coin obj1, Coin obj2)
        {
            return obj1.Symbol == obj2.Symbol;
        }

        public static bool operator !=(Coin obj1, Coin obj2)
        {
            return obj1.Symbol != obj2.Symbol;
        }

        public override string ToString()
        {
            return Symbol;
        }
    }
}
