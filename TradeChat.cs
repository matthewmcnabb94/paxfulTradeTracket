using System;
using System.Collections.Generic;
using System.Text;

namespace PaxfulTradesTracker
{
    /*  {
     "status": "success",
     "timestamp": 1572176490,
     "data": {
        "messages": [
           {
              "timestamp": 1571782122.251,
              "type": "trade_escrow_funded",
              "text": "The buyer is paying 15.81 USD for 0.00198869 BTC (16.11 USD) via Ethereum ETH.  0.00200857 bitcoins (16.27 USD) is now in escrow. It is now safe for the buyer to pay. The buyer will have 1 Hour to make their payment and click on the \"PAID\" button before the trade expires."
           },
           {
              "timestamp": 1571782121,
              "type": "trade_info",
              "text": "Instructions in chat"
           },
           {
              "timestamp": 1571782127.591,
              "type": "msg",
              "text": "Hi",
              "author": "quail3y3"
           },


      */



    class TradeChat
    {

        public Data data { get; set; }
    }



    public class Data
    {
        public List<Messages> messages { get; set; }

    }



    public class Messages
    {
        public int timetamp { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public string author { get; set; }


    }
}
