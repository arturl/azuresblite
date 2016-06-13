using ppatierno.AzureSBLite;
using ppatierno.AzureSBLite.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTHubServiceReceiver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ReceiveDataFromCloud();
        }

        string ConnectionString = "<...>";

        static string eventHubEntity = "<...>";
        string partitionId = "0";
        DateTime startingDateTimeUtc;
        EventHubConsumerGroup group;
        EventHubClient client;
        MessagingFactory factory;
        EventHubReceiver receiver;

        public async Task ReceiveDataFromCloud()
        {
            startingDateTimeUtc = DateTime.UtcNow - TimeSpan.FromHours(24);

            factory = MessagingFactory.CreateFromConnectionString(ConnectionString);

            client = factory.CreateEventHubClient(eventHubEntity);
            group = client.GetDefaultConsumerGroup();
            receiver = group.CreateReceiver(partitionId.ToString(), startingDateTimeUtc);

            try
            {

                while (true)
                {
                    EventData data = receiver.Receive();

                    if (data != null)
                    {
                        var receiveddata = Encoding.UTF8.GetString(data.GetBytes());

                        Debug.WriteLine("{0} {1} {2}", data.SequenceNumber, data.EnqueuedTimeUtc.ToLocalTime(), receiveddata);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception {0}", ex);
            }

            receiver.Close();
            client.Close();
            factory.Close();

        }

    }
}
