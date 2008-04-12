using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using System.ComponentModel.Design;
using AnkhSvn.Ids;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// Implements a simple VS Languageservice to implement syntaxcoloring on our LogMessages
    /// </summary>
    [Guid(AnkhId.LogMessageLanguageServiceId), ComVisible(true), CLSCompliant(false)]
    public partial class LogMessageLanguageService : LanguageService
    {
        public const string ServiceName = AnkhId.LogMessageServiceName;
        public LogMessageLanguageService()
		{
		}

		public override void UpdateLanguageContext(Microsoft.VisualStudio.TextManager.Interop.LanguageContextHint hint, Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer, Microsoft.VisualStudio.TextManager.Interop.TextSpan[] ptsSelection, Microsoft.VisualStudio.Shell.Interop.IVsUserContext context)
		{
			base.UpdateLanguageContext(hint, buffer, ptsSelection, context);
		}

        public override ViewFilter CreateViewFilter(CodeWindowManager mgr, IVsTextView newView)
        {
            return new LogMessageViewFilter(this, mgr, newView);
        }
        
		public override LanguagePreferences GetLanguagePreferences()
		{
            LanguagePreferences lp = new LanguagePreferences(this.Site, typeof(LogMessageLanguageService).GUID, ServiceName);

			lp.AutoListMembers = false;
			lp.AutoOutlining = false;
			lp.EnableCodeSense = false;
			lp.EnableCommenting = true;
			lp.EnableFormatSelection = false;
			lp.EnableLeftClickForURLs = true;
			lp.EnableMatchBraces = false;
			lp.EnableMatchBracesAtCaret = false;
			lp.EnableQuickInfo = false;
			lp.EnableShowMatchingBrace = false;
			lp.HideAdvancedMembers = false;
			lp.IndentStyle = IndentingStyle.Block;
			lp.InsertTabs = false;
			lp.ParameterInformation = false;
			lp.ShowNavigationBar = false;
			lp.TabSize = 2;			
			lp.VirtualSpace = false;
			lp.WordWrap = true;
			lp.WordWrapGlyphs = true;

			return lp;
		}

		CommentScanner _scanner;
		public override IScanner GetScanner(Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer)
		{
			if (_scanner == null)
				_scanner = new CommentScanner();
			return _scanner;
		}

		public override string Name
		{
			get { return ServiceName; }
		}

		public override AuthoringScope ParseSource(ParseRequest req)
		{
			return null;
		}

        public override Source CreateSource(Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer)
        {
            return new LogmessageSource(this, buffer, GetColorizer(buffer));
        }     

		class CommentScanner : IScanner
		{
			int _offset;
			string _line;
			#region IScanner Members

			public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
			{
				if (string.IsNullOrEmpty(_line) || _offset >= _line.Length)
					return false;

				int pState = state;
				state = 0;

				if (_offset == 0)
				{
					while (_offset < _line.Length)
					{
						if (char.IsWhiteSpace(_line, _offset))
							_offset++;
						else
							break;
					}

					if (_offset < _line.Length)
					{
						switch (_line[_offset])
						{
							case '#':
								if (tokenInfo != null)
								{
									tokenInfo.Color = TokenColor.Comment;
									tokenInfo.StartIndex = _offset;
									tokenInfo.EndIndex = _line.Length;
									tokenInfo.Trigger = TokenTriggers.None;
									tokenInfo.Type = TokenType.LineComment;
								}
								state = 1;
								_offset = _line.Length;
								return true;
							default:
								if (tokenInfo != null)
								{
									tokenInfo.Color = TokenColor.Text;
									tokenInfo.StartIndex = _offset;
									tokenInfo.EndIndex = _line.Length;
									tokenInfo.Trigger = TokenTriggers.None;
									tokenInfo.Type = TokenType.Text;
								}
								state = 0;
								_offset = _line.Length;
								return true;
						}
					}
				}
				return false;
			}

			public void SetSource(string source, int offset)
			{
				_line = source;
				_offset = offset;
			}

			#endregion
		}        
    }

    class LogmessageSource : Source
    {
        public LogmessageSource(LogMessageLanguageService service, IVsTextLines textLines, Colorizer colorizer)
            : base(service, textLines, colorizer)
        {
        }

        bool _initializedInfo;
        CommentInfo _commentInfo;
        public override CommentInfo GetCommentFormat()
        {
            if (!_initializedInfo)
            {
                _commentInfo.BlockStart = null;
                _commentInfo.BlockEnd = null;
                _commentInfo.LineStart = "#";
                _commentInfo.UseLineComments = true;

                _initializedInfo = true;
            }
            return _commentInfo;
        }
    }

    partial class LogMessageViewFilter : ViewFilter
    {
        public LogMessageViewFilter(LogMessageLanguageService service, CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view)
        {
        }

        public bool ShowLogMessageContextMenu(int menuId, Guid groupGuid, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target, int x, int y)
        {
            if (groupGuid == Microsoft.VisualStudio.Shell.VsMenus.guidSHLMainMenu && menuId == Microsoft.VisualStudio.Shell.VsMenus.IDM_VS_CTXT_CODEWIN)
            {
                groupGuid = AnkhId.CommandSetGuid;
                menuId = (int)AnkhCommandMenu.PendingChangesLogMessageMenu;
				return true;
            }
			return false;
        }
    }
}
