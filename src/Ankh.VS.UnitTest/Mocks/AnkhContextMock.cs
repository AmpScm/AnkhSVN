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
            return GetInstance(mocks, null);
        }

        public static IContext GetInstance(MockRepository mocks, IUIShell uiShell)
        {
            EnvDTE.DTE dte = DteMock.GetDteInstance(mocks);

            IContext context = mocks.DynamicMultiMock<IContext>(typeof(IDTEContext));
            Ankh.Configuration.AnkhConfig config = new Ankh.Configuration.AnkhConfig();

            using (mocks.Record())
            {
                Expect.Call(((IDTEContext)context).DTE).Return(dte).Repeat.Any();
                Expect.Call(context.Configuration.Instance).Return(config).Repeat.Any();

                if (uiShell != null)
                    Expect.Call(context.UIShell).Return(uiShell).Repeat.Any();
            }

            return context;
        }
    }
}
