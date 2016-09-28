﻿using FSO.Client.Network;
using FSO.Client.UI.Framework;
using FSO.Client.UI.Panels;
using FSO.Common.DataService;
using FSO.Common.DataService.Model;
using FSO.Server.DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FSO.Client.Controllers
{
    public class PersonPageController
    {
        private UIPersonPage View;
        private IClientDataService DataService;
        private uint AvatarId;
        private Timer ProgressTimer;

        private ITopicSubscription Topic;

        public PersonPageController(UIPersonPage view, IClientDataService dataService)
        {
            this.View = view;
            this.DataService = dataService;
            Topic = dataService.CreateTopicSubscription();
        }

        ~PersonPageController(){
            Topic.Dispose();
        }

        public void Close()
        {
            View.TrySaveDescription();
            View.Visible = false;
            ChangeTopic();
        }

        public void Show(uint avatarId){
            View.TrySaveDescription();
            AvatarId = avatarId;
            DataService.Get<Avatar>(avatarId).ContinueWith(x =>
            {
                View.CurrentAvatar.Value = x.Result;
            });

            View.CurrentTab = UIPersonPageTab.Description;
            View.SetOpen(false);
            View.Visible = true;
            ChangeTopic();
        }

        public void SaveDescription(Avatar target)
        {
            DataService.Sync(target, new string[] { "Avatar_Description" });
        }

        public void SaveValue(Avatar target, string name)
        {
            DataService.Sync(target, new string[] { name });
        }

        public void ForceRefreshData(UIPersonPageTab tab){
            Topic.Poll();
        }

        public void ChangeTopic()
        {
            List<ITopic> topics = new List<ITopic>();
            if (View.Visible && AvatarId != 0)
            {
                topics.Add(Topics.For(MaskedStruct.SimPage_Main, AvatarId));
                switch (View.CurrentTab)
                {
                    case UIPersonPageTab.Description:
                        topics.Add(Topics.For(MaskedStruct.SimPage_DescriptionPanel, AvatarId));
                        break;
                    case UIPersonPageTab.Accomplishments:
                        topics.Add(Topics.For(MaskedStruct.SimPage_SkillsPanel, AvatarId));
                        topics.Add(Topics.For(MaskedStruct.SimPage_JobsPanel, AvatarId));
                        break;
                }
            }
            Topic.Set(topics);
        }
    }
}