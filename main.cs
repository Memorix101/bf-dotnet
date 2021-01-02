using System;
using System.IO;

namespace bf_dotnet_core
{
    class Program
    {
        static byte[] register = new byte[8];
        static int[] loop_ptr = new int[8];
        static int reg_id = 0;
        static int loop_id = -1;

        static void add()
        {
          register[reg_id]++;
        }

        static void sub()
        {
          register[reg_id]--;
        }

        static void inc_cell()
        {
          if(reg_id+1 > register.Length-1)
          {
            Array.Resize<byte>(ref register, register.Length+1);
          }
          reg_id++;
        }

        static void dec_cell()
        {
          reg_id--;
        }

        static void print()
        {
            if (register[reg_id] == 0)
            {
              Console.Write($"{(char)32}");
            }
            else
            {
              Console.Write($"{(char)register[reg_id]}");
            }
            //debug_print_cells();
        }

        static void debug_print_cells()
        {
            Console.Write("\n");
            foreach (int r in register)
            {
              Console.Write($"[{r}]");
            }
            Console.Write("\n");
            string spacer = "";
            for(int i = 0; i < register.Length; i++)
            {
              if(reg_id == i)
              {
                for(int b = 0; b < reg_id; b++)
                {
                  spacer += $"[{register[b]}]";
                }

                for(int a = 0; a < spacer.ToString().Length; a++)
                {
                  Console.Write($"-");
                }
                Console.Write($"-^^");
                Console.Write($"({reg_id+1})");
              }
            }
           //Console.WriteLine("\n" + spacer);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("bf-dotnet-core v0.9 - Copyright (c) 2021 Memorix101\n");            
            string bfc_path = $"{AppDomain.CurrentDomain.BaseDirectory}main.bf";
            string[] cmd_args = new string[5];

            for (int i = 0; i < args.Length; i++)
            {
              cmd_args[i] = args[i];
            }

            if(cmd_args[0] != "-d" && cmd_args[0] != null)
            {
              bfc_path = cmd_args[0];
            }
            else if(cmd_args[0] == "-d" && cmd_args[1] != null)
            {
              bfc_path = cmd_args[1];
            }

            string code = File.ReadAllText(bfc_path);
            bool skip_forward = false;
            int skip_id = 0;
            for (int i = 0; i < code.Length; i++)
            {
              if(!skip_forward)
              {
                if (code[i] == '+')
                {
                  add();
                }
                else if (code[i] == '-')
                {
                  sub();
                }
                else if (code[i] == '>')
                {
                  inc_cell();
                }
                else if (code[i] == '<')
                {
                  dec_cell();
                }
                else if (code[i] == '.')
                {
                  print();
                }   
                else if (code[i] == '[')
                {
                  if (register[reg_id] == 0)
                  {
                    skip_forward = true;
                    skip_id++;
                  }
                  else
                  {
                    if(loop_id+1 > loop_ptr.Length-1)
                    {
                      Array.Resize<int>(ref loop_ptr, loop_ptr.Length+1);
                    }
                    loop_id++;
                    loop_ptr[loop_id] = i;
                  }
                }
                else if (code[i] == ']' && register[reg_id] != 0)
                {
                  i = loop_ptr[loop_id];
                }
                else if (code[i] == ']' && register[reg_id] == 0)
                {
                 loop_id--;
                }
              }
              else if (code[i] == '[' && skip_forward)
              {
                skip_id++;
              }
              else if (code[i] == ']' && skip_forward)
              {
                if(skip_id == 1)
                {
                  skip_forward = false;
                  skip_id = 0;
                }
                else
                {
                  skip_id--;
                }                
              }
            }

          if(cmd_args[0] == "-d")
          {
            debug_print_cells();
          }
        }
    }
}
