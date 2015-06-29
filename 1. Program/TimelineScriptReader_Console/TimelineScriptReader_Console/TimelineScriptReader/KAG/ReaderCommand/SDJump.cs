using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.ReaderCommand
{
    class SDJump : ScriptData
    {
        // static variable
        public const string TYPE = "SCRIPTDATA_COMMAND_JUMP";
        // Member variable

        // Constructor
        public SDJump(string a_name, IPageReader a_page)
            : base(SDJump.TYPE, "jump", a_page)
        {
            string[] info = a_name.Split('.');
            string filename = "";
            string pagename = "";

            if (info.Length >= 2)
                filename = (string)info[1];
            if (info.Length >= 3)
                pagename = (string)info[2];

            // add content object into page.content.
            this.Page.Add(this);
            this.Attribute.Add("storage", filename);
            this.Attribute.Add("target", pagename);
        }
    }
}
