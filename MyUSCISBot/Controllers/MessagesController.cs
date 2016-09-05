using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using MyUSCISBot.Flows;
using MyUSCISBot.Models;
using Newtonsoft.Json;

namespace MyUSCISBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type.ToLowerInvariant() == ActivityTypes.Message.ToLowerInvariant())
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                
                await Conversation.SendAsync(activity, MakeRoot);
                /*
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                await connector.Conversations.ReplyToActivityAsync(reply);*/
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        internal static IDialog<VisaStatusRequest> MakeRoot()
        {
            return Chain.From(() => FormDialog.FromForm(VisaCaseStatusFormBuilder.MakeForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        await context.PostAsync($"If you want to ask about another case number, just type something :)");
                    }
                    catch (FormCanceledException<VisaStatusRequest> e)
                    {
                        string reply;
                        reply = e.InnerException == null ? $"You have canceled the operation!" : "Sorry, an error was thrown. You can try later";
                        await context.PostAsync(reply);
                    }
                });
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}