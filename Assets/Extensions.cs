using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions {
	public static void Shuffle<T>(this Stack<T> stack) {
		Random rnd = new Random();
		var values = stack.ToArray();
		stack.Clear();
		foreach (var value in values.OrderBy(x => rnd.Next())) {
			stack.Push(value);
		}
	}

	public static Stack<T> Invert<T>(this Stack<T> stack) {
		Stack<T> returningStack = new Stack<T>(); 
		while(stack.Count > 0) {
			returningStack.Push(stack.Pop());
		}
		return returningStack;
	}
}

