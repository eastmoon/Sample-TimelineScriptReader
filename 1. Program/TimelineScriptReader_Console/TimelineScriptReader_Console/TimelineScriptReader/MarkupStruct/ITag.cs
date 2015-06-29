using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.MarkupStruct
{
    interface ITag
    {
        // Attribute
        string Name { get; }
        ITag Parent { set; get; }
        
        // Method
        void Execute(IScriptData a_data);

        bool isVariableAttribute(string a_value);
        bool isNumberAttribute(string a_value);
        bool isBooleanAttribute(string a_value);
        bool isDefaultAttribute(string a_value);

        string getVariable(string a_value);
        int getNumber(string a_value);
        bool getBoolean(string a_value);
    }
}
