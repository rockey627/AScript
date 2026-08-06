using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace AScript.Lang.Lua.io
{
    /// <summary>
    /// Lua io 模块实现
    /// </summary>
    public class LuaIO
    {
        private LuaFile _input;
        private LuaFile _output;

        public LuaIO()
        {
            // 初始化标准输入输出
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="mode">模式：r, w, a, r+, w+, a+</param>
        /// <returns>文件对象，失败返回nil</returns>
        public LuaFile open(string filename, string mode = "r")
        {
            try
            {
                FileMode fileMode;
                FileAccess access;
                FileShare share = FileShare.Read;

                switch (mode)
                {
                    case "r":
                        fileMode = FileMode.Open;
                        access = FileAccess.Read;
                        break;
                    case "w":
                        fileMode = FileMode.Create;
                        access = FileAccess.Write;
                        break;
                    case "a":
                        fileMode = FileMode.Append;
                        access = FileAccess.Write;
                        break;
                    case "r+":
                        fileMode = FileMode.Open;
                        access = FileAccess.ReadWrite;
                        break;
                    case "w+":
                        fileMode = FileMode.Create;
                        access = FileAccess.ReadWrite;
                        break;
                    case "a+":
                        fileMode = FileMode.Append;
                        access = FileAccess.ReadWrite;
                        break;
                    default:
                        return null;
                }

                var stream = new FileStream(filename, fileMode, access, share);
                return new LuaFile(stream);
            }
            catch
            {
                return null;
            }
        }

        public void close()
        {
			if (_output != null)
			{
				_output.close();
				_output = null;
			}
		}

        /// <summary>
        /// 关闭文件
        /// </summary>
        /// <param name="file">文件对象，如果为nil则关闭默认输出</param>
        public void close(LuaFile file)
        {
            if (file != null)
            {
                file.close();
            }
            else if (_output != null)
            {
                _output.close();
                _output = null;
            }
        }

        /// <summary>
        /// 刷新默认输出缓冲区
        /// </summary>
        public void flush()
        {
            _output?.flush();
        }

        /// <summary>
        /// 设置默认输入文件
        /// </summary>
        public LuaFile input(LuaFile input)
        {
            _input = input;
            return input;
        }

        /// <summary>
        /// 设置默认输出文件
        /// </summary>
        public LuaFile output(LuaFile output)
        {
            _output = output;
            return output;
        }

        /// <summary>
        /// 从默认输入读取
        /// </summary>
        public object read(params object[] formats)
        {
            if (_input == null)
            {
                // 从标准输入读取
                return ConsoleRead(formats);
            }
            return _input.read(formats);
        }

        /// <summary>
        /// 写入默认输出
        /// </summary>
        public void write(params object[] values)
        {
            if (_output == null)
            {
                // 写入标准输出
                foreach (var value in values)
                {
                    if (value != null)
                    {
                        Console.Write(value.ToString());
                    }
                }
            }
            else
            {
                _output.write(values);
            }
        }

        /// <summary>
        /// 从标准输入读取一行
        /// </summary>
        public string readline()
        {
            return Console.ReadLine();
        }

        /// <summary>
        /// 检查对象类型
        /// </summary>
        public string type(object obj)
        {
            if (obj == null)
                return null;

            if (obj is LuaFile file)
            {
                return file.Closed ? "closed file" : "file";
            }

            return null;
        }

        /// <summary>
        /// 返回临时文件
        /// </summary>
        public LuaFile tmpfile()
        {
            try
            {
                var stream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                return new LuaFile(stream);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 返回文件行迭代器
        /// </summary>
        public IEnumerable<string> lines(string filename = null)
        {
            LuaFile file;
            if (filename == null)
            {
                if (_input == null)
                {
                    // 使用标准输入
                    foreach (var line in ConsoleLines())
                    {
                        yield return line;
                    }
                    yield break;
                }
                file = _input;
            }
            else
            {
                file = open(filename, "r");
                if (file == null)
                    yield break;
            }

            foreach (var line in file.lines())
            {
                yield return line;
            }

            if (filename != null)
            {
                file.close();
            }
        }

        private IEnumerable<string> ConsoleLines()
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private object ConsoleRead(object[] formats)
        {
            if (formats == null || formats.Length == 0)
            {
                return Console.ReadLine();
            }

            var results = new List<object>();
            foreach (var format in formats)
            {
                results.Add(ConsoleReadOne(format));
            }
            return formats.Length == 1 ? results[0] : results;
        }

        private object ConsoleReadOne(object format)
        {
            if (format == null)
            {
                return Console.ReadLine();
            }

            if (format is double n && n >= 0)
            {
                // 读取n个字符
                char[] buffer = new char[(int)n];
                int count = 0;
                while (count < n)
                {
                    int ch = Console.Read();
                    if (ch == -1) break;
                    buffer[count++] = (char)ch;
                }
                return new string(buffer, 0, count);
            }

            string fmt = format as string;
            if (fmt != null)
            {
                switch (fmt)
                {
                    case "*a":
                    case "a":
                        return Console.In.ReadToEnd();
                    case "*l":
                    case "l":
                        return Console.ReadLine();
                    case "*L":
                    case "L":
                        // 读取一行包括换行符
                        int chL = Console.Read();
                        if (chL == -1) return null;
                        StringBuilder sb = new StringBuilder();
                        sb.Append((char)chL);
                        while (true)
                        {
                            chL = Console.Read();
                            if (chL == -1) break;
                            char c = (char)chL;
                            sb.Append(c);
                            if (c == '\n') break;
                        }
                        return sb.ToString();
                    default:
                        // 尝试解析为数字
                        if (double.TryParse(fmt, out double lineNum) && lineNum >= 0)
                        {
                            char[] buffer = new char[(int)lineNum];
                            int count = 0;
                            while (count < lineNum)
                            {
                                int chR = Console.Read();
                                if (chR == -1) break;
                                buffer[count++] = (char)chR;
                            }
                            return new string(buffer, 0, count);
                        }
                        break;
                }
            }

            throw new ArgumentException($"invalid format: {format}");
        }
    }
}
