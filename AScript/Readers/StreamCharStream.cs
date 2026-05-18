using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AScript.Readers
{
	public class StreamCharStream : ICharStream, IDisposable
	{
		private readonly bool _autoDisposeStream;
		private readonly char[] buffer = new char[32];
		private int _currentIndex = 0;
		private int _currentLength = 0;

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
			if (_currentLength > 0 && _currentIndex < _currentLength)
			{
				return buffer[_currentIndex++];
			}
			if (_reader == null && _stream != null)
			{
				_reader = new StreamReader(_stream, Encoding.UTF8);
			}
			if (_reader == null || _reader.EndOfStream)
			{
				return null;
			}
			_currentIndex = 0;
			_currentLength = _reader.Read(buffer, 0, buffer.Length);
			if (_reader.EndOfStream)
			{
				DisposeReader();
			}
			if (_currentLength <= 0)
			{
				return null;
			}
			return buffer[_currentIndex++];
		}

		public async Task<char?> NextAsync(CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (_currentLength > 0 && _currentIndex < _currentLength)
			{
				return buffer[_currentIndex++];
			}
			if (_reader == null && _stream != null)
			{
				_reader = new StreamReader(_stream, Encoding.UTF8);
			}
			if (_reader == null || _reader.EndOfStream)
			{
				return null;
			}
			_currentIndex = 0;
			_currentLength = await _reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
			if (_reader.EndOfStream)
			{
				DisposeReader();
			}
			if (_currentLength <= 0)
			{
				return null;
			}
			return buffer[_currentIndex++];
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
