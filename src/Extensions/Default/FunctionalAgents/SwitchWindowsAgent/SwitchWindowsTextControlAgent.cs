////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchWindowsTextControlAgent.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Automation;

namespace ACAT.Extensions.Default.FunctionalAgents.SwitchWindowsAgent
{
    /// <summary>
    /// Text control agent class for the switch windows agent. Disable
    /// spellcheck and abbreviations expansion
    /// </summary>
    public class SwitchWindowsTextControlAgent
        : EditTextControlAgent
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="handle">window handle</param>
        /// <param name="editControlElement">edit control that has focus</param>
        /// <param name="handled">was this handled?</param>
        public SwitchWindowsTextControlAgent(IntPtr handle, AutomationElement editControlElement, ref bool handled)
            : base(handle, editControlElement, ref handled)
        {
            Log.Debug();
        }

        /// <summary>
        /// Disable abbreviation expansion
        /// </summary>
        /// <returns>false always</returns>
        public override bool ExpandAbbreviations()
        {
            return false;
        }

        /// <summary>
        /// Disable spell check
        /// </summary>
        /// <returns>true always</returns>
        public override bool SupportsSpellCheck()
        {
            return true;
        }
    }
}