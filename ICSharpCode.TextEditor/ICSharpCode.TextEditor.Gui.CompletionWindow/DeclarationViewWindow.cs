using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow;

public class DeclarationViewWindow : Form, IDeclarationViewWindow
{
	private string description = string.Empty;

	public bool HideOnClick;

	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			description = value;
			if (value == null && base.Visible)
			{
				base.Visible = false;
			}
			else if (value != null)
			{
				if (!base.Visible)
				{
					ShowDeclarationViewWindow();
				}
				Refresh();
			}
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams result = base.CreateParams;
			AbstractCompletionWindow.AddShadowToWindow(result);
			return result;
		}
	}

	protected override bool ShowWithoutActivation => true;

	public DeclarationViewWindow(Form parent)
	{
		SetStyle(ControlStyles.Selectable, value: false);
		base.StartPosition = FormStartPosition.Manual;
		base.FormBorderStyle = FormBorderStyle.None;
		base.Owner = parent;
		base.ShowInTaskbar = false;
		base.Size = new Size(0, 0);
		CreateHandle();
	}

	protected override void OnClick(EventArgs e)
	{
		base.OnClick(e);
		if (HideOnClick)
		{
			Hide();
		}
	}

	public void ShowDeclarationViewWindow()
	{
		Show();
	}

	public void CloseDeclarationViewWindow()
	{
		Close();
		Dispose();
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		if (description != null && description.Length > 0)
		{
			TipPainterTools.DrawHelpTipFromCombinedDescription(this, pe.Graphics, Font, null, description);
		}
	}

	protected override void OnPaintBackground(PaintEventArgs pe)
	{
		pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
	}
}
