// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Windows.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

	BMenees	6.30.2003	Moved all of the DllImport code from all of my projects
						into this class.  This should give me a single point to
						modify when moving to .NET on 64-bit platforms.
-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

namespace Ankh.Diff
{
    internal sealed class NativeMethods
    {
        #region Public SendMessage Methods

        public static int SendMessage(IWin32Window Ctrl, int iMsg, int iWParam, int iLParam)
        {
            return (int)SendMessage(Ctrl.Handle, (IntPtr)iMsg, (IntPtr)iWParam, (IntPtr)iLParam);
        }

        #endregion

        #region Public Miscellaneous Methods

        public static void SetBorderStyle(CreateParams P, BorderStyle Style)
        {
            switch (Style)
            {
                case BorderStyle.Fixed3D:
                    P.Style = P.Style & ~WS_BORDER;
                    P.ExStyle = P.ExStyle | WS_EX_CLIENTEDGE;
                    break;
                case BorderStyle.FixedSingle:
                    P.Style = P.Style | WS_BORDER;
                    P.ExStyle = P.ExStyle & ~WS_EX_CLIENTEDGE;
                    break;
                case BorderStyle.None:
                    P.Style = P.Style & ~WS_BORDER;
                    P.ExStyle = P.ExStyle & ~WS_EX_CLIENTEDGE;
                    break;
            }
        }

        #endregion

        #region Public Scrolling Methods
        public static ScrollInfo GetScrollInfo(IWin32Window Ctrl, bool bHorz)
        {
            ScrollInfo Info = ScrollInfo.Create(SIF_ALL);
            GetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info);
            return Info;
        }

