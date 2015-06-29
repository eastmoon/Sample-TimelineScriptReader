using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.DataStruct
{
    interface IPageReader
    {
        // Attribute
        ScriptReader Script { get; }
        int Index { get; }
        int Count { get; }
        // Method
        void Add(IScriptData a_data);
        void Remove(IScriptData a_data);
        void Insert(IScriptData a_data, int a_index);
        IScriptData Last();
        IScriptData Current();
        IScriptData Prev();
        IScriptData Next();
        IScriptData JumpTo( int a_indexNumber );
    }
}
