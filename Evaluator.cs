using System;
using System.Collections.Generic;
using static dpl.ConsMethods;

namespace dpl {
  public static class Evaluator {
    public static Lexeme Eval(Lexeme tree, Lexeme env) {
      if (tree == null) {
        return null;
      }

      var f = GetEvalFunction(tree, env);
      if (f == null) {
        throw new Exception("No evaluation function for type " + tree.type);
      } else if (f.type == "INTEGER" || f.type == "STRING" || f.type == "ARRAY" || f.type == "arg") {
        return f;
      } else if (f.type == "statement") {
        return GetEvalFunction(f.Left, env);
      } else if (f.type == "ID") {
        return Environment.Lookup(f.sval, env);
      } else {
        return f;
      }
    }

    private static Lexeme GetEvalFunction(Lexeme tree, Lexeme env) {
      switch (tree.type) {
        //lists
        case "statement":
          var val = GetEvalFunction(tree.Left, env);
          if (tree.Right != null) {
            return GetEvalFunction(tree.Right, env);
          } else {
            return val;
          }
        case "arg":
          tree.Left = GetEvalFunction(tree.Left, env);
          if (tree.Right != null) {
            tree.Right = GetEvalFunction(tree.Right, env);
          }
          return tree;
        //self-evaluating
        case "INTEGER":
          return tree;
        case "STRING":
          return tree;
        case "ARRAY":
          return tree;
        case "ENV":
          return tree;
        //find the value of variables in the environment
        case "ID":
          if (tree.Right == null) { //variable reference
            if (tree.Left != null && tree.Left.type == "ID") { //object reference
              var field = tree.Left.sval;

              var obj = Environment.Lookup(tree.sval, env);
              return Environment.Lookup(field, obj);
            }
            return Environment.Lookup(tree.sval, env);
          }
          if (tree.Right.type == "paramList") { //functionDef
            return EvalFuncDef(tree, env);
          }
          else if (tree.Right.type == "argList") { //functionCall
            return EvalFuncCall(tree, env);
          }
          else if (tree.Right.type == "ASSIGN") {
            return EvalVarDef(tree, env);
          } else if (tree.Right.type == "INTEGER") { //array index
            return EvalArrayGetIndex(tree, env);
          } else {
            throw new Exception("Unexpected error in GetEvalFunction -> ID -> " + tree.Right.type);
          }
        case "LAMBDA_DEF":
          return EvalLambdaDef(tree, env);
        //case paranthesized expression?
        //operators (both sides evaluated) or are self inflicting ex. += or ++
        case "PLUS":
          return EvalSimpleOp(tree, env);
        case "PLUS_TO":
          return EvalSimpleOp(tree, env);
        case "INCREMENT":
          return EvalSimpleOp(tree, env);
        case "MINUS":
          return EvalSimpleOp(tree, env);
        case "MINUS_TO":
          return EvalSimpleOp(tree, env);
        case "DECREMENT":
          return EvalSimpleOp(tree, env);
        case "TIMES":
          return EvalSimpleOp(tree, env);
        case "TIMES_TO":
          return EvalSimpleOp(tree, env);
        case "DIVIDES":
          return EvalSimpleOp(tree, env);
        case "DIVIDES_TO":
          return EvalSimpleOp(tree, env);
        case "MOD":
          return EvalSimpleOp(tree, env);
        case "MOD_TO":
          return EvalSimpleOp(tree, env);
        case "EXPN":
          return EvalSimpleOp(tree, env);
        case "EXPN_TO":
          return EvalSimpleOp(tree, env);
        case "LESSTHAN":
          return EvalSimpleOp(tree, env);
        case "LESSTHAN_EQUALTO":
          return EvalSimpleOp(tree, env);
        case "GREATERTHAN":
          return EvalSimpleOp(tree, env);
        case "GREATERTHAN_EQUALTO":
          return EvalSimpleOp(tree, env);
        case "EQUAL":
          return EvalSimpleOp(tree, env);
        case "NOT_EQUAL":
          return EvalSimpleOp(tree, env);
        //AND and OR short-circuit
        case "OR":
          return EvalShortCircuitOp(tree, env);
        case "AND":
          return EvalShortCircuitOp(tree, env);
        //imperative constructs
        case "IF":
          return EvalIf(tree, env);
        case "WHILE":
          return EvalWhile(tree, env);
        case "ASSIGN":
          return EvalAssign(tree, env);
        //program and function body are parsed as blocks
        case "block":
          return EvalBlock(tree, env);
        case "RETURN":
          return EvalReturn(tree, env);
        case "array":
          return EvalArray(tree, env);
        //built-ins
        case "PRINTLN":
          return EvalPrintLn(tree, env);
        case "PRINT":
          return EvalPrint(tree, env);
        case "NOW":
          return EvalNow(tree, env);
        case "TODAY":
          return EvalToday(tree, env);
        case "KILL":
          return EvalKill(tree, env);
        case "SLEEP":
          return EvalSleep(tree, env);
        case "SLEEPLONG":
          return EvalSleepLong(tree, env);
        case "THREAD":
          return EvalThread(tree, env);
        case "RANDOM":
          return EvalRandom(tree, env);
        case "COIN":
          return EvalCoin();
        case "DICE":
          return EvalDice(6);
        case "D4":
          return EvalDice(4);
        case "D8":
          return EvalDice(8);
        case "D10":
          return EvalDice(10);
        case "D12":
          return EvalDice(12);
        case "D20":
          return EvalDice(20);
        case "D100":
          return EvalDice(100);
        case "CREATOR":
          return EvalLanguageCreator();
        case "FIB":
          return EvalFibonacci(tree, env);
        case "LEN":
          return EvalLength(tree, env);
        case "COMPARE_STRINGS":
          return EvalCompareStrings(tree, env);
      }
      throw new Exception("No evaluation function for type " + tree.type);
    }

