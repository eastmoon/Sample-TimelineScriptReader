using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Index
{
    class IndexOperateCall : Tag
    {
        public IndexOperateCall()
            : base("call")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            string filename = kag.CurrentReadFileName;
            string pagename = "";
            bool countpage = true;
            string value = "";

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "storage":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (!this.isDefaultAttribute(value))
                                filename = value;
                        }
                        break;
                    case "target":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (!this.isDefaultAttribute(value))
                                pagename = value;
                        }
                        break;
                    case "countpage":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (this.isBooleanAttribute(value))
                                countpage = value.Equals("true");
                            else if (this.isDefaultAttribute(value))
                                countpage = true;
                        }
                        break;
                }
            }

            // Retrieve module and setting action
            BookmarkModule bm = (BookmarkModule)kag.RetrieveModule(BookmarkModule.NAME);
            if( bm != null )
            {
                bm.SaveCallbackInfo();
                bm.JumpTo(pagename, filename);
                kag.ExecuteModules(BookmarkModule.NAME, false);
            }
        }
    }
}
