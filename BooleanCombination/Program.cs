using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BooleanCombination
{
    class Program
    {
        /// <summary>
        /// Container for the wanted result.
        /// </summary>
        private static bool[] Rule_Bool = { false, false, false, false };

        /// <summary>
        /// Set A
        /// </summary>
        private static bool[] T_Bool1 = { true, false, true, false }; //not readonly to allow custom inputs
        /// <summary>
        /// Set B
        /// </summary>
        private static bool[] T_Bool2 = { false, true, true, false }; //
        /// <summary>
        /// Array of {Set A, Set B} for formula construction
        /// </summary>
        private static readonly bool[][] T_Bools = { T_Bool1, T_Bool2 };
        /// <summary>
        /// Bit operators. NAXO order!
        /// </summary>
        public static Operator[] Operators = { Operator.NOT, Operator.AND, Operator.XOR, Operator.OR };
        /// <summary>
        /// Reversed TestObject linkage tracker
        /// </summary>
        public static Stack<TestObject> operqueue = new Stack<TestObject>();
        public static bool allow_doubleNOT = false;
        static void Main(string[] args)
        {
        HEAD:
            Console.Clear();
            //Read rules
            GridInput(ref Rule_Bool);//TT TF FT FF
            /*-------------DO WORK---------------*/

            //check self input
            TestObject res = Solve(), tmp = res;
            //Result(ref res);
            /*-------------PRINT  ---------------*/
            while (tmp != null) //reverse the chained node
            {
                operqueue.Push(tmp);
                tmp = tmp.parent;
            }
            //First object and top to bottom
            Console.WriteLine(Recursive_CreatePrint(BoolSetConversion(ref operqueue.Peek().itemA), ref operqueue));


            Console.WriteLine("Redo? Yes: 1 >");
            string input = Console.ReadLine();
            if (Regex.IsMatch(input, "1")) goto HEAD;

            
        }
        private static void GridInput(ref bool[] roa)
        {
            string input = "";
        JUMP:
            Console.WriteLine("Input 4 consecutive 1 or 0 >");
            Console.WriteLine(Convert(ref T_Bool1) + " - A");
            Console.WriteLine(Convert(ref T_Bool2) + " - B");
            Console.WriteLine("---- Expected Output");
            input = Console.ReadLine();
            if (input.Equals("setop")) SetOperators(); //change allowed operators
            if (input.Equals("setab")) SetAB(); //change set A and set B
            if (!Regex.IsMatch(input, "(1|0){4}")) //avoid random input
            {
                Console.Clear();
                goto JUMP;
            }
            uint c = 0;
            foreach (char item in input.Substring(0,4).ToCharArray())
            {
                roa[c++] = (item.Equals('1') ? true : false);
            }
            Console.WriteLine();
        }

        private static void SetOperators()
        {
            Operator[] def = { Operator.NOT, Operator.AND, Operator.XOR, Operator.OR };

            //NAXO order
            List<Operator> allowed = new List<Operator>();
            string[] opers = { "NOT: ", "AND: ", "XOR: ", "OR: " };
            uint i = 0;
            while (i<opers.Length)
            {
            JUMP:
                Console.WriteLine("Input 1 or 0 >");
                Console.WriteLine(opers[i]);
                string input = Console.ReadLine();
                if (!Regex.IsMatch(input, "(1|0){1}")) //avoid random input
                {
                    Console.Clear();
                    goto JUMP;
                }
                if (input.Equals("1"))
                {
                    allowed.Add(def[i]);
                }
                i++;
            }
            Operators = allowed.ToArray();

            Console.WriteLine("Allow double NOT? 1 or 0 >");
            string inp = Console.ReadLine();
            if (inp.Equals("1")) allow_doubleNOT = true;
        }


        /// <summary>
        /// Manual set new numbers
        /// </summary>
        private static void SetAB()
        {
            JUMP:
            Console.WriteLine("Input Set A with 4 consecutive 1 or 0 >");
            string input = Console.ReadLine();
            if (!Regex.IsMatch(input, "(1|0){4}")) //avoid random input
            {
                Console.Clear();
                goto JUMP;
            }
            uint c = 0;
            foreach (char item in input.Substring(0, 4).ToCharArray())
            {
                T_Bool1[c++] = (item.Equals('1') ? true : false);
            }

            JUMP2:
            Console.WriteLine("Input Set B with 4 consecutive 1 or 0 >");
            input = Console.ReadLine();
            if (!Regex.IsMatch(input, "(1|0){4}")) //avoid random input
            {
                Console.Clear();
                goto JUMP2;
            }
            c = 0;
            foreach (char item in input.Substring(0, 4).ToCharArray())
            {
                T_Bool2[c++] = (item.Equals('1') ? true : false);
            }
        }

        //DOABLE COMBINATION!!!
        //itemA * itemB
        //itemA * result
        //itemB * result
        //result * result
        //where * -> Operator
        //example if (a ^ b & a) return; //we dont want it to run when (r true f false)

        /// <summary>
        /// Solves the problem
        /// </summary>
        /// <returns>Solution-holding TestObject</returns>
        private static TestObject Solve(int queuemax = 5)
        {
            TestObject to = new TestObject(T_Bool1,T_Bool2,Operator.NONE); //processing container

            //NAXO

            TestObject[] toa = new TestObject[Operators.Length * T_Bools.Length]; //n * 2
            uint i = 0;
            foreach (Operator op in Operators)
            {
                toa[i++] = new TestObject(T_Bool1, T_Bool2, op);
                toa[i++] = new TestObject(T_Bool2, T_Bool1, op);
            }


            //add only the 
            Queue<TestObject> queue = new Queue<TestObject>(toa);
            
            while (queue.Count > 0)
            {
                to = queue.Dequeue();
                to.Solve();
                
                if (Check(ref Rule_Bool, ref to.result)) break;

                
                for (int a = 0; a < T_Bools.Length; a++)
                {
                    for (int j = 0; j < Operators.Length; j++)
                    {
                        if (!allow_doubleNOT && ((to.oper & Operators[j]) == Operator.NOT)) continue; //skip same
                        toa[(a * Operators.Length) + j] = new TestObject(to, T_Bools[a], Operators[j]);   
                        
                        //MUST create a new TestObject when chaining or else the parent is always re-linked back to itself
                        queue.Enqueue(toa[(a * Operators.Length) + j]);

                        if (toa[(a * Operators.Length) + j].Level == queuemax)
                        {
                            break; //safety lock to avoid infinite loop
                        }
                    }
                }
            }

            

            return to;
        }
        /// <summary>
        /// Convert bool[] to binary string
        /// </summary>
        /// <param name="arr">ref bool[]</param>
        /// <returns>converted string</returns>
        private static string Convert(ref bool[] arr)
        {
            string str = "";
            for (int i = 0; i < 4; i++)
            {
                str += (arr[i]) ? "1" : "0";
            }
            return str;
        }
        /// <summary>
        /// Creates the proper formula string with the given final TestObject
        /// </summary>
        /// <param name="res">Starting bool[]</param>
        /// <param name="sto">ref proper TestObject path</param>
        /// <returns>Formula</returns>
        private static string Recursive_CreatePrint(string res, ref Stack<TestObject> sto)
        {
            if (sto.Count == 0) return res;
            string tmp = "";
            TestObject to = sto.Pop();
            
            switch (to.oper)
            {
                case Operator.NOT:
                    tmp = string.Format("(NOT {0})", res);
                    break;
                case Operator.AND:
                case Operator.XOR:
                case Operator.OR:
                    tmp = string.Format("({0} {1} {2})", res, to.oper, BoolSetConversion(ref to.itemB));
                    break;
                case Operator.NONE:
                    tmp = string.Format("( {0} )", BoolSetConversion(ref to.result));
                    break;
            }

            return Recursive_CreatePrint(tmp, ref sto);
        }
        /// <summary>
        /// Converts ref bool[] to string. Returns "A" if bool[] == setA and "B" if bool[] == setB. Else return string-converted
        /// </summary>
        /// <param name="b">ref bool[] set</param>
        /// <returns>String representation of the bool[]</returns>
        private static string BoolSetConversion(ref bool[] b)
        {
            if (Check(ref b,ref T_Bool1)) return "A";
            if (Check(ref b,ref T_Bool2)) return "B";
            return Convert(ref b);
        }
        /// <summary>
        /// Checks two bool[] are same
        /// </summary>
        /// <param name="a">bool[]</param>
        /// <param name="b">bool[]</param>
        /// <returns>Equal or not</returns>
        private static bool Check(ref bool[] a,ref bool[] b)
        {
            for (int i = 0; i < 4; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Node for creating the formula
        /// </summary>
        public class TestObject
        {
            public bool[] itemA, itemB, result;
            public uint Level { get; private set; }

            public TestObject parent;
            public bool isNull = false;
            
            public Operator oper;
            
            public TestObject(bool[] a, bool[] b, Operator op)
            {
                itemA = a;
                itemB = b;
                oper = op;
                Level = 0;
                result = new bool[4];
            }
            public TestObject(TestObject par, bool[] b, Operator op)
            {
                parent = par;
                itemA = parent.result;
                itemB = b;
                oper = op;
                Level = parent.Level + 1;
                result = new bool[4];
            }
            public void Solve()
            {
                for (int i = 0; i < 4; i++)
                {
                    switch (oper)
                    {
                        case Operator.AND:
                            result[i] = itemA[i] & itemB[i];
                            break;
                        case Operator.OR:
                            result[i] = itemA[i] | itemB[i];
                            break;
                        case Operator.XOR:
                            result[i] = itemA[i] ^ itemB[i];
                            break;
                        case Operator.NOT:
                            result[i] = !itemA[i];
                            break;
                        case Operator.NONE:
                            result[i] = itemA[i];
                            break;
                    }
                }
            }
            public override string ToString()
            {
                return (this.oper == Operator.NONE) ? string.Format("{0} {1}", Convert(ref this.itemA), this.oper) :
                    (this.oper == Operator.NOT) ? string.Format("{0} {1}", this.oper, Convert(ref this.itemA)) :
                    string.Format("{0} {1} {2}", Convert(ref this.itemA), this.oper, Convert(ref this.itemB));
            }
            
        }
        /// <summary>
        /// Enum Operators. Includes a NONE enum for [want: self] stupid cases
        /// </summary>
        [Flags]
        public enum Operator
        {//NAXO order
            NOT = 1,
            AND = 1 << 1,
            XOR = 1 << 2,
            OR  = 1 << 3,
            NONE= 1 << 4
        }
        
    }
}