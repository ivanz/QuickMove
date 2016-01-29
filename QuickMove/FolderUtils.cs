using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickMove
{
    public static class FolderUtils
    {
        public static IList<FolderModel> GetFolderModelList()
        {
            return GetFolderList().Select(f => new FolderModel {
                Id = f.EntryID,
                Name = f.FullFolderPath,
            }).ToList();
        }

        private static IEnumerable<Folder> GetFolderList()
        {
            List<Folder> folders = new List<Folder>();
            FolderVisitor(Globals.ThisAddIn.Application.ActiveExplorer().Session.Folders.Cast<Folder>().ToList(), folders);

            return folders;
        }

        private static void FolderVisitor(IEnumerable<Folder> currentFolders, List<Folder> collection)
        {
            if (currentFolders == null || currentFolders.Count() == 0)
                return;

            foreach (Folder currentFolder in currentFolders.ToArray().ToList()) {
                if (currentFolder.DefaultItemType != OlItemType.olMailItem)
                    continue;

                collection.Add(currentFolder);
                FolderVisitor(currentFolder.Folders.Cast<Folder>().ToList(), collection);
            }
        }

        public static void MoveCurrentSelectionToFolder(string folderId)
        {
            Selection selection = Globals.ThisAddIn.Application.ActiveExplorer().Selection;

            if (selection == null || selection.Count == 0)
                return;

            Folder folder = GetFolderList().SingleOrDefault(f => f.EntryID.Equals(folderId));
            if (folder == null)
                return;

            foreach (MailItem mailItem in selection.OfType<MailItem>().ToList()) {
                try {
                    mailItem.Move(folder);
                } catch {
                    // do nothing
                }
            }
        }
    }
}
