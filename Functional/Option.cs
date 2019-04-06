using System;
using System.Collections;
using System.Collections.Generic;

namespace Functional
{
    public static partial class F
    {
        public static Option<T> Some<T>(T value) => new Option.Some<T>(value);
        public static Option.None None => Option.None.Default;
    }

    public struct Option<T>
    {
        private readonly T Value;
        private bool IsSome { get; }
        private bool IsNone => !IsSome;
        
        public Option(T value)
        {
            if (value == null)
            {
                throw new WrapException(nameof(Option<T>));
            }

            Value = value;
            IsSome = true;
        }

        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome) yield return Value;
        }
        
        public static implicit operator Option<T>(Option.None _) => new Option<T>();
        public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);
        public static implicit operator Option<T>(T value) => value == null ? F.None : F.Some(value);

        public bool Equals(Option<T> other) =>
            IsSome && other.IsSome
                   && (IsNone || Value.Equals(other.Value));

        public bool Equals(Option.None _) => IsNone;

        public static bool operator ==(Option<T> me, Option<T> other) => me.Equals(other);
        public static bool operator !=(Option<T> me, Option<T> other) => !(me == other);

        public override string ToString() => IsSome ? $"Some({Value})" : "None";
    }
    
    public static class Option
    {
        public struct Some<T>
        {
            internal T Value { get; }

            internal Some(T value)
            {
                if (value == null)
                {
                    throw new WrapException(nameof(Some<T>));
                }

                Value = value;
            }
        }
        
        public struct None
        {
            internal static readonly None Default = new None();
        }
    }

    public class WrapException : ArgumentException
    {
        private const string WrapMsg = "Cannot wrap null";
        public WrapException() : base($"{WrapMsg}.") {}
        public WrapException(string wrapper) : base($"{WrapMsg} with {wrapper}.") {}
    }
}