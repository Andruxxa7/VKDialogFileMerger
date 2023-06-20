using System;
using System.Windows.Forms;
using static VKDialogHistoryFileMergerService.VkDialogHistoryFileMergerService;

namespace VkDialogHistoryFileMergerForm
{
    public partial class MergeForm : Form
    {
        private string _inputPath = ".";
        private string _outputPath;

        public MergeForm()
        {
            FormClosing += (_, _) => Application.Exit();
            InitializeComponent();
        }

        private void exitButton_Click(object sender, EventArgs e) => Close();

        private void selectInputButton_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            var result = folderDialog.ShowDialog();
            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(folderDialog.SelectedPath)) return;
            _inputPath = folderDialog.SelectedPath;
            inputPathLabel.Text = $"Input path: {_inputPath}";
        }

        private void selectOutputButton_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            var result = folderDialog.ShowDialog();
            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(folderDialog.SelectedPath)) return;
            _outputPath = folderDialog.SelectedPath;
            outputPathLabel.Text = $"Output path: {_outputPath}";
        }

        private void clearInputButton_Click(object sender, EventArgs e)
        {
            inputPathLabel.Text = "Input path:";
            _inputPath = ".";
        }

        private void clearOutputButton_Click(object sender, EventArgs e)
        {
            outputPathLabel.Text = "Output path:";
            _outputPath = null;
        }

        private async void mergeButton_Click(object sender, EventArgs e)
        {
            if (!await ExistsDialogFiles(_inputPath))
            {
                MessageBox.Show("There are no \'messages\' files in this directory. Change directory and try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(MergeFiles(_inputPath, сssCheckBox.Checked, _outputPath), "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}