    //Builtins >>>>>>>>>>>>>>>>>>>>>>>>>>>>
    /// <summary>
    /// Converts strings to integer codes. Returns 1, 0, -1 for arg1 >, ==, < arg2
    /// </summary>
    private static Lexeme EvalCompareStrings(Lexeme tree, Lexeme env) {
      var arg1 = tree.Left;
      var arg2 = arg1.Right;

      var val = arg1.GetValue().CompareTo(arg2.GetValue());

      return new Lexeme("INTEGER", val);
    }

    /// <summary>
    /// returns the length of a string or array
    /// </summary>
    private static Lexeme EvalLength(Lexeme tree, Lexeme env) {
      var term = Eval(tree.Left.Left, env);
      if (term.type == "STRING") {
        return new Lexeme("INTEGER", term.sval.Length);
      } else {
        return new Lexeme("INTEGER", term.aval.Length);
      }
    }

    /// <summary>
    /// Returns the nth Fibonacci term
    /// </summary>
    private static Lexeme EvalFibonacci(Lexeme tree, Lexeme env) {
      int n = Convert.ToInt32(Eval(tree.Left.Left.Left, env).GetValue()) - 1;

      double sqrt5 = Math.Sqrt(5);
      double p1 = (1 + sqrt5) / 2;
      double p2 = -1 * (p1 - 1);


      double n1 = Math.Pow(p1, n + 1);
      double n2 = Math.Pow(p2, n + 1);
      return new Lexeme("INTEGER", (int)((n1 - n2) / sqrt5));
    }

    /// <summary>
    /// Returns developer of the Language
    /// </summary>
    private static Lexeme EvalLanguageCreator() {
      /*Credits also go to Microsoft for development of C#
        Xamarian (now part of Microsoft) for the development of Mono
      */
      return new Lexeme("STRING", "Brandon Poole");
    }

    /// <summary>
    /// Returns a die roll of the given sides
    /// </summary>
    private static Lexeme EvalDice(int sides) {
      var result = new Random().Next(1, sides + 1);
      System.Threading.Thread.Sleep(new Random().Next(15, 20)); //Without sleeping for at least 11-15 milliseconds repeats will be generated. Sleeping a random time between 15-20 for more random? :D

      return new Lexeme("INTEGER", result);
    }

    // <summary>
    /// returns Heads or Tails
    /// </summary>
    private static Lexeme EvalCoin() {
      var result = new Random().Next(2);
      string coin = "coin";
      if (result == 0) {
        coin = "Heads";
      } else {
        coin = "Tails";
      }

      System.Threading.Thread.Sleep(new Random().Next(15, 20));

      return new Lexeme("STRING", coin);
    }

