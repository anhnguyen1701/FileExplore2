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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initDriveInfo();
        }

        //init drive first time
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

        // action after select drive from treeview
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

        //open directory
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

        private void setCurrentDir(DirectoryInfo input)
        {
            currentDir = input;
            pathTextBox.Text = currentDir.FullName;
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
                contextMenuStrip1.Items[3].Enabled = false; //Copy
                contextMenuStrip1.Items[4].Enabled = false; //Move
                contextMenuStrip1.Items[5].Enabled = false; //Delete
                //contextMenuStrip1.Items[7].Enabled = true;  //New Folder
            }
            else if (listView2.SelectedIndices.Count == 1)
            {
                contextMenuStrip1.Items[0].Enabled = true;
                contextMenuStrip1.Items[1].Enabled = true;
                contextMenuStrip1.Items[3].Enabled = true;
                contextMenuStrip1.Items[4].Enabled = true;
                contextMenuStrip1.Items[5].Enabled = true;
                //contextMenuStrip1.Items[7].Enabled = false;
            }
            else
            {
                contextMenuStrip1.Items[0].Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
                contextMenuStrip1.Items[3].Enabled = true;
                contextMenuStrip1.Items[4].Enabled = true;
                contextMenuStrip1.Items[5].Enabled = true;
                //contextMenuStrip1.Items[7].Enabled = false;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}