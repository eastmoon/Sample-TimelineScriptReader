using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineScriptReader.DataStruct
{
    class ScriptModule
    {
        // Member variable
        private string m_name;
        private ScriptReader m_reader;
        private Timer m_timer;
        // Consturctor
        public ScriptModule(string a_triggerName = "") 
        {
            // Initial name
            this.m_name = a_triggerName;
            // Initial timer
            this.m_timer = new Timer(1);
        }
         // Method
        public virtual void Execute() { }
        public virtual void ExecuteSkip() { }
        // Attribute
        public virtual string Name 
        {
            get { return this.m_name; } 
        }
        public virtual ScriptReader Reader 
        {
            set { this.m_reader = value; }
            get { return this.m_reader; }
        }
        public virtual bool IsComplete 
        { 
            get { return true; } 
        }
        protected Timer timer
        {
            get { return this.m_timer; }
        }
    }
}
