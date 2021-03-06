////////////////////////////////////////////////////////////////////////////
// <copyright file="ChromeBrowserAgentBase.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.AppAgents.ChromeBrowser
{
    /// <summary>
    /// Base class for application agent for the Chrome browser. Enables
    /// easy browing of web pages, page up, page down, go back, forward,
    /// search etc.
    /// </summary>
    public class ChromeBrowserAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Name of the chrome browser process
        /// </summary>
        private const String ChromeProcessName = "chrome";

        /// <summary>
        /// Feature supported by this agent. Widgets that
        /// correspond to these features will be enabled
        /// </summary>
        private readonly String[] _supportedCommands =
        {
            "OpenFile",
            "SaveFile",
            "CmdFind",
            "CmdContextMenu",
            "CmdZoomIn",
            "CmdZoomOut",
            "CmdZoomFit",
            "CmdSelectModeToggle",
            "CmdSwitchApps"
        };

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Gets a list of processes supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(ChromeProcessName) }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            checkCommandEnabled(_supportedCommands, arg);
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs("ChromeBrowserContextMenu", "Chrome", monitorInfo));
        }

        /// <summary>
        /// Invoked when the foreground window focus changes.
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            if (monitorInfo.IsNewWindow)
            {
                _scannerShown = false;
            }

            if (!_scannerShown)
            {
                base.OnFocusChanged(monitorInfo, ref handled);

                showPanel(this, new PanelRequestEventArgs((autoSwitchScanners) ? "ChromeBrowserContextMenu" : PanelClasses.Alphabet, "Chrome", monitorInfo));

                _scannerShown = true;
            }

            handled = true;
        }

        /// <summary>
        /// Focus shifted to another app.  This agent is
        /// getting deactivated.
        /// </summary>
        public override void OnFocusLost()
        {
            _scannerShown = false;
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArg">Optional arguments for the command</param>
        /// <param name="handled">set this to true if handled</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "SwitchAppWindow":
                    DialogUtils.ShowTaskSwitcher(ChromeProcessName);
                    break;

                case "ChromeAddressBar":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.L);
                    break;

                case "ChromeZoomMenu":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs("ChromeBrowserZoomMenu", "Chrome", monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };
                        showPanel(this, panelArg);
                    }

                    break;

                case "SaveFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F);
                    AgentManager.Instance.Keyboard.Send(Keys.A);
                    break;

                case "CmdZoomIn":
                    TextControlAgent.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Add);
                    TextControlAgent.Resume();
                    break;

                case "CmdZoomOut":
                    TextControlAgent.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Subtract);
                    TextControlAgent.Resume();
                    break;

                case "CmdZoomFit":
                    TextControlAgent.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D0);
                    TextControlAgent.Resume();
                    break;

                case "CmdFind":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.F);
                    break;

                case "ChromeGoBackward":
                    AgentManager.Instance.Keyboard.Send(Keys.BrowserBack);
                    break;

                case "ChromeGoForward":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Right);
                    break;

                case "NewTab":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.T);
                    break;

                case "NextTab":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Tab);
                    break;

                case "CloseTab":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.W);
                    break;

                case "ChromeFavorites":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.O);
                    break;

                case "ChromeHistory":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.H);
                    break;

                case "ChromeAddFavorites":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    break;

                case "ChromeRefreshPage":
                    AgentManager.Instance.Keyboard.Send(Keys.BrowserRefresh);
                    break;

                case "ChromeHomePage":
                    AgentManager.Instance.Keyboard.Send(Keys.BrowserHome);
                    break;

                case "ChromeBrowserMenu":
                    showPanel(this, new PanelRequestEventArgs("ChromeBrowserMenu",
                                                                "Chrome",
                                                                WindowActivityMonitor.GetForegroundWindowInfo(),
                                                                true));
                    break;


                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Creates the text control agent for the Chrome browser
        /// </summary>
        /// <param name="handle">Handle to the browser window</param>
        /// <param name="focusedElement">currently focused element</param>
        /// <param name="handled">set to true if handled</param>
        /// <returns>the textcontrolagent</returns>
        protected override TextControlAgentBase createEditControlTextInterface(IntPtr handle,
                                                                    AutomationElement focusedElement,
                                                                    ref bool handled)
        {
            return new ChromeBrowserTextControlAgent(handle, focusedElement, ref handled);
        }
    }
}