    /// <summary>
    /// no args returns random int from 1-100 inclusive
    /// one arg returns random int from 1-arg1 inclusive
    /// two args returns random int from arg1-arg2 inclusive
    /// </summary>
    private static Lexeme EvalRandom(Lexeme tree, Lexeme env) {
      var args = tree.Left.Left;

      int lowerBound = 1;
      int upperBound = 100;

      if (tree.Left != null) {
        upperBound = Convert.ToInt32(Eval(args.Left, env).GetValue());

        if (tree.Left.Right != null) {
          lowerBound = upperBound;
          upperBound = Convert.ToInt32(Eval(args.Right, env).GetValue());
        }
      }

      var result = new Random().Next(lowerBound, upperBound + 1);
      System.Threading.Thread.Sleep(new Random().Next(15,20)); //Without sleeping for at least 11-15 milliseconds repeats will be generated. Sleeping a random time between 15-20 for more random? :D

      return new Lexeme("INTEGER", result);
    }

    /// <summary>
    /// Creates and runs a thread with optional statements
    /// Remember that main thread ending before other threads ends the program rule apply!
    /// </summary>
    private static Lexeme EvalThread(Lexeme tree, Lexeme env) {
      System.Threading.Tasks.Task.Run(() => {
        var statements = tree.Left;
        while (statements != null) {
          Eval(statements.Left, env);
          statements = statements.Right;
        }
      });

      return tree;
    }

    /// <summary>
    /// Sleeps the application for x seconds
    /// </summary>
    private static Lexeme EvalSleepLong(Lexeme tree, Lexeme env) {
      var time = Convert.ToInt32(Eval(tree.Left, tree).GetValue()) * 1000;
      System.Threading.Thread.Sleep(time);
      return tree;
    }

    /// <summary>
    /// Sleeps the application for x milliseconds
    /// </summary>
    private static Lexeme EvalSleep(Lexeme tree, Lexeme env) {
      var time = Convert.ToInt32(Eval(tree.Left, tree).GetValue());
      System.Threading.Thread.Sleep(time);
      return tree;
    }

    /// <summary>
    /// Kills the current application
    /// </summary>
    private static Lexeme EvalKill(Lexeme tree, Lexeme env) {
      System.Environment.Exit(0);
      return null;
    }

    /// <summary>
    /// Returns date string in "M/d/yyyy" format
    /// </summary>
    private static Lexeme EvalToday(Lexeme tree, Lexeme env) {
      return new Lexeme("STRING", DateTime.Today.ToShortDateString());
    }

    /// <summary>
    /// Returns date string in "M/d/yyyy - h:mm tt" format
    /// </summary>
    private static Lexeme EvalNow(Lexeme tree, Lexeme env) {
      return new Lexeme("STRING", DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString());
    }


    /// <summary>
    /// Prints arguments to console
    /// </summary>
    private static Lexeme EvalPrint(Lexeme tree, Lexeme env) {
      var args = tree.Left.Left;

      if (args == null) {
        Console.Write("");
      } else {
        string message = args.Left.GetValue();
        while (args.Right != null) {
          message += args.Right.Left.GetValue();
          args = args.Right;
        }
        Console.Write(message);
      }
      return tree;
    }

    /// <summary>
    /// Prints arguments to console followed by a newline
    /// </summary>
    private static Lexeme EvalPrintLn(Lexeme tree, Lexeme env) {
      var args = tree.Left.Left;

      if (args == null) {
        Console.WriteLine();
      } else {
        string message = Eval(args.Left, env).GetValue();
        while (args.Right != null) {
          message += Eval(args.Right.Left, env).GetValue();
          args = args.Right;
        }
        Console.WriteLine(message);
      }
      return tree;
    }
    //<<<<<<<<<<<<<<<<<<<<<<<< Builtins

    private static Lexeme EvalLambdaDef(Lexeme t, Lexeme env) {
      var body = t.Right;

      return Eval(body, env);
    }

    private static Lexeme EvalArraySetIndex(Lexeme t, Lexeme env) {
      var idx = t.Left.Right.ival;
      var val = t.Right.GetValue();
      var arrName = t.Left.sval;
      var arr = Environment.Lookup(arrName, env);

      if (arr.type == "ARRAY") {
        arr.aval[idx] = val;
        return t;
      } else if (arr.type == "STRING") {
        var sb = new System.Text.StringBuilder(arr.sval);
        sb[idx] = Convert.ToChar(val);
        arr.sval = sb.ToString();

        return t;
      } else {
        throw new Exception("Invalid type for indexing! Type: " + arr.type);
      }
    }

