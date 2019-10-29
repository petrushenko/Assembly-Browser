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
            var declaration = string.Format("{0} {1} {2} {3}",
                                            GetMethodDeclaration(methodInfo),
                                            returnType,
                                            GetMethodName(methodInfo),
                                            GetMethodParametersString(parameters));

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

        private string GetTypeDeclaration(System.Reflection.TypeInfo typeInfo)
        {
            var result = new StringBuilder();

            if (typeInfo.IsNestedPublic || typeInfo.IsPublic)
                result.Append("public ");
            else if (typeInfo.IsNestedPrivate)
                result.Append("private ");
            else if (typeInfo.IsNestedFamily)
                result.Append("protected ");
            else if (typeInfo.IsNestedAssembly)
                result.Append("internal ");
            else if (typeInfo.IsNestedFamORAssem)
                result.Append("protected internal ");
            else if (typeInfo.IsNestedFamANDAssem)
                result.Append("private protected ");
            else if (typeInfo.IsNotPublic)
                result.Append("private ");

            
            if (typeInfo.IsAbstract && typeInfo.IsSealed)
                result.Append("static ");
            else if (typeInfo.IsAbstract)
                result.Append("abstract ");
            else if (typeInfo.IsSealed)
                result.Append("sealed ");

            if (typeInfo.IsClass)
                result.Append("class ");
            else if (typeInfo.IsEnum)
                result.Append("enum ");
            else if (typeInfo.IsInterface)
                result.Append("interface ");
            else if (typeInfo.IsGenericType)
                result.Append("generic ");
            else if (typeInfo.IsValueType && !typeInfo.IsPrimitive)
                result.Append("struct ");

            result.Append(string.Format("{0} ", GetTypeName(typeInfo.AsType())));

            return result.ToString();
        }

        private string GetMethodDeclaration(MethodBase methodBase)
        {
            var result = new StringBuilder();

            if (methodBase.IsAssembly)
                result.Append("internal ");
            else if (methodBase.IsFamily)
                result.Append("protected ");
            else if (methodBase.IsFamilyOrAssembly)
                result.Append("protected internal ");
            else if (methodBase.IsFamilyAndAssembly)
                result.Append("private protected ");
            else if (methodBase.IsPrivate)
                result.Append("private ");
            else if (methodBase.IsPublic)
                result.Append("public ");

            if (methodBase.IsStatic)
                result.Append("static ");
            else if (methodBase.IsAbstract)
                result.Append("abstract ");
            else if (methodBase.IsVirtual)
                result.Append("virtual ");

            return result.ToString();
        }

        private string GetPropertyDeclaration(PropertyInfo propertyInfo)
        {            
            var result = new StringBuilder(GetTypeName(propertyInfo.PropertyType));
            result.Append(" ");
            result.Append(propertyInfo.Name);

            var accessors = propertyInfo.GetAccessors(true);
            foreach (var accessor in accessors)
            {
                if (accessor.IsSpecialName)
                {
                    result.Append(" { ");
                    result.Append(accessor.Name);
                    result.Append(" } ");
                }
            }

            return result.ToString();
        }

        private string GetEventDeclaration(EventInfo eventInfo)
        {
            var result = new StringBuilder();
            result.Append(string.Format("{0} {1}", GetTypeName(eventInfo.EventHandlerType), eventInfo.Name));
            result.Append(string.Format(" [{0}] ", eventInfo.AddMethod.Name));
            result.Append(string.Format(" [{0}] ", eventInfo.RemoveMethod.Name));
            return result.ToString();
        }

        private string GetFieldDeclaration(FieldInfo fieldInfo)
        {
            var result = new StringBuilder();
            if (fieldInfo.IsAssembly)
                result.Append("internal ");
            else if (fieldInfo.IsFamily)
                result.Append("protected ");
            else if (fieldInfo.IsFamilyOrAssembly)
                result.Append("protected internal ");
            else if (fieldInfo.IsFamilyAndAssembly)
                result.Append("private protected ");
            else if (fieldInfo.IsPrivate)
                result.Append("private ");
            else if (fieldInfo.IsPublic)
                result.Append("public ");

            if (fieldInfo.IsInitOnly)
                result.Append("readonly ");
            if (fieldInfo.IsStatic)
                result.Append("static ");

            result.Append(GetTypeName(fieldInfo.FieldType));
            result.Append(" ");
            result.Append(fieldInfo.Name);

            return result.ToString();
        }

        private TypeInfo GetTypeInfo(Type type)
        {
            var typeInfo = new TypeInfo() { Name = GetTypeDeclaration(type.GetTypeInfo()) };
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
                    memberInfo.Name = GetPropertyDeclaration((PropertyInfo)member);
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    memberInfo.Name = GetFieldDeclaration(((FieldInfo)member));
                }
                else if (member.MemberType == MemberTypes.Event)
                {
                    memberInfo.Name = GetEventDeclaration((EventInfo)member);
                }
                else if (member.MemberType == MemberTypes.Constructor)
                {
                    var constructorInfo = member as ConstructorInfo;
                    memberInfo.Name = constructorInfo.Name + GetMethodParametersString(constructorInfo.GetParameters());
                }
                else
                {
                    var memberTypeInfo = (System.Reflection.TypeInfo)member;
                    memberInfo.Name = GetTypeDeclaration(memberTypeInfo.GetTypeInfo());
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
