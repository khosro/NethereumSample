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
    class ConfirmationSample
    {
        //Refer to https://medium.com/pixelpoint/track-blockchain-transactions-like-a-boss-with-web3-js-c149045ca9bf

        public static async Task<BigInteger> GetConfirmation(string txid = "", string url = "http://localhost:8510")
        {
            Web3 web = new Web3(url);

            if (string.IsNullOrWhiteSpace(txid))
                txid = "0x3b8e1902c0ad9ca92da71b4802cfd451ff616a56dc3e69319d51cb0ae91c2a13";

            Nethereum.RPC.Eth.DTOs.Transaction trans = await web.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txid).ConfigureAwait(false);
            var blocknumber = await web.Eth.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
            var confirmation = blocknumber.Value - trans.BlockNumber.Value;
            Console.WriteLine($"Confirmation :  {confirmation}");
            return confirmation;
        }

        /// <summary>
        /// You must call this method periodically whe you get false unitl you get true
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="maxTryNum"></param>
        /// <param name="confirmations"></param>
        /// <returns></returns>
        public static async Task<bool> ConfirmEtherTransaction(string txHash, int maxTryNum = 10, int confirmations = 10)
        {
            //TODO.Found why transaction maybe does not cofirm and return false with certainty and inform user do not call it periodically.
            return await ConfirmEtherTransaction(txHash, maxTryNum, confirmations, 0);
        }

        static async Task<bool> ConfirmEtherTransaction(string txHash, int maxTryNum = 10, int confirmations = 10, int currentTryNum = 0)
        {
            //If confirmation takes long we get stack overflow exception, so that we use maxTryNum and currentTryNum.
            var trxConfirmations = await GetConfirmation(txHash);

            Console.WriteLine("Transaction with hash " + txHash + " has " + trxConfirmations + " confirmation(s)");
            if (trxConfirmations >= confirmations)
            {
                return true;
            }
            else if (maxTryNum < currentTryNum)
            {
                return false;
            }

            currentTryNum++;
            Thread.Sleep(TimeSpan.FromSeconds(5));//TODO.Found the best value
            return await ConfirmEtherTransaction(txHash, maxTryNum, confirmations, currentTryNum);
        }
    }
}
