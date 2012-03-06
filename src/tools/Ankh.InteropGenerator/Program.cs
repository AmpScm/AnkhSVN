using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Ankh.InteropGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: Ankh.InteropGenerator <assembly> <type>");
                Environment.Exit(1);
            }

            List<string> items = new List<string>(args);
            string assembly = args[0];
            items.RemoveAt(0);

            Assembly asm = Assembly.LoadFrom(assembly);
            CodeCompileUnit unit = new CodeCompileUnit();

            foreach (string typeName in items)
            {
                Type type = asm.GetType(typeName, true);

                CodeNamespace ns = new CodeNamespace(type.Namespace);
                unit.Namespaces.Add(ns);

                CodeTypeDeclaration typeDecl = new CodeTypeDeclaration(type.Name);
                typeDecl.IsInterface = true;

                CopyAttrs(CustomAttributeData.GetCustomAttributes(type), typeDecl.CustomAttributes);


                foreach (MemberInfo mi in type.GetMembers())
                {
                    CodeTypeMember member = null;

                    if (mi is MethodInfo)
                    {
                        MethodInfo mb = (MethodInfo)mi;
                        CodeMemberMethod codeMember = new CodeMemberMethod();

                        if (mb.IsSpecialName)
                            continue;

                        if (mb.ReturnType != typeof(void))
                            codeMember.ReturnType = new CodeTypeReference(mb.ReturnType);

                        CopyReturnAttrs(mb.ReturnTypeCustomAttributes, codeMember.CustomAttributes);

                        foreach (ParameterInfo pi in mb.GetParameters())
                        {
                            Type paramType = pi.ParameterType;
                            bool byRef = false;

                            if (paramType.IsByRef)
                            {
                                byRef = true;
                                paramType = paramType.GetElementType();
                            }

                            CodeParameterDeclarationExpression pde = new CodeParameterDeclarationExpression(paramType, pi.Name);

                            if (byRef)
                                pde.Direction = FieldDirection.Ref;
                            else if (pi.IsIn)
                                pde.Direction = FieldDirection.In;
                            else if (pi.IsOut)
                                pde.Direction = FieldDirection.Out;

                            CopyAttrs(CustomAttributeData.GetCustomAttributes(pi), pde.CustomAttributes);
                            codeMember.Parameters.Add(pde);
                        }

                        member = codeMember;
                    }
                    else if (mi is PropertyInfo)
                    {
                        PropertyInfo pi = (PropertyInfo)mi;
                        CodeMemberProperty codeMember = new CodeMemberProperty();

                        codeMember.Type = new CodeTypeReference(pi.PropertyType);

                        if (pi.CanRead)
                        {
                            codeMember.HasGet = true;
                            CopyReturnAttrs(pi.GetGetMethod().ReturnTypeCustomAttributes, codeMember.CustomAttributes);
                        }
                        if (pi.CanWrite)
                            codeMember.HasSet = true;

                        

                        member = codeMember;
                    }
                    else
                    {
                        Console.Error.WriteLine("Can't dump {0}", mi);
                        Environment.Exit(1);
                    }

                    member.Name = mi.Name;
                    CopyAttrs(CustomAttributeData.GetCustomAttributes(mi), member.CustomAttributes);

                    typeDecl.Members.Add(member);
                }

                ns.Types.Add(typeDecl);

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");


                provider.GenerateCodeFromCompileUnit(unit, Console.Out, new CodeGeneratorOptions());

                //EnvDTE80.SourceControl2
                //Console.WriteLine(unit.());
            }
        }

        private static void CopyReturnAttrs(ICustomAttributeProvider customAttributeProvider, CodeAttributeDeclarationCollection newAttrs)
        {
            if (customAttributeProvider != null)
                foreach (MarshalAsAttribute mas in customAttributeProvider.GetCustomAttributes(typeof(MarshalAsAttribute), false))
                {
                    CodeAttributeDeclaration cad = new CodeAttributeDeclaration(new CodeTypeReference(typeof(MarshalAsAttribute)));
                    cad.Name = "return: " + cad.Name; // HACK!?

                    cad.Arguments.Add(new CodeAttributeArgument(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(mas.Value.GetType().FullName), mas.Value.ToString())));

                    newAttrs.Add(cad);
                }
        }

        private static void CopyAttrs(IList<CustomAttributeData> list, CodeAttributeDeclarationCollection newAttrs)
        {
            foreach (CustomAttributeData attrData in list)
            {
                CodeAttributeDeclaration attrDec = new CodeAttributeDeclaration(new CodeTypeReference(attrData.Constructor.DeclaringType));
                foreach (CustomAttributeTypedArgument arg in attrData.ConstructorArguments)
                {
                    CodeExpression expr = null;
                    if (arg.ArgumentType.IsEnum)
                        expr = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(arg.ArgumentType.FullName), Enum.ToObject(arg.ArgumentType, arg.Value).ToString());
                    else if (arg.ArgumentType == typeof(string) || arg.ArgumentType == typeof(int) || arg.ArgumentType == typeof(short))
                        expr = new CodePrimitiveExpression(arg.Value);
                    else
                        expr = new CodeDefaultValueExpression(new CodeTypeReference(arg.ArgumentType));

                    if (expr != null)
                        attrDec.Arguments.Add(new CodeAttributeArgument(expr));
                }
                foreach (CustomAttributeNamedArgument arg in attrData.NamedArguments)
                {
                    CodeExpression expr = null;

                    if (arg.MemberInfo is FieldInfo && Equals(arg.TypedValue.Value, 0) || Equals(arg.TypedValue.Value, (short)0))
                        continue;

                    if (arg.TypedValue.ArgumentType.IsEnum)
                        expr = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(arg.TypedValue.ArgumentType.FullName), Enum.ToObject(arg.TypedValue.ArgumentType, arg.TypedValue.Value).ToString());
                    else if (arg.TypedValue.ArgumentType == typeof(string) || arg.TypedValue.ArgumentType == typeof(int) || arg.TypedValue.ArgumentType == typeof(short))
                        expr = new CodePrimitiveExpression(arg.TypedValue.Value);
                    else
                        expr = new CodeDefaultValueExpression(new CodeTypeReference(arg.TypedValue.ArgumentType));

                    if (expr != null)
                        attrDec.Arguments.Add(new CodeAttributeArgument(arg.MemberInfo.Name, expr));
                }
                //attr.
                newAttrs.Add(attrDec);
            }
        }
    }
}
