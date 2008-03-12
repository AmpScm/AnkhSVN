using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using System.ComponentModel.Design;
using AnkhSvn.Ids;
using System.Runtime.InteropServices;

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
}
