/* Purpose: This is a simple application that acts as an ISBM publication Consumer.
 *          It demonstrates the idea of using an ISBM Client Adapter to read publication 
 *          from an ISBM Server Adapter. It should be interoperable with any ISBM compatible
 *          adapters regardless of the actual service bus that delivers the messages.  
 *          
 * Author: Claire Wong
 * Date Created:  2022/08/15
 * 
 * (c) 2022
 * This code is licensed under MIT license
 * 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;
using RapidRedPanda.ISBM.ClientAdapter.EndpointOptions;

namespace ISBM20ConsumerRequestTestCSharp
{
    public partial class Form1 : Form
    {
        ConsumerRequestService myConsumerRequestService = new ConsumerRequestService();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string bodFilePath = AppDomain.CurrentDomain.BaseDirectory + "BODs\\GetMeasurements.json";
            textBoxBODRequest.Text = File.ReadAllText(bodFilePath);
        }

        private async void buttonOpenSession_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            myConsumerRequestService.Credential.Username = textBoxUserName.Text;
            myConsumerRequestService.Credential.Password = textBoxPassword.Text;

            OpenConsumerRequestSessionOptions myOpenConsumerRequestSessionOptions = new OpenConsumerRequestSessionOptions();

            // With Listener
            myOpenConsumerRequestSessionOptions.ListenerURL = "http://127.0.0.1:8080";
            OpenConsumerRequestSessionResponse myOpenSubscriptionSessionResponse = await myConsumerRequestService.OpenConsumerRequestSessionAsync(textBoxHostName.Text, textBoxChannelId.Text, myOpenConsumerRequestSessionOptions, CancellationToken.None);

            //Simple
            //OpenConsumerRequestSessionResponse myOpenSubscriptionSessionResponse = myConsumerRequestService.OpenConsumerRequestSession(textBoxHostName.Text, textBoxChannelId.Text);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myOpenSubscriptionSessionResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myOpenSubscriptionSessionResponse.ReasonPhrase;
            textBoxResponse.Text = myOpenSubscriptionSessionResponse.ISBMHTTPResponse;

            textBoxSessionId.Text = myOpenSubscriptionSessionResponse.SessionID;
        }

        private async void buttonPostRequest_Click(object sender, EventArgs e)
        {
            try
            {
                string mediaType = textBoxMediaType.Text.Trim();

                //Calling ISBM Adapter method
                PostRequestResponse myPostRequestResponse;
                if (string.IsNullOrWhiteSpace(mediaType))
                {
                    myPostRequestResponse = await myConsumerRequestService.PostRequestAsync(textBoxHostName.Text, textBoxSessionId.Text, textBoxTopic.Text, textBoxBODRequest.Text, CancellationToken.None);
                }
                else
                {
                    PostRequestOptions myPostRequestOptions = new PostRequestOptions();
                    myPostRequestOptions.Expiry = "P2D";
                    myPostRequestOptions.MediaType = mediaType;
                    myPostRequestResponse = await myConsumerRequestService.PostRequestAsync(textBoxHostName.Text, textBoxSessionId.Text, textBoxTopic.Text, textBoxBODRequest.Text, myPostRequestOptions, CancellationToken.None);
                }

                //ISBM Adapter Response
                textBoxStatusCode.Text = myPostRequestResponse.StatusCode.ToString();
                textBoxReasonPhrase.Text = myPostRequestResponse.ReasonPhrase;
                textBoxResponse.Text = myPostRequestResponse.ISBMHTTPResponse;

                textBoxMessageId.Text = myPostRequestResponse.MessageID;
                textBoxRequestMessageId.Text = myPostRequestResponse.MessageID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Post Request Failed");
            }
        }

        private async void buttonCloseSession_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            CloseConsumerRequestSessionResponse myCloseConsumerRequestSessionResponse = await myConsumerRequestService.CloseConsumerRequestSessionAsync(textBoxHostName.Text, textBoxSessionId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myCloseConsumerRequestSessionResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myCloseConsumerRequestSessionResponse.ReasonPhrase;
            textBoxResponse.Text = myCloseConsumerRequestSessionResponse.ISBMHTTPResponse;
        }

        private async void buttonRead_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            ReadResponseResponse myReadResponseResponse = await myConsumerRequestService.ReadResponseAsync(textBoxHostName.Text, textBoxSessionId.Text, textBoxRequestMessageId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myReadResponseResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myReadResponseResponse.ReasonPhrase;
            textBoxResponse.Text = myReadResponseResponse.ISBMHTTPResponse;

            if (myReadResponseResponse.StatusCode == 200)
            {
                textBoxMessageId.Text = myReadResponseResponse.MessageID;
                textBoxBODResponse.Text = myReadResponseResponse.MessageContent;
            }
        }
        private async void buttonRemove_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            RemoveResponseResponse myRemoveResponseResponse = await myConsumerRequestService.RemoveResponseAsync(textBoxHostName.Text, textBoxSessionId.Text, textBoxRequestMessageId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myRemoveResponseResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myRemoveResponseResponse.ReasonPhrase;
            textBoxResponse.Text = myRemoveResponseResponse.ISBMHTTPResponse;

            textBoxBODResponse.Text = "";
            textBoxMessageId.Text = "";
        }

        private async void buttonExpireRequest_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            ExpireRequestResponse myExpireRequestResponse = await myConsumerRequestService.ExpireRequestAsync(textBoxHostName.Text, textBoxSessionId.Text, textBoxRequestMessageId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myExpireRequestResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myExpireRequestResponse.ReasonPhrase;
            textBoxResponse.Text = myExpireRequestResponse.ISBMHTTPResponse;
        }
    }
}
