using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace FolderCompare
{
    public partial class MainForm : Form
    {
        public class FileData
        {
            public string fileName;
            public string filePath;
            public byte[] hash;
            public long size;
            public bool found;
        }

        private struct workData
        {
            public List<List<FileData>> CompareLists;
            public List<FileData> BaseList;
            public bool cName, cHash, delete, listUnequal;
        }

        private List<List<FileData>> compareFileLists;
        private List<FileData> baseList;

        private int numDeleted, numCompared;

        public MainForm()
        {
            InitializeComponent();
        }

        delegate void voidStringDelegate(string text);

        private void addLOG(string s)
        {
            if (logListBox.InvokeRequired)
            {
                logListBox.Invoke((voidStringDelegate)addLOG, new object[] { s });
            }
            else
            {
                logListBox.Items.Add(s);
            }
        }

        private void addSTATUS(string s)
        {
            if (statusTextBox.InvokeRequired)
            {
                statusTextBox.Invoke((voidStringDelegate)addSTATUS, new object[] { s });
            }
            else
            {
                statusTextBox.AppendText(s);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderDialog1.ShowDialog() == DialogResult.OK)
            {
                baseFoldertextBox.Text = folderDialog1.SelectedPath;
                logListBox.Items.Clear();
                compareFoldersTextBox.Clear();
                progressBar1.Value = 0;

                baseList = listFiles(folderDialog1.SelectedPath);
                compareFileLists = new List<List<FileData>>();

                statusTextBox.Text = baseList.Count + " file(s) found in " + folderDialog1.SelectedPath;
                button2.Enabled = true;
            }
        }
        private static List<FileData> listFiles(string CurrentFolder)
        {
            var DI = new DirectoryInfo(CurrentFolder);

            var list = DI.GetFiles().Select(file => new FileData
            {
                fileName = file.Name,
                filePath = file.FullName,
                size = file.Length
            }).ToList();

            foreach (var folder in DI.GetDirectories())
            {
                list.AddRange(listFiles(folder.FullName));
            }

            return list;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if ((folderDialog1.ShowDialog() == DialogResult.OK) & (folderDialog1.SelectedPath != baseFoldertextBox.Text))
            {
                compareFileLists.Add(listFiles(folderDialog1.SelectedPath));

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
        private void button3_Click(object sender, EventArgs e) // compare and delete
        {
            numDeleted = 0;
            progressBar1.Value = 0;
            numCompared = 0;
            progressBar1.Maximum = compareFileLists.Count * baseList.Count;
            UseWaitCursor = true;
            button3.Enabled = false;
            var wdata = new workData
            {
                BaseList = baseList,
                CompareLists = compareFileLists,
                cName = fastRadio.Checked || strictRadio.Checked,
                cHash = normalRadio.Checked || strictRadio.Checked,
                delete = true,
                listUnequal = false
            };
            backgroundWorker1.RunWorkerAsync(wdata);
        }

        private void button7_Click(object sender, EventArgs e) //compare only
        {
            numDeleted = 0;
            progressBar1.Value = 0;
            numCompared = 0;
            progressBar1.Maximum = compareFileLists.Count * baseList.Count;
            UseWaitCursor = true;
            button3.Enabled = false;
            var wdata = new workData
            {
                BaseList = baseList,
                CompareLists = compareFileLists,
                cName = fastRadio.Checked || strictRadio.Checked,
                cHash = normalRadio.Checked || strictRadio.Checked,
                delete = false,
                listUnequal = checkBox1.Checked
            };
            backgroundWorker1.RunWorkerAsync(wdata);
        }

        private void safeDelete(string fileName)
        {
            if (File.Exists(fileName))
            try
            {
                numDeleted++;
                File.Delete(fileName);
                addLOG(fileName + " deleted");
            }
            catch (Exception ex)
            {
                numDeleted--;
                addSTATUS("\r\n" + ex.Message);
            }
        }

        private void compareAndDelete(List<FileData> BaseList, List<FileData> listToCompare, bool cname, bool chash)
        {
            foreach (var baseFile in BaseList)
            {
                for (var compareIndex = 0; compareIndex < listToCompare.Count; compareIndex++)
                {
                    if (compareFileData(baseFile, listToCompare[compareIndex],
                        cname,
                        chash)) safeDelete(listToCompare[compareIndex].filePath);
                }
                numCompared++;
                backgroundWorker1.ReportProgress(numCompared);
            }
        }
        private void compareOnly(List<FileData> BaseList, List<FileData> listToCompare, bool cname, bool chash, bool cunEqual)
        {
            for (var baseFileIndex = 0; baseFileIndex < BaseList.Count; baseFileIndex++)
            {
                for (var compareIndex = 0; compareIndex < listToCompare.Count; compareIndex++)
                {
                    if (compareFileData(BaseList[baseFileIndex], listToCompare[compareIndex], cname, chash))
                    {
                        BaseList[baseFileIndex].found = true;
                        listToCompare[compareIndex].found = true;

                        if (!cunEqual)
                        {
                            addLOG("Match: " + listToCompare[compareIndex].filePath);
                        }
                    }
                }
                numCompared++;
                backgroundWorker1.ReportProgress(numCompared);
            }
        }

        #region comparers
        private static bool compareFileData(FileData file1, FileData file2, bool CompareName, bool CompareHash)
        {
            return (compareSize(file1, file2) &&
                    (!CompareName || compareName(file1, file2)) &&
                    (!CompareHash || compareHash(file1, file2)));
        }

        private static bool compareSize(FileData file1, FileData file2)
        {
            return (file1.size == file2.size);
        }

        private static bool compareName(FileData file1, FileData file2)
        {
            return (file1.fileName.Equals(file2.fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool compareHash(FileData file1, FileData file2)
        {
            var result = (File.Exists(file1.filePath) && File.Exists(file2.filePath));
            if (result)
            {
                if (file1.hash == null) file1.hash = createHash(file1.filePath);
                if (file2.hash == null) file2.hash = createHash(file2.filePath);

                result = compareHashValues(file1.hash, file2.hash);
            }
            return result;
        }

        private static byte[] createHash(string path)
        {
            var HashMaker = MD5.Create();
            var FileStream = File.OpenRead(path);
            var HashResult = HashMaker.ComputeHash(FileStream);
            HashMaker.Dispose();
            FileStream.Dispose();
            return HashResult;
        }

        static private bool compareHashValues(byte[] a, byte[] b)
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
        #endregion

        #region empty files/folders
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
        #endregion

        #region JPEG
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
        #endregion
        
        #region AsyncWorker
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                var wData = (workData) e.Argument;
                if (wData.delete)
                {
                    //compare and delete duplicates
                    for (var i = 0; i < wData.CompareLists.Count; i++)
                    {
                        compareAndDelete(wData.BaseList, wData.CompareLists[i], wData.cName, wData.cHash);
                    }
                }
                else
                {
                    //compare data
                    for (var i = 0; i < wData.CompareLists.Count; i++)
                    {
                        compareOnly(wData.BaseList, wData.CompareLists[i], wData.cName, wData.cHash, wData.listUnequal);
                    }

                    //show differnces
                    if (wData.listUnequal)
                    {
                        foreach (var fd in wData.BaseList)
                        {
                            if (!fd.found) addLOG("Differs: " + fd.filePath);
                        }

                        for (var i = 0; i < wData.CompareLists.Count; i++)
                        {
                            foreach (var fd in wData.CompareLists[i])
                            {
                                if (!fd.found) addLOG("Differs: " + fd.filePath);
                            }
                        }

                    }
                }
            }
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            UseWaitCursor = false;
            statusTextBox.AppendText("\r\n" + numDeleted + " file(s) deleted");
        }
        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        #endregion


    }
}
