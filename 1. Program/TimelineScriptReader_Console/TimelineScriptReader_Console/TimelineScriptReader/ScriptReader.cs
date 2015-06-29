using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.MarkupStruct;

namespace TimelineScriptReader
{
    class ScriptReader
    {
        // static variable
        public const string DEFAULT_PAGE = "default.system";

        // Member variable
        protected ScriptData m_pages;
        protected Hashtable m_modules;
        protected Hashtable m_triggers;
        protected ITag m_tagParser;
        private string m_currentReaderState;

        // Consturctor
        public ScriptReader()
        {
            // Initial pages table
            this.m_pages = new ScriptData();

            // Initial Modules table
            this.m_modules = new Hashtable();

            // Initial trigger table
            this.m_triggers = new Hashtable();
        }

        // Public Method

        // Startup reader, and loading first script to execute.
        public virtual void Start( string a_filename )
        {
            
        }

        // Execute script from assignment page.
        public virtual bool Execute(string a_pagename = "", int a_start = 0)
        {
            return false;
        }

        // Reading Script, read one row in current page, and use state to control how to execute script command.
        public virtual void ReadScript(string a_changeState)
        {
            
        }

        // Load file, analysis row to command object or content object, and store in page.
        public virtual bool LoadFile(string a_filename = "")
        {
            return true;
        }

        // Praser input string to script reader command.
        public virtual string PraserRow(string a_row)
        {
            return "";
        }

        // addition module in the script reader
        public void AddModule(ScriptModule a_module)
        {
            // Add module in list.
            if (this.m_modules[a_module.Name] == null)
                this.m_modules.Add(a_module.Name, a_module);
            else
            {
                this.m_modules[a_module.Name] = null;
                this.m_modules[a_module.Name] = a_module;
            }

            // Set reader
            a_module.Reader = this;
        }

        // Get module in script reader
        public ScriptModule RetrieveModule(string a_moduleName)
        {
            return (ScriptModule)this.m_modules[a_moduleName];
        }

        // addition trigger in the script reader
        public void AddTrigger(ScriptTrigger a_trigger)
        {
            // Add module in list.
            if (this.m_triggers[a_trigger.Name] == null)
                this.m_triggers.Add(a_trigger.Name, a_trigger);
            else
            {
                this.m_triggers[a_trigger.Name] = null;
                this.m_triggers[a_trigger.Name] = a_trigger;
            }

            // Set reader
            a_trigger.Reader = this;
        }

        // Get module in script reader
        public ScriptTrigger RetrieveTrigger(string a_triggerName)
        {
            return (ScriptTrigger)this.m_triggers[a_triggerName];
        }

        // Attribute
        public ITag TagParser
        {
            set { this.m_tagParser = value; }
            get { return this.m_tagParser;}
        }

        public string CurrentReaderState
        {
            get { return this.m_currentReaderState; }
            set { this.m_currentReaderState = value; }
        }
    }
}
