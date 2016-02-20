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
            public string fileName;
            public string filePath;
            public byte[] hash;
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

        // private static string removePath(string FullName, string Path)
        // {
        //     return FullName.Substring(Path.Length);
        // }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderDialog1.ShowDialog() == DialogResult.OK)
            {
                basePath = folderDialog1.SelectedPath;
                baseFoldertextBox.Text = basePath;
                logListBox.Items.Clear();
                compareFoldersTextBox.Clear();
                progressBar1.Value = 0;

                baseList = listFiles(basePath);
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
            if ((folderDialog1.ShowDialog() == DialogResult.OK) & (folderDialog1.SelectedPath != basePath))
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

        private void button3_Click(object sender, EventArgs e)
        {
            numDeleted = 0;
            progressBar1.Maximum = compareFileLists.Count * baseList.Count;

            for (var i = 0; i < compareFileLists.Count; i++)
            {
                compareAndDelete(compareFileLists[i]);
            }

            statusTextBox.AppendText("\r\n" + numDeleted + " file(s) deleted");
            button3.Enabled = false;
        }

        private void safeDelete(string fileName)
        {
            if (File.Exists(fileName))
            try
            {
                numDeleted++;
                File.Delete(fileName);
                logListBox.Items.Add(fileName + " deleted");
            }
            catch (Exception ex)
            {
                numDeleted--;
                statusTextBox.AppendText("\r\n" + ex.Message);
            }
        }

        private void compareAndDelete(List<FileData> listToCompare)
        {
            foreach (var baseFile in baseList)
            {
                for (var compareIndex = 0; compareIndex < listToCompare.Count; compareIndex++)
                {
                    if (compareFileData(baseFile, listToCompare[compareIndex],
                        fastRadio.Checked || strictRadio.Checked,
                        normalRadio.Checked || strictRadio.Checked)) safeDelete(listToCompare[compareIndex].filePath);
                }
            }
        }

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
                if (file1.hash == null)
                {
                    file1 = new FileData
                    {
                        fileName = file1.fileName,
                        filePath = file1.filePath,
                        size = file1.size,
                        hash = createHash(file1.filePath)
                    };
                }
                if (file2.hash == null)
                {
                    file2 = new FileData
                    {
                        fileName = file2.fileName,
                        filePath = file2.filePath,
                        size = file2.size,
                        hash = createHash(file2.filePath)
                    };
                }
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

    }
}
