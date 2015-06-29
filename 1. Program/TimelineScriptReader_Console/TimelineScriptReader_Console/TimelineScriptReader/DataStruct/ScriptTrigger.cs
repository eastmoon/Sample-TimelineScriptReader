using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineScriptReader.DataStruct
{
    class ScriptTrigger
    {
        // Member variable
        private string m_name;
        private ScriptReader m_reader;
        private Timer m_timer;
        // Consturctor
        public ScriptTrigger( string a_triggerName = "" ) 
        {
            this.m_name = a_triggerName;
            this.m_timer = new Timer(1);
        }
        
        // Method
        public virtual void Execute() { }

        // Protected method
        protected virtual void InitialTriggerEvent() { }

        // Attribute
        public virtual string Name 
        {
            get { return this.m_name; } 
        }
        public virtual ScriptReader Reader 
        {
            set { this.m_reader = value; this.InitialTriggerEvent(); }
            get { return this.m_reader; }
        }
        protected Timer timer
        {
            get { return this.m_timer; }
        }
    }
}