    private static Lexeme EvalArrayGetIndex(Lexeme tree, Lexeme env) {
      var idx = tree.Right.ival;
      var arr = Environment.Lookup(tree.sval, env);

      if (arr.type == "ARRAY") {
        var val = arr.aval[idx];

        try {
          return new Lexeme("INTEGER", Convert.ToInt32(val));
        } catch {
          return new Lexeme("STRING", val.ToString());
        }
      } else if (arr.type == "STRING") {
        var val = arr.sval[idx];
        return new Lexeme("STRING", val.ToString());
      } else {
        throw new Exception("Invalid type for indexing! Type: " + arr.type);
      }
    }

    private static Lexeme EvalArray(Lexeme tree, Lexeme env) {
      var treeVal = tree.Left;
      var vals = new List<object>();

      while (treeVal != null) {
        vals.Add(treeVal.GetValue());
        treeVal = treeVal.Right;
      }

      var arr = new Lexeme("ARRAY", vals.ToArray());

      return arr;
    }

    private static Lexeme EvalShortCircuitOp(Lexeme tree, Lexeme env) {
      var leftEval = Eval(tree.Left, env);
      switch (tree.type) {
        case "OR":
          if (leftEval.ival == 1) { //true
            return leftEval;
          } else {
            return Eval(tree.Right, env);
          }
        case "AND":
          var rightAndEval = Eval(tree.Right, env);

          if (leftEval.ival == 1 && rightAndEval.ival == 1) {
            return new Lexeme("INTEGER", 1);
          } else {
            return new Lexeme("INTEGER", 0);
          }
        default:
          throw new Exception("Invalid short circuit operation! Type: " + tree.type);
      }
    }

    private static Lexeme EvalWhile(Lexeme tree, Lexeme env) {
      var conditional = Eval(tree.Left, env);
      var whileBlock = tree.Right.Left;

      if (conditional.type != "INTEGER") {
        throw new Exception("Invalid conditional in WHILE statement");
      }

      if (conditional.ival == 1) { //execute loop
        var whileEnv = Environment.Extend(null, null, env);
        Eval(whileBlock, whileEnv);
        return EvalWhile(tree, env);
      } else {
        return tree;
      }
    }

    private static Lexeme EvalIf(Lexeme tree, Lexeme env) {
      var conditional = tree.Left.Left;
      var ifBlock = tree.Left.Right.Left;

      var origCond = conditional; //only used for error message
      conditional = Eval(conditional, env);
        
      if (conditional.type != "INTEGER") {
        throw new Exception("Invalid conditional in IF statement! Conditional is type: " + origCond.type);
      }

      if (conditional.ival == 1) { //true
        var ifEnv = Environment.Extend(null, null, env);
        return Eval(ifBlock, ifEnv);
      } else { //false
        if (tree.Right != null) {
          if (tree.Right.type == "ELIF") {
            return EvalIf(tree.Right, env); //shares same logic as IF so no need to make a new function
          }
          else if (tree.Right.type == "ELSE") {
            return EvalElse(tree.Right, env);
          } else {
            throw new Exception("Invalid link in if statement: " + tree.Right.type);
          }
        } else {
          return conditional; //if is false and no chain
        }
      }
    }

    private static Lexeme EvalElse(Lexeme tree, Lexeme env) {
      var elseBlock = tree.Left.Left;
      var elseEnv = Environment.Extend(null, null, env);
      return Eval(elseBlock, elseEnv);
    }

    private static Lexeme EvalReturn(Lexeme tree, Lexeme env) {
      return Eval(tree.Left, env);
    }

    private static Lexeme EvalBlock(Lexeme tree, Lexeme env) {
      Lexeme result = null;
      while (tree != null) {
        result = Eval(tree.Left, env);
        tree = tree.Right;
      }
      return result;
    }

    private static Lexeme EvalFuncDef(Lexeme t, Lexeme env) {
      var closure = Cons(new Lexeme("CLOSURE"), env, t);
      env = Environment.Insert(GetFuncDefName(t), closure, env);
      return t;
    }

