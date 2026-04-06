using System;

namespace AScript
{
    public class EvalControl
    {
        private bool _Terminal;

        /// <summary>
        /// 上级
        /// </summary>
        public EvalControl Parent { get; private set; }
        /// <summary>
        /// 当前是否循环语句
        /// </summary>
        public bool IsLoop { get; private set; }
        /// <summary>
        /// 是否终止
        /// </summary>
        public bool Terminal
        {
            get => _Terminal;
            set
            {
                var c = this;
                do
                {
                    c._Terminal = value;
                    c = c.Parent;
                } while (c != null);
            }
        }
        /// <summary>
        /// 循环语句有效
        /// </summary>
        public bool Continue { get; set; }
        /// <summary>
        /// 循环语句有效
        /// </summary>
        public bool Break { get; set; }

        public EvalControl() { }
        public EvalControl(EvalControl parent, bool isLoop)
        {
            Parent = parent;
            IsLoop = isLoop;
        }
    }
}
