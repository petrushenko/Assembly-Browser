using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AssemblyBrowserLib
{
    public class AssemblyBrowser : IAssemblyBrowser
    {
        public NamespaceInfo[] GetNamespaces(string assemblyPath)
        {
            var assembly = Assembly.LoadFile(assemblyPath);
            var types = assembly.GetTypes();
            var namespaces = new Dictionary<string, NamespaceInfo>();
            foreach (var type in types)
            {
                var namespce = type.Namespace;
                NamespaceInfo namespaceInfo = null;
                if (!namespaces.ContainsKey(namespce))
                {
                    namespaceInfo = new NamespaceInfo { Name = namespce };
                    namespaces.Add(namespce, namespaceInfo);
                }
                else
                {
                    namespaces.TryGetValue(namespce, out namespaceInfo);
                }
                TypeInfo typeInfo = GetTypeInfo(type);
                namespaceInfo.AddMember(typeInfo);
            }
            return namespaces.Values.ToArray();
        }

        private string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                return type.Name + GetGenericArgumentsString(type.GetGenericArguments());
            }
            else
            {
                return type.Name;
            }
        }
        private string GetMethodName(MethodBase method)
        {
            if (method.IsGenericMethod)
            {
                return method.Name + GetGenericArgumentsString(method.GetGenericArguments());
            }
            else
            {
                return method.Name;
            }
        }

        private string GetGenericArgumentsString(Type[] arguments)
        {
            var genericArgumentsString = new StringBuilder("<");
            for (int i = 0; i < arguments.Length; i++)
            {
                genericArgumentsString.Append(GetTypeName(arguments[i]));
                if (i != arguments.Length - 1)
                {
                    genericArgumentsString.Append(", ");
                }
            }
            genericArgumentsString.Append(">");

            return genericArgumentsString.ToString();
        }

        private string CreateMethodDeclarationString(MethodInfo methodInfo)
        {
            var returnType = GetTypeName(methodInfo.ReturnType);
            var name = methodInfo.Name;
            var parameters = methodInfo.GetParameters();
            var declaration = returnType + " " + name + GetMethodParametersString(parameters);
            
            return declaration;
        }

        private string GetMethodParametersString(ParameterInfo[] parameters)
        {
            var parametersString = new StringBuilder("(");
            for (int i = 0; i < parameters.Length; i++)
            {
                parametersString.Append(GetTypeName(parameters[i].ParameterType));
                if (i != parameters.Length - 1)
                {
                    parametersString.Append(", ");
                }
            }
            parametersString.Append(")");

            return parametersString.ToString();
        }

        private TypeInfo GetTypeInfo(Type type)
        {
            var typeInfo = new TypeInfo() { Name = GetTypeName(type) };
            var members = type.GetMembers();
            foreach (var member in members)
            {
                var memberInfo = new MemberInfo();
                if (member.MemberType == MemberTypes.Method)
                {
                    var methodInfo = member as MethodInfo;
                    if (methodInfo != null)
                    {
                        memberInfo.Name = GetMethodName(methodInfo);
                        memberInfo.DeclarationInfo = CreateMethodDeclarationString(methodInfo);
                    }
                }
                else if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                {
                    memberInfo.Name = member.Name; 
                    memberInfo.DeclarationInfo = GetTypeName(member.DeclaringType);
                }
                else if (member.MemberType == MemberTypes.Constructor)  
                {
                    var constructorInfo = member as ConstructorInfo;
                    memberInfo.Name = GetMethodName(constructorInfo);
                    memberInfo.DeclarationInfo = constructorInfo.Name + GetMethodParametersString(constructorInfo.GetParameters());
                }
                typeInfo.AddMember(memberInfo);
            }

            return typeInfo;
        }
    }
}
