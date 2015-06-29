using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.ReaderCommand
{
    class SDCommand : ScriptData
    {
        // static variable
        public const string TYPE = "SCRIPTDATA_COMMAND";
        public const string ATTRIBUTE_TYPE_STRING = "SDCommand_attr_type_string";
        public const string ATTRIBUTE_TYPE_VALUE = "SDCommand_attr_type_value";
        public const string ATTRIBUTE_TYPE_DEFAULT = "SDCommand_attr_type_default";
        // Member variable

        // Constructor
        public SDCommand(string a_name, IPageReader a_page) 
            : base(SDCommand.TYPE, a_name, a_page)
        {
            // add command object into page.content.
            this.Page.Add(this);
        }

        public void AddAttribute( string a_attribute, string a_type )
        {
            // remove the first empty space symbol
            a_attribute = a_attribute.Substring(1);
            string attributeName = "";
            string attributeValue = "";
            switch(a_type)
            {
                case SDCommand.ATTRIBUTE_TYPE_STRING:
                    {
                        // find = symbol
                        int index = a_attribute.IndexOf('=');
                        // splite string to name and value
                        attributeName = a_attribute.Substring(0, index);
                        attributeValue = a_attribute.Substring(index + 1);
                        // remove empty space symbol
                        attributeName = attributeName.Replace(" ", "");
                        attributeValue = attributeValue.Substring(attributeValue.IndexOf('\"'));
                        // remove " symbol
                        attributeValue = attributeValue.Replace("\"", "");
                    }
                    break;
                case SDCommand.ATTRIBUTE_TYPE_VALUE:
                    {
                        // find = symbol
                        int index = a_attribute.IndexOf('=');
                        // splite string to name and value, and remove empty space.
                        attributeName = a_attribute.Substring(0, index).Replace(" ", "");
                        attributeValue = a_attribute.Substring(index + 1).Replace(" ", "");
                    }
                    break;
                case SDCommand.ATTRIBUTE_TYPE_DEFAULT:
                    {
                        attributeName = a_attribute;
                        attributeValue = Tag.DEFAULT_ATTRIBUTE_VALUE;
                    }
                    break;
            }

            // make sure attribute no duplicate.
            // if duplicate then replace old data.
            if (this.Attribute[attributeName] == null)
                this.Attribute.Add(attributeName, attributeValue);
            else
                this.Attribute[attributeName] = attributeValue;
        }
    }
}
