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
                StoreId = f.StoreID,
                Name = f.FullFolderPath,
            }).ToList();
        }

        private static IEnumerable<Folder> GetFolderList()
        {
            List<Folder> folders = new List<Folder>();

            foreach (Folder rootFolder in Globals.ThisAddIn.Application.Session.Stores
                                               .Cast<Store>()
                                               .Select(s => s.GetRootFolder())
                                               .Cast<Folder>()
                                               .ToList()) {
                FolderVisitor(rootFolder.Folders.Cast<Folder>().ToList(), folders);
            }

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

        public static void MoveCurrentSelectionToFolder(string folderId, string storeId)
        {
            Selection selection = Globals.ThisAddIn.Application.ActiveExplorer().Selection;

            if (selection == null || selection.Count == 0)
                return;

            MAPIFolder folder = Globals.ThisAddIn.Application.ActiveExplorer().Session.GetFolderFromID(folderId, storeId);
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
