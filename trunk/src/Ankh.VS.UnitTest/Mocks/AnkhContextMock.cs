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

        public static IContext GetInstance(MockRepository mocks, IUIShell uiShell, ISelectionContext selContext)
        {
            EnvDTE.DTE dte = DteMock.GetDteInstance(mocks);

            IContext context = mocks.DynamicMultiMock<IContext>(typeof(IDTEContext));
            Ankh.Config.Config config = new Ankh.Config.Config();

            using (mocks.Record())
            {
                Expect.Call(((IDTEContext)context).DTE).Return(dte).Repeat.Any();
                Expect.Call(context.Config).Return(config).Repeat.Any();

                if (uiShell != null)
                    Expect.Call(context.UIShell).Return(uiShell).Repeat.Any();
                Expect.Call(context.SelectionContext).Return(selContext).Repeat.Any();
            }

            return context;
        }
    }
}
