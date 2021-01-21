using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PaxfulTradesTracker
{


    class HmacGenerate
    {
        public string GenerateHMAC(string secret, string payload)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var hmac = new HMACSHA256 { Key = keyBytes };
            var rawSig = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return BitConverter.ToString(rawSig).Replace("-", string.Empty).ToLower();
        }
    }




    class Program
    {



        public static double gbpRateNewAPI;
        public static double newAmount;
        public static double currency;
        public static List<double> TotalR = new List<double>();
        public static List<double> TotalB = new List<double>();
        public static List<double> TotalGbp = new List<double>();
        public static double total, totalBTC, totalPound;


        static void Main(string[] args)
        {

            getTradeHashAndCurrencyCode();
            Console.WriteLine("\n" + "******** Totals for today are: ********");
            Console.WriteLine("Total redeemed = £" + total);
            Console.WriteLine("Total BTC = " + totalBTC);
            Console.WriteLine("Total GBP = £" + totalPound);
            Console.WriteLine("Average price = " + Math.Round(totalPound / total, 3));

            Console.ReadKey();





        }


        

        public static void getTradeHashAndCurrencyCode()
        {


            HmacGenerate hmac = new HmacGenerate();
            //Generate today's date in the format of dd.MM.yyyy
            DateTime dateAndTime = DateTime.Now;
            var date = dateAndTime.ToString("dd.MM.yyyy");
            //Variables apiKey, secret are used for hmac generation, you need to use your secret and api key from user settings
            var apiKey = "LBwT6TBdxODSr1Ai6IPEGTi4ulZljjfw";
            var secret = "xwwynKwwgbj4TYwuUgA1SOIgdHlAEPKG";
            //Variables offerHash, margin are used for this example request, those variables should contain your information
            var page = 1;
            //var margin = 50;
            //Variables body and theKey is used for request body, theKey holds generate hmac
            var body = "apikey=" + apiKey + "&nonce=" + date + "&page=" + page; //+ "&margin=" + margin;
            var theKey = "&apiseal=" + hmac.GenerateHMAC(secret, body);

            // Create a request using a URL that can receive a post.
            WebRequest request = WebRequest.Create("https://paxful.com/api/trade/completed");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = body + theKey;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.Headers["Accept"] = "application/json";
            request.ContentType = "text/plain";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);



            DeserialiseJSONReadSteamTrades(responseFromServer);





            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();



        }

        public static void DeserialiseJSONReadSteamTrades(string strJSON)
        {
            var steamUser = JsonConvert.DeserializeObject<CompletedTrades>(strJSON);





            List<string> tradeHashs = new List<string>();


            List<string> currencyCodes = new List<string>();

            tradeHashs.Clear();
            currencyCodes.Clear();

            

            string dateNow = DateTime.Now.ToString("yyyy-MM-dd");
            Console.WriteLine("Date checking is: " + dateNow);


            foreach (var num in steamUser.data.trades)
            {

                //Console.WriteLine("(1) Payment method name: " + num.payment_method_name);
                //Console.WriteLine("(2) Status: " + num.status);
                //Console.WriteLine("(3) Completed at: " + num.completed_at);

                if (num.payment_method_name == "Steam Wallet Gift Card" && num.status == "successful" && num.completed_at.Contains(dateNow))
                {
                    tradeHashs.Add(num.trade_hash.ToString());
                    currencyCodes.Add(num.fiat_currency_code.ToString());

                }


            }

            Console.WriteLine("Number of successfull trades for today is: "+tradeHashs.Count);







            foreach (var pair in tradeHashs.Zip(currencyCodes, (a, b) => new { A = a, B = b }))
            {
                addUpTotals(pair.A, pair.B);
            }














        }




        public static void addUpTotals(string th, string cc)
        {

            //Console.WriteLine("Trade hash is: " + th + " Currency code is: "+cc);




            HmacGenerate hmac = new HmacGenerate();
            //Generate today's date in the format of dd.MM.yyyy
            DateTime dateAndTime = DateTime.Now;
            var date = dateAndTime.ToString("dd.MM.yyyy");

            //Variables apiKey, secret are used for hmac generation, you need to use your secret and api key from user settings
            var apiKey = "LBwT6TBdxODSr1Ai6IPEGTi4ulZljjfw";
            var secret = "xwwynKwwgbj4TYwuUgA1SOIgdHlAEPKG";
            //Variables offerHash, margin are used for this example request, those variables should contain your information
            var trade_hash = th;
            //var margin = 50;
            //Variables body and theKey is used for request body, theKey holds generate hmac
            var body = "apikey=" + apiKey + "&nonce=" + date + "&trade_hash=" + trade_hash; //+ "&margin=" + margin;
            var theKey = "&apiseal=" + hmac.GenerateHMAC(secret, body);
            

            // Create a request using a URL that can receive a post.
            WebRequest request = WebRequest.Create("https://paxful.com/api/trade-chat/get");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = body + theKey;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.Headers["Accept"] = "application/json";
            request.ContentType = "text/plain";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);



            DeserialiseJSONTradeChat(responseFromServer, cc);





            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
        }


        public static void DeserialiseJSONTradeChat(string strJSON, string currencyCode)
        {
            var paxful = JsonConvert.DeserializeObject<TradeChat>(strJSON);


            foreach (var num in paxful.data.messages)
            {
                if (num.type.Equals("trade_escrow_funded"))
                {
                    //Console.WriteLine("Found details of trade" + "\n");

                    //Console.WriteLine("Details are: " + "\n" + num.text.ToString());

                    var currency = currencyCode;

                    //Console.WriteLine("Currency code from method is: " + currency);

                    var indexOfC = num.text.ToString().IndexOf(currency);

                    //Console.WriteLine("Index positon after: "+indexOfC);


                    var indexOfBefore = num.text.ToString().IndexOf("ing") +3;

                    //Console.WriteLine("Index positon before: " + indexOfBefore);


                    //Console.WriteLine("Total redeemed is: "+ num.text.ToString().Substring(indexOfBefore, indexOfC - indexOfBefore));
                    String totalR = num.text.ToString().Substring(indexOfBefore, indexOfC - indexOfBefore).Trim();
                    //Console.WriteLine("Length is: "+ totalR.Length);

                    double TotalRedeemed = Convert.ToInt64(Convert.ToDouble(totalR));


                    double currencyRate = newCurrency(currencyCode);



                    double gbp = getLatestCurrencyRates();



                    var indexBeforeTotalBTC = num.text.ToString().IndexOf("Card.") +6;

                    var indexAfterTotalBTC = num.text.ToString().IndexOf("BTC", num.text.ToString().IndexOf("BTC") + 1);


                    double TotalBTC = double.Parse(num.text.ToString().Substring(indexBeforeTotalBTC, indexAfterTotalBTC - indexBeforeTotalBTC), System.Globalization.CultureInfo.InvariantCulture);

                    //Console.WriteLine("Total BTC: " + TotalBTC);

                    

                    TotalB.Add(TotalBTC);





                    var indexBeforeTotalGBP = GetNthOccurrenceOfString(num.text.ToString(), "BTC", 2);

                    //Console.WriteLine("Index before GBP is: " + indexBeforeTotalGBP);


                    var indexAfterTotalGBP = GetNthOccurrenceOfString(num.text.ToString(),currencyCode, 3)-6;

                    //Console.WriteLine("Index after GBP is: " + indexAfterTotalGBP);

                    //Console.WriteLine("Substring for GBP is: "+num.text.ToString().Substring(indexBeforeTotalGBP +5, indexAfterTotalGBP - indexBeforeTotalGBP));

                    string gbpAmount = num.text.ToString().Substring(indexBeforeTotalGBP + 5, indexAfterTotalGBP - indexBeforeTotalGBP);

                    //Console.WriteLine("Size of Stirng is: "+gbpAmount.Length);


                    double TotalRedeemedGBP = double.Parse(num.text.ToString().Substring(indexBeforeTotalGBP + 5, indexAfterTotalGBP - indexBeforeTotalGBP));


                    //TotalGbp.Add(TotalRedeemedGBP);





                    if (currencyCode != "GBP")
                    {
                        double finalPriceAfterSum = TotalRedeemed / currencyRate * gbp;
                        double finalGBPRedeemed = TotalRedeemedGBP / currencyRate * gbp;
                        //Console.WriteLine("Amount is ========================" + finalPriceAfterSum);
                        double x = Math.Truncate(finalPriceAfterSum * 100) / 100;
                        TotalR.Add(x);
                        TotalGbp.Add(finalGBPRedeemed);
                        
                    }
                    else
                    {
                        //Console.WriteLine("Amount is ======================== GBP" + TotalRedeemed);
                        double x = Math.Truncate(TotalRedeemed * 100) / 100;
                        TotalR.Add(x);
                        TotalGbp.Add(TotalRedeemedGBP);
                    }

                    

                    // var TotalRedeemed = num.text.ToString().Trim().Substring(20,4);

                    //Console.WriteLine("Total redeemed is: "+TotalRedeemed);
                }
            }

            total = Math.Truncate(TotalR.Sum() * 100) / 100;
            totalBTC = Math.Round(TotalB.Sum(), 4);
            totalPound = Math.Truncate(TotalGbp.Sum() * 100) / 100;







        }

        public static int GetNthOccurrenceOfString(string s, string c, int occ)
        {
            return String.Join(c.ToString(), s.Split(new string[] { c }, StringSplitOptions.None).Take(occ)).Length;
        }



        public static double newCurrency(String cCode)
        {

            try
            {

                //Console.WriteLine("New currency code is from new API ///////////////////////////////////////// ---------------------> " + cCode);
                HmacGenerate hmac = new HmacGenerate();
                //Generate today's date in the format of dd.MM.yyyy
                DateTime dateAndTime = DateTime.Now;
                var date = dateAndTime.ToString("dd.MM.yyyy");

                var apiKey = "LBwT6TBdxODSr1Ai6IPEGTi4ulZljjfw";
                var secret = "xwwynKwwgbj4TYwuUgA1SOIgdHlAEPKG";

                //var offer_hash = "xPzozn7QBoB";
                //var margin = marginRate;

                var body = "apikey=" + apiKey + "&nonce=" + date; //+ "&offer_hash=" + offer_hash + "&margin=" + margin;
                var theKey = "&apiseal=" + hmac.GenerateHMAC(secret, body);

                // Create a request using a URL that can receive a post.
                WebRequest request = WebRequest.Create("https://paxful.com/api/currency/rates");
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Create POST data and convert it to a byte array.
                string postData = body + theKey;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentType property of the WebRequest.


                request.ContentType = "application/text";
                request.ContentType = "Accept: application/json";
                request.ContentType = "text/plain";
                request.Headers["Accept"] = "application/json";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();

                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();


                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);


                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                newAmount = DeserialiseJSONTradeStatusCurrencyCode(responseFromServer, cCode);

                //Console.WriteLine("///////////////////////////////////////// new amount back from API is ------->" + newAmount);




                // Display the content.
                //Console.WriteLine("\n" + "rates are = :" + responseFromServer);

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

            }
            catch (WebException e)
            {
                if (e.GetType() == typeof(WebException))
                {
                    var webException = (WebException)e;
                    using (var writer = new StreamReader(webException.Response.GetResponseStream()))
                        Console.WriteLine("EXECEPTION IS *&^^*&*&*&*: " + writer.ReadToEnd());
                }
            }

            return newAmount;


        }



        private static double DeserialiseJSONTradeStatusCurrencyCode(string strJSON, string currencyCode)
        {

            


            var currencyJSON = JsonConvert.DeserializeObject<CurrencyNew>(strJSON);

            //double currency;








            foreach (var rate in currencyJSON.Data)
            {
                if (rate.Value.Code.Equals(currencyCode))
                {
                    currency = Convert.ToDouble(rate.Value.RateUsd);
                    //Console.WriteLine("Curency code did match API, rate is: /////////////////////////////////////////////////////////////////////////////// " + currency);

                }
                else
                {
                    // Console.WriteLine("Currency Code did not find API /////////////////////////////////////////////////////////////////////////////////////");
                }

            }




            //double gbpRate = getLatestCurrencyRates();

            //Console.WriteLine("GBP rate back from old API is ~@~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~: "+gbpRate);


            return currency;




        }























        public static double getLatestCurrencyRates()
        {

            //Console.WriteLine("USD amount into method is -------------------------------------------------------> "+USDAmount);

            string html = string.Empty;
            string url = @"https://api.exchangeratesapi.io/latest?base=USD";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
                gbpRateNewAPI = DeserialiseJSONGBPRate(html);
            }

            //Console.WriteLine("New GBP from OLDER API is @@@@@@@@@@@@@@@@@@ " + gbpRateNewAPI);

            double gbpFinalRate = gbpRateNewAPI;

            //Console.WriteLine("USD converted to GBP from new API is ################################ " + gbpFinalRate);

            return gbpFinalRate;




        }

        public static double DeserialiseJSONGBPRate(string strJSON)
        {
            var currencyJSON = JsonConvert.DeserializeObject<Currency>(strJSON);


            double gbpRate = currencyJSON.rates.GBP;

            return gbpRate;



        }











    }

    

}
    

