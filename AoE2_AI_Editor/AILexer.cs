using Newtonsoft.Json;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2_AI_Editor
{
    internal class AILexer
    {

        private readonly string[] load_if_keywords = ["#load-if-defined", "#load-if-not-defined"];
        private readonly string[] end_else_keywords = ["#else", "#end-if"];
        private readonly string[] operator_keywords = [">", "<", ">=", "<=", "==", "!="];
        private readonly string[] defrule_keywords = ["defrule", "=>", "and", "or", "nand", "nor", "not"];
        private List<string> unknownCommands = [];

        private Dictionary<String, String> condition_keywords;
        private Dictionary<String, String> action_keywords;
        private const string defconst_keyword = "defconst";

        private readonly Char[] separation_char = [' ', '\n', '\r', ';', '(', ')', '\t'];

        int lastCaretPos = 0;


        public AILexer() {
            using (StreamReader actionFile = new StreamReader("./actions.json"))
            {
                string json = actionFile.ReadToEnd();
                action_keywords = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);

            }

            using (StreamReader conditionFile = new StreamReader("./conditions.json"))
            {
                string json = conditionFile.ReadToEnd();
                condition_keywords = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
            }

        }

        public void Style(Scintilla scintilla, int startPos, int endPos)
        {
            // Back up to the line start
            var line = scintilla.LineFromPosition(startPos);
            startPos = scintilla.Lines[line].Position;
            int length = 0;
            LexerState state = LexerState.Unknown;

            // Start styling
            scintilla.StartStyling(startPos);
            //Console.WriteLine("#########"+startPos+ " " + endPos);

            while (startPos < endPos)
            {
                
                char c = (char)scintilla.GetCharAt(startPos);

            REPROCESS:
                //Console.WriteLine(state + " char : " + c);
                switch (state)
                {
                    case LexerState.Unknown:

                        if (separation_char.Contains(c))
                        {
                            string identifier = scintilla.GetTextRange(startPos - length, length);

                            if ( ((char)scintilla.GetCharAt(startPos - length - 1) == '(') &&
                                !(condition_keywords.ContainsKey(identifier)) &&
                                !(action_keywords.ContainsKey(identifier)) &&
                                !(unknownCommands.Contains(identifier)) &&
                                !(operator_keywords.Contains(identifier)) &&
                                !(defrule_keywords.Contains(identifier)))
                            {

                                unknownCommands.Add(identifier);
                                Console.WriteLine(unknownCommands.Last());
                            }


                            AiWordType type = getAiWordType(identifier);
                            if (c == ';')
                            {
                                //Console.WriteLine(identifier);
                                state = LexerState.Comment;
                                scintilla.SetStyling(length, (int)type);
                                length = 0;
                                goto REPROCESS;
                            }
                            else if (length == 0) {
                                scintilla.SetStyling(1, (int)AiWordType.Default);
                            }
                            else {
                                //Console.WriteLine(identifier);
                                scintilla.SetStyling(length, (int)type);
                                length = 0;
                                goto REPROCESS;
                            }
                            length = 0;
                        }
                        else if (c == '#')
                        {
                            state = LexerState.Load_if_declare;
                            goto REPROCESS;
                        }
                        else {
                            length++;
                        }

                        break;

                    case LexerState.Line_break_required:

                        if (c == ';')
                        {
                            state = LexerState.Comment;
                            scintilla.SetStyling(length, (int)AiWordType.Unknown);
                            length = 0;
                            goto REPROCESS;
                        }
                        else if (c ==  ' ')
                        {
                            scintilla.SetStyling(1, (int)AiWordType.Default);

                        }
                        else if (c == '\n')
                        {
                            scintilla.SetStyling(1, (int)AiWordType.Default);
                            state = LexerState.Unknown;
                        }
                        else
                        {
                            scintilla.SetStyling(1, (int)AiWordType.Unknown);
                        }
                        
                        break;


                    case LexerState.Comment:
                        if (c == '\n' || c == '\r')
                        {
                            state = LexerState.Unknown;
                            goto REPROCESS;

                        }
                        else
                        {
                            scintilla.SetStyling(1, (int)AiWordType.Comment);
                        }

                        break;

                    #region Loadif

                    case LexerState.Load_if_declare:
                        if (separation_char.Contains(c))
                        {

                            string identifier = scintilla.GetTextRange(startPos - length, length);
                            if (load_if_keywords.Contains(identifier))
                            {
                                if (c == '\n' || c == '\r' || c == ';')
                                {
                                    scintilla.SetStyling(length, (int)AiWordType.Unknown);
                                }
                                else if (c == ' ')
                                {
                                    scintilla.SetStyling(length, (int)AiWordType.Load_if_declare);
                                    state = LexerState.Load_if_object;
                                }

                            }
                            else if (end_else_keywords.Contains(identifier))
                            {
                                scintilla.SetStyling(length, (int)AiWordType.Load_if_declare);
                                state = LexerState.Line_break_required;
                            }
                            else
                            {
                                scintilla.SetStyling(length, (int)AiWordType.Unknown);
                            }


                            if (state == LexerState.Load_if_object || state == LexerState.Line_break_required)
                            {
                                length = 0;
                                goto REPROCESS;

                            }
                            else if (c == '\n' || c == '\r' || c == ' ')
                            {
                                length = 0;
                                state = LexerState.Unknown;
                                goto REPROCESS;
                            }
                            else if (c == ';')
                            {
                                length = 0;
                                state = LexerState.Comment;
                                goto REPROCESS;
                            }


                        }
                        else
                        {
                            length++;
                        }
                        break;

                    case LexerState.Load_if_object:
                        if (separation_char.Contains(c))
                        {
                            if (c == ' ' && length == 0)
                            {
                                scintilla.SetStyling(1, (int)AiWordType.Default);
                            }
                            else if (length == 0 && (c == '\n' || c == '\r'))
                            {
                                scintilla.SetStyling(1, (int)AiWordType.Unknown);
                                state = LexerState.Unknown;
                                goto REPROCESS;
                            }
                            else if (length == 0 && c == ';')
                            {
                                scintilla.SetStyling(1, (int)AiWordType.Unknown);
                                state = LexerState.Comment;
                                goto REPROCESS;
                            }
                            else
                            {
                                scintilla.SetStyling(length, (int)AiWordType.Load_if_object);
                                state = LexerState.Line_break_required;
                                length = 0;
                                goto REPROCESS;
                            }
                        }
                        else
                        {
                            length++;
                        }
                        break;

                    #endregion

                }

                startPos++;
            }
        }

        private AiWordType getAiWordType(String inputString)
        {
            AiWordType outputType = AiWordType.Default;
            if (inputString == defconst_keyword)
            {
                outputType = AiWordType.Defconst;
            }
             else if (defrule_keywords.Contains(inputString))
            {
                outputType = AiWordType.Defrule;
            }
            else if (operator_keywords.Contains(inputString))
            {
                outputType = AiWordType.Operator;
            }
            else if (condition_keywords.ContainsKey(inputString))
            {
                outputType = AiWordType.Condition;
            }
            else if (action_keywords.ContainsKey(inputString))
            {
                outputType = AiWordType.Action;
            }
            return outputType;
        }

        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        public void scintilla_UpdateUI(Scintilla scintilla)
        {
            // Has the caret changed position?
            var caretPos = scintilla.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                // Is there a brace to the left or right?
                if (caretPos > 0 && IsBrace(scintilla.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(scintilla.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = scintilla.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
                    {
                        scintilla.BraceBadLight(bracePos1);
                        scintilla.HighlightGuide = 0;
                    }
                    else
                    {
                        scintilla.BraceHighlight(bracePos1, bracePos2);
                        scintilla.HighlightGuide = scintilla.GetColumn(bracePos1);
                    }
                }
                else
                {
                    // Turn off brace matching
                    scintilla.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                    scintilla.HighlightGuide = 0;
                }
            }
        }



    }


    enum LexerState
    {
        Unknown,
        Comment,
        Load_if_declare,
        Load_if_object,
        Line_break_required,
    }

    enum AiWordType
    {
        Default,
        Unknown,
        Defrule,
        Defconst,
        Load_if_declare,
        Load_if_object,
        Action,
        Condition,
        Operator,
        Comment

    }


}
