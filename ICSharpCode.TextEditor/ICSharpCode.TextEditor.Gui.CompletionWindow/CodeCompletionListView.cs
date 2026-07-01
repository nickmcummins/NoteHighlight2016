using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow;

public class CodeCompletionListView : UserControl
{
	private ICompletionData[] completionData;

	private int firstItem = 0;

	private int selectedItem = -1;

	private ImageList imageList;

	public ImageList ImageList
	{
		get
		{
			return imageList;
		}
		set
		{
			imageList = value;
		}
	}

	public int FirstItem
	{
		get
		{
			return firstItem;
		}
		set
		{
			if (firstItem != value)
			{
				firstItem = value;
				OnFirstItemChanged(EventArgs.Empty);
			}
		}
	}

	public ICompletionData SelectedCompletionData
	{
		get
		{
			if (selectedItem < 0)
			{
				return null;
			}
			return completionData[selectedItem];
		}
	}

	public int ItemHeight => Math.Max(imageList.ImageSize.Height, checked((int)((double)Font.Height * 1.25)));

	public int MaxVisibleItem => base.Height / ItemHeight;

	public event EventHandler SelectedItemChanged;

	public event EventHandler FirstItemChanged;

	public CodeCompletionListView(ICompletionData[] completionData)
	{
		Array.Sort(completionData, DefaultCompletionData.Compare);
		this.completionData = completionData;
	}

	public void Close()
	{
		if (completionData != null)
		{
			Array.Clear(completionData, 0, completionData.Length);
		}
		Dispose();
	}

	public void SelectIndex(int index)
	{
		int num = selectedItem;
		int num2 = firstItem;
		index = Math.Max(0, index);
		checked
		{
			selectedItem = Math.Max(0, Math.Min(completionData.Length - 1, index));
			if (selectedItem < firstItem)
			{
				FirstItem = selectedItem;
			}
			if (firstItem + MaxVisibleItem <= selectedItem)
			{
				FirstItem = selectedItem - MaxVisibleItem + 1;
			}
			if (num != selectedItem)
			{
				if (firstItem != num2)
				{
					Invalidate();
				}
				else
				{
					int num3 = Math.Min(selectedItem, num) - firstItem;
					int num4 = Math.Max(selectedItem, num) - firstItem;
					Invalidate(new Rectangle(0, 1 + num3 * ItemHeight, base.Width, (num4 - num3 + 1) * ItemHeight));
				}
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
	}

	public void CenterViewOn(int index)
	{
		int num = FirstItem;
		checked
		{
			int num2 = index - unchecked(MaxVisibleItem / 2);
			if (num2 < 0)
			{
				FirstItem = 0;
			}
			else if (num2 >= completionData.Length - MaxVisibleItem)
			{
				FirstItem = completionData.Length - MaxVisibleItem;
			}
			else
			{
				FirstItem = num2;
			}
			if (FirstItem != num)
			{
				Invalidate();
			}
		}
	}

	public void ClearSelection()
	{
		checked
		{
			if (selectedItem >= 0)
			{
				int num = selectedItem - firstItem;
				selectedItem = -1;
				Invalidate(new Rectangle(0, num * ItemHeight, base.Width, (num + 1) * ItemHeight + 1));
				Update();
				OnSelectedItemChanged(EventArgs.Empty);
			}
		}
	}

	public void PageDown()
	{
		SelectIndex(checked(selectedItem + MaxVisibleItem));
	}

	public void PageUp()
	{
		SelectIndex(checked(selectedItem - MaxVisibleItem));
	}

	public void SelectNextItem()
	{
		SelectIndex(checked(selectedItem + 1));
	}

	public void SelectPrevItem()
	{
		SelectIndex(checked(selectedItem - 1));
	}

	public void SelectItemWithStart(string startText)
	{
		if (startText == null || startText.Length == 0)
		{
			return;
		}
		string text = startText;
		startText = startText.ToLower();
		int num = -1;
		int num2 = -1;
		double num3 = 0.0;
		checked
		{
			for (int i = 0; i < completionData.Length; i++)
			{
				string text2 = completionData[i].Text;
				string text3 = text2.ToLower();
				if (text3.StartsWith(startText))
				{
					double priority = completionData[i].Priority;
					int num4 = ((text3 == startText) ? ((!(text2 == text)) ? 2 : 3) : (text2.StartsWith(text) ? 1 : 0));
					if (num2 < num4 || (num != selectedItem && ((i != selectedItem) ? (num2 == num4 && num3 < priority) : (num2 == num4))))
					{
						num = i;
						num3 = priority;
						num2 = num4;
					}
				}
			}
			if (num < 0)
			{
				ClearSelection();
			}
			else if (num < firstItem || firstItem + MaxVisibleItem <= num)
			{
				SelectIndex(num);
				CenterViewOn(num);
			}
			else
			{
				SelectIndex(num);
			}
		}
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		float num = 1f;
		float num2 = ItemHeight;
		checked
		{
			int num3 = (int)(num2 * (float)imageList.ImageSize.Width / (float)imageList.ImageSize.Height);
			int i = firstItem;
			Graphics graphics = pe.Graphics;
			for (; i < completionData.Length; i++)
			{
				if (!(num < (float)base.Height))
				{
					break;
				}
				RectangleF rect = new RectangleF(1f, num, base.Width - 2, num2);
				if (rect.IntersectsWith(pe.ClipRectangle))
				{
					if (i == selectedItem)
					{
						graphics.FillRectangle(SystemBrushes.Highlight, rect);
					}
					else
					{
						graphics.FillRectangle(SystemBrushes.Window, rect);
					}
					int num4 = 0;
					if (imageList != null && completionData[i].ImageIndex < imageList.Images.Count)
					{
						graphics.DrawImage(imageList.Images[completionData[i].ImageIndex], new RectangleF(1f, num, num3, num2));
						num4 = num3;
					}
					if (i == selectedItem)
					{
						graphics.DrawString(completionData[i].Text, Font, SystemBrushes.HighlightText, num4, num);
					}
					else
					{
						graphics.DrawString(completionData[i].Text, Font, SystemBrushes.WindowText, num4, num);
					}
				}
				num += num2;
			}
			graphics.DrawRectangle(SystemPens.Control, new Rectangle(0, 0, base.Width - 1, base.Height - 1));
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		float num = 1f;
		int i = firstItem;
		float num2 = ItemHeight;
		checked
		{
			for (; i < completionData.Length; i++)
			{
				if (!(num < (float)base.Height))
				{
					break;
				}
				if (new RectangleF(1f, num, base.Width - 2, num2).Contains(e.X, e.Y))
				{
					SelectIndex(i);
					break;
				}
				num += num2;
			}
		}
	}

	protected override void OnPaintBackground(PaintEventArgs pe)
	{
	}

	protected virtual void OnSelectedItemChanged(EventArgs e)
	{
		if (this.SelectedItemChanged != null)
		{
			this.SelectedItemChanged(this, e);
		}
	}

	protected virtual void OnFirstItemChanged(EventArgs e)
	{
		if (this.FirstItemChanged != null)
		{
			this.FirstItemChanged(this, e);
		}
	}
}
