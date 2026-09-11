/* Purpose: This is a simple application that acts as an ISBM publication Provider.
 *          It demonstrates an IoT device using an ISBM Client Adapter to post-publication
 *          to an ISBM Server Adapter. It is interoperable with any ISBM compatible adapters
 *          regardless of the actual service bus that delivers the messages.
 *          
 * Remarks: 1. This is an UWP project that targets Windows 10 IoT core for deployment.
 *          2. This demo does not close publication session for simpliclty. It behaves as power
 *             loss when the program exits. 
 *          
 * Author: Pak Wong
 * Date Created:  2022/08/23
 * 
 * (c) 2022
 * This code is licensed under MIT license
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Threading;
using Windows.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ISBM20Pi3TestCSharp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static string _hostName = "";
        static string _channelId = "";
        static string _topic = "";
        static string _sessionId = "";

        static Boolean _authentication;
        static string _username = "";
        static string _password = "";

        static string _bodTemplate = "";

        static DispatcherTimer _timerPublish;
        static DispatcherTimer _timerDelayStart;

        static ProviderPublicationService _myProviderPublicationService = new ProviderPublicationService();

        public MainPage()
        {
            this.InitializeComponent();

            _timerDelayStart = new DispatcherTimer(); ;
            _timerDelayStart.Interval = TimeSpan.FromMilliseconds(2000);
            _timerDelayStart.Tick += TimerDelayStart_Tick;
            _timerDelayStart.Start();  

        }

        private async void TimerDelayStart_Tick(object sender, object e)
        {
            _timerDelayStart.Stop();

            await SetConfigurations();
            await GetBODTemplate();

            //Open an Provider Publication Session
            OpenPublicationSessionResponse myOpenPublicationSessionResponse = await _myProviderPublicationService.OpenPublicationSessionAsync(_hostName, _channelId, CancellationToken.None);

            if (myOpenPublicationSessionResponse.StatusCode == 201)
            {
                //SessionID is stored in a class level valuable for repeatedly used in every BPD post publication.
                _sessionId = myOpenPublicationSessionResponse.SessionID;
            }
           
            _timerPublish = new DispatcherTimer();
            _timerPublish.Interval = TimeSpan.FromMilliseconds(5000);
            _timerPublish.Tick += TimerPublish_Tick;
            _timerPublish.Start();
        }

        private async void TimerPublish_Tick(object sender, object e)
        {
            _timerPublish.Stop();

            await PublishBOD();

            _timerPublish.Start();
        }

        private async Task SetConfigurations()
        {
            //Read application configurations from Configs.json 
            string folderName = "Assets";
            string filename = "Configs.json";
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folderName);
            StorageFile file = await folder.GetFileAsync(filename);
            string JsonFromFile = await Windows.Storage.FileIO.ReadTextAsync(file);

            JObject JObjectConfigs = JObject.Parse(JsonFromFile);
            _hostName = JObjectConfigs["hostName"].ToString();
            _channelId = JObjectConfigs["channelId"].ToString();
            _topic = JObjectConfigs["topic"].ToString();

            _authentication = (Boolean)JObjectConfigs["authentication"];
            if (_authentication == true)
            {
                _myProviderPublicationService.Credential.Username = JObjectConfigs["userName"].ToString();
                _myProviderPublicationService.Credential.Password = JObjectConfigs["password"].ToString();
            }
        }

        private async Task GetBODTemplate()
        {
            string folderName = "Assets";
            string filename = "SyncMeasurements.json";

            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folderName);
            StorageFile file = await folder.GetFileAsync(filename);
            string JsonFromFile = await Windows.Storage.FileIO.ReadTextAsync(file);

            _bodTemplate = JsonFromFile;
        }

        private async Task PublishBOD()
        {

            //Create new BOD message from SyncMeasurements use case template
            string bodMessage = FillBODFields(_bodTemplate);

            //Post Publication - BOD message
            PostPublicationResponse myPostPublicationResponse = await _myProviderPublicationService.PostPublicationAsync(_hostName, _sessionId, _topic, bodMessage, CancellationToken.None);

            string MessageId = "";
            if (myPostPublicationResponse.StatusCode == 201)
            {
                MessageId = myPostPublicationResponse.MessageID;
            }
            else
            {
                return;
            }

        }

        private string FillBODFields(string bodTemplate)
        {
            
            //Create a sim value
            //-------------------------------------------------
            Random myRandom = new Random();
            double randomValue = (double)myRandom.Next(1, 100);

            double simMeasurement = randomValue / 100 + 8;
            //-------------------------------------------------
           
            JObject objBOD = JObject.Parse(_bodTemplate);

            objBOD["syncMeasurements"]["applicationArea"]["bODID"] = System.Guid.NewGuid().ToString();
            objBOD["syncMeasurements"]["applicationArea"]["creationDateTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurementLocation"]["UUID"] = "6EEBDB09-39EC-4E40-9FD9-3864FD49B0DB";
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurementLocation"]["shortName"] = "Headwater Elevation";
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurementLocation"]["infoSource"]["UUID"] = "9B005C91-A36F-4D69-A3ED-CC51E78E3A66";

            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["UUID"] = System.Guid.NewGuid().ToString();
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["recorded"]["dateTime"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["infoSource"]["UUID"] = "9B005C91-A36F-4D69-A3ED-CC51E78E3A66";

            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["data"]["measure"]["value"] = simMeasurement;
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["data"]["measure"]["unitOfMeasure"]["UUID"] = "73e8145d-7a92-49ef-9b10-6162a06f7876";
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["data"]["measure"]["unitOfMeasure"]["shortName"] = "Feet";
            objBOD["syncMeasurements"]["dataArea"]["measurements"][0]["measurement"][0]["data"]["measure"]["unitOfMeasure"]["infoSource"]["UUID"] = "cf3f3a8a-1e42-4f15-9288-9cf2241e163d";

            return objBOD.ToString(Newtonsoft.Json.Formatting.None);
        }
    
    }
}
