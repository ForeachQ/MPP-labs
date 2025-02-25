﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssemblyAnalyzer.Formatters
{
    public static class ConstructorFormatter
    {
        public static string Format(ConstructorInfo constrInfo)
        {
            return string.Join(" ", 
                GetTypeAccessorModifiers(constrInfo),
                constrInfo.Name, 
                GetConstructArguments(constrInfo));
        }

        private static string GetTypeAccessorModifiers(ConstructorInfo constrInfo)
        {
            if (constrInfo.IsPublic)
                return "public";
            if (constrInfo.IsPrivate)
                return "private";
            if (constrInfo.IsFamily)
                return "protected";
            if (constrInfo.IsAssembly)
                return "internal";
            if (constrInfo.IsFamilyOrAssembly)
                return "protected internal";

            return "";
        }
        private static string GetConstructArguments(ConstructorInfo constrInfo)
        {
            var stringBuilder = new StringBuilder("(");

            foreach (var parameter in constrInfo.GetParameters())
            {
                string parameterType;
                if (parameter.ParameterType.IsGenericType)
                {
                    parameterType = GetGenericType(parameter.ParameterType);
                }
                else parameterType = parameter.ParameterType.ToString();

                stringBuilder.Append(parameterType).Append(" ").Append(parameter.Name).Append(",");
            }

            if (stringBuilder.Length > 1)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

            stringBuilder.Append(")");

            return stringBuilder.ToString();

        }

        private static string GetGenericType(Type parameter)
        {

            var stringBuilder = new StringBuilder(Regex.Replace(parameter.Name, "`[0-9]+$", ""));

            stringBuilder.Append("<");
            if (parameter.IsGenericType)
            {
                stringBuilder.Append(GetGenericArgumentsType(parameter.GenericTypeArguments));
            }

            stringBuilder.Append(">");

            return stringBuilder.ToString();
        }


        private static string GetGenericArgumentsType(IEnumerable<Type> arguments)
        {
            var stringBuilder = new StringBuilder();

            foreach (var argument in arguments)
            {
                if (argument.IsGenericType)
                {
                    stringBuilder.Append(GetGenericType(argument));
                }
                else stringBuilder.Append(argument);

                stringBuilder.Append(", ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            return stringBuilder.ToString();

        }
    }
}
