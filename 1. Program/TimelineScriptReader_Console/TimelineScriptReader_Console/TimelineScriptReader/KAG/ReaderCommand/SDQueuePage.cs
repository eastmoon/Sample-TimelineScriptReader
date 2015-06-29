using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.ReaderCommand
{
    class SDQueuePage : ScriptData, IPageReader
    {
        // static variable
        public const string TYPE = "SCRIPTDATA_QUEUE_PAGE";
        // Member variable
        private ScriptReader m_scriptReader;
        private IScriptData m_lastData;

        // Constructor
        public SDQueuePage(ScriptReader a_scriptReader)
            : base(SDQueuePage.TYPE, "queue_page")
        {
            // Store script reader
            this.m_scriptReader = a_scriptReader;
            this.m_lastData = null;
        }

        // Attribute
        public ScriptReader Script
        {
            get { return this.m_scriptReader; }
        }

        public int Index
        {
            get { return 0; }
        }

        public int Count
        {
            get { return this.Content.Count; }
        }

        public string Label
        {
            get { return ""; }
        }
        // Method
        public void Add(IScriptData a_data)
        {
            this.Content.Add(a_data);
        }

        public void Remove(IScriptData a_data)
        {
            this.Content.Remove(a_data);
        }

        public void Insert(IScriptData a_data, int a_index)
        {
            this.Content.Insert(a_index, a_data);
        }

        public IScriptData Last()
        {
            return this.m_lastData;
        }

        public IScriptData Current()
        {
            if (this.Content.Count == 0)
                return null;
            return (IScriptData)this.Content[0];
        }

        public IScriptData Prev()
        {
            // This function is equal than Current function.
            return this.Current();
        }

        public IScriptData Next()
        {
            // First in, First out.
            if (this.Content.Count > 0)
            {
                this.m_lastData = this.Current();
                this.Content.RemoveAt(0);
            }
            return this.Current();
        }

        public IScriptData JumpTo( int a_indexNumber )
        {
            // This function is equal than Current function.
            return this.Current();
        }
    }
}
