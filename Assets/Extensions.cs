using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Extensions {
	public static void Shuffle<T>(this Stack<T> stack) {
		Random rnd = new Random();
		var values = stack.ToArray();
		stack.Clear();
		foreach (var value in values.OrderBy(x => rnd.Next()))
			stack.Push(value);
	}
}

