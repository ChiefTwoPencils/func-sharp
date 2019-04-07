using System;
using Unit = System.ValueTuple;

namespace Functional
{
    public static partial class F
    {
        public static Unit Unit() => default(Unit);
        
        /// <summary>
        /// General representation of an error. Meant for
        /// base of domain specific errors.
        /// </summary>
        public class Error
        {
            /// <summary>
            /// Description of the error.
            /// </summary>
            public virtual string Message { get; }
        }
    }
}