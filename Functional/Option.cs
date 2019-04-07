using System;
using System.Collections;
using System.Collections.Generic;

namespace Functional
{
    using static F;
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

        public R Match<R>(Func<R> none, Func<T, R> some)
            => IsSome ? some(Value) : none();
        
        public static implicit operator Option<T>(Option.None _) => new Option<T>();
        public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);
        public static implicit operator Option<T>(T value) => value == null ? None : Some(value);

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

    public static class OptionExt
    {
        public static Option<R> Map<T, R>(this Option.Some<T> some, Func<T, R> func)
            => Some(func(some.Value));

        public static Option<R> Map<T, R>(this Option.None _, Func<T, R> func)
            => None;

        public static Option<R> Map<T, R>(this Option<T> option, Func<T, R> func)
            => option.Match(() => None, t => Some(func(t)));

        public static Option<R> Bind<T, R>(this Option<T> option, Func<T, Option<R>> func)
            => option.Match(() => None, func);
    }

    public class WrapException : ArgumentException
    {
        private const string WrapMsg = "Cannot wrap null";
        public WrapException() : base($"{WrapMsg}.") {}
        public WrapException(string wrapper) : base($"{WrapMsg} with {wrapper}.") {}
    }
}