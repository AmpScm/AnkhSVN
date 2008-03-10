using System;
using System.Collections.Generic;
using System.Text;
using Ankh;
using Rhino.Mocks;
using Ankh.Selection;

namespace AnkhSvn_UnitTestProject.Mocks
{
    static class AnkhContextMock
    {
        public static IContext GetInstance(MockRepository mocks)
        {
            IContext context = mocks.DynamicMock<IContext>();
            ISelectionContext selectionContext = mocks.DynamicMock<ISelectionContext>();

            using (mocks.Record())
            {
                Expect.Call(context.SelectionContext).Return(selectionContext).Repeat.Any();
            }

            return context;
        }
    }
}
