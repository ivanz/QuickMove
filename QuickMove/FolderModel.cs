using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickMove
{
    public class FolderModel
    {
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string Name { get; set; }
    }

    public class QuickMoveModel
    {
        public QuickMoveModel()
        {
            Folders = new List<FolderModel>();
            SelectedFolder = null;
            IsLoaded = false;
        }

        public bool IsLoaded { get; private set; }

        public List<FolderModel> Folders { get; private set; }
        public FolderModel SelectedFolder { get; set; }

        public Task RefreshFolderList()
        {
            Folders = new List<FolderModel>();
            SelectedFolder = null;

            return Task.Run(() => {
                Folders = FolderUtils.GetFolderModelList().ToList();
                IsLoaded = true;
            });
        }

        public bool CheckValid()
        {
            return SelectedFolder != null;
        }
    }
}
