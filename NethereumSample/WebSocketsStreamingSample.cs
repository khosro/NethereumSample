using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NethereumSample
{
    public class WebSocketsStreamingSample
    {
        static StreamingWebSocketClient _streamingWebSocketClient;
        static EthNewPendingTransactionObservableSubscription PendingTransactionsSubscription;

        static void CreateWebSocketClient(string _websocketUrl)
        {
            _streamingWebSocketClient = new StreamingWebSocketClient(_websocketUrl);
            PendingTransactionsSubscription = new EthNewPendingTransactionObservableSubscription(_streamingWebSocketClient);
            Console.WriteLine($"Eth WebSocketsStreamingSample CreateWebSocketClient Init ...............");
        }

        static void Subscribe()
        {
            _streamingWebSocketClient.StartAsync().Wait();
            PendingTransactionsSubscription.SubscribeAsync().Wait();
            Console.WriteLine($"Eth WebSocketsStreamingSample  Subscribe .............");
        }

        static public void ConnectToEthereumD(string _websocketUrl = "ws://localhost:8547")
        {
            CreateWebSocketClient(_websocketUrl);

            PendingTransactionsSubscription.GetSubscribeResponseAsObservable().Subscribe(subscriptionId =>
                    Console.WriteLine("Eth subscriptionId : " + subscriptionId)
            );

            PendingTransactionsSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async transactionHash =>
                  Console.WriteLine("Eth transactionHash : " + transactionHash)
              );

            Subscribe();

            Console.WriteLine($"Connect to ConnectToEthereumD");
        }
    }
}
