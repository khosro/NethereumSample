using NBitcoin;
using NBXplorer.DerivationStrategy;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using Nito.AsyncEx;
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
            //SyncStatusSampleRun();

            // var task = Task.Run(async () => await ConfirmationSample.GetConfirmation());

            // HDWalletSampleRun(args);

            WebSocketsStreamingSampleRun();

            Console.ReadLine();
        }

        static void WebSocketsStreamingSampleRun()
        {
            string url = "wss://mainnet.infura.io/ws";
            url = "ws://localhost:8547";

            WebSocketsStreamingSample.ConnectToEthereumD(url);
        }

        static void SyncStatusSampleRun()
        {
            AsyncContext.Run(() => SyncStatusSample.SyncStatus());// Sample to run with AsyncContext
            SyncStatusSample.SyncStatus().GetAwaiter().GetResult();
        }

        static void HDWalletSampleRun(string[] args)
        {
            HDWalletSample.HdWallet(args[0]);

            HDWalletSample.HdWallet1();

            HDWalletSample.Drive();
        }
    }
}
