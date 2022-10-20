using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FileExplore2
{
    public partial class Form1 : Form
    {
        DirectoryInfo currentDir;
        private string currentPath = "My Computer";

        private int status = 0; // status = 1 -> copy' status = 2 -> cut
        private string sourcePath = "";
        private string targetPath = "";
        private string fileCpy = "";


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initDriveInfo();
        }

        public void initDriveInfo()
        {
            pathTextBox.Text = currentPath;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeNode driveNode = new TreeNode(drive.Name);
                driveNode.Tag = drive.RootDirectory;
                driveNode.ImageIndex = 4;
                driveNode.SelectedImageIndex = 4;

                treeView1.Nodes.Add(driveNode);
            }
        }

        private void ShowDirectory()
        {
            listView2.Items.Clear();

            foreach (DirectoryInfo dir in currentDir.GetDirectories())
            {
                ListViewItem item = listView2.Items.Add(dir.Name);
                item.Tag = dir;
                item.ImageIndex = 2;
                item.SubItems.Add("");
                item.SubItems.Add("Folder");
            }

            foreach (FileInfo file in currentDir.GetFiles())
            {
                ListViewItem item = listView2.Items.Add(file.Name);
                item.Tag = file;
                item.SubItems.Add(file.Length.ToString());

                string fileExtension = file.Extension;
                item.SubItems.Add(fileExtension);

                switch (fileExtension.ToUpper())
                {
                    case ".EXE":
                        item.ImageIndex = 0;
                        break;
                    case ".ZIP":
                    case ".RAR":
                        item.ImageIndex = 3;
                        break;
                    default:
                        item.ImageIndex = 1;
                        break;
                }
            }
        }

        private void resetCopyPath()
        {
            sourcePath = "";
            targetPath = "";
            fileCpy = "";
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;

            try
            {
                if (selectedNode.Tag.GetType() == typeof(DirectoryInfo))
                {
                    selectedNode.Nodes.Clear();

                    setCurrentDir((DirectoryInfo)selectedNode.Tag);

                    ShowDirectory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("cannot open drive");
                Console.WriteLine(ex.ToString());
            }
        }

        private void listView2_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                if (listView2.SelectedItems[0].Tag.GetType() == typeof(DirectoryInfo))
                {
                    setCurrentDir((DirectoryInfo)listView2.SelectedItems[0].Tag);

                    ShowDirectory();
                }
                else
                {
                    FileInfo file = (FileInfo)listView2.SelectedItems[0].Tag;

                    //mo ung dung (doc ghi file)
                    Process.Start(new ProcessStartInfo { FileName = file.FullName, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("cannot open file");
                Console.WriteLine(ex.ToString());
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            string path = pathTextBox.Text;
            path = path.Substring(0, path.LastIndexOf("\\"));
            setCurrentDir(new DirectoryInfo(path));
            ShowDirectory();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView2.SelectedIndices.Count == 0)
            {
                contextMenuStrip1.Items[0].Enabled = false; //Open
                contextMenuStrip1.Items[1].Enabled = false; //Rename
                contextMenuStrip1.Items[2].Enabled = false; //Copy
                if (sourcePath != null && targetPath != null)
                {
                    contextMenuStrip1.Items[3].Enabled = true;

                }
                else
                {
                    contextMenuStrip1.Items[3].Enabled = false;
                }
                contextMenuStrip1.Items[4].Enabled = false; //Move
                contextMenuStrip1.Items[5].Enabled = false; //Delete
                contextMenuStrip1.Items[6].Enabled = true;  //New Folder
            }
            else if (listView2.SelectedIndices.Count == 1)
            {
                contextMenuStrip1.Items[0].Enabled = true;
                contextMenuStrip1.Items[1].Enabled = true;
                contextMenuStrip1.Items[2].Enabled = true;
                contextMenuStrip1.Items[3].Enabled = false; //Paste
                contextMenuStrip1.Items[4].Enabled = true;
                contextMenuStrip1.Items[5].Enabled = true;
                contextMenuStrip1.Items[6].Enabled = false;
            }
            else
            {
                contextMenuStrip1.Items[0].Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
                contextMenuStrip1.Items[2].Enabled = true;
                contextMenuStrip1.Items[3].Enabled = false;
                contextMenuStrip1.Items[4].Enabled = true;
                contextMenuStrip1.Items[5].Enabled = true;
                contextMenuStrip1.Items[6].Enabled = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem x in listView2.SelectedItems)
                {
                    string pathh = "";
                    if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
                    {
                        pathh = currentPath + "\\" + x.Text;
                    }
                    else
                    {
                        pathh = currentPath + x.Text;
                    }

                    if (x.Tag.GetType() == typeof(DirectoryInfo))
                    {
                        Directory.Delete(pathh, true);
                    }
                    else
                    {
                        File.Delete(pathh);
                    }
                }
                ShowDirectory();

            }
            catch (Exception ex)
            {
                MessageBox.Show("cannot delete");
                Console.WriteLine(ex.ToString());
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
            {
                sourcePath = currentPath + "\\" + listView2.SelectedItems[0].Text;
                fileCpy = listView2.SelectedItems[0].Text;
            }
            else
            {
                sourcePath = currentPath + listView2.SelectedItems[0].Text;
                fileCpy = listView2.SelectedItems[0].Text;
            }
            status = 1;

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (status == 1)
                {
                    // copy all directory
                    if (Directory.Exists(sourcePath))
                    {
                        if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
                        {
                            targetPath = currentPath + "\\" + fileCpy;
                            Directory.CreateDirectory(targetPath);
                        }
                        else
                        {
                            targetPath = currentPath + fileCpy;
                            Directory.CreateDirectory(targetPath);
                        }

                        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                        }

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                        }

                        ShowDirectory();
                        resetCopyPath();
                    }
                    else
                    {
                        // copy a file
                        if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
                        {
                            targetPath = currentPath + "\\" + fileCpy;
                        }
                        else
                        {
                            targetPath = currentPath + fileCpy;
                        }

                        File.Copy(sourcePath, targetPath, true);
                        ShowDirectory();
                        resetCopyPath();
                    }
                }
                else if (status == 2)
                {
                    if (Directory.Exists(sourcePath))
                    {
                        if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
                        {
                            targetPath = currentPath + "\\" + fileCpy;
                            Directory.CreateDirectory(targetPath);
                        }
                        else
                        {
                            targetPath = currentPath + fileCpy;
                            Directory.CreateDirectory(targetPath);
                        }

                        DirectoryInfo sourceInfo = Directory.CreateDirectory(sourcePath);

                        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                        {
                            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                        }

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                        {
                            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                        }

                        sourceInfo.Delete(true);
                    }
                    else
                    {
                        if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
                        {
                            targetPath = currentPath + "\\" + fileCpy;
                        }
                        else
                        {
                            targetPath = currentPath + fileCpy;
                        }
                        File.Move(sourcePath, targetPath);
                    }

                    ShowDirectory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void setCurrentDir(DirectoryInfo input)
        {
            currentDir = input;
            currentPath = currentDir.FullName;
            pathTextBox.Text = currentPath;
        }



        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentPath.Split("\\").Length >= 2 && currentPath.Split("\\")[1] != "")
            {
                sourcePath = currentPath + "\\" + listView2.SelectedItems[0].Text;
                fileCpy = listView2.SelectedItems[0].Text;
            }
            else
            {
                sourcePath = currentPath + listView2.SelectedItems[0].Text;
                fileCpy = listView2.SelectedItems[0].Text;
            }
            status = 2;
        }
    }
}