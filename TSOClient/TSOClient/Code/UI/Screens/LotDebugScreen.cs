﻿/*The contents of this file are subject to the Mozilla Public License Version 1.1
(the "License"); you may not use this file except in compliance with the
License. You may obtain a copy of the License at http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is the TSOClient.

The Initial Developer of the Original Code is
ddfczm. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSOClient.Code.UI.Framework;
using TSOClient.Code.UI.Panels;
using TSOClient.Code.UI.Controls;
using tso.world;
using TSOClient.LUI;
using tso.world.model;
using TSO.Simantics;
using TSO.Simantics.utils;
using Microsoft.Xna.Framework;
using tso.debug;

namespace TSOClient.Code.UI.Screens
{
    public class LotDebugScreen : GameScreen
    {
        private UIUCP ucp;
        private World World;
        private UIButton VMDebug;
        private TSO.Simantics.VM vm;
        private UILotControl LotController;

        public LotDebugScreen()
        {
            var lotInfo = XmlHouseData.Parse(GameFacade.GameFilePath("housedata/blueprints/restaurant01_00.xml"));

            World = new World(GameFacade.Game.GraphicsDevice);
            GameFacade.Scenes.Add(World);

            vm = new TSO.Simantics.VM(new VMContext(World));
            vm.Init();

            var activator = new VMWorldActivator(vm, World);
            var blueprint = activator.LoadFromXML(lotInfo);

            World.InitBlueprint(blueprint);
            vm.Context.Blueprint = blueprint;

            var sim = activator.CreateAvatar();
            //sim.Position = new Vector3(31.5f, 55.5f, 0.0f);
            sim.Position = new Vector3(26.5f, 41.5f, 0.0f);

            VMDebug = new UIButton()
            {
                Caption = "Simantics",
                Y = 45,
                Width = 100,
                X = GlobalSettings.Default.GraphicsWidth - 110
            };
            VMDebug.OnButtonClick += new ButtonClickDelegate(VMDebug_OnButtonClick);
            this.Add(VMDebug);

            LotController = new UILotControl(vm, World);
            this.AddAt(0, LotController);

            ucp = new UIUCP(this);
            ucp.Y = ScreenHeight - 210;
            ucp.SetInLot(true);
            ucp.SetMode(UIUCP.UCPMode.LotMode);
            ucp.SelectedAvatar = sim;
            ucp.SetPanel(1);

            //ucp.ZoomLevel = 1;
            //ucp.OnZoomChanged += new UCPZoomChangeEvent(ucp_OnZoomChanged);
            //ucp.OnRotateChanged += new UCPRotateChangeEvent(ucp_OnRotateChanged);
            this.Add(ucp);
        }

        public override void Update(TSO.Common.rendering.framework.model.UpdateState state)
        {
            base.Update(state);
            vm.Update(state.Time);
        }

        void VMDebug_OnButtonClick(UIElement button)
        {
            System.Windows.Forms.Form gameWindowForm =
                (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(GameFacade.Game.Window.Handle);
            //gameWindowForm.Location = new System.Drawing.Point(0, 0);

            var debugTools = new Simantics(vm);
            debugTools.Show();
            debugTools.Location = new System.Drawing.Point(gameWindowForm.Location.X + gameWindowForm.Width, gameWindowForm.Location.Y);
            debugTools.UpdateAQLocation();
            
        }

        

        void ucp_OnZoomChanged(WorldZoom zoom)
        {
            World.State.Zoom = zoom;
            //Scene.Zoom = zoom;
        }
    }
}