    private static Lexeme GetFuncDefName(Lexeme t) {
      return new Lexeme("ID", t.sval);
    }

    private static Lexeme EvalFuncCall(Lexeme t, Lexeme env) {
      var args = GetFuncCallArgs(t);
      var closure = GetFuncCallClosure(t, env);

      var parameters = GetClosureParams(closure);
      var body = GetClosureBody(closure);
      var senv = GetClosureEnvironment(closure);
      var eargs = EvalArgs(args, env);
      var xenv = Environment.Extend(parameters, eargs, senv);

      return Eval(body, xenv);
    }

    private static Lexeme EvalArgs(Lexeme args, Lexeme env) {
      if (args == null) {
        return args;
      }

      var temp = args.Right;
      args = Eval(args, env).Left;
      args.Right = temp;
      if (args.Right != null) {
        args.Right = EvalArgs(args.Right, env);
      }
      return args;
    }

    private static Lexeme GetClosureEnvironment(Lexeme closure) {
      return closure.Left;
    }

    private static Lexeme GetClosureBody(Lexeme closure) {
      return closure.Right.Right.Right.Left;
    }

    private static Lexeme GetClosureParams(Lexeme closure) {
      return closure.Right.Right.Left;
    }

    private static Lexeme GetFuncCallClosure(Lexeme t, Lexeme env) {
      return Environment.Lookup(t.sval, env);
    }

    private static Lexeme GetFuncCallArgs(Lexeme t) {
      return t.Right.Left;
    }

    private static Lexeme EvalVarDef(Lexeme t, Lexeme env) {
      var setTo = Eval(t.Right, env);
      if (setTo.type == "STRING" || setTo.type == "INTEGER") {
        try {
          Environment.Lookup(t.sval, env);
          Environment.Update(t.sval, env, setTo.GetValue());
        }
        catch {
          env = Environment.Insert(t, setTo, env);
        }
      }
      else if (setTo.type == "ARRAY") {
        try {
          Environment.Lookup(t.sval, env);
          Environment.Update(t.sval, env, setTo.aval);
        }
        catch {
          env = Environment.Insert(t, setTo, env);
        }
      }
      else if (setTo.type == "ID") {
        env = Environment.Insert(t, Environment.Lookup(setTo.sval, env), env);
      }
      else if (setTo.type == "ENV") { //object
        try {
          Environment.Lookup(t.sval, env);
          Environment.Update(t.sval, env, setTo.envVal);
        }
        catch {
          env = Environment.Insert(t, setTo, env);
        }
      } else if (setTo.type == "CLOSURE") {
        try {
          Environment.Lookup(t.sval, env);
          Environment.Update(t.sval, env, setTo.Left.envVal);
        }
        catch {
          env = Environment.Insert(t, setTo, env);
        }
      } else {
        throw new Exception("Unexpected error occured in EvalVarDef for type " + setTo.type);
      }
      return t;
    }

    private static Lexeme EvalAssign(Lexeme t, Lexeme env) {
      if (t.Left != null && t.Left.Right != null && t.Left.Right.type == "INTEGER") { //array with index
        return EvalArraySetIndex(t, env);
      } else if (t.Left != null) { //object reference
        var obj = Environment.Lookup(t.Left.sval, env);
        var field = t.Left.Left;
        Lexeme val = null;
        if (t.Right.type == "array") {
          val = EvalArray(t.Right, env);
          Environment.Update(field.sval, obj, val.aval);
        } else {
          val = Eval(t.Right, env);
          if (val.type == "ENV") {
            Environment.Update(field.sval, obj, val.GetEnvValue());
          } else {
            Environment.Update(field.sval, obj, val.GetValue());
          }
        }
        return val;
      } else if(t.Right.type == "ID" && t.Right.Left != null && t.Right.Left.type == "ID") { // x = obj.method(params);
        var x = Eval(t.Right, env);
        var y = Eval(t.Right.Left, x);
        var z = Eval(y.Right, env);
        return z; //Didn't figure out this part so I'm returning some random Eval'd thing
      } else if (t.Right.type == "array") {
        return EvalArray(t.Right, env);
      } else {
        return Eval(t.Right, env);
      }
    }

