using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor;

internal class Ime
{
	[StructLayout(LayoutKind.Sequential)]
	private class COMPOSITIONFORM
	{
		public int dwStyle = 0;

		public POINT ptCurrentPos = null;

		public RECT rcArea = null;
	}

	[StructLayout(LayoutKind.Sequential)]
	private class POINT
	{
		public int x = 0;

		public int y = 0;
	}

	[StructLayout(LayoutKind.Sequential)]
	private class RECT
	{
		public int left = 0;

		public int top = 0;

		public int right = 0;

		public int bottom = 0;
	}

	[StructLayout(LayoutKind.Sequential)]
	private class LOGFONT
	{
		public int lfHeight = 0;

		public int lfWidth = 0;

		public int lfEscapement = 0;

		public int lfOrientation = 0;

		public int lfWeight = 0;

		public byte lfItalic = 0;

		public byte lfUnderline = 0;

		public byte lfStrikeOut = 0;

		public byte lfCharSet = 0;

		public byte lfOutPrecision = 0;

		public byte lfClipPrecision = 0;

		public byte lfQuality = 0;

		public byte lfPitchAndFamily = 0;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string lfFaceName = null;
	}

	private const int WM_IME_CONTROL = 643;

	private const int IMC_SETCOMPOSITIONWINDOW = 12;

	private const int CFS_POINT = 2;

	private const int IMC_SETCOMPOSITIONFONT = 10;

	private Font font = null;

	private IntPtr hIMEWnd;

	private IntPtr hWnd;

	private LOGFONT lf = null;

	private static bool disableIME;

	public Font Font
	{
		get
		{
			return font;
		}
		set
		{
			if (!value.Equals(font))
			{
				font = value;
				lf = null;
				SetIMEWindowFont(value);
			}
		}
	}

	public IntPtr HWnd
	{
		set
		{
			if (hWnd != value)
			{
				hWnd = value;
				if (!disableIME)
				{
					hIMEWnd = ImmGetDefaultIMEWnd(value);
				}
				SetIMEWindowFont(font);
			}
		}
	}

	public Ime(IntPtr hWnd, Font font)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
		if (environmentVariable == "IA64" || environmentVariable == "AMD64" || Environment.OSVersion.Platform == PlatformID.Unix)
		{
			disableIME = true;
		}
		else
		{
			hIMEWnd = ImmGetDefaultIMEWnd(hWnd);
		}
		this.hWnd = hWnd;
		this.font = font;
		SetIMEWindowFont(font);
	}

	[DllImport("imm32.dll")]
	private static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, COMPOSITIONFORM lParam);

	[DllImport("user32.dll")]
	private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [In][MarshalAs(UnmanagedType.LPStruct)] LOGFONT lParam);

	private void SetIMEWindowFont(Font f)
	{
		if (disableIME || hIMEWnd == IntPtr.Zero)
		{
			return;
		}
		if (lf == null)
		{
			lf = new LOGFONT();
			f.ToLogFont(lf);
			lf.lfFaceName = f.Name;
		}
		try
		{
			SendMessage(hIMEWnd, 643, new IntPtr(10), lf);
		}
		catch (AccessViolationException ex)
		{
			Handle(ex);
		}
	}

	public void SetIMEWindowLocation(int x, int y)
	{
		if (disableIME || hIMEWnd == IntPtr.Zero)
		{
			return;
		}
		POINT pOINT = new POINT();
		pOINT.x = x;
		pOINT.y = y;
		COMPOSITIONFORM cOMPOSITIONFORM = new COMPOSITIONFORM();
		cOMPOSITIONFORM.dwStyle = 2;
		cOMPOSITIONFORM.ptCurrentPos = pOINT;
		cOMPOSITIONFORM.rcArea = new RECT();
		try
		{
			SendMessage(hIMEWnd, 643, new IntPtr(12), cOMPOSITIONFORM);
		}
		catch (AccessViolationException ex)
		{
			Handle(ex);
		}
	}

	private void Handle(Exception ex)
	{
		Console.WriteLine(ex);
		if (!disableIME)
		{
			disableIME = true;
			MessageBox.Show("Error calling IME: " + ex.Message + "\nIME is disabled.", "IME error");
		}
	}
}
