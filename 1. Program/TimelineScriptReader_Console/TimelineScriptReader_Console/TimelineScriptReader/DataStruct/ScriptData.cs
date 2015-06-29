using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineScriptReader.DataStruct
{
    class ScriptData : IScriptData
    {
        // Member variable
        private string m_type;
        private string m_name;
        private IPageReader m_page;
        private Hashtable m_attribute;
        private ArrayList m_content;

        // Constructor
        public ScriptData(string a_type = "", string a_name = "", IPageReader a_page = null)
        {
            this.m_type = a_type;
            this.m_name = a_name;
            this.m_page = a_page;
            this.m_attribute = new Hashtable();
            this.m_content = new ArrayList();
        }

        // Attribute
        public string Type 
        { 
            get { return this.m_type; }
        }
        public string Name 
        {
            get { return this.m_name; }
        }
        public Hashtable Attribute 
        {
            get { return this.m_attribute; }
        }
        public ArrayList Content 
        {
            get { return this.m_content; } 
        }
        public IPageReader Page
        {
            get { return this.m_page; }
        }

        // Method
        public virtual void Execute()
        {
            // describe variable
            ScriptReader sr = this.Page.Script;
            // execute data
            sr.TagParser.Execute(this);
        }

        public virtual IScriptData Clone(IPageReader a_page = null)
        {
            IScriptData result = new ScriptData(this.m_type, this.m_name, a_page);
            // Clone Attribute
            foreach( string key in this.m_attribute.Keys )
                result.Attribute.Add(key, this.m_attribute[key]);
            // Clone Content
            for (int i = 0; i < this.m_content.Count; i++)
                result.Content.Add(this.m_content[i]);
            return result;
        }
    }
}
