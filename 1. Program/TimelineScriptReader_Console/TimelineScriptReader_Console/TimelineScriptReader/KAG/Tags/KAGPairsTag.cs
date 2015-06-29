using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.Tags
{
    class KAGPairsTag : PairsTag
    {
        // Member
        private int m_counter;
        // Constructor
        public KAGPairsTag( string a_name )
            : base(a_name)
        {
            this.Counter = 0;
        }

        public override void Execute(DataStruct.IScriptData a_data)
        {
            this.Counter = 0;
        }

        // Check capture data will end capture action or not.
        protected override bool IsEndCapture(IScriptData a_data)
        {
            bool result = base.IsCapture(a_data);
            string endtag = "end" + this.Name;
            if (this.Name.Equals(a_data.Name))
                this.Counter++;
            else if (("end" + this.Name).Equals(a_data.Name))
            {
                if (this.Counter > 0)
                    this.Counter--;
                else
                    result = false;
            }
            return result;
        }

        // Attribute
        protected int Counter
        {
            set {
                this.m_counter = value; 
                if (this.m_counter < 0) 
                    this.m_counter = 0;
            }
            get { return this.m_counter; }
        }
    }
}
