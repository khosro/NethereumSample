using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nito.AsyncEx;
using System;
using System.IO;

namespace NethereumSample
{
    public class TransactionSend
    {
        static string GetPrivateKey()
        {
            string keyStoreLocation = @"D:\1.New Project\BitCoin\Ethereum\devnet\node1\keystore\UTC--2019-08-28T05-04-42.166107700Z--a229990baed6c95b2a4845a646df23806394688d";
            string password = "p1";

            var acccount = Account.LoadFromKeyStore(File.ReadAllText(keyStoreLocation), password);

            return acccount.PrivateKey;
        }

        static async System.Threading.Tasks.Task<string> TransferAsync(TransactionInput transactionInput = null)
        {
            string privateKey = "0x601a52e4acbcf3a76df3e47e341e545711d4049ae0b35e8edbb5de875a90e717";

            string currentUrl = "http://127.0.0.1:8501";
            if (transactionInput == null)
            {
                transactionInput = new TransactionInput
                {
                    Value = new HexBigInteger(Web3.Convert.ToWei(.01)),
                    To = "0x8C3Aa059ed7358FdEB98C65C54beB2a470b4c74b",
                    From = "0xa229990baeD6C95B2A4845a646dF23806394688D"
                };
            }

            transactionInput.GasPrice = new HexBigInteger(Web3.Convert.ToWei(1, UnitConversion.EthUnit.Gwei));

            var web3 = new Web3(new Account(privateKey), currentUrl);
            var txnHash = await web3.Eth.TransactionManager.SendTransactionAsync(transactionInput).ConfigureAwait(false);
            return txnHash;
        }

    }
}
