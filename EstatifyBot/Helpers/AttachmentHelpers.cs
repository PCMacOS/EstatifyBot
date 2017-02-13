using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EstatifyBot.Helpers
{
    public class AttachmentsHelper
    {
        public static Attachment CreateHeroCardAttachment(string actionTitle, string actionText,
         string actionSubtitle, string picURL, IEnumerable<CardAction> actions)
        {
            var card = new HeroCard
            {
                Title = actionTitle,
            };
            if (actionText != null)
                card.Text = actionText;
            if (actionSubtitle != null)
                card.Subtitle = actionSubtitle;

            card.Images = new List<CardImage>
                         {
                                new CardImage
                                {
                                    Url = picURL
                                }
                         };
            card.Buttons = new List<CardAction>(actions);

            return card.ToAttachment();
        }

        public static CardAction CreateCardAction(string title, string value)
        {
            return new CardAction()
            {
                Title = title,
                Value = value,
                Type = ActionTypes.PostBack
            };
        }
    }
}