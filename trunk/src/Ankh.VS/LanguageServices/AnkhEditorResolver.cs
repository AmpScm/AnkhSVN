﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Ankh.VS.LanguageServices
{
    [GlobalService(typeof(IAnkhEditorResolver))]
    class AnkhEditorResolver : AnkhService, IAnkhEditorResolver
    {
        readonly EditorFactory _factory;
        public AnkhEditorResolver(IAnkhServiceProvider context)
            : base(context)
        {
            _factory = new EditorFactory();
            _factory.SetSite(this);
        }

        public bool TryGetLanguageService(string extension, out Guid languageService)
        {
            if (!string.IsNullOrEmpty(extension))
            {
                string value = _factory.GetLanguageService(extension);

                if (!string.IsNullOrEmpty(value))
                {
                    languageService = new Guid(value);
                    return true;
                }
            }
            languageService = Guid.Empty;
            return false;
        }
    }
}
