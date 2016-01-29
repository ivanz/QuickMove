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
        public string Name { get; set; }
    }

    public class QuickMoveModel
    {
        public List<FolderModel> Folders { get; set; }
        public FolderModel SelectedFolder { get; set; }

        public bool CheckValid()
        {
            return SelectedFolder != null;
        }
    }
}
