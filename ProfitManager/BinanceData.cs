using Binance.Net.Objects.Spot.UserStream;
using STDLib.Saveable;
using System.Collections.Generic;

namespace ProfitManager
{
    /// <summary>
    /// https://binance-docs.github.io/apidocs/spot/en/#public-api-definitions
    /// </summary>
    public class BinanceData : BaseSettingsV2<BinanceData>
    {
        public List<BinanceStreamAccountInfo> BinanceStreamAccountInfos { get => GetPar(new List<BinanceStreamAccountInfo>()); }
        public List<BinanceStreamOrderUpdate> BinanceStreamOrderUpdates { get => GetPar(new List<BinanceStreamOrderUpdate>()); }
        public List<BinanceStreamOrderList> BinanceStreamOrderLists { get => GetPar(new List<BinanceStreamOrderList>()); }
        public List<BinanceStreamPositionsUpdate> BinanceStreamPositionsUpdates { get => GetPar(new List<BinanceStreamPositionsUpdate>()); }
        public List<BinanceStreamBalanceUpdate> BinanceStreamBalanceUpdates { get => GetPar(new List<BinanceStreamBalanceUpdate>()); }
    }

}
