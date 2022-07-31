using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Collections.Generic;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using MusicBot.Dialogs;

namespace MusicBot
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;
        string strMessage;
        private string strWelcomeMessage = "[Music Bot]";
        
        int i = 0;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync); // 처음 시작할 때 보여줄말
            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(strWelcomeMessage);

            var message = context.MakeMessage();
            var actions = new List<CardAction>();

            actions.Add(new CardAction() { Title = "1. Chart 보기 ", Value = "1", Type = ActionTypes.ImBack });
            actions.Add(new CardAction() { Title = "2. Youtube 영상 보기 ", Value = "2", Type = ActionTypes.ImBack });

            message.Attachments.Add(
                new HeroCard { Title = "두 버튼 중 골라주세요 .", Buttons = actions }.ToAttachment()
            );

            await context.PostAsync(message);

            context.Wait(sendMessage);
            
        }

        public async Task sendMessage(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            string select = activity.Text.Trim();

            if (select == "1")
            {
                context.Call(new youtube(), DialogResumeAfter); // new 클래스이름(), 함수명
            }

            else if(select == "2")
            {
                context.Call(new youtube(), DialogResumeAfter);
            }
            else
            {
                strMessage = "잘못 고르셨습니다.";
                await context.PostAsync(strMessage);
                context.Wait(sendMessage);
            }
        }
        
        public async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                strMessage = await result;
                await context.PostAsync(strWelcomeMessage);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Error occurred.....");
            }
        }
        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }


    }
}