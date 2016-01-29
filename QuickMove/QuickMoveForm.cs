using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuickMove.Properties;

namespace QuickMove
{
    public partial class QuickMoveForm : Form
    {
        // Required for the WinForms designer
        protected QuickMoveForm()
        {
            InitializeComponent();

            Text = "Quick Move " + typeof(QuickMoveForm).Assembly.GetName().Version.ToString();

            if (Settings.Default.WindowSize != default(Size))
                Size = Settings.Default.WindowSize;
        }

        private readonly QuickMoveModel _model;

        public QuickMoveForm(QuickMoveModel model)
            : this()
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            _model = model;

            UpdateList();
        }

        private void UpdateList()
        {
            List<FolderModel> data = _model.Folders;
            var text = searchTextBox.Text;
            if (!string.IsNullOrWhiteSpace(text)) {
                data = _model.Folders
                    .Where(folder => folder.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    .ToList();
            }

            foldersList.BeginUpdate();
            foldersList.DataSource = data;
            foldersList.DisplayMember = "Name";
            foldersList.EndUpdate();
        }

        private void foldersList_SelectedValueChanged(object sender, EventArgs e)
        {
            _model.SelectedFolder = foldersList.SelectedItem as FolderModel;
        }

        private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) {
                Done();
            } else if (e.KeyCode == Keys.Escape) {
                Cancel();
            } else {
                UpdateList();
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
           Cancel();
        }
        private void Done()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel()
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            Done();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.WindowSize = Size;
            base.OnClosing(e);
        }

        private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && foldersList.Items.Count > 0) {
                foldersList.SelectedIndex = Math.Max(0, foldersList.SelectedIndex - 1); 
            } else if (e.KeyCode == Keys.Down && foldersList.Items.Count > 0) {
                foldersList.SelectedIndex = Math.Min(foldersList.Items.Count - 1, foldersList.SelectedIndex + 1); 
            }
        }
    }
}
