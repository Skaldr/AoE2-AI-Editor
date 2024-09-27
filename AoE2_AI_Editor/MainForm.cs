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

        private List<FileModel> files = new List<FileModel>();
        private FileModel currentFile;
        AILexer aILexer;


        public MainForm()
        {
            aILexer = new AILexer();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            currentFile = new FileModel(aILexer);

            tabPage1.Text = "New file";
            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            files.Add(currentFile);
            tabPage1.Controls.Add(panel);
            panel.Controls.Add(currentFile.textArea);
            tabControl1.SelectTab(files.Count() - 1);

        }


        private void saveFile_Click(object sender, EventArgs e)
        {
            // Append text to an existing file named "WriteLines.txt".
            if (currentFile.filePath == null)
            {
                saveAs_Click(sender, e);
            }
            else
            {
                using (StreamWriter outputFile = new StreamWriter(currentFile.filePath, false))
                {
                    outputFile.Write(currentFile.brutText);
                }
            }

        }

        private void switchTab(object sender, System.Windows.Forms.TabControlEventArgs e)
        {
            currentFile = files.ElementAt(e.TabPageIndex);
        }

        private void openFile_Click(object sender, EventArgs events)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    createNewTab();
                    currentFile.filePath = openFileDialog.FileName;
                    StreamReader streamReader = new StreamReader(openFileDialog.FileName);


                    string text = streamReader.ReadToEnd();

                    currentFile.brutText = text;
                    currentFile.textArea.Text = currentFile.brutText;

                    tabControl1.SelectedTab.Text = currentFile.filePath.Split('\\').Last();

                    streamReader.Dispose();

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
                currentFile.filePath = saveFileDialog.FileName;
                tabControl1.SelectedTab.Text = currentFile.filePath.Split('\\').Last();
                using (StreamWriter outputFile = new StreamWriter(currentFile.filePath, false))
                {
                    outputFile.Write(currentFile.brutText);
                }
            }
        }

        private void createNewTab()
        {
            currentFile = new FileModel(aILexer);

            System.Windows.Forms.TabPage tabPage = new System.Windows.Forms.TabPage();
            tabPage.Text = "New file *";
            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Controls.Add(tabPage);
            files.Add(currentFile);
            tabPage.Controls.Add(panel);
            panel.Controls.Add(currentFile.textArea);
            tabControl1.SelectTab(files.Count() - 1);
        }

        private void newFile_Click(object sender, EventArgs e)
        {
            createNewTab();

        }


        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

    }
}
