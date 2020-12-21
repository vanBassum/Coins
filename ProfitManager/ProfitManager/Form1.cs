using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.OrderBook;
using STDLib.Saveable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfitManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }
    }

    public class Settings : BaseSettingsV2<Settings>
    {
        public string BinanceAPIKEY { get => GetPar("APIKEY"); set => SetPar(value); }
        public string BinanceAPISECRET { get => GetPar("APISECRET"); set => SetPar(value); }

    }
}
