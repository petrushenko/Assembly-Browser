using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AssemblyBrowserLib
{
    public class AssemblyBrowser : IAssemblyBrowser
    {
        private List<MethodInfo> _extensionMethods = new List<MethodInfo>();

        private void AddExtensionMethods(ContainerInfo[] containers)
        {
            foreach (var method in _extensionMethods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length < 0) continue;
                var param = parameters[0];
                var extentedType = param.ParameterType;
                var namespce = GetNamespaceByName(containers, extentedType.Namespace);
                if (namespce == null) continue;
                var types = namespce.Members;
                foreach (var type in types)
                {
                    if (type.Name == GetTypeDeclaration(extentedType.GetTypeInfo()))
                    {
                        ((TypeInfo)type).AddMember(new MemberInfo() { Name = "exteinsion method " + CreateMethodDeclarationString(method) });
                    }
                }
            }
        }

        private ContainerInfo GetNamespaceByName(ContainerInfo[] containers, string name)
        {
            foreach (var namespce in containers)
            {
                if (namespce.Name == name)
                {
                    return namespce;
                }
            }
            return null;
        }

        public ContainerInfo[] GetNamespaces(string assemblyPath)
        {
            var assembly = Assembly.LoadFile(assemblyPath);
            var types = assembly.GetTypes();
            var namespaces = new Dictionary<string, NamespaceInfo>();
            foreach (var type in types)
            {
                var namespce = type.Namespace;
                if (namespce == null) continue;
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
            var result = namespaces.Values.ToArray();
            AddExtensionMethods(result);
            return result;
        }

        private string GetTypeName(Type type)
        {
            var result = $"{type.Namespace}.{type.Name}";
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
            return method.Name;
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
            var declaration =
                $"{GetMethodDeclaration(methodInfo)} {returnType} {GetMethodName(methodInfo)} {GetMethodParametersString(parameters)}";

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

            result.Append($"{GetTypeName(typeInfo.AsType())} ");

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
            result.Append($"{GetTypeName(eventInfo.EventHandlerType)} {eventInfo.Name}");
            result.Append($" [{eventInfo.AddMethod.Name}] ");
            result.Append($" [{eventInfo.RemoveMethod.Name}] ");

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

        private string GetConstructorDeclaration(ConstructorInfo constructorInfo)
        {
            return
                $"{GetMethodDeclaration(constructorInfo)} {GetMethodName(constructorInfo)} {GetMethodParametersString(constructorInfo.GetParameters())}";
        }

        private TypeInfo GetTypeInfo(Type type)
        {
            var typeInfo = new TypeInfo() { Name = GetTypeDeclaration(type.GetTypeInfo()) };
            var members = type.GetMembers(BindingFlags.NonPublic
                                          | BindingFlags.Instance
                                          | BindingFlags.Public
                                          | BindingFlags.Static);
            foreach (var member in members)
            {
                var memberInfo = new MemberInfo();
                if (member.MemberType == MemberTypes.Method)
                {
                    var method = (MethodInfo)member;
                    if (method.IsDefined(typeof(ExtensionAttribute), false))
                    {
                        _extensionMethods.Add(method);
                    }
                    memberInfo.Name = CreateMethodDeclarationString(method);
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
                    memberInfo.Name = GetConstructorDeclaration((ConstructorInfo)member);
                }
                else
                {
                    memberInfo.Name = GetTypeDeclaration((System.Reflection.TypeInfo)member);
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
