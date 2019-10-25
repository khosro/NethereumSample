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
using NBitcoin.Altcoins;
namespace NethereumSample
{
    public class HDWalletSampleAltCoin
    {
        public static void MasterPublicKeyAndPrivateKeyGen()
        {
            string words = "raise gadget source kingdom suit indicate draft prison sister evoke cheese mom";

            words = "cotton maid write chase short daring regular ensure tobacco nature teach choose";

            words = "wash december flock cinnamon lava battle wreck you tone file easy midnight";

            words = "object sand mutual custom dove ghost display arrest session theme express apple";//test server store mylasttest


            //MasterPublicKeyAndPrivateKey("m/44'/60'/0'/0", Network.Main, words, null);

            //MasterPublicKeyAndPrivateKey("m/44'/0'/0'", Network.RegTest, words, null);

            //  MasterPublicKeyAndPrivateKey1(Network.RegTest, words, null);

            #region Works

            CreateMnemonic();

            HdWalletEthereum(words, Network.TestNet);

            CorrectOne("m/44'/0'/0'", Network.TestNet, words, ScriptPubKeyType.Segwit, null);//BTC This old one

            /* CorrectOne("m/44'/0'/0'", Network.Main, words, ScriptPubKeyType.Segwit, null);//BTC 
             CorrectOne("m/44'/1'/0'", Network.TestNet, words, ScriptPubKeyType.Segwit, null);//BTC

             CorrectOne("m/44'/5'/0'", Dash.Instance.Mainnet, words, ScriptPubKeyType.Legacy, null);//Dash
             CorrectOne("m/44'/1'/0'", Dash.Instance.Testnet, words, ScriptPubKeyType.Legacy, null);//Dash



             CorrectOne("m/44'/2'/0'", Litecoin.Instance.Mainnet, words, ScriptPubKeyType.Segwit, null);//LTC 
            CorrectOne("m/44'/1'/0'", Litecoin.Instance.Testnet, words, ScriptPubKeyType.Segwit, null);//LTC*/


            //GenerateAddress("m/44'/0'/0'", Network.TestNet, words, ScriptPubKeyType.Segwit);

            #endregion

        }

        public static void HdWalletEthereum(string words, Network net)
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

            CorrectOne("m/60'/0'/0'", net, words, ScriptPubKeyType.Legacy, null);

        }

        static void CreateMnemonic()
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

            var mnemonicValue = mnemonic.ToString();

            Console.WriteLine(mnemonicValue);
        }

        static void GenerateAddress(string basePath, Network net, string words, ScriptPubKeyType scriptPubKeyType)
        {
            Mnemonic restoreNnemo = new Mnemonic(words);
            Console.WriteLine("GenerateAddress ########################## Start");
            ExtKey masterKey = restoreNnemo.DeriveExtKey();

            for (int i = 0; i < 10; i++)
            {
                //NBitcoin.KeyPath keypth = new NBitcoin.KeyPath("m/44'/0'/0'/0/" + i);
                NBitcoin.KeyPath keypth = new NBitcoin.KeyPath(basePath + "/0/" + i);
                ExtKey key = masterKey.Derive(keypth);

                string address = key.PrivateKey.PubKey.GetAddress(scriptPubKeyType, net).ToString();
                string privateKey = key.PrivateKey.GetBitcoinSecret(net).ToString();
                Console.WriteLine($" publicKey {address} , privateKey {privateKey} ");
            }
            Console.WriteLine("GenerateAddress ########################## End");
        }


        static void CorrectOne(string basePath, Network net, string words, ScriptPubKeyType scriptPubKeyType, string password)
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($" basePath: {basePath}");
            Console.WriteLine($" words: {words}");
            Console.WriteLine();
            //https://stackoverflow.com/questions/46550818/nbitcoin-and-mnemonic-standards
            Mnemonic mnemo = new Mnemonic(words, Wordlist.English);
            ExtKey hdroot = mnemo.DeriveExtKey();
            for (int i = 0; i < 10; i++)
            {
                var privkey = hdroot.Derive(new NBitcoin.KeyPath(basePath + "/0/" + i.ToString()));
                var publicKey = privkey.Neuter().PubKey;
                var privateKey = privkey.Neuter().GetWif(net);
                var address = publicKey.GetAddress(scriptPubKeyType, net).ToString();
                Console.WriteLine($"{ net.ToString() } , public key : { address} , privateKey {privateKey}");
                Console.WriteLine("");
            }


            ExtKey masterKey = hdroot.Derive(new NBitcoin.KeyPath(basePath));
            string masterPublicKey = masterKey.Neuter().GetWif(net).ToString();
            string masterPrivateKey = masterKey.GetWif(net).ToString();
            Console.WriteLine($"");
            Console.WriteLine($"MasterPrivateKey : {net.ToString()} {masterPrivateKey} ");
            Console.WriteLine($"MasterPublicKey  : {net.ToString()} {masterPublicKey} ");
            Console.WriteLine($"");
            Console.WriteLine("-------------------------------------");


            /* Console.WriteLine("Master key 11111 : " + hdroot.ToString(net));
             ExtPubKey masterPubKey = hdroot.Neuter();
             Console.WriteLine("Master PubKey  " + masterPubKey.ToString(net));
             Console.WriteLine();
             Console.WriteLine();
             */
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
