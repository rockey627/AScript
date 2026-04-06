using System;
using System.Threading.Tasks;

namespace AScript
{
	/// <summary>
	/// 记号流
	/// </summary>
	public interface ITokenStream
	{
		/// <summary>
		/// 读取下一个token
		/// </summary>
		/// <returns></returns>
		Token? Next();
		/// <summary>
		/// 异步读取下一个token
		/// </summary>
		/// <returns></returns>
		Task<Token?> NextAsync();
	}
}
