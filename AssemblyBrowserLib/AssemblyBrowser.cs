using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AssemblyBrowserLib
{
    public class AssemblyBrowser : IAssemblyBrowser
    {
        public ContainerInfo[] GetNamespaces(string assemblyPath)
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
            var result = type.Name;
            if (type.IsGenericType)
            {
                result += GetGenericArgumentsString(type.GetGenericArguments());
            }
            return result;
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
            var parameters = methodInfo.GetParameters();
            var declaration = returnType + " " + GetMethodName(methodInfo) + GetMethodParametersString(parameters);
            
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
            var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            foreach (var member in members)
            {
                var memberInfo = new MemberInfo();
                if (member.MemberType == MemberTypes.Method)
                {
                    var methodInfo = member as MethodInfo;
                    if (methodInfo != null)
                    {
                        memberInfo.Name = CreateMethodDeclarationString(methodInfo);
                    }
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    var property = ((PropertyInfo)member);
                    memberInfo.Name = GetTypeName(property.PropertyType) + " " + member.Name;
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    memberInfo.Name = GetTypeName(((FieldInfo)member).FieldType) + " " + member.Name;
                }
                else if (member.MemberType == MemberTypes.Event)
                {
                    memberInfo.Name = "event " + GetTypeName(((EventInfo)member).EventHandlerType) + " " + member.Name;
                }
                else if (member.MemberType == MemberTypes.Constructor)  
                {
                    var constructorInfo = member as ConstructorInfo;
                    memberInfo.Name = constructorInfo.Name + GetMethodParametersString(constructorInfo.GetParameters());
                }
                else
                {
                    var kek = (System.Reflection.TypeInfo)member;
                    memberInfo.Name = GetTypeName(kek) + " " + member.Name;
                }
                if (memberInfo.Name != null)
                {
                    typeInfo.AddMember(memberInfo);
                }
            }

            return typeInfo;
        }
    }
}
