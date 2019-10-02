using NBitcoin;
using NBXplorer.DerivationStrategy;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            var task = Task.Run(async () => await Confirmation());

            //SyncStatus().GetAwaiter().GetResult();

            // HdWallet(args[0]);

            //  HdWallet1();

            //  Drive();

            Console.ReadLine();
        }


        static async Task Confirmation()
        {
            Web3 web = new Web3("http://localhost:8510");
            string txid = "0x3b8e1902c0ad9ca92da71b4802cfd451ff616a56dc3e69319d51cb0ae91c2a13";
            Nethereum.RPC.Eth.DTOs.Transaction trans = await web.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txid).ConfigureAwait(false);

            var blocknumber = await web.Eth.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
            Console.WriteLine($"Confirmation :  {blocknumber.Value - trans.BlockNumber.Value}");
        }

        #region From BtcPayserver
        static void Drive()
        {
            string masterPublickey = "xpub6EadKVNnpzBqV11L1NtEnxik6awzxxmFmGisfZjW73b45iwJJ3LRuZqHZLJodWzja1F2cPQAAMCRkGuBqWm4N21fMM1groibrd7A5nZfSRJ-[p2sh]";
            //  masterPublickey = "xpub6EadKVNnpzBqV11L1NtEnxik6awzxxmFmGisfZjW73b45iwJJ3LRuZqHZLJodWzja1F2cPQAAMCRkGuBqWm4N21fMM1groibrd7A5nZfSRJ-[p2sh]";
            // masterPublickey = "0xf20cAf38A4B1C322b386f58FcC43eeE6915caa0F";
            //   masterPublickey = "tpubDExFqjCUYG9HkoCBTZsKzt47Ri3exMGKJuKB27neSxLwq1c1NGBX41W7oNPbA1wyMumwDxv2hT2USTPoENcJFTTxQwkzYLNqSahZqqMekzo-[p2sh]";
            // masterPublickey = "tpubDExFqjCUYG9HkoCBTZsKzt47Ri3exMGKJuKB27neSxLwq1c1NGBX41W7oNPbA1wyMumwDxv2hT2USTPoENcJFTTxQwkzYLNqSahZqqMekzo";
            Network network = Network.Main;
            var parser = new DerivationSchemeParser(network);

            var derivationSchemeSettings = new DerivationSchemeSettings(parser.Parse(masterPublickey), network);

            ShowAddresses(derivationSchemeSettings, network);
        }
        private static void ShowAddresses(DerivationSchemeSettings strategy, Network network)
        {
            Dictionary<string, string> keyPath2Addresses = new Dictionary<string, string>();

            var deposit = new NBXplorer.KeyPathTemplates(null).GetKeyPathTemplate(DerivationFeature.Deposit);
            //  var deposit = new NBXplorer.KeyPathTemplates(new NBXplorer.KeyPathTemplate(new NBitcoin.KeyPath("m/44'/60'/0'/0"), new NBitcoin.KeyPath("m/44'/60'/0'/0"))).GetKeyPathTemplate(DerivationFeature.Deposit);
            //var deposit = new NBXplorer.KeyPathTemplates(new NBXplorer.KeyPathTemplate(new NBitcoin.KeyPath("m/44'/60'/0'/0"), new NBitcoin.KeyPath("m/44'/60'/0'/0"))).GetKeyPathTemplate(new NBitcoin.KeyPath("m/44'/60'/0'/0"));

            var line = strategy.AccountDerivation.GetLineFor(deposit);

            //var line1 = strategy.AccountDerivation.GetDerivation(new NBitcoin.KeyPath("m/44/60/0/0"));
            var d = strategy.AccountDerivation.GetDerivation(new NBitcoin.KeyPath("m/44/60/0/0/2")).ScriptPubKey.GetDestinationAddress(network).ToString();
            Console.WriteLine("d ::: " + d);
            for (int i = 0; i < 10; i++)
            {
                var address = line.Derive((uint)i);
                string keyPath = deposit.GetKeyPath((uint)i).ToString();
                string destinationAddress = address.ScriptPubKey.GetDestinationAddress(network).ToString();
                Console.WriteLine($" keyPath {keyPath} , destinationAddress {destinationAddress}");
            }
        }
        #endregion

        #region Ethereum Hd Wallet
        private static void HdWallet(string words)
        {
            string password = null;
            words = words.Trim();
            Console.WriteLine($"Words : {words}");
            Wallet wallet1 = new Wallet(words, password);
            Console.WriteLine($"Path : {wallet1.Path}");
            for (int i = 0; i < 10; i++)
            {
                Account account = wallet1.GetAccount(i);
                Console.WriteLine("Account index : " + i + " - Address : " + account.Address + " - Private key : " + account.PrivateKey);
            }

            MasterPublicKeyAndPrivateKey(Network.Main, words, password);
            MasterPublicKeyAndPrivateKey(Network.TestNet, words, password);
            MasterPublicKeyAndPrivateKey(Network.RegTest, words, password);
        }

        private static void MasterPublicKeyAndPrivateKey(Network net, string words, string password)
        {
            //  Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            Mnemonic mnemo = new Mnemonic(words);
            ExtKey extKey = mnemo.DeriveExtKey(password);

            ExtKey masterKey = extKey.Derive(new NBitcoin.KeyPath("m/44'/60'/0'/0"));
            string masterPublicKey = masterKey.Neuter().GetWif(net).ToString();
            string masterPrivateKey = masterKey.GetWif(net).ToString();


            Console.WriteLine($"");
            Console.WriteLine($"MasterPrivateKey : {net.ToString()} {masterPrivateKey} ");
            Console.WriteLine($"MasterPublicKey  : {net.ToString()} {masterPublicKey} ");
            Console.WriteLine($"");
        }
        #endregion


        #region  From Nethereum 

        static void HdWallet1()
        {
            ExtKey d = new ExtKey();


            string Path = "m/44'/60'/0'/0/x";
            string words = "";
            var mneumonic = new Mnemonic(words);
            var seed = mneumonic.DeriveSeed("123456@a").ToHex();
            var masterKey = new ExtKey(seed);
            var d1 = new Account(masterKey.PrivateKey.ToBytes()).Address;
            for (int i = 0; i < 10; i++)
            {
                var keyPath = new NBitcoin.KeyPath(Path.Replace("x", i.ToString()));
                ExtKey extKey = masterKey.Derive(keyPath);
                byte[] privateKey = extKey.PrivateKey.ToBytes();
                var account = new Account(privateKey);

                Console.WriteLine("Account index : " + i + " - Address : " + account.Address + " - Private key : " + account.PrivateKey);
            }
        }

        #endregion



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