    //operators
    private static Lexeme EvalSimpleOp(Lexeme t,Lexeme env) {
      switch (t.type) {
        case "PLUS":
          return EvalPlus(t, env);
        case "PLUS_TO":
          return EvalPlusTo(t, env);
        case "INCREMENT":
          return EvalIncrement(t, env);
        case "MINUS":
          return EvalMinus(t, env);
        case "MINUS_TO":
          return EvalMinusTo(t, env);
        case "DECREMENT":
          return EvalDecrement(t, env);
        case "TIMES":
          return EvalTimes(t, env);
        case "TIMES_TO":
          return EvalTimesTo(t, env);
        case "DIVIDES":
          return EvalDivides(t, env);
        case "DIVIDES_TO":
          return EvalDivideTo(t, env);
        case "MOD":
          return EvalMod(t, env);
        case "MOD_TO":
          return EvalModTo(t, env);
        case "EXPN":
          return EvalExpn(t, env);
        case "EXPN_TO":
          return EvalExpnTo(t, env);
        case "LESSTHAN":
          return EvalLessThan(t, env);
        case "LESSTHAN_EQUALTO":
          return EvalLessThanEqualTo(t, env);
        case "GREATERTHAN":
          return EvalGreaterThan(t, env);
        case "GREATERTHAN_EQUALTO":
          return EvalGreaterThanEqualTo(t, env);
        case "EQUAL":
          return EvalEqual(t, env);
        case "NOT_EQUAL":
          return EvalNotEqual(t, env);
      }

      throw new Exception("Unrecognized operator!");
    }

    private static Lexeme EvalNotEqual(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        if (left.ival != right.ival) {
          return new Lexeme("INTEGER", 1);
        }
        else {
          return new Lexeme("INTEGER", 0);
        }
      }

      throw new Exception("Invalid NOT_EQUAL!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalEqual(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        if (left.ival == right.ival) {
          return new Lexeme("INTEGER", 1);
        }
        else {
          return new Lexeme("INTEGER", 0);
        }
      }

