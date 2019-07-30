﻿using System;
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

        // Uploads file & stores its entire contents into a string array
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            Counted.Text = "";
            totalWords.Text = "";
            if (FileUpload1.HasFile)
            {
                try
                {
                    string filename = Path.GetFileName(FileUpload1.FileName);
                    string path = Server.MapPath("~/") + "Files/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileUpload1.SaveAs(path + filename);
                    Status.Text = filename + " uploaded successfully";
                    string[] allWords = new StreamReader(path + filename).ReadToEnd().Split(' ', '\n');
                    Session["allwords"] = allWords;
                    MapReduce();
                }
                catch (Exception ex)
                {
                    Status.Text = "Error - " + ex.Message;
                }
            }
            else { Status.Text = "Unable to upload file. Please try a different file."; }
        }

        // Initializes the name node, & displays the final results.
        private void MapReduce()
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
            Counted.Text = namenode.allocate().ToString();
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