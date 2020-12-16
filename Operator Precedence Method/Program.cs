using System;
using System.Collections.Generic;
using System.IO;

namespace Operator_Precedence_Method
{
    
    class Program
    {
        static readonly List<char> terminals = new List<char>     { '+', '*', '(', ')', 'i', '#', '-'};
        static readonly char[,] precedenceMatrix = new char[7,7]{
                                                                  { '>', '<', '<', '>', '<', '>', '<'},//+
                                                                  { '>', '>', '<', '>', '<', '>', '>'},//*
                                                                  { '<', '<', '<', '=', '<', '>', '<'},//(
                                                                  { '>', '>', ' ', '>', ' ', '>', '>'},//)
                                                                  { '>', '>', ' ', '>', ' ', '>', '>'},//i
                                                                  { '<', '<', '<', '<', '<', ' ', '<'},//#
                                                                  { '>', '<', '<', '>', '<', '>', '>'} //-
                                                                };
        static readonly string inputPath = "input.txt";
        static readonly string outputPath = "output.txt";
        static void Main()
        {
            int Ni = 1;
            string input= "";
            if (File.Exists(inputPath))
            {
                input = File.ReadAllText(inputPath);
                if (input[0] != '#') 
                {
                    input = "#" + input + "#";
                }
            }
            else 
            {
                Console.WriteLine("Входной файл не найден");
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            string onlyTerminals = input;
            //input = '#' + input + '#';
            Console.WriteLine("{0,-20}{1,-10}{2,-10}{3,-4}", "Входная цепочка", "Основа a", "A->a", "N(i)");
            File.WriteAllText(outputPath, string.Format("{0,-20}{1,-10}{2,-10}{3,-4}\n", "Входная цепочка", "Основа a", "A->a", "N(i)"));
            Console.WriteLine("{0,-20}", input);
            File.AppendAllText(outputPath, string.Format("{0,-20}\n", input));
            bool exit = false;
            while(!exit)
            {
                int index = Analysis(onlyTerminals);
                if (index == -1)
                {
                    exit = true;
                }
                for (int m = 0; m < input.Length; m++)
                {
                    if (terminals.Contains(input[m])) 
                    {
                        index--;
                        if (index == -1) 
                        {
                            input = Productions(input, m, Ni++);
                            Console.WriteLine(input);
                            File.AppendAllText(outputPath, input + "\n");
                        }
                    }
                }
                onlyTerminals = "";
                for (int m = 0; m < input.Length; m++)
                {
                    if (terminals.Contains(input[m]))
                    {
                        onlyTerminals += input[m];
                    }   
                }
                
            }
            Console.ReadKey();
        }

        static int Analysis(string input)
        { 
            string output = "";
            for (int strChar = 0; strChar < input.Length - 1; strChar++)
            {
                char ratio = GetLevel(input, strChar);
                if (ratio != ' ')
                {
                    output += input[strChar].ToString() + ratio;
                    if (ratio == '>')
                    {
                        output += input[strChar + 1];
                        int finishIndex = strChar;
                        Console.Write("{0,-20}", output);
                        File.AppendAllText(outputPath, string.Format("{0,-20}", output));
                        return finishIndex;
                    }
                }
                else 
                {
                    if (input!="##") 
                    {
                        Console.WriteLine("Полученная сентенциальная форма грамматики содержит\nдва соседних терминальных символа, для которых не\nсуществует отношение предшествования");
                        File.AppendAllText(outputPath, "\n Полученная сентенциальная форма грамматики содержит два соседних терминальных символа, для которых не существует отношение предшествования");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                   
                }
            }
            if (input != "##") 
            {
                Console.WriteLine("Невозможно выделить основу сентенциальной формы");
                File.AppendAllText(outputPath, "\n Невозможно выделить основу сентенциальной формы");
                Console.ReadKey();
                Environment.Exit(0);
            }            
            return -1;
        }

        static char GetLevel(string input, int index)
        { 
            int indexOfLeftTerminal = terminals.IndexOf(input[index]);
            int indexOfRightTerminal = terminals.IndexOf(input[index+1]);
            if (indexOfLeftTerminal != -1 && indexOfRightTerminal !=-1)
            {
                return precedenceMatrix[indexOfLeftTerminal, indexOfRightTerminal];
            }
            return ' ';
        }

        static string Productions(string input, int pos, int Ni) 
        {
            if (input[pos] == 'i') 
            {
                Console.WriteLine("{0,-10}{1,-10}{2,-4}", input.Substring(pos,1), "M->i", "N"+Ni);
                File.AppendAllText(outputPath, string.Format("{0,-10}{1,-10}{2,-4}\n", input.Substring(pos, 1), "M->i", "N" + Ni));
                return input.Remove(pos, 1).Insert(pos, "N" + Ni);
            }
            if (input[pos] == '+' || input[pos] == '*') 
            {
                int leftPos = 0;
                int rightPos = 0;
                for (int i = pos-1; i >= 0; i--)
                {
                    if (terminals.Contains(input[i])) 
                    {
                        leftPos = i + 1;
                        i = 0;
                    }
                }
                for (int i = pos+1; i <= input.Length; i++)
                {
                    if (terminals.Contains(input[i]))
                    {
                        rightPos = i - 1;
                        i = input.Length;
                    }
                }
                if (input[pos] == '+') 
                { 
                    Console.WriteLine("{0, -10}{1, -10}{2,-4}", input.Substring(leftPos, rightPos - leftPos + 1), "E->E+T", "N" + Ni);
                    File.AppendAllText(outputPath, string.Format("{0, -10}{1, -10}{2,-4}\n", input.Substring(leftPos, rightPos - leftPos + 1), "E->E+T", "N" + Ni));
                }
                if (input[pos] == '*')
                {
                    Console.WriteLine("{0, -10}{1, -10}{2,-4}", input.Substring(leftPos, rightPos - leftPos + 1), "T->T*M", "N" + Ni);
                    File.AppendAllText(outputPath, string.Format("{0, -10}{1, -10}{2,-4}\n", input.Substring(leftPos, rightPos - leftPos + 1), "T->T*M", "N" + Ni));
                }
                return input.Remove(leftPos, rightPos - leftPos + 1).Insert(leftPos, "N" + Ni);
            }
            if (input[pos] == ')') 
            {
                for (int i = pos; i >= 0; i--)
                {
                    if (input[i] == '(') 
                    {
                        Console.WriteLine("{0, -10}{1, -10}{2,-4}", input.Substring(i, pos - i + 1), "M->(E)", "N" + Ni);
                        File.AppendAllText(outputPath, string.Format("{0, -10}{1, -10}{2,-4}\n", input.Substring(i, pos - i + 1), "M->(E)", "N" + Ni));
                        return input.Remove(i, pos - i + 1).Insert(i, "N" + Ni);
                    }
                }
            }
            return "";
        }

    }
}
