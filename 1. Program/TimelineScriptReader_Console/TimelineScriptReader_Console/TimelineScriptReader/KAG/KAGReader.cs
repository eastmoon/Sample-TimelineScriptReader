using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.ReaderCommand;
using TimelineScriptReader.MarkupStruct;

namespace TimelineScriptReader.KAG
{
    class KAGReader : ScriptReader
    {
        // static variable
        public const string STATE_DISPLAY = "state.display";
        public const string STATE_PARSER = "state.parser";
        public const string STATE_FINISH = "state.finish";
        public const string STATE_STOP = "state.stop";

        // Delegate handler declare
        public delegate void ReaderEventHandler(KAGReader a_reader);

        // Event handler declare
        public ReaderEventHandler OnSkipTrigger;
        public ReaderEventHandler OnStateChange;

        // Load file and page control
        private string m_currentLoadFileDefaultPageName;
        private string m_currentLoadPageName;
        private string m_currentLoadPageLabel;
        private string m_autoLoadPageName;
        private int m_autoLoadPageCount;

        // Reader control
        private IPageReader m_dynmicQueue;
        private string m_currentReadFileName;
        private string m_currentReadPageName;
        private string m_currentReadPageIndex;
        private IPageReader m_currentReadPage;
        private IScriptData m_currentReaderDataObject;
        private bool m_needConfirm;
        private bool m_isSkip;
        private bool m_isAutoSkip;
        private bool m_isAutoRead;
        private bool m_canSkip;
        private bool m_canAutoSkip;
        private bool m_canAutoRead;

        // Execute modules control
        private ArrayList m_executeModules;

        // Special character symbol
        private Hashtable m_specialCharacterSymbol;

        // Time control
        private int m_characterDelay;
        private int m_systemDelay;

        // Consturctor
        public KAGReader()
            : base()
        {
            // Initial pages table
            // pages.Attribute store page object and name mapping, base have default page.
            // pages.Content store loading filename.
            this.m_currentReadPageIndex = ScriptReader.DEFAULT_PAGE;
            this.m_currentLoadFileDefaultPageName = ScriptReader.DEFAULT_PAGE;
            this.m_currentLoadPageName = ScriptReader.DEFAULT_PAGE;
            this.m_pages.Attribute.Add(this.m_currentLoadPageName, new SDPage(this, this.m_currentLoadPageName, ""));

            // Initial execute modules 
            this.m_executeModules = new ArrayList();

            // Initial special character symbol
            this.m_specialCharacterSymbol = new Hashtable();

            // Initial time control
            this.SystemDelay = 0;
            this.CharacterDelay = 100;

            // Initial skip and read control
            this.EnabledSkip = this.EnabledAutoSkip = this.EnabledAutoRead = true;
            this.IsAutoSkip = this.IsAutoRead = false;

            // Initial dynamic queue
            this.m_dynmicQueue = new SDQueuePage(this);
        }

        // Public Method
        // Startup reader, and loading first script to execute.
        public override void Start(string a_filename)
        {
            // 1. Clear old data in Reader.
            this.m_pages.Attribute.Clear();
            this.m_pages.Content.Clear();
            this.m_pages = null;
            // new pages table
            this.m_pages = new ScriptData();

            // 2. Load the first file.
            this.LoadFile(a_filename);

            // 3. Execute reader 
            this.m_currentReadFileName = a_filename.Split('.')[0];
            this.Execute(this.m_currentLoadFileDefaultPageName);
        }

        // Execute script from assignment page.
        public override bool Execute(string a_pagename = "", int a_start = 0)
        {
            // Change page
            if( this.ChangeCurrentPage( a_pagename, a_start ))
            { 
                // Read Script and execute function.
                this.ReadScript();
            }
            return true;
        }

