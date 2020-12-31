using Binance.Net.Objects.Spot.UserStream;
using System;

namespace ProfitManager.Finance
{
    public class BalanceUpdate
    {
        public DateTime Timestamp { get; set; }
        public string Asset { get; set; }
        public decimal Amount { get; set; }



        public BalanceUpdate()
        {

        }

        public BalanceUpdate(BinanceStreamBalanceUpdate update)
        {
            Timestamp = update.EventTime;
            Asset = update.Asset;
            Amount = update.BalanceDelta;
        }
    }
}
