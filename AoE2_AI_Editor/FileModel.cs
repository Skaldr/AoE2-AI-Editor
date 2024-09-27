using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoE2_AI_Editor
{
    internal class FileModel
    {
        public string filePath { get; set; }
        public string brutText { get; set; }
        public ScintillaNET.Scintilla textArea { get; set; }
        private AILexer aILexer;
        public FileModel(AILexer aILexer)
        {
            this.aILexer = aILexer;
            filePath = null;
            brutText = "";
            textArea = new ScintillaNET.Scintilla();
            textArea.TextChanged += (this.OnTextChanged);
            textArea.StyleNeeded += (this.scintilla_StyleNeeded);
            textArea.UpdateUI += (this.scintilla_UpdateUI);
            textArea.Dock = System.Windows.Forms.DockStyle.Fill;

            initNumberMargin();
            initSyntaxColoring();
            this.aILexer = aILexer;
        }

        private void scintilla_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            var startPos = textArea.GetEndStyled();
            var endPos = e.Position;

            aILexer.Style(textArea, startPos, endPos);
        }


        private void initSyntaxColoring()
        {

            // Configure the default style
            textArea.StyleResetDefault();
            textArea.Styles[Style.Default].Font = "Consolas";
            textArea.Styles[Style.Default].Size = 10;
            textArea.Styles[Style.Default].BackColor = Color.White;
            textArea.Styles[Style.Default].ForeColor = Color.Black;
            textArea.StyleClearAll();


            textArea.Styles[(int)AiWordType.Default].ForeColor = Color.Black;
            textArea.Styles[(int)AiWordType.Comment].ForeColor = Color.Green;

            textArea.Styles[(int)AiWordType.Load_if_declare].ForeColor = Color.Orange;
            textArea.Styles[(int)AiWordType.Load_if_declare].Weight = 900;

            textArea.Styles[(int)AiWordType.Load_if_object].ForeColor = Color.DarkRed;

            textArea.Styles[(int)AiWordType.Unknown].ForeColor = Color.Red;

            textArea.Styles[(int)AiWordType.Defconst].ForeColor = Color.DarkBlue;
            textArea.Styles[(int)AiWordType.Defconst].Weight = 900;

            textArea.Styles[(int)AiWordType.Defrule].ForeColor = Color.Blue;
            textArea.Styles[(int)AiWordType.Defrule].Weight = 900;

            textArea.Styles[(int)AiWordType.Action].ForeColor = Color.DarkCyan;
            textArea.Styles[(int)AiWordType.Condition].ForeColor = Color.Purple;

            textArea.Styles[(int)AiWordType.Operator].ForeColor = Color.Red;

            textArea.IndentationGuides = IndentView.LookBoth;
            textArea.Styles[Style.BraceLight].BackColor = Color.LightGray;
            textArea.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            textArea.Styles[Style.BraceBad].ForeColor = Color.Red;

            textArea.Lexer = Lexer.Container;

        }

        private void initNumberMargin()
        {

            textArea.Styles[Style.LineNumber].ForeColor = Color.Black;


            var nums = textArea.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

        }

        private void scintilla_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            aILexer.scintilla_UpdateUI(textArea);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            brutText = textArea.Text;
        }
    }
}