        // Reading Script, read one row in current page, and use state to control how to execute script command.
        public override void ReadScript(string a_changeState = KAGReader.STATE_PARSER)
        {
            // describe variable
            IPageReader page = null;
            IScriptData data = null;
            // Read one data object in page, and Parser
            // if state is PARSER, run again.
            // if state is DISPLAY, run animate object, and set call back.
            // if state is FINISH, stop run.

            // Input State change trigger
            if (this.CurrentReaderState != a_changeState)
            {
                // Save currect state
                this.CurrentReaderState = a_changeState;
                // Trigger event
                if( this.OnStateChange != null)
                    this.OnStateChange(this);
            }

            // Parser script data.
            while (this.CurrentReaderState == KAGReader.STATE_PARSER)
            {
                // Parser
                // If dynamic queue hava data object, take it first.
                if (this.m_dynmicQueue.Count > 0)
                    page = this.m_dynmicQueue;
                else
                    page = this.CurrentReadPage;
                // take back current data object, if current data object is null, take current page index, else take the next index.
                if (this.m_currentReaderDataObject == null)
                    data = page.Current();
                else
                    data = page.Next();

                if (data != null && 
                    ( (page == this.m_dynmicQueue) || (page != this.m_dynmicQueue && page.Current() != page.Last())) )
                {
                    // save currenct object. 
                    this.m_currentReaderDataObject = data;

                    // run data object function.
                    this.m_currentReaderDataObject.Execute();
                }
                else if( page != this.m_dynmicQueue )
                {
                    // if current data object is the same wiht current data in page, state change to finish
                    this.CurrentReaderState = KAGReader.STATE_FINISH;
                }
            }

            // Output State change trigger
            if (this.CurrentReaderState != a_changeState && this.OnStateChange != null)
                this.OnStateChange(this);

            // 
            switch (this.CurrentReaderState)
            {
                case KAGReader.STATE_DISPLAY:
                    {
                        // DISPLAY
                        //Console.WriteLine("\n===Run display===\n");
                        // if have more than 1 modules, then execute array list.
                        if (this.m_executeModules.Count > 0)
                        {
                            ScriptModule module = null;
                            for (int i = 0; i < this.m_executeModules.Count; i++)
                            {
                                module = (ScriptModule)this.m_executeModules[i];
                                module.Execute();
                            }
                        }
                    }
                    break;
                case KAGReader.STATE_FINISH:
                default:
                    {
                        // FINISH
                        //Console.WriteLine("\n===Finsih algorithm===\n");
                    }
                    break;
            }
        }

        // Execute Modules and change state to display
        public void ExecuteModules(String a_moduleName, bool a_needConfirm = true)
        {
            // add moduels to execute array list.
            this.AddExecuteModules(a_moduleName);

            // Set confirm
            this.m_needConfirm = a_needConfirm;

            // Change state to display
            // if state is parser, it mean timeline still on work.
            // if state isn't parser, it mean timeline is stop, then re-start timeline.
            if (this.CurrentReaderState == KAGReader.STATE_PARSER)
                this.CurrentReaderState = KAGReader.STATE_DISPLAY;
            else
                this.ReadScript(KAGReader.STATE_DISPLAY);
        }

        // Add Modules, make sure modules could synchronous execute. It will not change state.
        public void AddExecuteModules(String a_moduleName)
        {
            // retrieve modules
            ScriptModule executeModules = this.RetrieveModule(a_moduleName);
            // if modules exist, add to array, and wait for execute.
            if (executeModules != null)
            {
                // Add into execute array list.
                this.m_executeModules.Add(executeModules);
            }
        }

        // Check all execute modules is complete. 
        public void CheckExecuteModulesComplete()
        {
            bool isAllComplete = true;
            ScriptModule module = null;

            // if have one module does not complete, stop check.
            for (int i = 0; i < this.m_executeModules.Count && isAllComplete; i++)
            {
                module = (ScriptModule)this.m_executeModules[i];
                isAllComplete = module.IsComplete;
            }

            // if all complete, and no need to wait, and state are DISPLAY, read next script.
            // Confirm is system need to waiting user to skip or not. So, if system didn't need to waiting, that skip action will ignore.
            if (this.CurrentReaderState == KAGReader.STATE_DISPLAY && isAllComplete && (!this.m_needConfirm || this.IsSkip))
            {
                // Reset control variable
                this.m_needConfirm = this.IsSkip = false;
                // Clear list
                this.m_executeModules.Clear();
                // Next script
                this.ReadScript(KAGReader.STATE_PARSER);
            }
        }

