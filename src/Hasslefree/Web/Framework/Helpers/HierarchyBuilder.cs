using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Web.Framework.Helpers
{
	public static class HierarchyBuilder
	{
		public static IList<Node<T>> GroupToTree<T>(this IEnumerable<IEnumerable<T>> source)
		{
			return GroupToTree(source.Select(sequence => sequence.GetEnumerator()));
		}

		private static IList<Node<T>> GroupToTree<T>(IEnumerable<IEnumerator<T>> source)
		{
			return source.WhereHasNext()
				.GroupBy(iterator => iterator.Current)
				.Select(group => new Node<T>(group.Key, GroupToTree(group)))
				.ToList();
		}

		//This ensures that the iterators all get disposed
		private static IEnumerable<IEnumerator<T>> WhereHasNext<T>(this IEnumerable<IEnumerator<T>> source)
		{
			foreach (var iterator in source)
			{
				if (iterator.MoveNext())
					yield return iterator;
				else
					iterator.Dispose();
			}
		}
	}
	
	public class Node<T>
	{
		public Node(T value, IEnumerable<Node<T>> children)
		{
			Value = value;
			Children = children;
		}
		public T Value { get; }
		public IEnumerable<Node<T>> Children { get; }
	}
}