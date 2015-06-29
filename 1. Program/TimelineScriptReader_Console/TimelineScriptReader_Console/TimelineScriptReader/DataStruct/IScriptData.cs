using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelineScriptReader.DataStruct
{
    interface IScriptData
    {
        // Attribute
        string Type { get; }
        string Name { get; }
        Hashtable Attribute { get; }
        ArrayList Content { get; }
        IPageReader Page { get; }

        // Method
        void Execute();
        IScriptData Clone(IPageReader a_page = null);
    }
}
