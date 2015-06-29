using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineScriptReader.MarkupStruct
{
    class TagContainer : Tag
    {
        // Member variable
        Hashtable m_tagsTable;
        // Constructor
        public TagContainer(string a_name)
            : base( a_name )
        {
            //Console.WriteLine("Class Pairs Tag");

            // Initial member variable
            this.m_tagsTable = new Hashtable();
        }

        // Attribute
        // Method
        public bool Register(ITag a_tag)
        {
            // Check duplicate register
            if (this.Has(a_tag.Name))
                return false;

            // Register tag
            this.m_tagsTable.Add(a_tag.Name, a_tag);
            a_tag.Parent = this;

            return true;
        }

        public bool Remove(string a_tagName)
        {
            // check tag exist
            if (!this.Has(a_tagName))
                return false;
            
            // Remove tag
            this.m_tagsTable.Remove(a_tagName);
            return true;
        }

        public ITag Retrieve(string a_tagName)
        {
            return (ITag)this.m_tagsTable[a_tagName];
        }

        public bool Has(string a_tagName)
        {
            if (this.m_tagsTable[a_tagName] == null)
                return false;
            return true;
        }
    }
}
