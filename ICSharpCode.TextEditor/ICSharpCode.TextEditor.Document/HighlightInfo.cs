namespace ICSharpCode.TextEditor.Document;

public class HighlightInfo
{
	public bool BlockSpanOn = false;

	public bool Span = false;

	public Span CurSpan = null;

	public HighlightInfo(Span curSpan, bool span, bool blockSpanOn)
	{
		CurSpan = curSpan;
		Span = span;
		BlockSpanOn = blockSpanOn;
	}
}
