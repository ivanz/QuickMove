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


            Parallel.ForEach(rootFolder.Folders.Cast<Folder>(), (folder, state) => {
                List<FolderModel> folders = new List<FolderModel>();
                FolderEnumerator(folder, folders);
                foldersList.Add(folders);
            });

            return foldersList.SelectMany(list => list).ToList();
        }


        private static void FolderEnumerator(Folder currentFolder, List<FolderModel> collection)
        {
            collection.Add(CreateModel(currentFolder));

            foreach (Folder childFolder in currentFolder.Folders) {
                FolderEnumerator(childFolder, collection);
            }
        }

        private static FolderModel CreateModel(Folder childFolder)
        {
            return new FolderModel() {
                Id = childFolder.EntryID,
                StoreId = childFolder.StoreID,
                Name = childFolder.Name
            };
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
