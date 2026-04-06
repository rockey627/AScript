using System;
using System.Threading.Tasks;

namespace AScript
{
	/// <summary>
	/// 字符流
	/// </summary>
	public interface ICharStream
	{
		/// <summary>
		/// 读取下一个字符
		/// </summary>
		/// <returns></returns>
		char? Next();
		/// <summary>
		/// 异步读取下一个字符
		/// </summary>
		/// <returns></returns>
		Task<char?> NextAsync();
	}
}
