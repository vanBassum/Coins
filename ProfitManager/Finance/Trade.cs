using STDLib.Extentions;
using System;
using System.Data;
using System.Linq;
using FRMLib.Scope;
using Binance.Net.Objects.Spot.SpotData;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.UserStream;

namespace ProfitManager
{
    public class Trade
    {
        public string ReferenceID { get; set; }
        public DateTime Timestamp { get; set; }
        public string BoughtAsset { get; set; }
        public string SoldAsset { get; set; }
        public decimal BoughtQuantity { get; set; }
        public decimal SoldQuantity { get; set; }



        public Trade()
        {

        }

        public Trade(BinanceTrade binanceTrade, BinanceSymbol binanceSymbol)
        {
            Timestamp = binanceTrade.TradeTime;
            ReferenceID = binanceTrade.Id.ToString();
            if (binanceTrade.IsBuyer)
            {
                BoughtAsset = binanceSymbol.BaseAsset;
                BoughtQuantity = binanceTrade.Quantity;
                SoldAsset = binanceSymbol.QuoteAsset;
                SoldQuantity = binanceTrade.QuoteQuantity;
            }
            else
            {
                SoldAsset = binanceSymbol.BaseAsset;
                SoldQuantity = binanceTrade.Quantity;
                BoughtAsset = binanceSymbol.QuoteAsset;
                BoughtQuantity = binanceTrade.QuoteQuantity;
            }
        }

        public Trade(BinanceOrder binanceOrder, BinanceSymbol binanceSymbol)
        {
            Timestamp = binanceOrder.UpdateTime;
            ReferenceID = binanceOrder.OrderId.ToString();
            if (binanceOrder.Side == Binance.Net.Enums.OrderSide.Buy)
            {
                BoughtAsset = binanceSymbol.BaseAsset;
                BoughtQuantity = binanceOrder.QuantityFilled;
                SoldAsset = binanceSymbol.QuoteAsset;
                SoldQuantity = binanceOrder.QuoteQuantityFilled;
            }
            else
            {
                SoldAsset = binanceSymbol.BaseAsset;
                SoldQuantity = binanceOrder.QuantityFilled;
                BoughtAsset = binanceSymbol.QuoteAsset;
                BoughtQuantity = binanceOrder.QuoteQuantityFilled;
            }
        }

        public Trade(BinanceStreamOrderUpdate binanceOrder, BinanceSymbol binanceSymbol)
        {
            Timestamp = binanceOrder.UpdateTime;
            ReferenceID = binanceOrder.OrderId.ToString();
            if (binanceOrder.Side == Binance.Net.Enums.OrderSide.Buy)
            {
                BoughtAsset = binanceSymbol.BaseAsset;
                BoughtQuantity = binanceOrder.QuantityFilled;
                SoldAsset = binanceSymbol.QuoteAsset;
                SoldQuantity = binanceOrder.QuoteQuantityFilled;
            }
            else
            {
                SoldAsset = binanceSymbol.BaseAsset;
                SoldQuantity = binanceOrder.QuantityFilled;
                BoughtAsset = binanceSymbol.QuoteAsset;
                BoughtQuantity = binanceOrder.QuoteQuantityFilled;
            }
        }

        public decimal GetAmount(string asset)
        {
            if (asset == SoldAsset)
                return -SoldQuantity;
            else if (asset == BoughtAsset)
                return BoughtQuantity;
            return 0;
        }

        public decimal GetPrice()
        {
            decimal price = 0;
            if (BoughtQuantity != 0)
                price = SoldQuantity / BoughtQuantity;
            return price;
        }

        public override string ToString()
        {
            return $"{SoldAsset} => {BoughtAsset}";
        }
    }

}


