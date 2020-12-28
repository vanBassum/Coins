using System;
using Binance.Net;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using Binance.Net.Objects.Spot.UserStream;
using Binance.Net.Objects.Spot.WalletData;
using CryptoExchange.Net.Authentication;
using STDLib.Commands;
using STDLib.Saveable;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Binance.Net.Objects.Spot.MarketStream;

namespace BinanceSync
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings.Load();
            BinanceWrapper synchronizer = new BinanceWrapper();
            BaseCommand.Do();
        }
    }



    public class BinanceWrapper
    {
        readonly LogList<BinanceStreamAccountInfo> AccountInfos = new LogList<BinanceStreamAccountInfo>("AccountInfos.json");
        readonly LogList<BinanceStreamOrderUpdate> OrderUpdates = new LogList<BinanceStreamOrderUpdate>("OrderUpdates.json");
        readonly LogList<BinanceStreamOrderList> OrderLists = new LogList<BinanceStreamOrderList>("OrderLists.json");
        readonly LogList<BinanceStreamPositionsUpdate> PositionsUpdates = new LogList<BinanceStreamPositionsUpdate>("PositionsUpdates.json");
        readonly LogList<BinanceStreamBalanceUpdate> BalanceUpdates = new LogList<BinanceStreamBalanceUpdate>("BalanceUpdates.json");
        readonly LogList<BinanceStreamAggregatedTrade> AggregatedTrades = new LogList<BinanceStreamAggregatedTrade>("AggregatedTrades.json");
        readonly LogList<BinanceStreamTrade> Trades = new LogList<BinanceStreamTrade>("Trades.json");
        BinanceClient client;

        public BinanceWrapper()
        {

            client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(Settings.Items.BinanceAPIKEY, Settings.Items.BinanceAPISECRET)
            });

            var startResult = client.Spot.UserStream.StartUserStream();

            if (!startResult.Success)
            {
                Console.WriteLine($"Failed to start user stream: {startResult.Error}");
            }
            else
            {
                var symbols = GetAllSymbols();
                var v = from a in symbols
                        select a.Name;

                var socketClient = new BinanceSocketClient();
                socketClient.Spot.SubscribeToUserDataUpdates(
                    startResult.Data, 
                    (a) => AccountInfos.Add(a),
                    (a) => OrderUpdates.Add(a),
                    (a) => OrderLists.Add(a),
                    (a) => PositionsUpdates.Add(a),
                    (a) => BalanceUpdates.Add(a));
                

                socketClient.Spot.SubscribeToAggregatedTradeUpdates(v, (a) => AggregatedTrades.Add(a));
                socketClient.Spot.SubscribeToTradeUpdates(v, (a) => Trades.Add(a));
            }
        }


        public List<BinanceSymbol> GetAllSymbols()
        {
            SaveableBindingList<BinanceSymbol> result = new SaveableBindingList<BinanceSymbol>();
            BinanceExchangeInfo exchangeInfo = client.Spot.System.GetExchangeInfo().Data;
            foreach (BinanceSymbol symbol in exchangeInfo.Symbols)
            {
                result.Add(symbol);
            }
            return result.ToList();
        }
    }
}
