////////////////////////////////////////////////////////////////////////////
// <copyright file="ShowPhraseSpeakHandler.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using System;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Activates the Phrase Speak functional agent that displays
    /// a list of canned phrases that can be converted to speech.
    /// </summary>
    public class ShowPhraseSpeakHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public ShowPhraseSpeakHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            bool retVal = true;

            handled = true;

            switch (Command)
            {
                case "CmdPhraseSpeak":
                    if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
                    {
                        return false;
                    }

                    handlePhraseSpeak();
                    break;

                case "CmdShowEditPhrasesSettings":
                    if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
                    {
                        return false;
                    }

                    handlePhrasesEdit();
                    break;

                default:
                    handled = false;
                    retVal = false;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Launches the phrase speak agent to edit phrases
        /// </summary>
        private async void handlePhrasesEdit()
        {
            IApplicationAgent agent = Context.AppAgentMgr.GetAgentByCategory("PhraseSpeakAgent");
            if (agent != null)
            {
                if (agent is IExtension)
                {
                    agent.GetInvoker().SetValue("PhraseListEdit", true);
                }

                await Context.AppAgentMgr.ActivateAgent(agent as IFunctionalAgent);
            }
        }

        /// <summary>
        /// Launches the phrase speak agent to display list of phrases
        /// </summary>
        private async void handlePhraseSpeak()
        {
            IApplicationAgent agent = Context.AppAgentMgr.GetAgentByCategory("PhraseSpeakAgent");
            if (agent != null)
            {
                if (agent is IExtension)
                {
                    var invoker = agent.GetInvoker();
                    invoker.SetValue("EnableSearch", true);
                    invoker.SetValue("PhraseListEdit", false);
                }

                await Context.AppAgentMgr.ActivateAgent(agent as IFunctionalAgent);
            }
        }
    }
}