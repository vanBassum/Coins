﻿using STDLib.Saveable;
using System.ComponentModel;

namespace ProfitManager
{
    public class Settings : BaseSettingsV2<Settings>
    {
        public string BinanceAPIKEY { get => GetPar("APIKEY"); set => SetPar(value); }
        public string BinanceAPISECRET { get => GetPar("APISECRET"); set => SetPar(value); }
    }



    
}


