using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AScript.Readers
{
	public class StreamCharStream : ICharStream, IDisposable
	{
		private readonly bool _autoDisposeStream;
		private readonly char[] buffer = new char[1];

		private Stream _stream;
		private StreamReader _reader;

		public StreamCharStream(Stream stream, bool autoDisposeStream)
		{
			_stream = stream;
			_autoDisposeStream = autoDisposeStream;
		}

		~StreamCharStream()
		{
			DisposeReader();
		}

		public char? Next()
		{
			if (_reader == null && _stream != null)
			{
				_reader = new StreamReader(_stream, Encoding.UTF8);
			}
			if (_reader == null || _reader.EndOfStream)
			{
				return null;
			}
			int d = _reader.Read();
			if (_reader.EndOfStream)
			{
				DisposeReader();
			}
			if (d == -1)
			{
				return null;
			}
			return (char)d;
		}

		public async Task<char?> NextAsync()
		{
			if (_reader == null && _stream != null)
			{
				_reader = new StreamReader(_stream, Encoding.UTF8);
			}
			if (_reader == null || _reader.EndOfStream)
			{
				return null;
			}
			int d = await _reader.ReadAsync(buffer, 0, 1).ConfigureAwait(false);
			if (_reader.EndOfStream)
			{
				DisposeReader();
			}
			if (d <= 0)
			{
				return null;
			}
			return buffer[0];
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			DisposeReader();
		}

		private void DisposeReader()
		{
			if (_reader != null)
			{
				try { _reader.Dispose(); } catch { }
				_reader = null;
			}
			if (_autoDisposeStream && _stream != null)
			{
				try { _stream.Dispose(); } catch { }
				_stream = null;
			}
		}
	}
}
