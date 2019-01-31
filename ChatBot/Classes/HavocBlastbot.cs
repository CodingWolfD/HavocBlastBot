using System;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Api;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Communication.Clients;
using TwitchLib.PubSub.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Communication.Models;

namespace ChatBot
{
    internal class HavocBlastbot
    {
        readonly ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.BotUsername, TwitchInfo.BotToken);
        TwitchClient client;
        TwitchAPI api;

        internal void connect()
        {
            Console.Write("Connecting!");

            var options = new ClientOptions()
            {
                MessagesAllowedInPeriod = 100,
                ThrottlingPeriod = TimeSpan.FromSeconds(60)
            };

            var websocketClient = new WebSocketClient(options);

            client = new TwitchClient(websocketClient);
            client.Initialize(credentials, TwitchInfo.ChannelName);

            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnUserTimedout += Client_OnUserTimedout;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnLeftChannel += Client_OnLeftChannel;
            client.OnChatCleared += Client_OnChatCleared;
            client.OnMessageThrottled += Client_OnMessageThrottled;

            client.Connect();

            Console.WriteLine("Connected!");
            Console.WriteLine("HavocBot has Joined: " + TwitchInfo.ChannelName);

            api = new TwitchAPI();
        }

        private void Client_OnMessageThrottled(object sender, TwitchLib.Communication.Events.OnMessageThrottledEventArgs e)
        {
            client.SendMessage(TwitchInfo.ChannelName, "Messages have been throttled!");
        }

        private void Client_OnChatCleared(object sender, OnChatClearedArgs e)
        {
            client.SendMessage(e.Channel, "Chat has been cleared!");
        }

        private void Client_OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            Console.WriteLine(TwitchInfo.BotUsername + "has left " + TwitchInfo.ChannelName);
            client.SendMessage(e.Channel, "Peace out!");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            client.SendMessage(e.Channel, "HavocBot has arrived");
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            client.SendMessage(TwitchInfo.ChannelName, $"{e.Username} has left :(");
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            if (e.Username != TwitchInfo.ChannelName && e.Username != TwitchInfo.BotUsername)
            {
                client.SendMessage(TwitchInfo.ChannelName, $"{e.Username} has joined!");
            }
        }

        private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
        {
            client.SendMessage(TwitchInfo.ChannelName, $"Get rekt bro");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            client.SendWhisper(e.WhisperMessage.Username, $"You said: {e.WhisperMessage.Message}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            switch (e.ChatMessage.Message)
            {
                case "!hello":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"Hey there {e.ChatMessage.DisplayName}");
                }
                    break;

                case "!schedule":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"@{e.ChatMessage.DisplayName} XxHavocBlastxX has no schedule at all sadly, he streams mostly in the evenings and on weekends");
                }
                    break;
                case "!uptime":
                {
                    // TODO            
                }
                    break;
                case "!havocBot":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"Hello, I am HavocBot, I am HavocBlast's custom made bot, I am pretty dumb at the moment :)");
                }
                    break;
                case "!twitter":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"You can follow HavocBlast on Twitter at {TwitchInfo.TwitterUrl}");
                }
                    break;
                case "!currentProject":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"@{e.ChatMessage.DisplayName} Civies™ is a Civilian Roleplaying game where you have just moved into a new town and have to rebuild your life from scratch. Find a job or other way to make money and make your life.");
                }
                    break;
                case "!clear":
                {
                    if(e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator)
                    {
                         client.ClearChat(TwitchInfo.ChannelName);
                    }
                }
                    break;
                case "!devTools":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"@{e.ChatMessage.DisplayName} xXHavocBlastxX uses Unreal Engine 4 as his primary engine, Autodesk Maya as his primary 3D Modelling Software and Substance Painter and Designer for Textures and such");
                }
                    break;
                case "!break":
                {
                    client.SendMessage(TwitchInfo.ChannelName, $"XxHavocBlastxX has gone for a break, the stream will resume shortly");
                }
                    break;
                case "!help":
                {
                    client.SendMessage(TwitchInfo.ChannelName, "The current implemented commands are: \n 1) !Hello (Allows HavocBot to say Hi!) \n2) !schedule (Gives information about XxHavocBlastXx's schedule for streaming \n3) !twitter (Prints out a link to XxHavocBlastXx's Twitter) " +
                        "\n4) !currentProject (Gives everyone current information based on XxHavocBlastXx's current project he is working on \n5) !devTools (Displays what current tools XxHavocBlastXx is currently using for work");
                }
                    break;
                case "!donate":
                {
                        client.SendMessage(TwitchInfo.ChannelName, "You can donate via patreon: https://www.patreon.com/ or my tip page: https://streamlabs.com/xxhavocblastxx");
                }
                    break;
                case "!leave":
                {
                    if (e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator)
                    {
                        client.SendMessage(TwitchInfo.ChannelName, $"HavocBot out! Peace!");
                        disconnect();
                    }
                }
                    break;
            }
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"Error!! {e.Error}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            
        }

        private void client_SayMessage(String messageText)
        {
            client.SendMessage(TwitchInfo.ChannelName, messageText);
        }

        internal void disconnect()
        {
            Console.Write("Disconnection!");
            client.Disconnect();
        }
    }
}