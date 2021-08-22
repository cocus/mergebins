using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace mergebins
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static String BytesToString(long byteCount)
        {
            // https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        private void buttonMerge_Click(object sender, System.EventArgs e)
        {
            var even = labelEvenPath.Tag as string;
            var odd = labelOddPath.Tag as string;
            if (even == null || odd == null)
            {
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = System.IO.Path.GetFullPath(even);
            saveFileDialog1.Title = "Save merged binary";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "bin";
            saveFileDialog1.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var output = saveFileDialog1.FileName;

            byte[] evenFileBytes = System.IO.File.ReadAllBytes(even);
            byte[] oddFileBytes = System.IO.File.ReadAllBytes(odd);

            if (evenFileBytes.Length != oddFileBytes.Length)
            {
                return;
            }

            var inputSize = oddFileBytes.Length;

            byte[] outputBytes = new byte[inputSize * 2];
            for (int i = 0; i < inputSize; i++)
            {
                outputBytes[(i * 2) + 0] = evenFileBytes[i];
                outputBytes[(i * 2) + 1] = oddFileBytes[i];
            }

            System.IO.File.WriteAllBytes(output, outputBytes);
        }
        private void generic_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void generic_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var file = files.First();
            if (System.IO.File.Exists(file))
            {
                long size = new System.IO.FileInfo(file).Length;

                ((Label)sender).Text = $"{file} ({BytesToString(size)})";
                ((Label)sender).Tag = file;
            }
        }
    }
}
