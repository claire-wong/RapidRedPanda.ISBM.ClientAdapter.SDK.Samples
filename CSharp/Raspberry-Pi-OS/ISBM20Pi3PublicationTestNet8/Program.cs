/* Purpose: This is a simple application that acts as an ISBM publication Provider.
 *          It demonstrates an IoT device using an ISBM Client Adapter to post-publication
 *          to an ISBM Server Adapter. It is interoperable with any ISBM compatible adapters
 *          regardless of the actual service bus that delivers the messages.
 *          
 * Remarks: 1. This is a .NET 8 project that targets Raspberry Pi OS for deployment.
 *          2. This demo does not close publication session for simpliclty. It behaves as power
 *             loss when the program exits. 
 *          
 * Author: Pak Wong
 * Date Created:  2022/08/31
 * 
 * Version Upgrade
 * 
 * Remarks: 1. The demo now provides a graceful exit for the program. Please follow the on-screen instructions.
 *                                  
 * Modified By : Claire Wong
 * Date Modified : 2023/12/23
 * 
 * (c) 2022
 * This code is licensed under MIT license
*/

using System;
using System.Threading;
using System.Threading.Tasks;
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ISBM20Pi3PublicationTestNet8
{
    class Program
    {
        static string _hostName = "";
        static string _channelId = "";
        static string _topic = "";
        static string _sessionId = "";

        static Boolean _authentication;
        static string _username = "";
        static string _password = "";

        static string _bodTemplate = "";

        static ProviderPublicationService _myProviderPublicationService = new ProviderPublicationService();

        static async Task Main(string[] args)
        {

            SetConfigurations();
            GetBODTemplate();

            _myProviderPublicationService.Credential.Username = _username;
            _myProviderPublicationService.Credential.Password = _password;

            //Open an Provider Publication Session
            OpenPublicationSessionResponse myOpenPublicationSessionResponse = await _myProviderPublicationService.OpenPublicationSessionAsync(_hostName, _channelId, CancellationToken.None);
            Console.WriteLine("Host Address " + _hostName);
            Console.WriteLine("Channel Id " + _channelId);


            if (myOpenPublicationSessionResponse.StatusCode == 201)
            {
                //SessionID is stored in a class level valuable for repeatedly used in every BPD post publication.
                _sessionId = myOpenPublicationSessionResponse.SessionID;
                Console.WriteLine("Publication Session " + _sessionId);
                Console.WriteLine("ISBM 2.0 IoT Device is ready!!");
            }
            else
            {
                Console.WriteLine(myOpenPublicationSessionResponse.StatusCode + " " + myOpenPublicationSessionResponse.ISBMHTTPResponse);
                Console.WriteLine("Please check configurations!!");
            }

            await Task.Delay(1000);

            Boolean continueLoop = true;

            // Start a separate thread for user input
            Thread userInputThread = new Thread(() =>
            {
                Console.WriteLine("Press Enter to stop the publication!");
                Console.ReadLine();
                continueLoop = false;
            });
            userInputThread.Start();

            while (continueLoop)
            {
                await PublishBOD();
                await Task.Delay(5000);
            }

            // Calling ISBM Adaper method
            ClosePublicationSessionResponse myClosePublicationSessionResponse = await _myProviderPublicationService.ClosePublicationSessionAsync(_hostName, _sessionId, CancellationToken.None);

            //ISBM Adapter Response
            if (myClosePublicationSessionResponse.StatusCode == 204)
            {
                Console.WriteLine("The Provider Request Session is closed sucessfully!");
                Console.WriteLine(" ");
            }
            else
            {
                Console.WriteLine("ISBM HTTP Response : " + myClosePublicationSessionResponse.ISBMHTTPResponse);
                Console.WriteLine(" ");
            }

            Console.WriteLine("Press Enter to end program!");
            string key = Console.ReadKey().Key.ToString();

        }

        private static void SetConfigurations()
        {
            //Read application configurations from Configs.json 
            string filename = "Configs.json";

            string JsonFromFile = System.IO.File.ReadAllText(filename);

            JObject JObjectConfigs = JObject.Parse(JsonFromFile);
            _hostName = JObjectConfigs["hostName"].ToString();
            _channelId = JObjectConfigs["channelId"].ToString();
            _topic = JObjectConfigs["topic"].ToString();

            _authentication = (Boolean)JObjectConfigs["authentication"];
            if (_authentication == true)
            {
                _username = JObjectConfigs["userName"].ToString();
                _password = JObjectConfigs["password"].ToString();
            }
        }

        private static void GetBODTemplate()
        {
            string filename = "SyncMeasurements.json";

            string JsonFromFile = System.IO.File.ReadAllText(filename);

            _bodTemplate = JsonFromFile;
        }

        private static async Task PublishBOD()
        {

            //Create new BOD message from SyncMeasurements use case template
            string bodMessage = FillBODFields(_bodTemplate);

            //Post Publication - BOD message
            PostPublicationResponse myPostPublicationResponse = await _myProviderPublicationService.PostPublicationAsync(_hostName, _sessionId, _topic, bodMessage, CancellationToken.None);

            string MessageId = "";
            if (myPostPublicationResponse.StatusCode == 201)
            {
                MessageId = myPostPublicationResponse.MessageID;
                Console.WriteLine("Message " + MessageId + " has been pusblished!!");
            }
            else
            {
                Console.WriteLine(myPostPublicationResponse.StatusCode + " " + myPostPublicationResponse.ISBMHTTPResponse);
            }

        }

        private static string FillBODFields(string bodTemplate)
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
