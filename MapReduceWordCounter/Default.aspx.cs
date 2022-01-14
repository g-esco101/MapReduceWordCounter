using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MapReduceWordCounter
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Reads file & stores its entire contents into a string array
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            char[] delimiterChars = { ' ', '\n' };
            Counted.Text = "";
            totalWords.Text = "";
            if (FileUpload1.HasFile)
            {
                try
                {
                    string fileName = Path.GetFileName(FileUpload1.FileName);
                    string extension = Path.GetExtension(fileName);
                    if (extension == ".txt" || extension == ".doc" || extension == ".pdf" || extension == ".docx")
                    {
                        using (StreamReader reader = new StreamReader(FileUpload1.PostedFile.InputStream))
                        {
                            string[] allWords = reader.ReadToEnd().Split(delimiterChars);
                            Session["allwords"] = allWords;
                            Status.Text = fileName + " read successfully";
                            CountWords();
                        }
                    } else
                    {
                        Status.Text = "Error - please use one of the following file types: txt, doc, docx, or pdf.";
                        return;
                    }

                }
                catch (Exception ex)
                {
                    Status.Text = "Error - " + ex.Message;
                }
            }
            else { Status.Text = "Unable to upload file. Please try a different file."; }
        }

        // Initializes the name node, & displays the final results.
        private void CountWords()
        {
            int partitions;
            string[] allWords = (string[])Session["allwords"];
            try
            {
                partitions = Convert.ToInt32(threadCount.Text);
            }
            catch
            {
                partitions = 1;
                //System.Diagnostics.Debug.WriteLine("Error: thread count will be assigned 1.");
            }
            if (partitions <= 0)
            {
                partitions = 1;   // Error: thread count will be assigned 1; it must be greater than 0.
            }
            NameNode namenode = new NameNode(TextBoxMap.Text, TextBoxReduce.Text, TextBoxCombiner.Text, allWords, partitions);
            Counted.Text = namenode.Allocate().ToString();
            totalWords.Text = allWords.Length.ToString();
            //try
            //{
            //    totalWords.Text = allWords.Length.ToString();
            //}
            //catch (Exception ex)
            //{
            //    totalWords.Text = "error - " + ex.Message;
            //}
        }
    }
}