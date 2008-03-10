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
            return GetInstance(mocks, null, mocks.DynamicMock<ISelectionContext>());
        }

        public static IContext GetInstance(MockRepository mocks, ISelectionContext selContext)
        {
            return GetInstance(mocks, null, selContext);
        }

        public static IContext GetInstance(MockRepository mocks, IUIShell uiShell)
        {
            return GetInstance(mocks, uiShell, mocks.DynamicMock<ISelectionContext>());
        }

        static IContext GetInstance(MockRepository mocks, IUIShell uiShell, ISelectionContext selContext)
        {
            IContext context = mocks.DynamicMock<IContext>();
            

            using (mocks.Record())
            {
                if (uiShell != null)
                    Expect.Call(context.UIShell).Return(uiShell).Repeat.Any();
                Expect.Call(context.SelectionContext).Return(selContext).Repeat.Any();
            }

            return context;
        }
    }
}
