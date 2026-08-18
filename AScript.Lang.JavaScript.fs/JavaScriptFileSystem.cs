using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Lang.JavaScript.fs
{
	public class JavaScriptFileSystem
	{
		public string readFileSync(string path, string encodeName)
		{
			return File.ReadAllText(path, Encoding.GetEncoding(encodeName));
		}

		public async Task<string> readFile(string path, string encodeName)
		{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
			return await File.ReadAllTextAsync(path, Encoding.GetEncoding(encodeName)).ConfigureAwait(false);
#else
			using (var stream = File.OpenRead(path))
			{
				using (var reader = new StreamReader(stream, Encoding.GetEncoding(encodeName)))
				{
					return await reader.ReadToEndAsync().ConfigureAwait(false);
				}
			}
#endif
		}

		public void readFile(string path, string encodeName, Action<object, string> callback)
		{
			readFile(path, encodeName).ContinueWith(t =>
			{
				object err = t.Exception?.InnerException.Message;
				string data = t.Exception == null ? t.Result : null;
				callback?.Invoke(err, data);
			});
		}

		public void writeFileSync(string path, string data, string encodeName)
		{
			File.WriteAllText(path, data, Encoding.GetEncoding(encodeName));
		}

		public async Task writeFile(string path, string data, string encodeName)
		{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
			await File.WriteAllTextAsync(path, data, Encoding.GetEncoding(encodeName)).ConfigureAwait(false);
#else
			using (var writer = new StreamWriter(path, append: false, Encoding.GetEncoding(encodeName)))
			{
				await writer.WriteAsync(data).ConfigureAwait(false);
			}
#endif
		}

		public void writeFile(string path, string data, string encodeName, Action<object> callback)
		{
			writeFile(path, data, encodeName).ContinueWith(t =>
			{
				object err = t.Exception?.InnerException.Message;
				callback?.Invoke(err);
			});
		}

		public void appendFileSync(string path, string data, string encodeName)
		{
			File.AppendAllText(path, data, Encoding.GetEncoding(encodeName));
		}

		public async Task appendFile(string path, string data, string encodeName)
		{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
			await File.AppendAllTextAsync(path, data, Encoding.GetEncoding(encodeName)).ConfigureAwait(false);
#else
			using (var writer = new StreamWriter(path, append: true, Encoding.GetEncoding(encodeName)))
			{
				await writer.WriteAsync(data).ConfigureAwait(false);
			}
#endif
		}

		public void appendFile(string path, string data, string encodeName, Action<object> callback)
		{
			appendFile(path, data, encodeName).ContinueWith(t =>
			{
				object err = t.Exception?.InnerException.Message;
				callback?.Invoke(err);
			});
		}

		public void copyFileSync(string sourcePath, string targetPath)
		{
			File.Copy(sourcePath, targetPath, true);
		}

		public async Task copyFile(string sourcePath, string targetPath)
		{
			var buffer = new byte[1024];
			using (var sourceStream = File.OpenRead(sourcePath))
			{
				using (var targetStream = File.Open(targetPath, FileMode.Create, FileAccess.Write))
				{
					while (true)
					{
						var count = await sourceStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
						if (count == 0) break;
						await targetStream.WriteAsync(buffer, 0, count).ConfigureAwait(false);
					}
				}
			}
		}

		public void copyFile(string sourcePath, string targetPath, Action<object> callback)
		{
			copyFile(sourcePath, targetPath).ContinueWith(t =>
			{
				object err = t.Exception?.InnerException.Message;
				callback?.Invoke(err);
			});
		}

		public void unlinkSync(string path)
		{
			File.Delete(path);
		}

		public Task unlink(string path)
		{
			return Task.Run(() => File.Delete(path));
		}

		public void unlink(string path, Action<object> callback)
		{
			unlink(path).ContinueWith(t =>
			{
				object err = t.Exception?.InnerException.Message;
				callback?.Invoke(err);
			});
		}

		public JavaScriptReadStream createReadStream(string path)
		{
			return new JavaScriptReadStream(path);
		}

		public JavaScriptReadStream createReadStream(string path, IDictionary<string, object> options)
		{
			return new JavaScriptReadStream(path, options);
		}

		public JavaScriptWriteStream createWriteStream(string path)
		{
			return new JavaScriptWriteStream(path);
		}

		public JavaScriptWriteStream createWriteStream(string path, IDictionary<string, object> options)
		{
			return new JavaScriptWriteStream(path, options);
		}
	}
}
