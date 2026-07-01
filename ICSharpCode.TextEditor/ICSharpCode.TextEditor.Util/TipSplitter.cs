#define DEBUG
using System;
using System.Diagnostics;
using System.Drawing;

namespace ICSharpCode.TextEditor.Util;

internal class TipSplitter : TipSection
{
	private bool isHorizontal;

	private float[] offsets;

	private TipSection[] tipSections;

	public TipSplitter(Graphics graphics, bool horizontal, params TipSection[] sections)
		: base(graphics)
	{
		Debug.Assert(sections != null);
		isHorizontal = horizontal;
		offsets = new float[sections.Length];
		tipSections = (TipSection[])sections.Clone();
	}

	public override void Draw(PointF location)
	{
		checked
		{
			if (isHorizontal)
			{
				for (int i = 0; i < tipSections.Length; i++)
				{
					tipSections[i].Draw(new PointF(location.X + offsets[i], location.Y));
				}
			}
			else
			{
				for (int i = 0; i < tipSections.Length; i++)
				{
					tipSections[i].Draw(new PointF(location.X, location.Y + offsets[i]));
				}
			}
		}
	}

	protected override void OnMaximumSizeChanged()
	{
		base.OnMaximumSizeChanged();
		float num = 0f;
		float num2 = 0f;
		SizeF maximumSize = base.MaximumSize;
		for (int i = 0; i < tipSections.Length; i = checked(i + 1))
		{
			TipSection tipSection = tipSections[i];
			tipSection.SetMaximumSize(maximumSize);
			SizeF requiredSize = tipSection.GetRequiredSize();
			offsets[i] = num;
			if (isHorizontal)
			{
				float num3 = (float)Math.Ceiling(requiredSize.Width);
				num += num3;
				maximumSize.Width = Math.Max(0f, maximumSize.Width - num3);
				num2 = Math.Max(num2, requiredSize.Height);
			}
			else
			{
				float num3 = (float)Math.Ceiling(requiredSize.Height);
				num += num3;
				maximumSize.Height = Math.Max(0f, maximumSize.Height - num3);
				num2 = Math.Max(num2, requiredSize.Width);
			}
		}
		TipSection[] array = tipSections;
		foreach (TipSection tipSection in array)
		{
			if (isHorizontal)
			{
				tipSection.SetAllocatedSize(new SizeF(tipSection.GetRequiredSize().Width, num2));
			}
			else
			{
				tipSection.SetAllocatedSize(new SizeF(num2, tipSection.GetRequiredSize().Height));
			}
		}
		if (isHorizontal)
		{
			SetRequiredSize(new SizeF(num, num2));
		}
		else
		{
			SetRequiredSize(new SizeF(num2, num));
		}
	}
}
