using Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

struct RecursiveResponse
{
    public int value;
    public bool isResultOfOperation;
}

namespace Compiler.MyMsil
{
    public class MsilGenerator
    {
        public void Generate(TreeNode treeNode)
        {
            RevertTree(treeNode);
            StreamWriter code = new StreamWriter("code.il");
            code.WriteLine(".assembly PrintString {}");
            code.WriteLine(".method static void main()");
            code.WriteLine("{");
            code.WriteLine(".entrypoint");
            code.WriteLine(".maxstack 5");
            code.WriteLine(".locals init (int32 first,int32 second)");
            var a = GetResult(treeNode, code);
            code.WriteLine("call void [mscorlib]System.Console::Write(int32)");
            code.WriteLine("ret");
            code.WriteLine("}");
            code.Close();
        }

        private RecursiveResponse GetResult(TreeNode treeNode, StreamWriter writer)
        {
            RecursiveResponse response = new RecursiveResponse();

            if (treeNode.TermType != Helper.enums.TermType.Int)
            {
                if (treeNode.ChildNodes.Count == 1 && treeNode.TermType == Helper.enums.TermType.Minis)
                {
                    if (treeNode.ChildNodes[0].TermType == Helper.enums.TermType.Int)
                    {
                        int value = -1;
                        Int32.TryParse(treeNode.ChildNodes[0].Value, out value);
                        response.value = 0 - value;
                        response.isResultOfOperation = false;
                        return response;
                    }
                    else
                    {
                        var res = GetResult(treeNode.ChildNodes[0], writer);
                        writer.WriteLine("stloc first");
                        writer.WriteLine("ldc.i4 0");
                        writer.WriteLine("ldloc first");
                        writer.WriteLine("sub");
                        response.isResultOfOperation = true;
                        return response;
                    }
                }

                var firstResult = GetResult(treeNode.ChildNodes[0], writer);
                var secondResult = GetResult(treeNode.ChildNodes[1], writer);
               

                if (firstResult.isResultOfOperation && secondResult.isResultOfOperation)
                {
                    WriteCommand(writer, treeNode.TermType);
                    response.isResultOfOperation = true;
                    return response;
                }

                if (!firstResult.isResultOfOperation && !secondResult.isResultOfOperation)
                {
                    WriteInt(writer, firstResult.value);
                    WriteInt(writer, secondResult.value);
                    WriteCommand(writer, treeNode.TermType);
                    response.isResultOfOperation = true;
                    return response;
                }

                if (firstResult.isResultOfOperation && !secondResult.isResultOfOperation)
                {
                    WriteInt(writer, secondResult.value);
                    WriteCommand(writer, treeNode.TermType);
                    response.isResultOfOperation = true;
                    return response;
                }

                if (!firstResult.isResultOfOperation && secondResult.isResultOfOperation)
                {
                    writer.WriteLine("stloc second");
                    WriteInt(writer, firstResult.value);
                    writer.WriteLine("ldloc second");
                    WriteCommand(writer, treeNode.TermType);
                    response.isResultOfOperation = true;
                    return response;
                }

                response.isResultOfOperation = true;
                return response;
            }
            else
            {
                int value = -1;
                Int32.TryParse(treeNode.Value, out value);
                response.value = value;
                response.isResultOfOperation = false;

                return response;
            }
        }
        
        private void RevertTree(TreeNode treeNode)
        {
            if (treeNode.ChildNodes != null)
            {
                if (treeNode.ChildNodes.Count == 2)
                {
                    var temp = treeNode.ChildNodes[0];
                    treeNode.ChildNodes[0] = treeNode.ChildNodes[1];
                    treeNode.ChildNodes[1] = temp;
                    RevertTree(treeNode.ChildNodes[0]);
                    RevertTree(treeNode.ChildNodes[1]);
                }
            }
        }

        private void WriteInt(StreamWriter writer, int number)
        {
            writer.WriteLine("ldc.i4 " + number);
        }

        private void WriteCommand(StreamWriter writer, Helper.enums.TermType term)
        {
            if (term == Helper.enums.TermType.Plus)
            {
                writer.WriteLine("add");
            }

            if (term == Helper.enums.TermType.Division)
            {
                writer.WriteLine("div");
            }

            if (term == Helper.enums.TermType.Minis)
            {
                writer.WriteLine("sub");
            }

            if (term == Helper.enums.TermType.Multiple)
            {
                writer.WriteLine("mul");
            }
        }
    }
}
