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

namespace BinanceSync
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings.Load();
            Data.Load();

            BinanceWrapper synchronizer = new BinanceWrapper();

            BaseCommand.Do();

        }
    }



    public class BinanceWrapper
    {
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
                var socketClient = new BinanceSocketClient();

                socketClient.Spot.SubscribeToUserDataUpdates(startResult.Data, OnAccountInfoMessage, OnOrderUpdateMessage, OnOcoOrderUpdateMessage, OnAccountPositionMessage, OnAccountBalanceUpdate);

                BaseCommand.Register("a", GetAll);
            }

        }


        void OnAccountInfoMessage(BinanceStreamAccountInfo ee)
        {
            Data.Items.BinanceStreamAccountInfos.Add(ee);
            Data.Save();
        }

        void OnOrderUpdateMessage(BinanceStreamOrderUpdate ee)
        {
            Data.Items.BinanceStreamOrderUpdates.Add(ee);
            Data.Save();
        }

        void OnOcoOrderUpdateMessage(BinanceStreamOrderList ee)
        {
            Data.Items.BinanceStreamOrderLists.Add(ee);
            Data.Save();
        }

        void OnAccountPositionMessage(BinanceStreamPositionsUpdate ee)
        {
            Data.Items.BinanceStreamPositionsUpdates.Add(ee);
            Data.Save();
        }

        void OnAccountBalanceUpdate(BinanceStreamBalanceUpdate ee)
        {
            Data.Items.BinanceStreamBalanceUpdates.Add(ee);
            Data.Save();
        }


        void GetAll(string[] arg)
        {
            List<BinanceWithdrawal> withdrawals = GetAllWithdrawals();
            List<BinanceDeposit> deposits = GetAllDeposits();
            List<BinanceSymbol> symbols = GetFilteredSymbols();
            List<BinanceOrder> orders = GetAllOrders();
            List<BinanceBalance> assets = GetAllAssets();
            List<BinanceTrade> trades = GetAllTrades();
            Dictionary<string, decimal> calc = new Dictionary<string, decimal>();


            calc["ETH"] = 0.73891M;
            calc["USDT"] = 236.66606M + 114.647497M + 58.268888M + 115.106757M + 56.942678M + 56.94857M;
            calc["BTC"] = 0.010207M;
            calc["VET"] = 9528M;



            foreach (var v in withdrawals)
            {
                if (!calc.ContainsKey(v.Asset))
                    calc[v.Asset] = 0;

                calc[v.Asset] -= v.Amount;
            }

            foreach (var v in deposits)
            {
                if (!calc.ContainsKey(v.Coin))
                    calc[v.Coin] = 0;

                calc[v.Coin] += v.Amount;
            }
            /*
            foreach(var v in orders)
            {
                if(v.Status == Binance.Net.Enums.OrderStatus.Filled)
                {
                    BinanceSymbol sym = symbols.First(a=>a.Name == v.Symbol);

                    if (!calc.ContainsKey(sym.QuoteAsset))
                        calc[sym.QuoteAsset] = 0;

                    if (!calc.ContainsKey(sym.BaseAsset))
                        calc[sym.BaseAsset] = 0;


                    if (v.Side == Binance.Net.Enums.OrderSide.Sell)
                    {
                        calc[sym.BaseAsset] -= v.QuantityFilled;
                        calc[sym.QuoteAsset] += v.QuoteQuantityFilled;
                    }
                    else
                    {
                        calc[sym.BaseAsset] += v.QuantityFilled;
                        calc[sym.QuoteAsset] -= v.QuoteQuantityFilled;
                    }
                }
            }
            */


            foreach (var v in trades)
            {
                BinanceSymbol sym = symbols.First(a => a.Name == v.Symbol);
                if (!calc.ContainsKey(sym.QuoteAsset))
                    calc[sym.QuoteAsset] = 0;

                if (!calc.ContainsKey(sym.BaseAsset))
                    calc[sym.BaseAsset] = 0;


                if (!v.IsBuyer)
                {
                    calc[sym.BaseAsset] -= v.Quantity;
                    calc[sym.QuoteAsset] += v.QuoteQuantity;
                }
                else
                {
                    calc[sym.BaseAsset] += v.Quantity;
                    calc[sym.QuoteAsset] -= v.QuoteQuantity;
                }


            }






        }







        public List<BinanceTrade> GetAllTrades()
        {
            SaveableBindingList<BinanceTrade> result = new SaveableBindingList<BinanceTrade>();
            string file = "data\\Trades.json";
            if (File.Exists(file))
            {
                result.Load(file);
            }
            else
            {
                List<BinanceSymbol> symbols = GetFilteredSymbols();

                foreach (var symbol in symbols)
                {
                    var v = client.Spot.Order.GetMyTrades(symbol.Name).Data;
                    result.AddRange(v);

                }
                result.Save(file);
            }
            return result.ToList();
        }

        public List<BinanceOrder> GetAllOrders()
        {
            SaveableBindingList<BinanceOrder> result = new SaveableBindingList<BinanceOrder>();
            string file = "data\\Orders.json";
            if (File.Exists(file))
            {
                result.Load(file);
            }
            else
            {
                List<BinanceSymbol> symbols = GetFilteredSymbols();

                foreach (var symbol in symbols)
                {
                    var v = client.Spot.Order.GetAllOrders(symbol.Name).Data;
                    result.AddRange(v);

                }
                result.Save(file);
            }
            return result.ToList();
        }



        public List<BinanceWithdrawal> GetAllWithdrawals()
        {
            SaveableBindingList<BinanceWithdrawal> result = new SaveableBindingList<BinanceWithdrawal>();
            string file = "data\\Withdrawals.json";
            if (File.Exists(file))
            {
                result.Load(file);
            }
            else
            {
                var v = client.WithdrawDeposit.GetWithdrawalHistory().Data;
                result.AddRange(v);
                result.Save(file);
            }
            return result.ToList();
        }

        public List<BinanceDeposit> GetAllDeposits()
        {
            SaveableBindingList<BinanceDeposit> result = new SaveableBindingList<BinanceDeposit>();
            string file = "data\\Deposits.json";
            if (File.Exists(file))
            {
                result.Load(file);
            }
            else
            {
                var v = client.WithdrawDeposit.GetDepositHistory().Data;
                result.AddRange(v);
                result.Save(file);
            }
            return result.ToList();
        }


        public List<BinanceBalance> GetAllAssets()
        {
            SaveableBindingList<BinanceBalance> result = new SaveableBindingList<BinanceBalance>();
            string file = "data\\Assets.json";
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




        public List<BinanceSymbol> GetAllSymbols()
        {
            SaveableBindingList<BinanceSymbol> result = new SaveableBindingList<BinanceSymbol>();
            string file = "data\\Symbols.json";
            if (File.Exists(file))
            {
                result.Load(file);
            }
            else
            {
                BinanceExchangeInfo exchangeInfo = client.Spot.System.GetExchangeInfo().Data;
                foreach (BinanceSymbol symbol in exchangeInfo.Symbols)
                {
                    result.Add(symbol);
                }
                result.Save(file);
            }

            return result.ToList();
        }

        public List<BinanceSymbol> GetFilteredSymbols()
        {
            List<BinanceSymbol> allSymbols = GetAllSymbols();
            List<BinanceBalance> allAssets = GetAllAssets();
            List<BinanceSymbol> result = new List<BinanceSymbol>();


            foreach (BinanceSymbol symbol in allSymbols)
            {
                if (allAssets.Any(a => a.Asset == symbol.BaseAsset) && allAssets.Any(a => a.Asset == symbol.QuoteAsset))
                {
                    result.Add(symbol);
                }
            }

            return result;
        }

    }
}