        // Load file, analysis row to command object or content object, and store in page.
        public override bool LoadFile(string a_filename = "")
        {
            // describe variable
            int i = 0, j = 0, k = 0;
            StreamReader file = null;
            string inputFilename = a_filename.Split('.')[0];
            string row = "";
            // check file load is duplicate or not.
            for (i = 0; i < this.m_pages.Content.Count; i++)
            {
                if ((string)this.m_pages.Content[i] == inputFilename)
                    break;
            }

            // if i equal Total load file number, than this filename didn't loading.
            if (i < this.m_pages.Content.Count)
                return false;
            else
            {
                // Make sure filename exist.
                if (File.Exists(a_filename))
                {
                    // Open file, read only.
                    file = new StreamReader(a_filename, Encoding.UTF8);

                    // Create default page for this file
                    this.m_currentLoadFileDefaultPageName = "default." + inputFilename;
                    this.m_currentLoadPageName = this.m_currentLoadFileDefaultPageName;
                    this.m_autoLoadPageCount = 1;
                    this.m_pages.Attribute.Add(this.m_currentLoadPageName, new SDPage(this, this.m_currentLoadPageName, ""));
                }
                else
                    return false;
            }

            // Read one row and Parser.
            while ((row = file.ReadLine()) != null)
            {
                this.PraserRow(row);
            }

            // file loading, save filename.
            this.m_pages.Content.Add(inputFilename);

            // Close file
            file.Close();
            return true;
        }

        // Praser one row in the file or input string.
        public override string PraserRow(string a_row)
        {
            // first character is a symbol, "*" mean bookmark, "@" mean command, ";" mean comment and one row only could be an bookmark, command, comment.
            // "[xxx]" mean tag ,and one row could have multi tag.
            // first character isn't symbol or tag, that text could be a content.
            if (this.ParserComment(a_row))
            {
                return "COMMENT";
            }
            else if (this.ParserBookmark(a_row))
            {
                return "BOOKMARK";
            }
            else if (this.ParserCommand(a_row))
            {
                return "COMMAND";
            }
            else if (this.ParserTag(a_row))
            {
                return "TAG_AND_CONTENT";
            }
            else
            {
                this.ParserContent(a_row);
                return "CONTENT";
            }
        }

        // Clone input list into dynamic queue.
        public void AddDynamicQueue( ArrayList a_list )
        {
            int i = 0;
            IScriptData target = null;
            IScriptData clone = null;
            for (i = a_list.Count - 1; i >= 0 ; i--)
            {
                // clone object
                target = (IScriptData)a_list[i];
                clone = null;
                if (target != null)
                    clone = target.Clone(this.m_dynmicQueue);

                // Insert to dynamic queue.
                this.m_dynmicQueue.Insert(clone, 0);
            }
            // clear current data object
            this.m_currentReaderDataObject = null;
        }

        // Jump to page.
        public void ChangePage(string a_pagename, string a_filename, int a_startIndex )
        {
            // describe variable
            int i = 0, j = 0, k = 0;
            string inputFilename = a_filename.Split('.')[0];
            bool isLoaded = false;
            // 1. Check file is exist
            if ( inputFilename != "")
            {
                for( i = 0 ; i < this.m_pages.Content.Count && !isLoaded ; i++ )
                {
                    if (inputFilename.Equals((string)this.m_pages.Content[i]))
                        isLoaded = true;
                }
            }
            // 2. file exist, find page and run
            if( isLoaded )
            {
                this.ChangeCurrentPage(this.GetPageName(a_pagename, inputFilename), a_startIndex);
            }
            // 3. file not exist, Load file and run page
            else
            {
                // 3.1 Load the first file.
                this.LoadFile(a_filename);

                // 3.2 Execute reader 
                this.ChangeCurrentPage(this.m_currentLoadFileDefaultPageName, a_startIndex);
            }
        }

        // Public Get / Set Method
        public string GetPageName(string a_pagename = "", string a_filename = "" )
        {
            string pagename = "";
            if (a_filename == "")
                a_filename = "system";

            pagename = "default." + a_filename.Split('.')[0];
            if (a_pagename != "")
                pagename += "." + a_pagename;
            return pagename;
        }

        // Get page by name
        public IPageReader GetPage(string a_pagename)
        {
            if (this.m_pages.Attribute[a_pagename] == null)
                return null;
            return (IPageReader)this.m_pages.Attribute[a_pagename];
        }

        // Get data object in current page.
        public IScriptData GetDataObject()
        {
            return this.GetPage(this.m_currentReadPageIndex).Current();
        }

