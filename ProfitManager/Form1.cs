using CryptoExchange.Net.OrderBook;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using FRMLib;
using System.ComponentModel;
using STDLib.Serializers;
using System.Diagnostics;
using STDLib.Saveable;
using Binance.Net.Objects.Spot.UserStream;
using Binance.Net.Objects.Spot.MarketStream;
using ProfitManager.Finance;
using FRMLib.Scope;
using Trace = FRMLib.Scope.Trace;
using CoinGecko.Interfaces;
using CoinGecko.Clients;
using STDLib.Math;
using STDLib.Extentions;
using FRMLib.Scope.Controls;
using System.Collections;

namespace ProfitManager
{
    public partial class Form1 : Form
    {
        ScopeController scopeController = new ScopeController();
        BindingList<Balance> balances = new BindingList<Balance>();
        SaveableBindingList<Trade> trades = new SaveableBindingList<Trade>("data\\trades.json");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.Load();
            BinanceAPI.Init();
            trades.Load();

            AddWithdrawalsAndDeposits();
            AddFIAT();
            ParseBinanceOrders();
            ParseBinanceStreamOrders();
            CalcBalances();

            trades.Save();

            dataGridView1.DataSource = trades;
            dataGridView2.DataSource = balances;
            scopeView1.DataSource = scopeController;
            markerView1.DataSource = scopeController;
            traceView1.DataSource = scopeController;

