using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.MarkupStruct
{
    class PairsTag : TagContainer
    {
        // Member
        private ArrayList m_captureDataList;
        private IScriptData m_captureData;
        // Constructor
        public PairsTag(string a_name)
            : base( a_name )
        {
            //Console.WriteLine("Class Pairs Tag");
            this.m_captureDataList = null;
            this.m_captureData = null;
        }

        // Clone input list into data list. It is use when other tag will insert data.
        public void CloneCaptureData(ArrayList a_cloneTarget)
        {
            if( a_cloneTarget != null )
            {
                for (int i = 0; i < a_cloneTarget.Count; i++)
                    this.CaptureDataList.Add(a_cloneTarget[i]);
            }
        }

        // Capture the content between start-tag and end-tag
        public virtual void CaptureContent( IPageReader a_page )
        {
            if (this.CaptureDataList != null && a_page != null )
            {
                while(this.IsEndCapture(a_page.Next()))
                {
                    if(this.IsCapture(a_page.Current()))
                        this.CaptureDataList.Add(a_page.Current());
                }
            }
        }

        // Check capture data is action or not.
        protected virtual bool IsCapture( IScriptData a_data )
        {
            return true;
        }

        // Check capture data will end capture action or not.
        protected virtual bool IsEndCapture( IScriptData a_data )
        {
            if (this.m_captureData != a_data)
            {
                this.m_captureData = a_data;
                return true;
            }
            return false;
        }

        // Attribute
        public ArrayList CaptureDataList
        {
            set { this.m_captureDataList = value; }
            get { return this.m_captureDataList; }
        }
    }
}
