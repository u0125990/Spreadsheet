using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;

namespace TestingFormula {
    class Program {
        static void Main(string[] arg) { 
            Formula f = new Formula("((x+y+3*4)*(100-96)+((5+2)-1+z)) / 2");
            double d = ((4 + 6 + 3 * 4) * (100 - 96) + ((5 + 2) - 1 + 8)) / 2;

            Formula f1 = new Formula("x + y");
            f1.Evaluate(v => { throw new UndefinedVariableException(v); });

            


        }
        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double Lookup3(String v) {
            switch (v) {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
