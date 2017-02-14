using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using EstatifyBot.Helpers;
using System.Net;
using Autofac.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;


namespace EstatifyBot.Dialogs
{
    [LuisModel("LuisAppID", "YourLuisID")]
    [Serializable()]
    public class WelcomeDialog : LuisDialog<object>
    {
        [LuisIntent("Hi")]
        public async Task SayHi(IDialogContext context, LuisResult result)
        {
            //await base.StartAsync(context);
            await context.PostAsync("Hi there! I am EstatifyBot and Im here to help you for anything about Estatify. Ask me something for available listings... :)");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Show Active Estate")]
        public virtual async Task ShowSpace(IDialogContext context, LuisResult result)
        {
            var reply = context.MakeMessage();
            
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetSpaceCardsAttachments();
            await context.PostAsync("Ok! These are our listings...");
            await context.PostAsync(reply);

            context.Wait(MessageReceived);
        }

        private static IList<Attachment> GetSpaceCardsAttachments()
        {
            var jsonUrL = new WebClient().DownloadString("http://estatify.mitzelos.com/all_properties.json");
            var converter = new ExpandoObjectConverter();
            dynamic jsn = JsonConvert.DeserializeObject<ExpandoObject>(jsonUrL, converter);
            List <Attachment> popa = new List<Attachment>();
            for (int i = 0; i < jsn.properties.Count; i++)
            {
                string showReviw = null;
                var review = 0;
                for (int j = 0; j < (jsn.properties[i].review.Count); j++)
                {
                    review += jsn.properties[i].review[j].rating; 
                }
                if (jsn.properties[i].review.Count!=0) review = review/jsn.properties[i].review.Count;
                else review = 0;
                if (review != 0)
                {
                    showReviw = " Review: " + review + "/5,";
                }
                popa.Add(GetHeroCard(
                        jsn.properties[i].listing_name,
                        "Adress: " + jsn.properties[i].address + "," + " Space type: " + jsn.properties[i].space_type +
                        "," + " Dimensions: " + jsn.properties[i].dimensions + "τ.μ," + " Price: " +
                        jsn.properties[i].price + "$," + " Charge per: " + jsn.properties[i].charge_per + "," +
                        " Minimum stay time: " + jsn.properties[0].min_time + "," + showReviw + " Owner: " +
                        jsn.properties[i].owner + ".",
                        jsn.properties[i].summary,
                        new CardImage(url: "http://estatify.mitzelos.com" + jsn.properties[i].photos[0].medium_photo),
                        new CardAction(ActionTypes.OpenUrl, "Go to this space!",
                            value: "http://estatify.mitzelos.com/properties/" + jsn.properties[i].id),
                        new CardAction(ActionTypes.OpenUrl, "Open in map",
                            value: "https://www.google.com/maps?ll=" + jsn.properties[i].latitude + "," + jsn.properties[i].longitude+ "&z=16"),
                        new CardAction(ActionTypes.OpenUrl, "Go to Owner page!",
                            value: "http://estatify.mitzelos.com/users/" + jsn.properties[i].user_id)
                    ));}
            return popa;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I have no idea what you are talking about. Plese resend me something");
            context.Wait(MessageReceived);
        }
        ///////////////////////Carusel///////////////////////
        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardButton1, CardAction cardButton2, CardAction cardButton3)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardButton1, cardButton2, cardButton3 },
            };

            return heroCard.ToAttachment();
        }

        private static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }
    }
}