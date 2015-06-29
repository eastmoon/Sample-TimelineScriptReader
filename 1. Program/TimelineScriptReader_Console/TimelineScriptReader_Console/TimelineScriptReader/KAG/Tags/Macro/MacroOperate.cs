using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.Tags;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Macro
{
    class MacroOperate : KAGPairsTag
    {
        // Member 
        private ScriptData m_macroData;

        // Constructor
        public MacroOperate()
            : base("macro")
        {
            this.m_macroData = null;
        }

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            Parser parser = (Parser)kag.TagParser;
            string name = "";

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "name":
                        {
                            name = a_data.Attribute[key].ToString();
                        }
                        break;
                }
            }

            if (name != "" && parser != null)
            {
                // Create macro data
                this.m_macroData = new ScriptData("MACRO_DATA", name, null);
                
                // Capture content
                this.CaptureDataList = this.m_macroData.Content;
                this.CaptureContent(a_data.Page);

                // Retrieve module and setting action
                {
                    parser.AddMacro(name, this.m_macroData);
                }
            }
        }

        // Check capture action.
        protected override bool IsCapture(IScriptData a_data)
        {
            // Check data object need use macro attribute or not.
            // if need, make index in macroData.attribute
            int index = 0;
            string value = "";
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "*":
                        {
                            // Use all macro attribute
                            index = this.m_macroData.Content.Count;
                            this.m_macroData.Attribute.Add(index, "*" );
                        }
                        break;
                    default:
                        {
                            value = (string)a_data.Attribute[key];
                            if(value.Substring(0, 1).Equals("%"))
                            {
                                index = this.m_macroData.Content.Count;
                                value = key;
                                this.m_macroData.Attribute.Add(index, value);
                            }
                        }
                        break;
                }
            }

            //
            return base.IsCapture(a_data);
        }
    }
}
