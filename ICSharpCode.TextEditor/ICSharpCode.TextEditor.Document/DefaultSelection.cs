#define DEBUG
using System.Diagnostics;

namespace ICSharpCode.TextEditor.Document;

public class DefaultSelection : ISelection
{
	private IDocument document;

	private bool isRectangularSelection;

	private TextLocation startPosition;

	private TextLocation endPosition;

	public TextLocation StartPosition
	{
		get
		{
			return startPosition;
		}
		set
		{
			DefaultDocument.ValidatePosition(document, value);
			startPosition = value;
		}
	}

	public TextLocation EndPosition
	{
		get
		{
			return endPosition;
		}
		set
		{
			DefaultDocument.ValidatePosition(document, value);
			endPosition = value;
		}
	}

	public int Offset => document.PositionToOffset(startPosition);

	public int EndOffset => document.PositionToOffset(endPosition);

	public int Length => checked(EndOffset - Offset);

	public bool IsEmpty => startPosition == endPosition;

	public bool IsRectangularSelection
	{
		get
		{
			return isRectangularSelection;
		}
		set
		{
			isRectangularSelection = value;
		}
	}

	public string SelectedText
	{
		get
		{
			if (document != null)
			{
				if (Length < 0)
				{
					return null;
				}
				return document.GetText(Offset, Length);
			}
			return null;
		}
	}

	public DefaultSelection(IDocument document, TextLocation startPosition, TextLocation endPosition)
	{
		DefaultDocument.ValidatePosition(document, startPosition);
		DefaultDocument.ValidatePosition(document, endPosition);
		Debug.Assert(startPosition <= endPosition);
		this.document = document;
		this.startPosition = startPosition;
		this.endPosition = endPosition;
	}

	public override string ToString()
	{
		return $"[DefaultSelection : StartPosition={startPosition}, EndPosition={endPosition}]";
	}

	public bool ContainsPosition(TextLocation position)
	{
		if (IsEmpty)
		{
			return false;
		}
		return (startPosition.Y < position.Y && position.Y < endPosition.Y) || (startPosition.Y == position.Y && startPosition.X <= position.X && (startPosition.Y != endPosition.Y || position.X <= endPosition.X)) || (endPosition.Y == position.Y && startPosition.Y != endPosition.Y && position.X <= endPosition.X);
	}

	public bool ContainsOffset(int offset)
	{
		return Offset <= offset && offset <= EndOffset;
	}
}
