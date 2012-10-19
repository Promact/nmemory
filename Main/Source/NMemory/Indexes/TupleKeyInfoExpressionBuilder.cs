﻿// ----------------------------------------------------------------------------------
// <copyright file="TupleKeyInfoExpressionBuilder.cs" company="NMemory Team">
//     Copyright (C) 2012 by NMemory Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// ----------------------------------------------------------------------------------

namespace NMemory.Indexes
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using NMemory.Common;

    internal class TupleKeyInfoExpressionBuilder : IKeyInfoExpressionBuilder
    {
        private Type tupleType;

        public TupleKeyInfoExpressionBuilder(Type tupleType)
        {
            if (!ReflectionHelper.IsTuple(tupleType))
            {
                throw new ArgumentException("", "tupleType");
            }

            this.tupleType = tupleType;
        }

        public Expression CreateKeyFactoryExpression(params Expression[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            var genericArguments = this.tupleType.GetGenericArguments();

            if (arguments.Length != genericArguments.Length)
            {
                throw new ArgumentException("", "arguments");
            }

            for (int i = 0; i < genericArguments.Length; i++)
            {
                if (genericArguments[i] != arguments[i].Type)
                {
                    throw new ArgumentException("", "arguments");
                }
            }

            return Expression.New(this.tupleType.GetConstructors()[0], arguments);
        }

        public Expression CreateKeyMemberSelectorExpression(Expression source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (source.Type != this.tupleType)
            {
                throw new ArgumentException("", "source");
            }

            string propertyName = string.Format("Item{0}", index + 1);
            PropertyInfo property = this.tupleType.GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException("", "index");
            }

            return Expression.Property(source, property);
        }
    }
}
