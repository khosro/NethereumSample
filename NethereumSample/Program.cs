using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace NethereumSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //AsyncContext.Run(Web3);
            //var task = Task.Run(async () => await Web3());

            GetLastedBlockAsync().GetAwaiter().GetResult();

            SyncStatus().GetAwaiter().GetResult();
        }

        private static async Task SyncStatus()
        {
            int numberOfMatched = 0;
            int iteration = 10;
            BigInteger maxDiff = 0;

            string url = "http://192.168.41.93:8510";
            url = "http://127.0.0.1:8511";

            Web3 _web3 = new Web3(url);
            for (int i = 0; i < iteration; i++)
            {
                Console.WriteLine("-------------------------------");
                Console.WriteLine($"Index : {i}");
                try
                {
                    HexBigInteger block = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
                    long lastBlocknumber = await GetLastedBlockAsync();
                    SyncingOutput syncing = await _web3.Eth.Syncing.SendRequestAsync().ConfigureAwait(false);
                    Console.WriteLine("");
                    Console.WriteLine("");
                    if (syncing != null)
                    {
                        Console.WriteLine($"IsSyncing : {syncing.IsSyncing} ,CurrentBlock : {syncing.CurrentBlock} ,HighestBlock : {syncing.HighestBlock} " +
                                $",StartingBlock : {syncing.StartingBlock} ");
                    }
                    else
                    {
                        Console.WriteLine("syncing is null");
                    }
                    Console.WriteLine("");
                    Console.WriteLine($"ropsten lastBlocknumber == {lastBlocknumber}");
                    Console.WriteLine("");
                    Console.WriteLine($"My Node lastBlocknumber == {block.Value} , Url :  {url}");

                    BigInteger currentMaxDiff = lastBlocknumber - block.Value;
                    if (currentMaxDiff < 0)
                    {
                        currentMaxDiff *= -1;
                    }
                    if (currentMaxDiff > maxDiff)
                    {
                        maxDiff = currentMaxDiff;
                    }


                    bool isMatched = lastBlocknumber == block.Value;
                    Console.WriteLine(isMatched);
                    if (isMatched)
                    {
                        ++numberOfMatched;
                    }
                    Console.WriteLine("");

                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                Console.WriteLine("-------------------------------");
            }
            Console.WriteLine($"MaxDiff {maxDiff} ,  NumberOfMatched : {numberOfMatched} , matchedPercent {((decimal)numberOfMatched / iteration) * 100} %");
        }

        private static async Task<long> GetLastedBlockAsync()
        {
            string url = "https://api-ropsten.etherscan.io/api?module=proxy&action=eth_blockNumber&apikey=YourApiKeyToken";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            // Above three lines can be replaced with new helper method below
            // string responseBody = await client.GetStringAsync(uri);
            Eth_BlockNumber eth_BlockNumber = JsonConvert.DeserializeObject<Eth_BlockNumber>(responseBody);
            return eth_BlockNumber.LastBlockNumber;
        }

        public class Eth_BlockNumber
        {
            public string jsonrpc { get; set; }
            public string result { get; set; }
            public long LastBlockNumber => Convert.ToInt64(result, 16);
            public int id { get; set; }
        }


    }
}
