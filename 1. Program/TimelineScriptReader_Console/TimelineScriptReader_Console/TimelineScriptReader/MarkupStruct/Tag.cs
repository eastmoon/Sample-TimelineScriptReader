using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.MarkupStruct
{
    /// <summary>
    /// Tag is base class of Script reader, and also mean single tag.
    /// Single tag has tag name, attribute, but tag don't co-work with other tag. 
    /// </summary>
    class Tag : ITag
    {
        // static variable
        public const string DEFAULT_ATTRIBUTE_VALUE = "TAG_ATTRIBUTE_VALUE_IS_DEFAULT";
        // Member variable
        private string m_name;
        private ITag m_parent;
        
        // Constructor
        public Tag(string a_name = "TAG_NAME")
        {
            this.m_name = a_name.ToLower();
            this.m_parent = null;
        }

        // Attribute
        public string Name
        {
            get
            { return this.m_name;  }
        }

        public ITag Parent
        {
            set { this.m_parent = value; }
            get { return this.m_parent; }
        }

        // Method
        public virtual void Execute( IScriptData a_data )
        {

        }

        public bool isVariableAttribute(string a_value)
        {
            // variable pattern
            string pattern = @"&[^&]*";

            // Match by regular expressions
            Match match = Regex.Match(a_value, pattern);

            // first character is a symbol "&", it mean variable
            // &(variable name)
            if (match.Success && match.Index == 0 && match.Length - match.Index > 1)
                return true;
            return false;
        }
        public bool isNumberAttribute(string a_value)
        {
            // number pattern
            string pattern = @"[0-9]*";

            // Match by regular expressions
            Match match = Regex.Match(a_value, pattern);

            // match size equal string size, it is number
            if (match.Index == 0 && match.Length == a_value.Length && match.Length - match.Index > 1)
                return true;
            return false;

        }
        public bool isBooleanAttribute(string a_value)
        {
            // number pattern
            string pattern = @"true|false";

            // Match by regular expressions
            Match match = Regex.Match(a_value.ToLower(), pattern);

            // match size equal string size, it is boolean
            if (match.Success && match.Index == 0 && match.Length == a_value.Length)
                return true;
            return false;
        }
        public bool isDefaultAttribute(string a_value)
        {
            return String.Equals(a_value, Tag.DEFAULT_ATTRIBUTE_VALUE);
        }

        public string getVariable(string a_value)
        {
            return a_value.Substring(1);
        }
        public int getNumber(string a_value)
        {
            return Int32.Parse(a_value);
        }
        public bool getBoolean(string a_value)
        {
            return bool.Parse(a_value);
        }
    }
}
