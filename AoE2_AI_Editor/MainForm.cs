using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace AoE2_AI_Editor
{


    public partial class MainForm : Form
    {

        ScintillaNET.Scintilla textArea;
        private string brutText = "";
        private string filePath;
        private AILexer aILexer;

        public MainForm()
        {
            aILexer = new AILexer();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textArea = new ScintillaNET.Scintilla();
            textPanel.Controls.Add(textArea);
            textArea.Dock = System.Windows.Forms.DockStyle.Fill;
            textArea.TextChanged += (this.OnTextChanged);
            textArea.StyleNeeded += (this.scintilla_StyleNeeded);
            initNumberMargin();
            initSyntaxColoring();


        }

        #region Initializers

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
        #endregion

        private void setWindowTitle()

        {
            string fileName = filePath.Split('\\').Last();
            this.Text = "AoE2 AI Editor | " + fileName;
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            // Append text to an existing file named "WriteLines.txt".
            if (filePath == null)
            {
                saveAs_Click(sender, e);
            }
            else
            {
                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.Write(brutText);
                }
            }

        }
        private void openFile_Click(object sender, EventArgs events)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filePath = openFileDialog.FileName;
                    StreamReader streamReader = new StreamReader(openFileDialog.FileName);


                    string text = streamReader.ReadToEnd();

                    brutText = text;
                    textArea.Text = brutText;

                    streamReader.Dispose();
                    setWindowTitle();

                }
                catch (IOException exeption)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(exeption.Message);
                }
            }
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath= saveFileDialog.FileName;
                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.Write(brutText);
                }
                setWindowTitle();
            }
        }

        private void newFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath= saveFileDialog.FileName;
                brutText = "";
                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.Write(brutText);
                }
                setWindowTitle();
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            brutText = textArea.Text;
        }


        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

    }
}
