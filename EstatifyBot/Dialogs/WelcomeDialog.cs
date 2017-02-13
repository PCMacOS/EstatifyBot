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
    [Serializable()]
    public class WelcomeDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            //await base.StartAsync(context);
            await context.PostAsync("Hi there! I am EstatifyBot and Im here to help you for anything about Estatify");
            context.Wait(ShowSpace);
        }

        
        private async Task ShowSpace(IDialogContext context, IAwaitable<object> result)
        {
            var reply = context.MakeMessage();
            
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetSpaceCardsAttachments();
            await context.PostAsync("OK! This is the available spaces");
            await context.PostAsync(reply);

            //context.Wait(MessageReceived);
        }

        private static IList<Attachment> GetSpaceCardsAttachments()
        {
            var jsonUrL = new WebClient().DownloadString("http://estatify.mitzelos.com/all_properties.json");
            var converter = new ExpandoObjectConverter();
            dynamic jsn = JsonConvert.DeserializeObject<ExpandoObject>(jsonUrL, converter);
                var review = 0;
                string showReviw = null;
                if (review != 0)
                {
                    showReviw = " Review: " + review + "/5,";
                }

                return new List<Attachment>()
                {

                    GetHeroCard(
                        jsn.properties[0].listing_name,
                        "Adress: " + jsn.properties[0].address + "," + " Space type: " + jsn.properties[0].space_type +
                        "," + " Dimensions: " + jsn.properties[0].dimensions + "τ.μ," + " Price: " +
                        jsn.properties[0].price + "$," + " Charge per: " + jsn.properties[0].charge_per + "," +
                        " Minimum stay time: " + jsn.properties[0].min_time + "," + showReviw + " Owner: " +
                        jsn.properties[0].owner + ".",
                        jsn.properties[0].summary,
                        new CardImage(url: "http://estatify.mitzelos.com" + jsn.properties[0].photos[0].medium_photo),
                        new CardAction(ActionTypes.OpenUrl, "Go to this space!",
                            value: "http://estatify.mitzelos.com/properties/" + jsn.properties[0].id),
                        new CardAction(ActionTypes.OpenUrl, "Open in map",
                            value: "https://www.google.gr/maps/@" + jsn.properties[0].latitude+ "," +jsn.properties[0].longitude),
                        new CardAction(ActionTypes.OpenUrl, "Go to Owner page!",
                            value: "http://estatify.mitzelos.com/users/" + jsn.properties[0].user_id)
                    ),

                };
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