using CoinGecko.Clients;
using CoinGecko.Interfaces;
using ProfitManager.Finance;
using System.Linq;

namespace ProfitManager
{
    public class Balance
    {
        public string Asset { get; set; }
        public decimal Amount { get; set; }


        decimal _UsdValue = 0;
        private decimal UsdValue 
        { 
            get
            {
                if (_UsdValue == 0)
                    GetusdValue();
                return _UsdValue;
            }
        }

        void GetusdValue()
        {
            _UsdValue = 1;
            ICoinGeckoClient client = CoinGeckoClient.Instance;
            var coinList = client.CoinsClient.GetCoinList().Result;
            var coin = coinList.Where(a => a.Symbol.ToLower() == Asset.ToLower()).FirstOrDefault();
            if (coin != null)
            {
                var v = client.SimpleClient.GetSimplePrice(new string[] { coin.Id }, new string[] { "usd"}).Result;
                _UsdValue = (decimal)v[coin.Id]["usd"].Value;
                    
            }
        }

        public decimal USDValue
        {
            get
            {
                return UsdValue * Amount;
            }
        }
    }

    







}