        // Set special character in list. It will use on GetWaitingTimeBySpecialCharacter.
        public void SetSpecialCharacter( string a_character, int a_characterAmount )
        {
            if (this.m_specialCharacterSymbol[a_character] != null)
                this.m_specialCharacterSymbol[a_character] = a_characterAmount;
            else
                this.m_specialCharacterSymbol.Add(a_character, a_characterAmount);
        }

        // Get Waiting time, if input character in symbol list, use defined character amount.
        // if input character isn't in symbol list, return time at character amount is 1.
        public int GetWaitingTimeByCharacter( string a_character )
        {
            if (this.m_specialCharacterSymbol[a_character] != null)
                return this.GetWaitingTimeByCharacter((int)this.m_specialCharacterSymbol[a_character]);
            return this.GetWaitingTimeByCharacter(1);
        }

        // Get waiting time by character amount.
        public int GetWaitingTimeByCharacter( int a_characterAmount = 1 )
        {
            if (this.m_canSkip && (this.IsAutoSkip || this.m_isSkip))
                return this.m_systemDelay;
            return a_characterAmount * this.m_characterDelay;
        }

        // Get waiting time, use it to make sure waiting time could use.
        public int GetWaitingTime(int a_millisecond)
        {
            if (this.m_canSkip && this.IsAutoSkip)
                return this.m_systemDelay;
            return a_millisecond;
        }

        // Attribute
        public int PagesCount
        {
            get { return this.m_pages.Attribute.Count; }
        }

        public IPageReader CurrentReadPage
        {
            get { return this.m_currentReadPage; }
        }

        public string CurrentReadFileName
        {
            get { return this.m_currentReadFileName; }
        }

        public string CurrentReadPageName
        {
            get { return this.m_currentReadPageName; }
        }

        // Is skip, skip state control.
        public bool IsSkip
        {
            set 
            {
                this.m_isSkip = value;
                
                // if input is true, complete all execute module
                if( this.m_isSkip )
                {
                    for (int i = 0; i < this.m_executeModules.Count; i++)
                    {
                        ((ScriptModule)this.m_executeModules[i]).ExecuteSkip();
                    }

                    // Skip trigger
                    if( this.OnSkipTrigger != null )
                        this.OnSkipTrigger(this);
                }
            }
            get
            {
                if (!this.m_isSkip)
                {
                    this.m_isSkip = this.IsAutoSkip || this.IsAutoRead;
                }
                return this.m_isSkip;
            }
        }

        // Auto skip, if autoskip is true, than isSkip will always true, and waiting time always is system delay.
        public bool IsAutoSkip
        {
            set { this.m_isAutoSkip = value; }
            get 
            {
                if (!this.m_canAutoSkip)
                    return false;
                return this.m_isAutoSkip; 
            } 
        }

        // Auto read, if autoread is true, than isSkip will always true, but waiting time isn't system delay.
        public bool IsAutoRead
        {
            set { this.m_isAutoRead = value; }
            get 
            {
                if (!this.m_canAutoRead)
                    return false;
                return this.m_isAutoRead; 
            }
        }

        // Can skip or not, if value is false, than all the get waiting time function will not affect by IsSkip
        public bool EnabledSkip
        {
            set { this.m_canSkip = value; }
        }

        // Can auto skip or not, IsAutoSkip will always false
        public bool EnabledAutoSkip
        {
            set { this.m_canAutoSkip = value; }
        }

        // Can auto read or not, IsAutoRead will always false
        public bool EnabledAutoRead
        {
            set { this.m_canAutoRead = value; }
        }

        // System delay, the value minimum can't less than 1ms.
        public int SystemDelay
        {
            set 
            { 
                this.m_systemDelay = value;
                if (this.m_systemDelay < 1)
                    this.m_systemDelay = 1;
            }
            get { return this.m_systemDelay; }
        }

