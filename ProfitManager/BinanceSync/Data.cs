using Binance.Net.Objects.Spot.UserStream;
using STDLib.Saveable;
using System.Collections.Generic;

namespace BinanceSync
{
    public class Data : BaseSettingsV2<Data>
    {
        public List<BinanceStreamAccountInfo> BinanceStreamAccountInfos { get => GetPar(new List<BinanceStreamAccountInfo>()); }
        public List<BinanceStreamOrderUpdate> BinanceStreamOrderUpdates { get => GetPar(new List<BinanceStreamOrderUpdate>()); }
        public List<BinanceStreamOrderList> BinanceStreamOrderLists { get => GetPar(new List<BinanceStreamOrderList>()); }
        public List<BinanceStreamPositionsUpdate> BinanceStreamPositionsUpdates { get => GetPar(new List<BinanceStreamPositionsUpdate>()); }
        public List<BinanceStreamBalanceUpdate> BinanceStreamBalanceUpdates { get => GetPar(new List<BinanceStreamBalanceUpdate>()); }
    }

}