      throw new Exception("Invalid EQUAL!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalGreaterThanEqualTo(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        if (left.ival >= right.ival) {
          return new Lexeme("INTEGER", 1);
        }
        else {
          return new Lexeme("INTEGER", 0);
        }
      }

      throw new Exception("Invalid GREATERTHAN_EQUALTO!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalGreaterThan(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        if (left.ival > right.ival) {
          return new Lexeme("INTEGER", 1);
        }
        else {
          return new Lexeme("INTEGER", 0);
        }
      }

      throw new Exception("Invalid GREATERTHAN!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalLessThanEqualTo(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        if (left.ival <= right.ival) {
          return new Lexeme("INTEGER", 1);
        }
        else {
          return new Lexeme("INTEGER", 0);
        }
      }

      throw new Exception("Invalid LESSTHAN_EQUALTO!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalLessThan(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        if (left.ival < right.ival) {
          return new Lexeme("INTEGER", 1);
        } else {
          return new Lexeme("INTEGER", 0);
        }
      }

      throw new Exception("Invalid LESSTHAN!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalExpnTo(Lexeme t, Lexeme env) {
      var left = t.Left;
      var right = Eval(t.Right, env);
      var leftVal = Environment.Lookup(left.sval, env);
      String newVal = null;
      if (leftVal.type == "INTEGER" && right.type == "INTEGER") {
        newVal = (Math.Pow(leftVal.ival, right.ival)).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      }
      else {
        throw new Exception("Unexpected types in EvalExpnTo!" + ExceptionTypesLeftRight(leftVal, right));
      }

      t = t.Left;
      return t;
    }

    private static Lexeme EvalExpn(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        return new Lexeme("INTEGER", (int)Math.Pow(left.ival, right.ival));
      }

      throw new Exception("Invalid EXPN!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalModTo(Lexeme t, Lexeme env) {
      var left = t.Left;
      var right = Eval(t.Right, env);
      var leftVal = Environment.Lookup(left.sval, env);
      String newVal = null;
      if (leftVal.type == "INTEGER" && right.type == "INTEGER") {
        newVal = (leftVal.ival % right.ival).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      }
      else {
        throw new Exception("Unexpected types in EvalModTo!" + ExceptionTypesLeftRight(leftVal, right));
      }

      t = t.Left;
      return t;
    }

    private static Lexeme EvalMod(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        return new Lexeme("INTEGER", left.ival % right.ival);
      }

      throw new Exception("Invalid MOD!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalDivideTo(Lexeme t, Lexeme env) {
      var left = t.Left;
      var right = Eval(t.Right, env);
      var leftVal = Environment.Lookup(left.sval, env);
      String newVal = null;
      if (leftVal.type == "INTEGER" && right.type == "INTEGER") {
        newVal = (leftVal.ival / right.ival).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      }
      else {
        throw new Exception("Unexpected types in EvalDividesTo!" + ExceptionTypesLeftRight(leftVal, right));
      }

      t = t.Left;
      return t;
    }

    private static Lexeme EvalDivides(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        return new Lexeme("INTEGER", left.ival / right.ival);
      }

      throw new Exception("Invalid DIVIDES!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalTimesTo(Lexeme t, Lexeme env) {
      var left = t.Left;
      var right = Eval(t.Right, env);
      var leftVal = Environment.Lookup(left.sval, env);
      String newVal = null;
      if (leftVal.type == "INTEGER" && right.type == "INTEGER") {
        newVal = (leftVal.ival * right.ival).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      }
      else {
        throw new Exception("Unexpected types in EvalTimesTo!" + ExceptionTypesLeftRight(leftVal, right));
      }

      t = t.Left;
      return t;
    }

    private static Lexeme EvalTimes(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        return new Lexeme("INTEGER", left.ival * right.ival);
      }

      throw new Exception("Invalid TIMES!" + ExceptionTypesLeftRight(left, right));
    }

    private static Lexeme EvalDecrement(Lexeme t, Lexeme env) {
      var id = t.Left.sval;
      var newVal = Environment.Lookup(id, env);
      if (newVal.type != "INTEGER") {
        throw new Exception("Error! Tried to call INCREMENT on type " + newVal.type);
      }
      Environment.Update(id, env, (newVal.ival - 1).ToString());
      return newVal;
    }

    private static Lexeme EvalMinusTo(Lexeme t, Lexeme env) {
      var left = t.Left;
      var right = Eval(t.Right, env);
      var leftVal = Environment.Lookup(left.sval, env);
      String newVal = null;
      if (leftVal.type == "INTEGER" && right.type == "INTEGER") {
        newVal = (leftVal.ival - right.ival).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      } else {
        var baseStr = Eval(left, env).GetValue();
        var removeStr = Eval(right, env).GetValue();
        newVal = baseStr.Replace(removeStr, "");
        Environment.Update(t.Left.sval, env, newVal);
      }

      t = t.Left;
      return t;
    }

    private static Lexeme EvalMinus(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        return new Lexeme("INTEGER", left.ival - right.ival);
      } else {
        var baseStr = left.GetValue().ToString();
        var removeStr = right.GetValue().ToString();
        var newString = baseStr.Replace(removeStr, "");
        return new Lexeme("STRING", newString);
      }
    }

    private static Lexeme EvalIncrement(Lexeme t, Lexeme env) {
      var id = t.Left.sval;
      var newVal = Environment.Lookup(id, env);
      if (newVal.type != "INTEGER") {
        throw new Exception("Error! Tried to call INCREMENT on type " + newVal.type);
      }
      Environment.Update(id, env, (newVal.ival + 1).ToString());
      return newVal;
    }

    private static Lexeme EvalPlusTo(Lexeme t, Lexeme env) {
      var left = t.Left;
      var right = Eval(t.Right, env);
      var leftVal = Environment.Lookup(left.sval, env);
      if (leftVal.type == "INTEGER" && right.type == "INTEGER") {
        var newVal = (leftVal.ival + right.ival).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      } else {
        var newVal = (leftVal.GetValue() + right.GetValue()).ToString();
        Environment.Update(t.Left.sval, env, newVal);
      }

      t = t.Left;
      return t;
    }

    private static Lexeme EvalPlus(Lexeme t, Lexeme env) {
      var left = Eval(t.Left, env);
      var right = Eval(t.Right, env);
      if (left.type == "INTEGER" && right.type == "INTEGER") {
        return new Lexeme("INTEGER", left.ival + right.ival);
      } else {
        return new Lexeme("STRING", left.GetValue() + right.GetValue());
      }
    }

    private static string ExceptionTypesLeftRight(Lexeme l, Lexeme r) {
      return String.Format(" Left: {0}, Right: {1}", l.type, r.type);
    }
  }
}
