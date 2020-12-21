using Binance.Net;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using Binance.Net.Objects.Spot.UserStream;
using Binance.Net.Objects.Spot.WalletData;
using CryptoExchange.Net.Authentication;
using STDLib.Commands;
using STDLib.Saveable;
using System;
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

            Synchronizer synchronizer = new Synchronizer();

            BaseCommand.Do();

        }
    }



    public class Synchronizer
    {
        BinanceClient client;
        public Synchronizer()
        {

            client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(Settings.Items.BinanceAPIKEY, Settings.Items.BinanceAPISECRET)
            });

            //var startResult = client.Spot.UserStream.StartUserStream();

            //if (!startResult.Success)
            //    throw new Exception($"Failed to start user stream: {startResult.Error}");

            //var socketClient = new BinanceSocketClient();

            //socketClient.Spot.SubscribeToUserDataUpdates(startResult.Data, OnAccountInfoMessage, OnOrderUpdateMessage, OnOcoOrderUpdateMessage, OnAccountPositionMessage, OnAccountBalanceUpdate);




            BaseCommand.Register("a", GetAll);

        }



        void OnAccountInfoMessage(BinanceStreamAccountInfo e)
        {

        }

        void OnOrderUpdateMessage(BinanceStreamOrderUpdate e)
        {

        }

        void OnOcoOrderUpdateMessage(BinanceStreamOrderList e)
        {

        }

        void OnAccountPositionMessage(BinanceStreamPositionsUpdate e)
        {

        }

        void OnAccountBalanceUpdate(BinanceStreamBalanceUpdate e)
        {
            
        }


        void GetAll(string[] arg)
        {
            List<BinanceWithdrawal> withdrawals = GetAllWithdrawals();
            List<BinanceDeposit> deposits = GetAllDeposits();
            List<BinanceSymbol> symbols = GetFilteredSymbols();
            List<BinanceOrder> orders = GetAllOrders();


        }




        List<BinanceOrder> GetAllOrders()
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

                foreach(var symbol in symbols)
                {
                    var v = client.Spot.Order.GetAllOrders(symbol.Name).Data;
                    result.AddRange(v);

                }
                result.Save(file);
            }
            return result.ToList();
        }



        List<BinanceWithdrawal> GetAllWithdrawals()
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

        List<BinanceDeposit> GetAllDeposits()
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


        List<BinanceBalance> GetAllAssets()
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




        List<BinanceSymbol> GetAllSymbols()
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

        List<BinanceSymbol> GetFilteredSymbols()
        {
            List<BinanceSymbol> allSymbols = GetAllSymbols();
            List<BinanceBalance> allAssets = GetAllAssets();
            List<BinanceSymbol> result = new List<BinanceSymbol>();


            foreach(BinanceSymbol symbol in allSymbols)
            {
                if(allAssets.Any(a=>a.Asset == symbol.BaseAsset) && allAssets.Any(a => a.Asset == symbol.QuoteAsset))
                {
                    result.Add(symbol);
                }
            }

            return result;
        }

    }
}
