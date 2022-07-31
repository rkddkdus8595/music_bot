using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace MusicBot.Dialogs
{
    [Serializable]
    public class youtube : IDialog<string>
    {
        string strMessage;
        string str;

        string[] arr = new string[15]; // 비디오 주소 담으려고 최대 15개
        int i = 0;

        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("입력해주세요 > ");
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;

        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity=await result as Activity;

            if (activity.Text.Trim() == "Exit")
             {
                await context.PostAsync("Exit");
                str = null;
                context.Done("search Exit");
            }
            else
            {

                YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer() // youtube 검색결과 쓰는 소스
                {
                    ApiKey = "AIzaSyDbQIfTCPUjNgOjljnqBTIxqCALxhcr6Hg",
                    ApplicationName = "Youtube Test"
                });
                var request = youtube.Search.List("snippet");
                request.Q = activity.Text.Trim();
                request.MaxResults = 15;

                var message = context.MakeMessage();
                var actions = new List<CardAction>();
                string video = ""; // 버튼에서 사용하기 위한 동영상 이름 저장

                var searchResult = await request.ExecuteAsync();

                foreach (var item in searchResult.Items)
                {
                    if (item.Id.Kind == "youtube#video")
                    {
                        arr[i] = item.Id.VideoId.ToString();
                        video = "▶" + (i + 1) + "◀   " + item.Snippet.Title + "\n";
                        actions.Add(new CardAction() { Title = video, Value = i.ToString(), Type = ActionTypes.ImBack });
                        i++;
                    }
                }
                message.Attachments.Add(new HeroCard { Title = "누르면 이동합니다. 나갈 시 Exit 입력 ", Buttons = actions }.ToAttachment()
                       );
                await context.PostAsync(message);
                context.Wait(sendMessage);

                }
        }

        private async Task sendMessage(IDialogContext context, IAwaitable<object> result)
        {
            Activity activity = await result as Activity;
            int j = 0;
            string select = activity.Text.Trim();
            while (true) // 반복문 돌면서 내가 선택한 숫자 찾기
            {
                if (select == j.ToString())
                {
                    System.Diagnostics.Process.Start("http://youtube.com/watch?v=" + arr[j]); 
                    break;
                }
                else
                {
                    j++;
                }
            }
                context.PostAsync("입력해주세요 > ");
                context.Wait(MessageReceivedAsync);
           
        }
    }
}