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
                    string filename = Path.GetFileName(FileUpload1.FileName);
                    using (StreamReader reader = new StreamReader(FileUpload1.PostedFile.InputStream))
                    {
                        string[] allWords = reader.ReadToEnd().Split(delimiterChars);
                        Session["allwords"] = allWords;
                        Status.Text = filename + " read successfully";
                        PrepWork();
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
        private void PrepWork()
        {
            int threadNumber = 1;
            string[] allWords = (string[])Session["allwords"];
            try
            {
                threadNumber = Convert.ToInt32(threadCount.Text);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Error: thread count will be assigned 1.");
            }
            if (threadNumber <= 0)
            {
                threadNumber = 1;   // Error: thread count will be assigned 1; it must be greater than 0.
            }
            NameNode namenode = new NameNode(TextBoxMap.Text, TextBoxReduce.Text, TextBoxCombiner.Text, allWords, threadNumber);
            Counted.Text = namenode.Allocate().ToString();
            try
            {
                totalWords.Text = allWords.Length.ToString();
            }
            catch (Exception ex)
            {
                totalWords.Text = "error - " + ex.Message;
            }
        }
    }
}