using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace QuickMove
{
    public partial class QuickMoveRibbon
    {
        private static readonly QuickMoveModel model = new QuickMoveModel();
        
        private void QuickMoveRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void quickMoveButton_Click(object sender, RibbonControlEventArgs e)
        {
            using (var dialog = new QuickMoveForm(model))
            {
                dialog.ShowDialog();
                if (dialog.DialogResult == System.Windows.Forms.DialogResult.OK) {
                    if (model.CheckValid())
                        FolderUtils.MoveCurrentSelectionToFolder(model.SelectedFolder.Id, model.SelectedFolder.StoreId);
                }
            }
            
        }
    }
}
