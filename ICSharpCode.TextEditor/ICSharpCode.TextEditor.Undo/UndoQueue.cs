#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.TextEditor.Undo;

internal sealed class UndoQueue : IUndoableOperation
{
	private List<IUndoableOperation> undolist = new List<IUndoableOperation>();

	public UndoQueue(Stack<IUndoableOperation> stack, int numops)
	{
		if (stack == null)
		{
			throw new ArgumentNullException("stack");
		}
		Debug.Assert(numops > 0, "ICSharpCode.TextEditor.Undo.UndoQueue : numops should be > 0");
		if (numops > stack.Count)
		{
			numops = stack.Count;
		}
		for (int i = 0; i < numops; i = checked(i + 1))
		{
			undolist.Add(stack.Pop());
		}
	}

	public void Undo()
	{
		for (int i = 0; i < undolist.Count; i = checked(i + 1))
		{
			undolist[i].Undo();
		}
	}

	public void Redo()
	{
		checked
		{
			for (int num = undolist.Count - 1; num >= 0; num--)
			{
				undolist[num].Redo();
			}
		}
	}
}
