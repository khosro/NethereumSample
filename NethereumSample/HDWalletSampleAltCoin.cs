﻿using NBitcoin;
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
    public class HDWalletSampleAltCoin
    {
        public static void MasterPublicKeyAndPrivateKeyGen()
        {
            string words = "raise gadget source kingdom suit indicate draft prison sister evoke cheese mom";

            words = "cotton maid write chase short daring regular ensure tobacco nature teach choose";


            //MasterPublicKeyAndPrivateKey("m/44'/60'/0'/0", Network.Main, words, null);

            //MasterPublicKeyAndPrivateKey("m/44'/0'/0'", Network.RegTest, words, null);

            //  MasterPublicKeyAndPrivateKey1(Network.RegTest, words, null);

            CorrectOne("m/44'/0'/0'", Network.RegTest, words, null);

            //CorrectOne("m/44'/0'/0'", Network.Main, words, null);

        }


        static void CorrectOne(string basePath, Network net, string words, string password)
        {
            //https://stackoverflow.com/questions/46550818/nbitcoin-and-mnemonic-standards
            Mnemonic mnemo = new Mnemonic(words, Wordlist.English);
            ExtKey hdroot = mnemo.DeriveExtKey();
            for (int i = 0; i < 10; i++)
            {
                var firstprivkey = hdroot.Derive(new NBitcoin.KeyPath(basePath + "/0/" + i.ToString()));
                var firstpubKey = firstprivkey.Neuter().PubKey;
                var privateKey = firstprivkey.Neuter().GetWif(net);
                var address = firstpubKey.GetAddress(ScriptPubKeyType.Legacy, net).ToString();
                Console.WriteLine($"{ net.ToString() } , public key : { address} , privateKey {privateKey}");
            }


            ExtKey masterKey = hdroot.Derive(new NBitcoin.KeyPath(basePath));
            string masterPublicKey = masterKey.Neuter().GetWif(net).ToString();
            string masterPrivateKey = masterKey.GetWif(net).ToString();
            Console.WriteLine($"");
            Console.WriteLine($"MasterPrivateKey : {net.ToString()} {masterPrivateKey} ");
            Console.WriteLine($"MasterPublicKey  : {net.ToString()} {masterPublicKey} ");
            Console.WriteLine($"");
        }


        private static void MasterPublicKeyAndPrivateKey(string path, Network net, string words, string password)
        {
            //  Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            Mnemonic mnemo = new Mnemonic(words);
            ExtKey extKey = mnemo.DeriveExtKey(password);

            ExtKey masterKey = extKey.Derive(new NBitcoin.KeyPath(path));
            string masterPublicKey = masterKey.Neuter().GetWif(net).ToString();
            string masterPrivateKey = masterKey.GetWif(net).ToString();
            Console.WriteLine($"");
            Console.WriteLine($"MasterPrivateKey : {net.ToString()} {masterPrivateKey} ");
            Console.WriteLine($"MasterPublicKey  : {net.ToString()} {masterPublicKey} ");
            Console.WriteLine($"");

            var drivekey = masterKey.Derive(0);
            string publicKey = masterKey.Neuter().GetWif(net).ToString();
            string privateKey = masterKey.GetWif(net).ToString();

            Console.WriteLine($"");
            Console.WriteLine($"first publicKey : {net.ToString()} {publicKey} ");
            Console.WriteLine($"first privateKey  : {net.ToString()} {privateKey} ");
            Console.WriteLine($"");



        }


        static void MasterPublicKeyAndPrivateKey1(Network net, string words, string password)
        {
            Mnemonic mnemonic = new Mnemonic(words);

            ExtKey fatherKey = mnemonic.DeriveExtKey();

            ExtPubKey pubKey = fatherKey.Neuter();

            string wifStr = pubKey.Derive(0).ToString(net);
            Console.WriteLine(wifStr);
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                BitcoinAddress address1 = pubKey.Derive((uint)i).PubKey.GetAddress(ScriptPubKeyType.Legacy, net);
                Console.WriteLine(address1.ToString());
            }

            //var wiff = "";
            //for (int i = 0; i < 10; i++)
            //{
            //    ExtKey key = fatherKey.Derive((uint)i);
            //    var c = ("Key " + i + " : " + key.PrivateKey.GetBitcoinSecret(net).GetAddress(ScriptPubKeyType.Legacy).ToString() + " , wif:" + key.GetWif(net));
            //    var c1 = ("Key " + i + " : " + key.PrivateKey.GetBitcoinSecret(net).GetAddress(ScriptPubKeyType.Segwit).ToString() + " , wif:" + key.GetWif(net));
            //    var c2 = ("Key " + i + " : " + key.PrivateKey.GetBitcoinSecret(net).GetAddress(ScriptPubKeyType.SegwitP2SH).ToString() + " , wif:" + key.GetWif(net));
            //    Console.WriteLine(c);
            //    Console.WriteLine(c1);
            //    Console.WriteLine(c2);
            //}
        }
    }
}