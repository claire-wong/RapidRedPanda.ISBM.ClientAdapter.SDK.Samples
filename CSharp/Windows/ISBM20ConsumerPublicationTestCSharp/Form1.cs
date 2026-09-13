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
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;
using RapidRedPanda.ISBM.ClientAdapter.EndpointOptions;

namespace ISBM21ConsumerPublicationTestCSharp
{
    public partial class Form1 : Form
    {
        ConsumerPublicationService myConsumerPublicationService = new ConsumerPublicationService();

        public Form1()
        {
            InitializeComponent();
        }

        private async void buttonOpenSession_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            
            myConsumerPublicationService.Credential.Username = textBoxUserName.Text;
            myConsumerPublicationService.Credential.Password = textBoxPassword.Text;

            OpenSubscriptionSessionOptions myOpenSubscriptionSessionOptions = new OpenSubscriptionSessionOptions();

            //// With Listener
            //myOpenSubscriptionSessionOptions.ListenerURL = "http://127.0.0.1:8080";

            //// With Filter
            //FilterExpression myFilterExpression = new FilterExpression()
            //{
            //    ApplicableMediaTypes = new List<string>
            //    {
            //        "application/json"
            //    },
            //    ExpressionString = new ExpressionString
            //    {
            //        Expression = "$.DataArea.Show.Measurement[?(@.value > 100)]",
            //        Language = "JsonPath",
            //        LanguageVersion = "1.0"
            //    }
            //};

            //myOpenSubscriptionSessionOptions.FilterExpressions.Add(myFilterExpression);


            //OpenSubscriptionSessionResponse myOpenSubscriptionSessionResponse = await myConsumerPublicationService.OpenSubscriptionSessionAsync(textBoxHostName.Text, textBoxChannelId.Text, textBoxTopic.Text, myOpenSubscriptionSessionOptions, CancellationToken.None);

            //Simple
            OpenSubscriptionSessionResponse myOpenSubscriptionSessionResponse = await myConsumerPublicationService.OpenSubscriptionSessionAsync(textBoxHostName.Text, textBoxChannelId.Text, textBoxTopic.Text, CancellationToken.None);
           
            //ISBM Adapter Response
            textBoxStatusCode.Text = myOpenSubscriptionSessionResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myOpenSubscriptionSessionResponse.ReasonPhrase;
            textBoxResponse.Text = myOpenSubscriptionSessionResponse.ISBMHTTPResponse;

            textBoxSessionId.Text = myOpenSubscriptionSessionResponse.SessionID;
        }

        private async void buttonCloseSession_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            CloseSubscriptionSessionResponse myCloseSubscriptionSessionResponse = await myConsumerPublicationService.CloseSubscriptionSessionAsync(textBoxHostName.Text, textBoxSessionId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myCloseSubscriptionSessionResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myCloseSubscriptionSessionResponse.ReasonPhrase;
            textBoxResponse.Text = myCloseSubscriptionSessionResponse.ISBMHTTPResponse;
        }

        private async void buttonRead_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            ReadPublicationResponse myReadPublicationResponse = await myConsumerPublicationService.ReadPublicationAsync(textBoxHostName.Text, textBoxSessionId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myReadPublicationResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myReadPublicationResponse.ReasonPhrase;
            textBoxResponse.Text = myReadPublicationResponse.ISBMHTTPResponse;

            if (myReadPublicationResponse.StatusCode == 200)
            {
                textBoxMessageID.Text = myReadPublicationResponse.MessageID;
                textBoxTopic.Text = myReadPublicationResponse.Topics[0];
                textBoxBOD.Text = myReadPublicationResponse.MessageContent;
            }
        }
        private async void buttonRemove_Click(object sender, EventArgs e)
        {
            //Calling ISBM Adaper method
            RemovePublicationResponse myRemovePublicationResponse = await myConsumerPublicationService.RemovePublicationAsync(textBoxHostName.Text, textBoxSessionId.Text, CancellationToken.None);

            //ISBM Adapter Response
            textBoxStatusCode.Text = myRemovePublicationResponse.StatusCode.ToString();
            textBoxReasonPhrase.Text = myRemovePublicationResponse.ReasonPhrase;
            textBoxResponse.Text = myRemovePublicationResponse.ISBMHTTPResponse;

            textBoxBOD.Text = "";
            textBoxMessageID.Text = "";
        }
    }
}
