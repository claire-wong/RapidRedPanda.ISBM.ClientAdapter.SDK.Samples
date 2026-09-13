/* Purpose: This is a simple application that acts as an ISBM Request Provider.
 *          It demonstrates an IoT device using an ISBM Client Adapter to respond request posted by
 *          Request Consumer to an ISBM Server Adapter. It is interoperable with any ISBM compatible
 *          adapters regardless of the actual service bus that delivers the messages.
 *          
 * Remarks: 1. This is a .NET 8 project that targets Raspberry Pi OS for deployment.
 *          2. Please close Request Provider Session to maintain best pracitce as the session is
 *             no longer needed. 
 *          
 * Author: Pak Wong
 * Date Created:  2023/03/09
 * 
 * (c) 2023
 * This code is licensed under MIT license
*/

using System;
using System.Threading;
using System.Threading.Tasks;
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ISBM20Pi3RequestTestNet8
{
    class Program
    {
        static string _hostName = "";
        static string _channelId = "";
        static string _topic = "";
        static string _sessionId = "";
        static string _requestMessageId = "";

        static Boolean _authentication;
        static string _username = "";
        static string _password = "";

        static string _bodTemplate = "";

        static ProviderRequestService _myProviderRequestService = new ProviderRequestService();

        static async Task Main(string[] args)
        {
            SetConfigurations();
            GetBODTemplate();

            Console.WriteLine("Welcome to .NET 8 ISBM 2.0 Client Adapter Test Program!");
            Console.WriteLine("This program will serve as Request Provider.");
            Console.WriteLine(" ");

            Console.WriteLine("Press Enter to open a Provider Request Service Session.....");
            Console.WriteLine(" ");
            string key = Console.ReadKey().Key.ToString();
            if (key == "Enter")
            {
                //Calling ISBM Adapter method
                _myProviderRequestService.Credential.Username = _username;
                _myProviderRequestService.Credential.Password = _password;
                OpenProviderRequestSessionResponse myProviderRequestServiceResponse = await _myProviderRequestService.OpenProviderRequestSessionAsync(_hostName, _channelId, _topic, CancellationToken.None);

                //ISBM Adapter Response
                if (myProviderRequestServiceResponse.StatusCode == 201)
                {
                    _sessionId = myProviderRequestServiceResponse.SessionID;
                    Console.WriteLine("Provider Request Session is opened sucessfully!");
                    Console.WriteLine("Session Id : " + _sessionId);
                    Console.WriteLine(" ");
                }
                else
                {
                    Console.WriteLine("Provider Request Session is failed to open! Please check your settings in Configs.json file.");
                    Console.WriteLine(" ");
                    Console.WriteLine("Press Enter to end program!");
                    key = Console.ReadKey().Key.ToString();
                    if (key == "Enter")
                    {
                        Environment.Exit(0);
                    }
                }
            }

            ReadRequestResponse myReadRequestResponse = new ReadRequestResponse();
            do
            {
                Console.WriteLine("Press Enter to read request.....");
                Console.WriteLine(" ");
                key = Console.ReadKey().Key.ToString();
                if (key == "Enter")
                {
                    //Calling ISBM Adaper method
                    myReadRequestResponse = await _myProviderRequestService.ReadRequestAsync(_hostName, _sessionId, CancellationToken.None);

                    //ISBM Adapter Response
                    if (myReadRequestResponse.StatusCode == 200)
                    {
                        _requestMessageId = myReadRequestResponse.MessageID;
                        Console.WriteLine("Message Id : " + _requestMessageId);
                        Console.WriteLine(" ");
                        Console.WriteLine("Message Content : " + myReadRequestResponse.MessageContent);
                        Console.WriteLine(" ");
                    }
                    else
                    {
                        Console.WriteLine("ISBM HTTP Response : " + myReadRequestResponse.ISBMHTTPResponse);
                        Console.WriteLine(" ");
                    }
                }
            }
            while (myReadRequestResponse.StatusCode != 200);

            Console.WriteLine("Press Enter to post response!");
            Console.WriteLine(" ");
            key = Console.ReadKey().Key.ToString();

            if (key == "Enter")
            {
                //Calling ISBM Adapter method
                PostResponseResponse myPostResponseResponse = await _myProviderRequestService.PostResponseAsync(_hostName, _sessionId, _requestMessageId, _bodTemplate, CancellationToken.None);

                //ISBM Adapter Response
                if (myPostResponseResponse.StatusCode == 201)
                {
                    Console.WriteLine("A resoponse is posted sucessfully!");
                    Console.WriteLine("Message Id : " + myPostResponseResponse.MessageID);
                    Console.WriteLine(" ");
                    JObject JObjectBodTemplate = JObject.Parse(_bodTemplate);
                    Console.WriteLine("Message Content : " + JObjectBodTemplate.ToString(Formatting.Indented));
                    Console.WriteLine(" ");
                }
                else
                {
                    Console.WriteLine("ISBM HTTP Response : " + myReadRequestResponse.ISBMHTTPResponse);
                    Console.WriteLine(" ");
                }
            }

            Console.WriteLine("Press Enter to remove request!");
            Console.WriteLine(" ");
            key = Console.ReadKey().Key.ToString();

            if (key == "Enter")
            {
                //Calling ISBM Adaper method
                RemoveRequestResponse myRemoveRequestResponse = await _myProviderRequestService.RemoveRequestAsync(_hostName, _sessionId, CancellationToken.None);

                //ISBM Adapter Response
                if (myRemoveRequestResponse.StatusCode == 204)
                {
                    Console.WriteLine("The previously read request has been removed!");
                    Console.WriteLine(" ");
                }
                else
                {
                    Console.WriteLine("ISBM HTTP Response : " + myReadRequestResponse.ISBMHTTPResponse);
                    Console.WriteLine(" ");
                }
            }

            Console.WriteLine("Press Enter to close Provider Request Session!");
            Console.WriteLine(" ");
            key = Console.ReadKey().Key.ToString();

            if (key == "Enter")
            {
                //Calling ISBM Adaper method
                CloseProviderRequestSessionResponse myCloseProviderRequestSessionResponse = await _myProviderRequestService.CloseProviderRequestSessionAsync(_hostName, _sessionId, CancellationToken.None);

                //ISBM Adapter Response
                if (myCloseProviderRequestSessionResponse.StatusCode == 204)
                {
                    Console.WriteLine("The Provider Request Session is closed sucessfully!");
                    Console.WriteLine(" ");
                }
                else
                {
                    Console.WriteLine("ISBM HTTP Response : " + myReadRequestResponse.ISBMHTTPResponse);
                    Console.WriteLine(" ");
                }
            }

            Console.WriteLine("Press Enter to end program!");
            key = Console.ReadKey().Key.ToString();
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
            string filename = "ShowMeasurements.json";

            string JsonFromFile = System.IO.File.ReadAllText(filename);

            _bodTemplate = JsonFromFile;
        }
    }
}
