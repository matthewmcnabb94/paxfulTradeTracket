using System;
using System.Collections.Generic;
using System.Text;

namespace PaxfulTradesTracker
{
    class CompletedTrades
    {


        public Datac data { get; set; }

    }





    public class Datac
    {
        public List<Trades> trades { get; set; }
    }




    public class Trades
    {
        public string trade_hash { get; set; }
        public string payment_method_name { get; set; }
        public string buyer { get; set; }
        public string status { get; set; }
        public string fiat_currency_code { get; set; }
        
        public string completed_at { get; set; }

        public string ended_at { get; set; }

    }
}