        // Character Delay, the value minimum can't less than system delay.
        public int CharacterDelay
        {
            set
            {
                this.m_characterDelay = value;
                if (this.m_characterDelay < this.m_systemDelay)
                    this.m_characterDelay = this.m_systemDelay;
            }
            get { return this.m_characterDelay; }
        }
        // Private Method
        // Page control method
        private bool ChangeCurrentPage(string a_pagename = "", int a_start = 0)
        {
            // 0. Check page is exists.
            // if input assign is empty, use script reader default page.
            if (a_pagename == "")
                a_pagename = ScriptReader.DEFAULT_PAGE;

            // Save page information.
            string[] info = a_pagename.Split('.');
            this.m_currentReadPageIndex = a_pagename;
            if (info.Length >= 2)
                this.m_currentReadFileName = info[1];
            else
                this.m_currentReadFileName = "";
            if (info.Length >= 3)
                this.m_currentReadPageName = info[2];
            else
                this.m_currentReadPageName = "";

            // Make sure page name is exists in page list.
            this.m_currentReadPage = this.GetPage(this.m_currentReadPageIndex);
            if (this.CurrentReadPage == null)
                return false;

            // page go to the assign start point.
            this.CurrentReadPage.JumpTo(a_start);

            // clear read data object point.
            this.m_currentReaderDataObject = null;

            return true;
        }
        // Praser method
        private bool ParserBookmark(string a_row)
        {
            // bookmark pattern
            string pattern = @"\*[^\*]*";

            // Match by regular expressions
            Match match = Regex.Match(a_row, pattern);

            // first character is a symbol "*", it mean bookmark
            // bookmark name | bookmark content
            if (match.Index == 0 && match.Length - match.Index > 1)
            {
                // process bookmark
                // bookmark[0] = bookmark name, bookmark[1] = bookmark content.
                string[] bookmark = a_row.Substring(1).Split('|');
                string name = "";
                string label = this.m_currentLoadPageLabel;

                // check bookmark name, if name is empty, use current name and addition count number. 
                if (bookmark.Length > 0)
                {
                    if (bookmark[0] != "")
                    {
                        name = bookmark[0];
                        this.m_autoLoadPageName = name;
                        this.m_autoLoadPageCount = 1;
                    }
                    else
                    {
                        name = this.m_autoLoadPageName + ":" + this.m_autoLoadPageCount++;
                    }
                }

                // Add file name at first.
                name = this.m_currentLoadFileDefaultPageName + "." + name;

                // check content, if content is empty, use current content. 
                if (bookmark.Length > 1)
                {
                    if (bookmark[1] != "")
                    {
                        label = bookmark[1];
                    }
                }

                // 1. create new page.
                SDPage newpage = new SDPage(this, name, label);

                if (this.m_pages.Attribute[name] != null)
                {
                    // duplicate page, remove old page
                    IScriptData oldpage = (IScriptData)this.m_pages.Attribute[name];
                    this.m_pages.Attribute[name] = null;
                    oldpage = null;

                    // assign newpage
                    this.m_pages.Attribute[name] = newpage;
                }
                else
                    this.m_pages.Attribute.Add(name, newpage);

                // 2. finish current page , and store a jump command that to jump to new page.
                IPageReader currentpage = (IPageReader)this.m_pages.Attribute[this.m_currentLoadPageName];
                // if current page could take back, stop function.
                if (currentpage == null)
                    return false;
                // create jump command
                IScriptData command = new SDJump(name, currentpage);

                // 3. change current page pointer to new page.
                this.m_currentLoadPageName = name;
                this.m_currentLoadPageLabel = label;
                return true;

            }
            return false;
        }
        private bool ParserCommand(string a_row)
        {
            // command pattern
            string pattern = @"@[^@ ]*";
            string attributeString = "";

            // Match by regular expressions
            Match match = Regex.Match(a_row, pattern);

            // first character is a symbol "@", it mean command
            // command name {attribute=value}*
            if (match.Index == 0 && match.Length - match.Index > 1)
            {
                // process command
                SDCommand command = null;

                // 1. create command, and store command into current page.
                // take match string and ignore symbol "@".
                command = new SDCommand(match.Value.Substring(1), (IPageReader)this.m_pages.Attribute[this.m_currentLoadPageName]);
                attributeString = a_row.Substring(match.Index + match.Length);

                // 2. addition attribute 
                // Use regular expressions to find attribute string.
                // 2.1 match 『 xxx = "yyy zzz"』 token
                attributeString = this.ParserCommandAttribute(
                    attributeString,
                    " [^ ]+ *= *\"[^\"]*\"",
                    command,
                    SDCommand.ATTRIBUTE_TYPE_STRING);

                // 2.2 match 『 xxx = yyy』 token
                attributeString = this.ParserCommandAttribute(
                    attributeString,
                    " [^ ]+ *= *[^ ]+",
                    command,
                    SDCommand.ATTRIBUTE_TYPE_VALUE);

                // 2.3 match 『 xxx』 token
                attributeString = this.ParserCommandAttribute(
                    attributeString,
                    " [^ ]+",
                    command,
                    SDCommand.ATTRIBUTE_TYPE_DEFAULT);
                /*
                pattern = " [^ ]+";
                match = Regex.Match(defaultAttributeString, pattern);
                while (match.Success)
                {
                    command.AddAttribute(defaultAttributeString.Substring(match.Index, match.Length));
                    match = match.NextMatch();
                }
                 */
                return true;
            }
            return false;
        }
        private string ParserCommandAttribute(string a_attributeString, string a_pattern, SDCommand a_command, string a_commandType)
        {
            int pointer = 0;
            string otherAttributeString = "";
            Match match = Regex.Match(a_attributeString, a_pattern);
            while (match.Success)
            {
                // make sure no more string before match token.
                if (pointer < match.Index)
                {
                    // if have more string, it need use other way to match
                    otherAttributeString += " " + a_attributeString.Substring(pointer, match.Index - pointer);
                }
                // take attribute.
                a_command.AddAttribute(match.Value, a_commandType);
                // change pointer.
                pointer = match.Index + match.Length;
                // next match token.
                match = match.NextMatch();
            }
            // make sure no more string after match token.
            if (pointer < a_attributeString.Length)
            {
                otherAttributeString += " " + a_attributeString.Substring(pointer, a_attributeString.Length - pointer);
            }
            return otherAttributeString;
        }
        private bool ParserComment(string a_row)
        {
            // comment pattern
            string pattern = @";";

            // Match by regular expressions
            Match match = Regex.Match(a_row, pattern);

            // first character is a symbol ";", it mean comment
            // comment content
            if (match.Index == 0 && match.Length - match.Index >= 1)
            {
                // process comment, do not thing.
                return true;
            }
            return false;
        }
        private bool ParserTag(string a_row)
        {
            // tag pattern
            string pattern = @"\[[^\[]*\]";
            int pointer = 0;
            // Match by regular expressions
            Match match = Regex.Match(a_row, pattern);
            while (match.Success)
            {
                // if match token length is lower than 2, it isn't tag.
                if (match.Length > 2)
                {
                    // if pointer number equal match start point, that mean didn't have any content before start point.
                    if (pointer != match.Index)
                    {
                        // Content in row
                        // Process content
                        this.ParserContent(a_row.Substring(pointer, match.Index - pointer));
                    }

                    // Tag in row
                    // Process tag
                    //Console.WriteLine("Match at {0}-{1}, {2}", match.Index, match.Length, match.Value);
                    this.ParserCommand("@" + a_row.Substring(match.Index + 1, match.Length - 2));

                    // move pointer
                    pointer = match.Index + match.Length;
                }

                match = match.NextMatch();
            }

            // row don't have any tag
            if (pointer == 0)
                return false;

            // row still have cotent in string
            if (pointer != a_row.Length)
            {
                //Console.WriteLine("Content at {0}-{1}", pointer, a_row.Length - pointer);
                this.ParserContent(a_row.Substring(pointer, a_row.Length - pointer));
            }
            //
            return true;
        }
        private bool ParserContent(string a_row)
        {
            // tag pattern
            string pattern = @"\[[^\[]*";
            string content = "";
            bool haveEscapes = false;
            Match match = Regex.Match(a_row, pattern);
            int pointer = 0;
            // if match false, than row is content
            if (!match.Success)
                content = a_row;
            else
            {
                // if match start point bigger than 0, have more string before start point.
                content = a_row.Substring(0, match.Index);
                while (match.Success)
                {
                    // if match token length lower than 1, it is escapes.
                    if (haveEscapes || match.Length > 1)
                    {
                        // escapes exists, don't ignore first character
                        if (haveEscapes)
                            content += match.Value;
                        else
                            content += match.Value.Substring(1);

                        // escapes affect turn to false
                        haveEscapes = false;
                    }
                    else
                    {
                        // escapes affect turn to true
                        haveEscapes = true;
                        // move pointer
                        pointer = match.Index + match.Length;
                    }
                    match = match.NextMatch();
                }
            }
            new SDContent(content, (IPageReader)this.m_pages.Attribute[this.m_currentLoadPageName]);
            return true;
        }
    }
}