            dataGridView1.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            dataGridView2.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
            //DoScope();
        }

        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(sender is DataGridView dataGridView)
            {
                if(dataGridView.DataSource is IList list)
                {
                    SortOrder order = dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection;
                    if (order == SortOrder.Ascending)
                        order = SortOrder.Descending;
                    else
                        order = SortOrder.Ascending;

                    dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = order;
                    Type type = list.GetType().GetGenericArguments().Single();
                    PropertyInfo pi = type.GetProperty(dataGridView.Columns[e.ColumnIndex].DataPropertyName);
                    list.OrderBy(pi, order == SortOrder.Ascending);
                }
            }
        }

        void CalcBalances()
        {
            balances.Clear();
            foreach (Trade trade in trades)
            {
                AccumilateBalance(trade.BoughtAsset, trade.BoughtQuantity);
                AccumilateBalance(trade.SoldAsset, -trade.SoldQuantity);
            }
        }

        void AccumilateBalance(string asset, decimal value)
        {
            if (asset != null)
            {
                Balance b = balances.FirstOrDefault(a => a.Asset == asset);
                if (b == null)
                    balances.Add(b = new Balance() { Asset = asset });
                b.Amount += value;
            }
        }
        


        public void ParseBinanceOrders()
        {
            var binanceOrders = BinanceAPI.Instance.GetAllOrdersBruteForce();
            var symbols = BinanceAPI.Instance.GetAllSymbols();

            foreach (var order in binanceOrders.Where(a=>a.Status == Binance.Net.Enums.OrderStatus.Filled))
            {
                var symbol = symbols.FirstOrDefault(a=>a.Name == order.Symbol);
                Trade trade = new Trade(order, symbol);
                AddTrade(trade);
            }
        }

        public void ParseBinanceStreamOrders()
        {
            LogList<BinanceStreamOrderUpdate> binanceStreamOrderUpdates = new LogList<BinanceStreamOrderUpdate>("temp\\OrderUpdates.json");
            File.Delete(binanceStreamOrderUpdates.LogFile);
            File.Copy(@"V:\vanBassum\BinanceSync\OrderUpdates.json", binanceStreamOrderUpdates.LogFile);
            List<BinanceStreamOrderUpdate> orderUpdates = binanceStreamOrderUpdates.ReadAll();


            foreach(BinanceStreamOrderUpdate orderUpdate in orderUpdates.Where(a=>a.Status == Binance.Net.Enums.OrderStatus.Filled))
            {
                var symbols = BinanceAPI.Instance.GetAllSymbols();
                var symbol = symbols.FirstOrDefault(a => a.Name == orderUpdate.Symbol);
                Trade trade = new Trade(orderUpdate, symbol);
                AddTrade(trade);
            }
        }


        public void AddWithdrawalsAndDeposits()
        {
            AddTrade(new Trade { ReferenceID = "With_01", Timestamp = DateTime.Parse("2020-12-27 23:14:34"), SoldAsset = "USDT" , SoldQuantity = (decimal)113 });
            AddTrade(new Trade { ReferenceID = "With_02", Timestamp = DateTime.Parse("2020-12-23 12:10:15"), SoldAsset = "TWT"  , SoldQuantity = (decimal)99.5 });
            AddTrade(new Trade { ReferenceID = "With_03", Timestamp = DateTime.Parse("2020-12-12 17:01:36"), SoldAsset = "USDT" , SoldQuantity = (decimal)77 });
            AddTrade(new Trade { ReferenceID = "With_04", Timestamp = DateTime.Parse("2020-10-28 16:41:35"), SoldAsset = "ETH"  , SoldQuantity = (decimal)0.73391 });
            AddTrade(new Trade { ReferenceID = "With_05", Timestamp = DateTime.Parse("2020-10-16 12:49:40"), SoldAsset = "USDT" , SoldQuantity = (decimal)58.268973 });
            AddTrade(new Trade { ReferenceID = "With_06", Timestamp = DateTime.Parse("2020-10-15 13:24:38"), SoldAsset = "COTI" , SoldQuantity = (decimal)3318.71678 });
            AddTrade(new Trade { ReferenceID = "With_07", Timestamp = DateTime.Parse("2020-09-29 20:13:30"), SoldAsset = "COCOS", SoldQuantity = (decimal)158038.6362 });

            AddTrade(new Trade { ReferenceID = "Depo_01", Timestamp = DateTime.Parse("2020-12-12 16:20:26"), SoldAsset = "ETH", SoldQuantity = (decimal)0.09683326 });
            AddTrade(new Trade { ReferenceID = "Depo_02", Timestamp = DateTime.Parse("2020-12-12 16:18:08"), SoldAsset = "USDT", SoldQuantity = (decimal)161.965317 });
            AddTrade(new Trade { ReferenceID = "Depo_03", Timestamp = DateTime.Parse("2020-11-30 20:04:21"), SoldAsset = "ETH", SoldQuantity = (decimal)0.05974283 });
            AddTrade(new Trade { ReferenceID = "Depo_04", Timestamp = DateTime.Parse("2020-11-18 18:48:45"), SoldAsset = "COTI", SoldQuantity = (decimal)3310 });
        }

        public void AddFIAT()
        {
            AddTrade(new Trade { ReferenceID = "01", Timestamp = DateTime.Parse("2020-12-25 13:22:46"), BoughtAsset = "USDT", BoughtQuantity = (decimal)243.184019, SoldAsset = "EUR", SoldQuantity = (decimal)200.00 });
            AddTrade(new Trade { ReferenceID = "02", Timestamp = DateTime.Parse("2020-12-07 19:54:06"), BoughtAsset = "USDT", BoughtQuantity = (decimal)236.66606, SoldAsset = "EUR", SoldQuantity = (decimal)200.00 });
            AddTrade(new Trade { ReferenceID = "03", Timestamp = DateTime.Parse("2020-11-11 14:08:55"), BoughtAsset = "USDT", BoughtQuantity = (decimal)114.647497, SoldAsset = "EUR", SoldQuantity = (decimal)100.00 });
            AddTrade(new Trade { ReferenceID = "04", Timestamp = DateTime.Parse("2020-10-28 15:36:52"), BoughtAsset = "ETH", BoughtQuantity = (decimal)0.73891, SoldAsset = "EUR", SoldQuantity = (decimal)250.00 });
            AddTrade(new Trade { ReferenceID = "05", Timestamp = DateTime.Parse("2020-10-18 12:08:57"), BoughtAsset = "BTC", BoughtQuantity = (decimal)0.010207, SoldAsset = "EUR", SoldQuantity = (decimal)100.00 });
            AddTrade(new Trade { ReferenceID = "06", Timestamp = DateTime.Parse("2020-10-16 10:40:00"), BoughtAsset = "USDT", BoughtQuantity = (decimal)58.268888, SoldAsset = "EUR", SoldQuantity = (decimal)50.00 });
            AddTrade(new Trade { ReferenceID = "07", Timestamp = DateTime.Parse("2020-10-09 19:03:43"), BoughtAsset = "USDT", BoughtQuantity = (decimal)115.106757, SoldAsset = "EUR", SoldQuantity = (decimal)100.00 });
            AddTrade(new Trade { ReferenceID = "08", Timestamp = DateTime.Parse("2020-09-29 12:00:57"), BoughtAsset = "USDT", BoughtQuantity = (decimal)56.942678, SoldAsset = "EUR", SoldQuantity = (decimal)50.00 });
            AddTrade(new Trade { ReferenceID = "09", Timestamp = DateTime.Parse("2020-09-29 11:56:30"), BoughtAsset = "USDT", BoughtQuantity = (decimal)56.94857, SoldAsset = "EUR", SoldQuantity = (decimal)50.00 });
            AddTrade(new Trade { ReferenceID = "10", Timestamp = DateTime.Parse("2020-09-23 15:42:31"), BoughtAsset = "VET", BoughtQuantity = (decimal)9528, SoldAsset = "EUR", SoldQuantity = (decimal)100.00 });
        }


        public void AddTrade(Trade trade)
        {
            if (!trades.Any(a => a.ReferenceID == trade.ReferenceID))
                trades.Add(trade);
        }

        long Unix(DateTime foo)
        {
            return ((DateTimeOffset)foo).ToUnixTimeSeconds();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(scopeController.Traces.Count == 0)
            {
                DoScope();
            }
        }


        void DoScope()
        {
            ICoinGeckoClient client = CoinGeckoClient.Instance;
            var coinList = client.CoinsClient.GetCoinList().Result;
            scopeController.Clear();
            trades.SortBy(a => a.Timestamp);
            DateTime from = trades.First().Timestamp;
            List<string> assets = new List<string>();
            foreach (Trade t in trades)
            {
                assets.Add(t.BoughtAsset);
                assets.Add(t.SoldAsset);
            }
            var uniqueAssets = assets.Distinct();
            foreach (string asset in uniqueAssets.Where(a=>a != null))
            {
                Trace trace = new Trace()
                {
                    Name = asset,
                    DrawStyle = Trace.DrawStyles.NonInterpolatedLine,
                    Pen = Palettes.DistinctivePallet[scopeController.Traces.Count],
                };
                scopeController.Traces.Add(trace);
                var filteredTrades = trades.Where(a => a.SoldAsset == asset || a.BoughtAsset == asset);
                var coin = coinList.Where(a => a.Symbol.ToLower() == asset.ToLower()).FirstOrDefault();
                if (coin != null)
                {
                    var hist = client.CoinsClient.GetMarketChartRangeByCoinId(coin.Id, "usd", Unix(from).ToString(), Unix(DateTime.Now).ToString()).Result;
                    foreach (var t in hist.Prices)
                    {
                        DateTime time = UnixTimeStampToDateTime(t[0] / 1000);
                        var v = filteredTrades.Where(a => a.Timestamp < time).Sum(a => a.GetAmount(asset));
                        trace.Points.Add(time.Ticks, t[1] * (double)v);
                    }

                    foreach(Trade t in filteredTrades)
                    {
                        LinkedMarker marker = new LinkedMarker(trace, t.Timestamp.Ticks) { Text = t.ToString() };

                        scopeController.Markers.Add(marker);
                    }

                }
                scopeView1.AutoScaleTraceKeepZero(trace);
            }

            scopeView1.AutoScaleHorizontal();
        }
    }

}


