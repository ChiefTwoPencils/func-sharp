using System;
using System.Collections.Generic;
using Unit = System.ValueTuple;

namespace Functional
{
    using static F;
    
    public static partial class F
    {
        public static Either.Left<L> Left<L>(L value) => new Either.Left<L>(value);
        public static Either.Right<R> Right<R>(R value) => new Either.Right<R>(value);
    }

    public struct Either<L, R>
    {
        internal L Left { get; }
        internal R Right { get; }
        internal bool IsRight { get; }
        internal bool IsLeft => !IsRight;
    
        internal Either(L left)
        {
            Left = left;
            Right = default(R);
            IsRight = false;
        }

        internal Either(R right)
        {
            Right = right;
            Left = default(L);
            IsRight = true;
        }

        public RR Match<RR>(Func<L, RR> left, Func<R, RR> right)
            => IsRight ? right(Right) : left(Left);

        public Unit Match(Action<L> left, Action<R> right)
            => Match(left.ToFunc(), right.ToFunc());

        public IEnumerable<R> AsEnumerable()
        {
            if (IsRight) yield return Right;
        }
        
        public static implicit operator Either<L, R>(L left) => new Either<L, R>(left);
        public static implicit operator Either<L, R>(R right) => new Either<L, R>(right);
        public static implicit operator Either<L, R>(Either.Left<L> left) => new Either<L, R>(left.Value);
        public static implicit operator Either<L, R>(Either.Right<R> right) => new Either<L, R>(right.Value);
    }
    
    public static class Either
    {
        public struct Left<L>
        {
            internal L Value { get; }

            internal Left(L value)
            {
                Value = value;
            }

            public override string ToString() => $"Left({Value})";
        }

        public struct Right<R>
        {
            internal R Value { get; }

            internal Right(R value)
            {
                Value = value;
            }

            public Right<RR> Map<L, RR>(Func<R, RR> func) => new Right<RR>(func(Value));
            public Either<L, RR> Bind<L, RR>(Func<R, Either<L, RR>> func) => func(Value);
            public override string ToString() => $"Right({Value})";
        }
    }

    public static class EitherExt
    {
        public static Either<L, RR> Map<L, R, RR>(this Either<L, R> either, Func<R, RR> func)
            => either.Match<Either<L, RR>>(Left, r => Right(func(r)));

        public static Either<L, RR> Bind<L, R, RR>(this Either<L, R> either, Func<R, RR> func)
            => either.Match<Either<L, RR>>(Left, r => func(r));

        public static Either<L, Unit> ForEach<L, R>(this Either<L, R> either, Action<R> action)
            => Map(either, action.ToFunc());

        public static Either<L, RR> Select<L, R, RR>(this Either<L, R> either, Func<R, RR> func)
            => either.Map(func);

        public static Either<L, P> SelectMany<L, R, RR, P>(this Either<L, R> either, Func<R, Either<L, RR>> bind,
            Func<R, RR, P> project) => either.Match(
            Left,
            r => bind(either.Right).Match<Either<L, P>>(
                    Left,
                    rr => project(r, rr)));
    }
}