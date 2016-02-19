using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace FolderCompare
{
    public partial class MainForm : Form
    {
        public struct FileData
        {
            public string fileName; // file.ext
            public string filePath; // subpath/otherpath/file.ext
            public byte[] hash;
            public bool hashPerformed;
            public long size;
        }

        private List<List<FileData>> compareFileLists;
        private List<FileData> baseList;
        private string basePath;
        private int numDeleted;

        public MainForm()
        {
            InitializeComponent();
        }

        private static string removePath(string FullName, string Path)
        {
            return FullName.Substring(Path.Length);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (folderDialog1.ShowDialog() == DialogResult.OK)
            {
                basePath = folderDialog1.SelectedPath;
                baseFoldertextBox.Text = basePath;
                logListBox.Items.Clear();
                compareFoldersTextBox.Clear();
                progressBar1.Value = 0;

                //baseList = new List<FileData>();
                baseList = listFiles(basePath, basePath);
                compareFileLists = new List<List<FileData>>();

                statusTextBox.Text = baseList.Count + " file(s) found in " + folderDialog1.SelectedPath;
                button2.Enabled = true;
            }
        }
        private static List<FileData> listFiles(string StartFolder, string CurrentFolder)
        {
            var DI = new DirectoryInfo(CurrentFolder);
            var list = DI.GetFiles().Select(file => new FileData
            {
                fileName = file.Name,
                filePath = removePath(file.FullName, StartFolder),
                hashPerformed = false,
                size = file.Length
            }).ToList();
            foreach (var folder in DI.GetDirectories())
            {
                list.AddRange(listFiles(StartFolder, folder.FullName));
            }

            return list;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if ((folderDialog1.ShowDialog() == DialogResult.OK) & (folderDialog1.SelectedPath != basePath))
            {
                compareFileLists.Add(listFiles(folderDialog1.SelectedPath, folderDialog1.SelectedPath));

                compareFoldersTextBox.AppendText(folderDialog1.SelectedPath + "\r\n");

                statusTextBox.AppendText("\r\n" + compareFileLists[compareFileLists.Count - 1].Count + " file(s) found in " + folderDialog1.SelectedPath);
                button3.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            compareFoldersTextBox.Clear();
            progressBar1.Value = 0;
            compareFileLists = new List<List<FileData>>();
            button3.Enabled = false;
        }

        static private bool compareHashData(byte[] a, byte[] b)
        {
            var result = (a.Length == b.Length);
            if (result)
            {
                for (var i = 0; i < a.Length; i++)
                {
                    result = result & (a[i] == b[i]);
                }
            }
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            numDeleted = 0;
            progressBar1.Maximum = compareFileLists.Count;

            for (var i = 0; i < compareFileLists.Count; i++)
            {
                compareAndDelete(compareFileLists[i], compareFoldersTextBox.Lines[i]);
                progressBar1.Value = i + 1;
                Refresh();
            }

            statusTextBox.AppendText("\r\n" + numDeleted + " file(s) deleted");
            button3.Enabled = false;
        }

        private void safeDelete(string fileName)
        {
            try
            {
                numDeleted++;
                File.Delete(fileName);
                logListBox.Items.Add(fileName + " deleted");
            }
            catch (IOException ex)
            {
                numDeleted--;
                statusTextBox.AppendText("\r\n" + ex.Message);
            }
        }

        private void compareAndDelete(List<FileData> listToCompare, string StartFolder)
        {
            for (var baseIndex = 0; baseIndex < baseList.Count; baseIndex++)
            {
                for (var compareIndex = 0; compareIndex < listToCompare.Count; compareIndex++)
                {
                    if ((baseList[baseIndex].size == listToCompare[compareIndex].size) && ((listToCompare[compareIndex].fileName == baseList[baseIndex].fileName) || !strictcheckBox.Checked))
                    {
                        if (File.Exists(StartFolder + listToCompare[compareIndex].filePath))
                        {
                            var compareHashMaker = MD5.Create();
                            var compareFileStream =
                                File.OpenRead(StartFolder + listToCompare[compareIndex].filePath);
                            var compareHashResult = compareHashMaker.ComputeHash(compareFileStream);
                            compareHashMaker.Dispose();
                            compareFileStream.Dispose();

                            byte[] baseHashResult;

                            if (baseList[baseIndex].hashPerformed) baseHashResult = baseList[baseIndex].hash;

                            else
                            {
                                var baseHashMaker = MD5.Create();
                                var baseFileStream = File.OpenRead(basePath + baseList[baseIndex].filePath);
                                baseHashResult = baseHashMaker.ComputeHash(baseFileStream);
                                baseHashMaker.Dispose();
                                baseFileStream.Dispose();

                                baseList[baseIndex] = new FileData
                                {
                                    fileName = baseList[baseIndex].fileName,
                                    filePath = baseList[baseIndex].filePath,
                                    hash = baseHashResult,
                                    hashPerformed = true,
                                    size = baseList[baseIndex].size
                                };
                            }
                            // Same Data
                            if (compareHashData(baseHashResult, compareHashResult))
                            {

                                safeDelete(StartFolder + listToCompare[compareIndex].filePath);
                            }
                            else statusTextBox.AppendText("\r\nhash Mismatch: " + baseList[baseIndex].filePath + " =/= " + listToCompare[compareIndex].filePath);

                        }
                        else
                        {
                            statusTextBox.AppendText("\r\nFile not found (deleted?): " +
                                                     listToCompare[compareIndex].filePath);
                        }

                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //delete empty files
            logListBox.Items.Clear();
            numDeleted = 0;
            if (baseFoldertextBox.Text != "") searchEmptyFiles(baseFoldertextBox.Text);
            if (compareFoldersTextBox.Lines.Any())
            {
                foreach (var paths in compareFoldersTextBox.Lines)
                {
                    if (paths != "") searchEmptyFiles(paths);
                }
            }
            statusTextBox.AppendText("\r\n" + numDeleted + " empty file(s) deleted");

            //delete empty folders
            numDeleted = 0;
            if (baseFoldertextBox.Text != "") searchEmptyFolders(baseFoldertextBox.Text);
            if (compareFoldersTextBox.Lines.Any())
            {
                foreach (var paths in compareFoldersTextBox.Lines)
                {
                    if (paths != "") searchEmptyFolders(paths);
                }
            }
            statusTextBox.AppendText("\r\n" + numDeleted + " empty folder(s) deleted");
        }

        private void searchEmptyFiles(string path)
        {
            var DI = new DirectoryInfo(path);
            foreach (var file in DI.GetFiles())
            {
                if (file.Length == 0)
                {
                    logListBox.Items.Add(file.FullName);
                    try
                    {
                        numDeleted++;
                        File.Delete(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        numDeleted--;
                        statusTextBox.AppendText("\r\n" + ex.Message);
                    }
                }
            }
            foreach (var folder in DI.GetDirectories())
            {
                searchEmptyFiles(folder.FullName);
            }
        }

        private void searchEmptyFolders(string path)
        {
            var DI = new DirectoryInfo(path);
            foreach (var folder in DI.GetDirectories())
            {
                if (!folder.FullName.Contains(".git"))
                {
                    if ((!folder.GetFiles().Any()) & (!folder.GetDirectories().Any()))
                    {
                        logListBox.Items.Add(folder.FullName);
                        try
                        {
                            numDeleted++;
                            Directory.Delete(folder.FullName, false);
                        }
                        catch (Exception ex)
                        {
                            numDeleted--;
                            statusTextBox.AppendText("\r\n" + ex.Message);
                        }
                    }
                    else
                    {
                        searchEmptyFolders(folder.FullName);
                    }
                }
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            //find corrupted JPG
            logListBox.Items.Clear();
            numDeleted = 0;

            if (baseFoldertextBox.Text != "") searchBadJPG(baseFoldertextBox.Text);
            if (compareFoldersTextBox.Lines.Any())
            {
                foreach (var paths in compareFoldersTextBox.Lines)
                {
                    if (paths != "") searchBadJPG(paths);
                }
            }
            statusTextBox.AppendText("\r\n" + numDeleted + " corrupted JPG(s) found");
        }


        private void searchBadJPG(string path)
        {
            var DI = new DirectoryInfo(path);
            foreach (var file in DI.GetFiles())
            {
                if (string.Equals(file.Extension, ".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    if (!isValidJPG(file.FullName))
                    {
                        logListBox.Items.Add(file.FullName);
                        numDeleted++;
                    }
                }
            }
            foreach (var folder in DI.GetDirectories())
            {
                searchBadJPG(folder.FullName);
            }
        }
        private static bool isValidJPG(string file)
        {
            try
            {
                using (var buffer = Image.FromFile(file))
                {
                    return buffer.RawFormat.Equals(ImageFormat.Jpeg);
                }
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }




    }
}
