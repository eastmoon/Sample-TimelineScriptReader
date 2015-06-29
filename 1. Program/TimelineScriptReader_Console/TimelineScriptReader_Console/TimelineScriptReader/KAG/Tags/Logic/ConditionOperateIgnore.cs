using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.Tags;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Logic
{
    class ConditionOperateIgnore : KAGPairsTag
    {
        public ConditionOperateIgnore()
            : base("ignore")
        {
            
        }

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            JavaScriptModule jsm = (JavaScriptModule)kag.RetrieveModule(JavaScriptModule.NAME);
            ArrayList captureDataList = new ArrayList();
            string expression = "";
            bool isExcute = false;
            // Capture content
            this.CaptureDataList = captureDataList;
            this.CaptureContent(a_data.Page);

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "exp":
                        {
                            expression = a_data.Attribute[key].ToString();
                        }
                        break;
                }
            }
            // Retrieve module and setting action
            isExcute = jsm.Emb<bool>(expression);
            if(isExcute)
            {
                kag.AddDynamicQueue(this.CaptureDataList);
            }
        }
    }
}
