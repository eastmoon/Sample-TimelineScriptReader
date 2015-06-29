using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.Modules
{
    class BookmarkModule : ScriptModule
    {
        // static variable
        public const string NAME = "BOOKMARK_MODULE";

        // Bookmark variable
        private string m_filename;
        private string m_pagename;
        private int m_startIndex;

        // Callback variable
        private string m_callbackFilename;
        private string m_callbackPagename;
        private int m_callbackStartIndex;
        private bool m_haveSavePoint;

        // Constructure
        public BookmarkModule() 
            : base( BookmarkModule.NAME )
        {
            this.timer.Elapsed += new ElapsedEventHandler(this.Timeout);
        }

        // Attribute
        public override bool IsComplete
        {
            get 
            {
                return !this.timer.Enabled; 
            }
        }

        // Method
        public override void Execute()
        {
            if (this.Reader != null)
            { 
                KAGReader kag = (KAGReader)this.Reader;
                kag.ChangePage(this.m_pagename, this.m_filename, this.m_startIndex);

                // Execute 1ms timeout
                this.timer.Start();
            }
        }

        public override void ExecuteSkip()
        {
            
            // Execute jump action.
            this.Execute();
        }

        // 
        public void JumpTo(string a_pagename, string a_filename)
        {
            this.m_filename = a_filename;
            this.m_pagename = a_pagename;
            this.m_startIndex = 0;
        }

        public void JumpToCallback()
        {
            if( this.m_haveSavePoint )
            {
                this.m_filename = this.m_callbackFilename;
                this.m_pagename = this.m_callbackPagename;
                this.m_startIndex = this.m_callbackStartIndex;
            }
            this.m_haveSavePoint = false;
        }

        public void SaveCallbackInfo()
        {
            KAGReader kag = (KAGReader)this.Reader;
            this.m_haveSavePoint = true;

            // Get currect filename
            this.m_callbackFilename = kag.CurrentReadFileName;
            
            // Get currect pagename
            this.m_callbackPagename = kag.CurrentReadPageName;

            // Get current page row index
            this.m_callbackStartIndex = kag.CurrentReadPage.Index + 1;
        }

        // Private Method
        private void Timeout(object sender = null, System.Timers.ElapsedEventArgs e = null)
        {
            // Always setting interval is 1ms, and stop timer.
            this.timer.Interval = 1;
            this.timer.Stop();
            if (this.Reader != null)
                ((KAGReader)this.Reader).CheckExecuteModulesComplete();
        }
    }
}
