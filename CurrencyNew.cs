using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaxfulTradesTracker
{
    public partial class CurrencyNew
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

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

        [JsonProperty("min_trade_amount_usd")]
        public string MinTradeAmountUsd { get; set; }
    }
}

