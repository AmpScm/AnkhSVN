using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;

namespace Ankh.UI.PendingChanges.Conflicts
{
    class ConflictListView : ListViewWithSelection<ConflictListItem>
    {
        IAnkhServiceProvider _context;

        public ConflictListView()
        {
            Initialize();
        }

        void Initialize()
        {
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                if (value != null)
                {
                    IFileIconMapper mapper = value.GetService<IFileIconMapper>();
                    SmallImageList = mapper.ImageList;
                }
            }
        }
    }
}