        public static int GetScrollPos(IWin32Window Ctrl, bool bHorz)
        {
            ScrollInfo Info = ScrollInfo.Create(SIF_POS);
            if (GetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info) != 0)
                return Info.nPos;
            else
                return 0;
        }

        public static void SetScrollPos(IWin32Window Ctrl, bool bHorz, int iPos)
        {
            ScrollInfo Info = ScrollInfo.Create(SIF_POS);
            Info.nPos = iPos;
            SetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info, 1);
        }

        public static void SetScrollPageAndRange(IWin32Window Ctrl, bool bHorz, int iMin, int iMax, int iPage)
        {
            ScrollInfo Info = ScrollInfo.Create(SIF_RANGE | SIF_PAGE);
            Info.nMin = iMin;
            Info.nMax = iMax;
            Info.nPage = (uint)iPage;
            SetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info, 1);
        }

        public static int ScrollWindow(IWin32Window Ctrl, int dx, int dy, ref Rectangle rcScroll, ref Rectangle rcClip)
        {
            return ScrollWindow(Ctrl.Handle, dx, dy, ref rcScroll, ref rcClip);
        }

        public static int ScrollWindow(IWin32Window Ctrl, int dx, int dy)
        {
            return ScrollWindow(Ctrl.Handle, dx, dy, (IntPtr)0, (IntPtr)0);
        }

        public static int GetScrollPage(IWin32Window Ctrl, bool bHorz)
        {
            ScrollInfo Info = ScrollInfo.Create(SIF_PAGE);
            if (GetScrollInfo(Ctrl.Handle, bHorz ? SB_HORZ : SB_VERT, ref Info) != 0)
                return (int)Info.nPage;
            else
                return 0;
        }

        #endregion

        #region Public Caret Methods

        public static bool CreateCaret(IWin32Window Ctrl, int nWidth, int nHeight)
        {
            return CreateCaret(Ctrl.Handle, IntPtr.Zero, nWidth, nHeight);
        }

        public static bool HideCaret(IWin32Window Ctrl)
        {
            return HideCaret(Ctrl.Handle);
        }

        public static bool ShowCaret(IWin32Window Ctrl)
        {
            return ShowCaret(Ctrl.Handle);
        }

        [DllImport("user32.dll")]
        public static extern bool DestroyCaret();

        [DllImport("user32.dll")]
        public static extern bool SetCaretPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCaretPos(ref Point lpPoint);

        #endregion

        #region Public Sound Methods

        public static bool PlaySound(string strSound)
        {
            return PlaySound(strSound, IntPtr.Zero, 0);
        }

        [DllImport("kernel32.dll")]
        public static extern bool Beep(int iFrequency, int iDuration);

        #endregion

        #region Public RichEdit Methods

        public static string GetText(RichTextBox rt)
        {
            int iCharLength = GetTextLength(rt) + 1;
            int iByteLength = 2 * iCharLength;

            GETTEXTEX GT = new GETTEXTEX();
            GT.iCb = iByteLength;
            GT.iFlags = GT_DEFAULT;
            GT.iCodepage = CP_UNICODE;

            StringBuilder SB = new StringBuilder(iCharLength);
            SendMessage(rt.Handle, EM_GETTEXTEX, ref GT, SB);
            string strText = SB.ToString();
            return strText;
        }

        public static int GetTextLength(RichTextBox rt)
        {
            GETTEXTLENGTHEX GTL = new GETTEXTLENGTHEX();
            GTL.uiFlags = GTL_DEFAULT;
            GTL.uiCodePage = CP_UNICODE;
            return (int)SendMessage(rt.Handle, (IntPtr)EM_GETTEXTLENGTHEX, ref GTL, IntPtr.Zero);
        }

        #endregion

        //************************

        #region Private Methods

        private NativeMethods()
        {
            //So no one can create an instance.
        }

        #endregion

        #region Private SendMessage Methods

        //We have to import this function multiple ways because sometimes the parameters
        //are pointers (i.e. references), and sometimes they're not.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, IntPtr msg, ref IntPtr wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, IntPtr msg, IntPtr wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, IntPtr msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Private Scrolling Methods

        [DllImport("user32.dll")]
        private extern static int GetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo Info);

        [DllImport("user32.dll")]
        private extern static int SetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo Info, int bRedraw);

        [DllImport("user32.dll")]
        private extern static int ScrollWindow(IntPtr hWnd, int dx, int dy, ref Rectangle rcScroll, ref Rectangle rcClip);

        [DllImport("user32.dll")]
        private extern static int ScrollWindow(IntPtr hWnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip);

        #endregion

        #region Private Caret Methods

        [DllImport("user32.dll")]
        private static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowCaret(IntPtr hWnd);

        #endregion

        #region Private Sound Methods

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        private static extern bool PlaySound(string strSound, IntPtr hModule, uint uiSound);

        #endregion

        #region Private RichEdit Methods

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, IntPtr msg, ref GETTEXTLENGTHEX wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, ref GETTEXTEX wParam, StringBuilder lParam);

        #endregion

        //************************

        #region WinUser.h Constants

        //Dialog Codes
        public const int DLGC_WANTARROWS = 0x0001      /* Control wants arrow keys         */;
        public const int DLGC_WANTTAB = 0x0002      /* Control wants tab keys           */;
        public const int DLGC_WANTALLKEYS = 0x0004      /* Control wants all keys           */;
        public const int DLGC_WANTMESSAGE = 0x0004      /* Pass message to control          */;
        public const int DLGC_HASSETSEL = 0x0008      /* Understands EM_SETSEL message    */;
        public const int DLGC_DEFPUSHBUTTON = 0x0010      /* Default pushbutton               */;
        public const int DLGC_UNDEFPUSHBUTTON = 0x0020     /* Non-default pushbutton           */;
        public const int DLGC_RADIOBUTTON = 0x0040      /* Radio button                     */;
        public const int DLGC_WANTCHARS = 0x0080      /* Want WM_CHAR messages            */;
        public const int DLGC_STATIC = 0x0100      /* Static item: don't include       */;
        public const int DLGC_BUTTON = 0x2000      /* Button item: can be checked      */;

        //ScrollBars for Get/SetScrollInfo
        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_CTL = 2;

        //Scroll Bar Commands
        public const int SB_LINEUP = 0;
        public const int SB_LINELEFT = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINERIGHT = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGELEFT = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGERIGHT = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_LEFT = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_RIGHT = 7;
        public const int SB_ENDSCROLL = 8;

        //Get/SetScrollInfo
        public const int SIF_RANGE = 0x0001;
        public const int SIF_PAGE = 0x0002;
        public const int SIF_POS = 0x0004;
        public const int SIF_DISABLENOSCROLL = 0x0008;
        public const int SIF_TRACKPOS = 0x0010;
        public const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

        //ScrollWindowEx
        public const int SW_SCROLLCHILDREN = 0x0001;  /* Scroll children within *lprcScroll. */
        public const int SW_INVALIDATE = 0x0002;  /* Invalidate after scrolling */
        public const int SW_ERASE = 0x0004;  /* If SW_INVALIDATE, don't send WM_ERASEBACKGROUND */
        public const int SW_SMOOTHSCROLL = 0x0010;  /* Use smooth scrolling */

        //Window Styles
        public const int WS_OVERLAPPED = 0x00000000;
        public const int WS_POPUP = -1; //0x80000000
        public const int WS_CHILD = 0x40000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_GROUP = 0x00020000;
        public const int WS_TABSTOP = 0x00010000;
        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int WS_TILED = WS_OVERLAPPED;
        public const int WS_ICONIC = WS_MINIMIZE;
        public const int WS_SIZEBOX = WS_THICKFRAME;
        public const int WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;
        public const int WS_CHILDWINDOW = WS_CHILD;
        public const int WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const int WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);

        //Extended Window Styles
        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_ACCEPTFILES = 0x00000010;
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_WINDOWEDGE = 0x00000100;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;
        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LTRREADING = 0x00000000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_RIGHTSCROLLBAR = 0x00000000;
        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const int WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);

        //Window Messages
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;
        public const int WM_GETDLGCODE = 0x0087;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_CAPTURECHANGED = 0x0215;

        //Static Styles
        public const int SS_LEFT = 0x00000000;
        public const int SS_CENTER = 0x00000001;
        public const int SS_RIGHT = 0x00000002;
        public const int SS_ICON = 0x00000003;
        public const int SS_BLACKRECT = 0x00000004;
        public const int SS_GRAYRECT = 0x00000005;
        public const int SS_WHITERECT = 0x00000006;
        public const int SS_BLACKFRAME = 0x00000007;
        public const int SS_GRAYFRAME = 0x00000008;
        public const int SS_WHITEFRAME = 0x00000009;
        public const int SS_USERITEM = 0x0000000A;
        public const int SS_SIMPLE = 0x0000000B;
        public const int SS_LEFTNOWORDWRAP = 0x0000000C;

        //Edit Control Message Constants
        public const int EM_LINEINDEX = 0x00BB;
        public const int EM_LINELENGTH = 0x00C1;
        public const int EM_GETSEL = 0x00B0;
        public const int EM_LINEFROMCHAR = 0x00C9;
        public const int EM_SETTABSTOPS = 0x00CB;
        public const int EM_LINESCROLL = 0x00B6;
        public const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        public const int EM_GETLINECOUNT = 0x00BA;

        //ListView Constants
        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETCOLUMNWIDTH = LVM_FIRST + 29;

        //SystemParametersInfo Constants
        public const int SPI_GETCURSORSHADOW = 0x101A;
        public const int SPI_SETCURSORSHADOW = 0x101B;

        //GetDeviceCaps Constants
        public const int BITSPIXEL = 12;
        public const int PLANES = 14;
        public const int PHYSICALOFFSETX = 112;
        public const int PHYSICALOFFSETY = 113;

        #endregion

        #region WinUser.h Types

        [StructLayout(LayoutKind.Sequential)]
        public struct ScrollInfo
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;

            public static ScrollInfo Create(uint uiMask)
            {
                ScrollInfo Info = new ScrollInfo();
                Info.cbSize = (uint)Marshal.SizeOf(typeof(ScrollInfo));
                Info.fMask = uiMask;
                return Info;
            }
        }

        #endregion

        #region RichEdit.h Types and Constants

        private const int WM_USER = 0x0400;
        private const int EM_GETTEXTEX = (WM_USER + 94);
        private const int EM_GETTEXTLENGTHEX = (WM_USER + 95);

        // Flags for the GETEXTEX data structure 
        private const int GT_DEFAULT = 0;
        private const int GT_USECRLF = 1;
        private const int GT_SELECTION = 2;
        private const int GT_RAWTEXT = 4;
        private const int GT_NOHIDDENTEXT = 8;

        //Flags for EM_GETTEXTLENGTHEX
        private const int GTL_DEFAULT = 0;	// Do default (return # of chars)		
        private const int GTL_USECRLF = 1;	// Compute answer using CRLFs for paragraphs
        private const int GTL_PRECISE = 2;	// Compute a precise answer					
        private const int GTL_CLOSE = 4;	// Fast computation of a "close" answer		
        private const int GTL_NUMCHARS = 8;	// Return number of characters			
        private const int GTL_NUMBYTES = 16;	// Return number of _bytes_				

        private const int CP_ANSI = 0;	//Ansi Code Page (same as CP_ACP)
        private const int CP_UNICODE = 1200;	//Unicode Code Page

        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct GETTEXTLENGTHEX
        {
            public uint uiFlags;
            public uint uiCodePage;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GETTEXTEX
        {
            public int iCb;
            public int iFlags;
            public int iCodepage;
            public IntPtr lpDefaultChar;
            public IntPtr lpUsedDefChar;
        }

        #endregion
    }
}
