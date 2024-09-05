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

namespace AoE2_AI_Editor
{
    public partial class mainScreen : Form
    {

        private string aiText;
        private string filePath;

        public mainScreen()
        {
            InitializeComponent();
            
        }

        private void setWindowTitle()

        {
            string fileName = filePath.Split('\\').Last();
            this.Text = "AoE2 AI Editor | " + fileName;
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            // Append text to an existing file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(filePath, false))
            {
                outputFile.Write(aiText);
                Console.WriteLine("Saved!");
            }
        }

        private void openFile_Click(object sender, EventArgs events)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filePath = openFileDialog.FileName;
                    // Open the text file using a stream reader.
                    StreamReader streamReader = new StreamReader(openFileDialog.FileName);

                    // Read the stream as a string.
                    string text = streamReader.ReadToEnd();
                    mainTextBox.Text = text;
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

        private void mainTextBox_TextChanged(object sender, EventArgs e)
        {
            aiText = mainTextBox.Text;
        }

        private void saveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath= saveFileDialog.FileName;
                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.Write(aiText);
                }
                setWindowTitle();
            }
        }

        private void newFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath= saveFileDialog.FileName;
                aiText = "";
                mainTextBox.Text = aiText;
           
                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.Write(aiText);
                }
                setWindowTitle();
            }
        }
        
    }
}
