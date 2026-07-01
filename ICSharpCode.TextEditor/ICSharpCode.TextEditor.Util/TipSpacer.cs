using System;
using System.Drawing;

namespace ICSharpCode.TextEditor.Util;

internal class TipSpacer : TipSection
{
	private SizeF spacerSize;

	public TipSpacer(Graphics graphics, SizeF size)
		: base(graphics)
	{
		spacerSize = size;
	}

	public override void Draw(PointF location)
	{
	}

	protected override void OnMaximumSizeChanged()
	{
		base.OnMaximumSizeChanged();
		SetRequiredSize(new SizeF(Math.Min(base.MaximumSize.Width, spacerSize.Width), Math.Min(base.MaximumSize.Height, spacerSize.Height)));
	}
}
