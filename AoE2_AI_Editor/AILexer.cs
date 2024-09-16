using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2_AI_Editor
{
    internal class AILexer
    {

        private readonly string[] if_else_keywords = ["load-if-defined", "load-if-not-defined", "else", "end-if"];
        private const string defrule_keyword = "defrule";
        private const string defconst_keyword = "defrule";


        public AILexer() { 
        
        }

        public void Style(Scintilla scintilla, int startPos, int endPos)
        {
            // Back up to the line start
            var line = scintilla.LineFromPosition(startPos);
            startPos = scintilla.Lines[line].Position;
            int length = 0;
            AiWordType state = AiWordType.Default;

            // Start styling
            scintilla.StartStyling(startPos);
            while (startPos < endPos)
            {
                var c = (char)scintilla.GetCharAt(startPos);

            REPROCESS:
                switch (state)
                {
                    case AiWordType.Default:
                    case AiWordType.Unknown:
                        if (c == ';')
                        {
                            // Start of "string"
                            state = AiWordType.Comment;
                            goto REPROCESS;
                        }
                        else if (c == '#')
                        {
                            scintilla.SetStyling(1, (int)AiWordType.Load_if_declare);
                            scintilla.SetStyling(0, (int)AiWordType.Default);
                            state = AiWordType.Load_if_declare;
                        }
                        else
                        {
                            // Everything else
                            scintilla.SetStyling(1, (int) AiWordType.Default);
                        }
                        break;

                    
                    case AiWordType.Comment:
                        scintilla.SetStyling(1, (int)AiWordType.Comment);


                        if (c == '\n' || c ==  '\r')
                        {
                            state = AiWordType.Default;
                        }
                    
                        break;

                    case AiWordType.Load_if_declare:

                        if (c == '\n' || c== '\r' || c == ' ' || c == ';')
                        {
                            
                            string identifier = scintilla.GetTextRange(startPos - length, length);
                            identifier = identifier.Replace("\n", string.Empty);

                            if (if_else_keywords.Contains(identifier))
                            {
                                scintilla.SetStyling(length, (int)AiWordType.Load_if_declare);


                            }
                            else
                            {
                                Console.WriteLine("set to unknown");
                                scintilla.SetStyling(length, (int)AiWordType.Unknown);
                            }


                            if (c == '\n' || c == '\r')
                            {
                                state = AiWordType.Default;
                            }
                            else if (c == ';')
                            {
                                state = AiWordType.Comment;
                                goto REPROCESS;
                            }
                            else if (c == ' ')
                            {
                                state = AiWordType.Default;
                            }

                            length = 0;

                        }
                        else
                        {
                            length++;
                        }
                        break;
                }

                startPos++;
            }
        }

    }

    enum LexerState
    {

    }

    enum AiWordType
    {
        Default,
        Unknown,
        Defrule,
        Defconst,
        Load_if_declare,
        Load_if_object,
        Condition_declare,
        Condition_object,
        Action_declare,
        Action_object,
        Operator,
        Comment

    }
}
