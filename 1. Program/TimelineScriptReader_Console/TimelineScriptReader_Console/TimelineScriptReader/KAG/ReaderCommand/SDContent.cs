using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.ReaderCommand
{
    class SDContent : ScriptData
    {
        // static variable
        public const string TYPE = "SCRIPTDATA_CONTENT";
        // Member variable

        // Constructor
        public SDContent(string a_name, IPageReader a_page)
            : base(SDContent.TYPE, "content", a_page)
        {
            // add content object into page.content.
            this.Page.Add(this);
            
            this.Content.Add(a_name);
        }
    }
}
