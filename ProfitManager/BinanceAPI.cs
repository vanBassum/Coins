using Binance.Net;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using Binance.Net.Objects.Spot.WalletData;
using CryptoExchange.Net.Authentication;
using STDLib.Saveable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProfitManager
{
    public class BinanceAPI
    {
        public static BinanceAPI Instance { get; private set; }

        BinanceClient client;

        public static void Init()
        {
            Instance = new BinanceAPI();

            Instance.client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(Settings.Items.BinanceAPIKEY, Settings.Items.BinanceAPISECRET)
            });
        }

        static LogList<BinanceTrade> Trades = new LogList<BinanceTrade>("temp\\Trades.json");
        static LogList<BinanceOrder> Orders = new LogList<BinanceOrder>("temp\\Orders.json");
        static LogList<BinanceWithdrawal> Withdrawals = new LogList<BinanceWithdrawal>("temp\\Withdrawals.json");
        static LogList<BinanceDeposit> Deposits = new LogList<BinanceDeposit>("temp\\Deposits.json");
        static LogList<BinanceSymbol> Symbols = new LogList<BinanceSymbol>("temp\\Symbols.json");

        public List<BinanceTrade> GetAllTradesBruteForce()
        {
            
            if (!File.Exists(Trades.LogFile))
            {
                List<BinanceSymbol> symbols = GetAllSymbols();

                for(int i=0; i<symbols.Count; i++)
                {
                    var v = client.Spot.Order.GetMyTrades(symbols[i].Name).Data;
                    if (v != null)
                    {
                        foreach (var order in v)
                            Trades.Add(order);
                    }
                }
            }

            return Trades.ReadAll();
        }


        public List<BinanceOrder> GetAllOrdersBruteForce()
        {
            

            if (!File.Exists(Orders.LogFile))
            {
                List<BinanceSymbol> symbols = GetAllSymbols();

                foreach (var symbol in symbols)
                {
                    var v = client.Spot.Order.GetAllOrders(symbol.Name).Data;
                    if (v != null)
                    {
                        foreach (var order in v)
                            Orders.Add(order);
                    }

                }
            }

            return Orders.ReadAll();
        }


        public decimal GetPriceUSDT(string symbol)
        {
            var y = GetAllSymbols();
            var z = y.Where(a => a.BaseAsset == "USDT" && a.QuoteAsset == symbol);
            var x = y.Where(a => a.BaseAsset == symbol && a.QuoteAsset == "USDT");

            if(z.Any())
                return client.Spot.Market.GetCurrentAvgPrice(z.FirstOrDefault().Name).Data.Price;
            else
                return client.Spot.Market.GetCurrentAvgPrice(x.FirstOrDefault().Name).Data.Price;


        }

        public List<BinanceWithdrawal> GetAllWithdrawals()
        {
            

            if (!File.Exists(Withdrawals.LogFile))
            {
                var v = client.WithdrawDeposit.GetWithdrawalHistory().Data;
                if (v != null)
                {
                    foreach (var order in v)
                        Withdrawals.Add(order);
                }
            }

            return Withdrawals.ReadAll();
        }



        public List<BinanceDeposit> GetAllDeposits()
        {


            if (!File.Exists(Deposits.LogFile))
            {
                var v = client.WithdrawDeposit.GetDepositHistory().Data;
                if (v != null)
                {
                    foreach (var order in v)
                        Deposits.Add(order);
                }
            }

            return Deposits.ReadAll();
        }

        public List<BinanceSymbol> GetAllSymbols()
        {


            if (!File.Exists(Symbols.LogFile))
            {
                BinanceExchangeInfo exchangeInfo = client.Spot.System.GetExchangeInfo().Data;
                foreach (BinanceSymbol symbol in exchangeInfo.Symbols)
                {
                    Symbols.Add(symbol);
                }
            }

            return Symbols.ReadAll();
        }


        /*
        public List<BinanceBalance> GetAllAssets()
        {
            SaveableBindingList<BinanceBalance> result = new SaveableBindingList<BinanceBalance>();
            string file = "data\\temp\\Assets.json";
            if (File.Exists(file))
            {
                result.Load(file);
            }
            else
            {
                BinanceAccountInfo accountInfo = client.General.GetAccountInfo().Data;
                foreach (BinanceBalance balance in accountInfo.Balances)
                {
                    if (balance.Total > 0)
                    {
                        result.Add(balance);
                    }
                }
                result.Save(file);
            }
            return result.ToList();
        }
        */




    }
}


