using Binance.Net.Objects.Spot.UserStream;
using System;

namespace ProfitManager.Finance
{
    public class Transaction
    {
        public DateTime Timestamp { get; set; }
        public string Asset { get; set; }
        public decimal Amount { get; set; }
        public string Reciever { get; set; }
        public string Sender { get; set; }


        public static Transaction FromBinanceStreamBalanceUpdate(BinanceStreamBalanceUpdate update)
        {
            Transaction t = new Transaction();
            t.Asset = update.Asset;
            t.Amount = update.BalanceDelta;
            t.Timestamp = update.EventTime;
            return t;
        }
    }
}
