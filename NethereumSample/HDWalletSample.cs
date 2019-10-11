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
    class HDWalletSample
    {

        #region From BtcPayserver
        public static void Drive()
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
        public static void HdWallet(string words)
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

        #region  From Nethereum  HdWallet1

        public static void HdWallet1()
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
    }
}
