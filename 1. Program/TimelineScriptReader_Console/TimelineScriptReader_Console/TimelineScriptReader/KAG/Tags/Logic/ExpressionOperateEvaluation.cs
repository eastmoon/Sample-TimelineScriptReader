using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Logic
{
    class ExpressionOperateEvaluation : Tag
    {
        public ExpressionOperateEvaluation()
            : base("eval")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            JavaScriptModule jsm = (JavaScriptModule)kag.RetrieveModule(JavaScriptModule.NAME);
            string expression = "";

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
            if (jsm != null && expression != "")
            {
                jsm.Eval(expression);
            }
        }
    }
}
