using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.ReaderCommand
{
    class SDPage : ScriptData, IPageReader
    {
        // static variable
        public const string TYPE = "SCRIPTDATA_PAGE";
        // Member variable
        private ScriptReader m_scriptReader;
        private int m_index;
        private string m_pagelabel;
        private IScriptData m_lastData;

        // Constructor
        public SDPage( ScriptReader a_scriptReader, string a_name, string a_pagelabel ) 
            : base( SDPage.TYPE, a_name )
        {
            // Store script reader
            this.m_scriptReader = a_scriptReader;

            // Store page label
            this.m_pagelabel = a_pagelabel;

            // Initial index
            this.m_index = 0;

            // Last data object.
            this.m_lastData = null;
        }

        // Attribute
        public ScriptReader Script
        {
            get { return this.m_scriptReader; }
        }

        public int Index
        {
            get { return this.m_index; }
        }

        public int Count
        {
            get { return this.Content.Count; }
        }

        public string Label
        {
            get { return this.m_pagelabel; }
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
            return (IScriptData)this.Content[this.m_index];
        }

        public IScriptData Prev()
        {
            // Save last object
            this.m_lastData = this.Current();
            // desc index.
            this.m_index--;
            // Make sure index isn't low than 0.
            if (this.m_index < 0)
                this.m_index = 0;
            return this.Current();
        }

        public IScriptData Next()
        {
            // Save last object
            this.m_lastData = this.Current();
            // add index.
            this.m_index++;
            // Make sure index isn't equal or higher than total.
            if (this.m_index >= this.Content.Count)
                this.m_index = this.Content.Count - 1;
            return this.Current();
        }

        public IScriptData JumpTo( int a_indexNumber )
        {
            // Save last object
            this.m_lastData = null;

            // Make sure index number in the content range.
            if (a_indexNumber < 0)
                a_indexNumber = 0;
            else if (a_indexNumber >= this.Content.Count)
                a_indexNumber = this.Content.Count - 1;
            
            // In range, save index and return current
            this.m_index = a_indexNumber;
            return this.Current();
        }
    }
}
