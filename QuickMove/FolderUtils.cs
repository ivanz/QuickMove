using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace QuickMove
{
    public static class FolderUtils
    {
        public static IEnumerable<FolderModel> GetFolderModelList()
        {
            BlockingCollection<List<FolderModel>> foldersList = new BlockingCollection<List<FolderModel>>();

            Folder rootFolder = (Folder) Globals.ThisAddIn.Application.Session.DefaultStore.GetRootFolder();


            Parallel.ForEach(rootFolder.Folders.Cast<Folder>(), (source, state) => {
                List<FolderModel> folders = new List<FolderModel>();
                FolderVisitor(source.Folders.Cast<Folder>(), folders);
                foldersList.Add(folders);
            });

            return foldersList.SelectMany(list => list).ToList();
        }


        private static void FolderVisitor(IEnumerable<Folder> currentFolders, List<FolderModel> collection)
        {
            if (currentFolders == null || !currentFolders.Any())
                return;

            foreach (Folder currentFolder in currentFolders) {
                if (currentFolder.DefaultItemType != OlItemType.olMailItem)
                    continue;

                collection.Add(new FolderModel() {
                    Id = currentFolder.EntryID,
                    StoreId = currentFolder.StoreID,
                    Name = currentFolder.Name
                });

                FolderVisitor(currentFolder.Folders.Cast<Folder>(), collection);
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
