using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaxfulTradesTracker
{
    class CurrencyNew
    {
        [JsonProperty("data")]
        public Dictionary<string, Datum> Data { get; set; }
    }








    public partial class Datum
    {
        [JsonProperty("rate_USD")]
        public double RateUsd { get; set; }

        [JsonProperty("rate_BTC")]
        public string RateBtc { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("copy_from_fiat_currency_id")]
        public long? CopyFromFiatCurrencyId { get; set; }
    }
}
