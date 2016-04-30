using System;
using System.Collections.Generic;

namespace dpl {
  public class Recognizer {
    List<Lexeme> tokens;
    Lexeme CurrentLexeme
    {
      get { return tokens[0]; }
    }

    Lexeme programTree;
    public Lexeme ProgramTree
    {
      get { return programTree; }
    }

    public Recognizer(List<Lexeme> tokens) {
      this.tokens = tokens;

      parse();
    }

    void parse() {
      programTree = program();
      match("END_OF_FILE");
    }

    //support functions
    bool check(string type) {
      return CurrentLexeme.type == type;
    }

    void advance() {
      tokens.RemoveAt(0);
    }

    Lexeme match(string type) {
      matchNoAdvance(type);
      var matchedLex = CurrentLexeme;
      advance();
      return matchedLex;
    }

    void matchNoAdvance(string type) {
      if (!check(type)) {
        throw new System.Exception(String.Format("Syntax error!\n" +
        "\tCurrentLexeme type : {0}\n" +
        "\tExpecting type : {1}", CurrentLexeme.type, type));
      }
    }

    bool checkAhead(string type, int i) {
      if (tokens.Count < i + 1 || tokens[i].type != type) {
        return false;
      } else {
        return true;
      }
    }

    //Grammar Rules
    Lexeme program() {
      if (CurrentLexeme.type == "END_OF_FILE") {
        return null;
      } else {
        return optStatementList();
      }
    }

    Lexeme funcDef() {
      Lexeme tree;

      tree = match("ID");
      match("OPAREN");
      tree.Right = optParamList();
      match("CPAREN");
      tree.Right.Right = block();

      return tree;
    }
    bool funcDefPending() {
      bool isFuncDefPending = false;
      if (check("ID") && checkAhead("OPAREN", 1)) {
        int i = 2;
        while (i != tokens.Count && (tokens[i].type != "OBRACE" && tokens[i].type != "SEMI")) {
          i++;
        }
        if (checkAhead("OBRACE", i)) {
          isFuncDefPending = true;
        }
      }
      return isFuncDefPending;
    }

    Lexeme optParamList() {
      var tree = new Lexeme("paramList");

      if (!check("CPAREN")) {
        tree.Left = paramList();
      }

      return tree;
    }

    Lexeme paramList() {
      Lexeme tree;

      tree = expr1();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right = paramList();
      }

      return tree;
    }

    Lexeme optArgList() {
      Lexeme tree = new Lexeme("argList");

      if (!check("CPAREN")) {
        tree.Left = argList();
      }

      return tree;
    }

    Lexeme argList() {
      Lexeme tree = new Lexeme("arg");

      tree.Left = expr1();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right = argList();
      }

      return tree;
    }

    Lexeme expr1() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr2();
      while (op1Pending()) {
        temp = op1();
        temp.Left = tree;
        temp.Right = expr2();
        tree = temp;
      }

      return tree;
    }
    bool expr1Pending() {
      return expr2Pending();
    }

    Lexeme expr2() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr3();
      while (op2Pending()) {
        temp = op2();
        temp.Left = tree;
        temp.Right = expr3();
        tree = temp;
      }

      return tree;
    }
    bool expr2Pending() {
      return expr3Pending();
    }

    Lexeme expr3() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr4();
      while (op3Pending()) {
        temp = op3();
        temp.Left = tree;
        temp.Right = expr4();
        tree = temp;
      }

      return tree;
    }
    bool expr3Pending() {
      return expr4Pending();
    }

    Lexeme expr4() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr5();
      while (op4Pending()) {
        temp = op4();
        temp.Left = tree;
        temp.Right = expr5();
        tree = temp;
      }

      return tree;
    }
    bool expr4Pending() {
      return expr5Pending();
    }

    Lexeme expr5() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr6();
      while (op5Pending()) {
        temp = op5();
        temp.Left = tree;
        temp.Right = expr6();
        tree = temp;
      }

      return tree;
    }
    bool expr5Pending() {
      return expr6Pending();
    }

    Lexeme expr6() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr7();
      while (op6Pending()) {
        temp = op6();
        temp.Left = tree;
        temp.Right = expr6();
        tree = temp;
      }

      return tree;
    }
    bool expr6Pending() {
      return expr7Pending();
    }

    Lexeme expr7() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr8();
      while (op7Pending()) {
        temp = op7();
        temp.Left = tree;
        temp.Right = expr8();
        tree = temp;
      }

      return tree;
    }
    bool expr7Pending() {
      return expr8Pending();
    }

    Lexeme expr8() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr9();
      while (op8Pending()) {
        temp = op8();
        temp.Left = tree;
        temp.Right = expr9();
        tree = temp;
      }

      return tree;
    }
    bool expr8Pending() {
      return expr9Pending();
    }

    Lexeme expr9() {
      Lexeme tree = null;
      Lexeme temp;

      tree = expr10();
      while (op9Pending()) {
        temp = op9();
        temp.Left = tree;
        temp.Right = expr10();
        tree = temp;
      }

      return tree;
    }
    bool expr9Pending() {
      return expr10Pending();
    }

    Lexeme expr10() {
      Lexeme tree = null;
      Lexeme temp;

      tree = primary();
      while (op10Pending()) {
        temp = op10();
        temp.Left = tree;
        temp.Right = primary();
        tree = temp;
      }

      return tree;
    }
    bool expr10Pending() {
      return primaryPending();
    }

    Lexeme primary() {
      Lexeme tree = null;

      if (check("ID")) {
        tree = match("ID");
        if (check("OPAREN")) {
          match("OPAREN");
          tree.Right = optArgList();
          match("CPAREN");
        } else if (check("OBRACKET")) { //array get/set ex. x[1]
          match("OBRACKET");
          tree.Right = match("INTEGER");
          match("CBRACKET");
        } else if (check("DOT")) {
          match("DOT");
          tree.Left = match("ID");
        }
      } else if (check("STRING")) {
        tree = match("STRING");
      } else if (check("INTEGER")) {
        tree = match("INTEGER");
      } else if (lambdaPending()) {
        tree = lambda();
      } else if (funcDefPending()) {
        tree = funcDef();
      } else if (check("MINUS")) {
        tree = match("MINUS");
        tree.Left = new Lexeme("INTEGER", 0);
        if (check("INTEGER")) {
          tree.Right = match("INTEGER");
        } else if (check("ID")) {
          tree.Right = match("ID");
        }
      } else if (dictionaryPending()) {
        tree = dictionary();
      } else if (optArrayPending()) {
        tree = optArray();
      } else if (builtinPending()) {
        tree = builtin();
      }

      return tree;
    }
    bool primaryPending() {
      return check("ID") || check("STRING") || check("INTEGER") || check("OPAREN") ||
        lambdaPending() || funcDefPending() || check("MINUS") ||
        optArrayPending() || dictionaryPending() || builtinPending();
    }

    Lexeme builtin() {
      Lexeme tree = null;

      if (check("PRINTLN")) {
        tree = match("PRINTLN");
        match("OPAREN");
        tree.Left = optArgList();
        match("CPAREN");
      } else if (check("PRINT")) {
        tree = match("PRINT");
        match("OPAREN");
        tree.Left = optArgList();
        match("CPAREN");
      } else if (check("NOW")) {
        tree = match("NOW");
      } else if (check("TODAY")) {
        tree = match("TODAY");
      } else if (check("KILL")) {
        tree = match("KILL");
      } else if (check("SLEEP")) {
        tree = match("SLEEP");
        match("OPAREN");
        tree.Left = match("INTEGER");
        match("CPAREN");
      } else if (check("SLEEPLONG")) {
        tree = match("SLEEPLONG");
        match("OPAREN");
        tree.Left = match("INTEGER");
        match("CPAREN");
      } else if (check("THREAD")) {
        tree = match("THREAD");
        match("OPAREN");
        tree.Left = optStatementList();
        match("CPAREN");
      } else if (check("RANDOM")) {
        tree = match("RANDOM");
        match("OPAREN");
        tree.Left = optArgList();
        match("CPAREN");
      } else if (check("COIN")) {
        tree = match("COIN");
      } else if (check("DICE")) {
        tree = match("DICE");
      } else if (check("D4")) {
        tree = match("D4");
      } else if (check("D8")) {
        tree = match("D8");
      } else if (check("D10")) {
        tree = match("D10");
      } else if (check("D12")) {
        tree = match("D12");
      } else if (check("D20")) {
        tree = match("D20");
      } else if (check("D100")) {
        tree = match("D100");
      } else if (check("CREATOR")) {
        tree = match("CREATOR");
      } else if (check("FIB")) {
        tree = match("FIB");
        match("OPAREN");
        tree.Left = optArgList();
        match("CPAREN");
      } else if (check("LEN")) {
        tree = match("LEN");
        match("OPAREN");
        tree.Left = argList();
        match("CPAREN");
      } else if (check("COMPARE_STRINGS")) {
        tree = match("COMPARE_STRINGS");
        match("OPAREN");
        tree.Left = match("STRING");
        match("COMMA");
        tree.Left.Right = match("STRING");
        match("CPAREN");
      }

      return tree;
    }
    bool builtinPending() {
      return check("PRINTLN") || check("PRINT") || check("NOW") || check("TODAY") || check("KILL") ||
        check("SLEEP") || check("SLEEPLONG") || check("THREAD") || check("RANDOM") ||
        check("COIN") || check("DICE") || check("D4") || check("D8") || check("D10") ||
        check("D12") || check("D20") || check("D100") || check("CREATOR") || check("FIB") || check("LEN") ||
        check("COMPARE_STRINGS");
    }

    Lexeme block() {
      var tree = new Lexeme("block");

      match("OBRACE");
      tree.Left = optStatementList();
      match("CBRACE");

      return tree;
    }

    Lexeme optStatementList() {
      var tree = new Lexeme("empty");

      if (!check("CBRACE")) {
        tree = statementList();
      }

      return tree;
    }
    bool optStatementListPending() {
      return (!check("CBRACE"));
    }

    Lexeme statementList() {
      var tree = new Lexeme("statement");

      tree.Left = statement();
      if (statementListPending()) {
        tree.Right = statementList();
      }

      return tree;
    }
    bool statementListPending() {
      return statementPending();
    }

    Lexeme statement() {
      Lexeme tree = null;

      if (funcDefPending()) {
        tree = funcDef();
      } else if (varDefPending()) {
        tree = varDef();
        if (check("OPAREN")) { //getting value from object method
          match("OPAREN");
          tree.Right.Right.Left.Left = expr1();
          match("CPAREN");
        }
        match("SEMI");
      } else if (whileLoopPending()) {
        tree = whileLoop();
      } else if (expr1Pending()) {
        tree = expr1();
        if (check("OPAREN")) { //calling object method
          match("OPAREN");
          tree.Left = expr1();
          match("CPAREN");
        }
        match("SEMI");
      } else if (ifStatementPending()) {
        tree = ifStatement();
      } else if (check("RETURN")) {
        tree = match("RETURN");
        tree.Left = expr1();
        match("SEMI");
      }

      return tree;
    }
    bool statementPending() {
      return funcDefPending() || whileLoopPending() || expr1Pending() ||
      ifStatementPending() || check("RETURN") || varDefPending();
    }

    Lexeme op1() {
      Lexeme tree = null;

      if (check("PLUS_TO")) {
        tree = match("PLUS_TO");
      }else if (check("MINUS_TO")) {
        tree = match("MINUS_TO");
      } else if (check("TIMES_TO")) {
        tree = match("TIMES_TO");
      } else if (check("DIVIDES_TO")) {
        tree = match("DIVIDES_TO");
      } else if (check("MOD_TO")) {
        tree = match("MOD_TO");
      } else if (check("EXPN_TO")) {
        tree = match("EXPN_TO");
      } else if (check("ASSIGN")) {
        tree = match("ASSIGN");
      }

      return tree;
    }
    bool op1Pending() {
      return check("PLUS_TO") || check("MINUS_TO") || check("TIMES_TO") || check("DIVIDES_TO") || check("MOD_TO") || check("EXPN_TO") || check("ASSIGN");
    }

    Lexeme op2() {
      Lexeme tree = null;

      if (check("OR")) {
        tree = match("OR");
      }

      return tree;
    }
    bool op2Pending() {
      return check("OR");
    }

    Lexeme op3() {
      Lexeme tree = null;

      if (check("AND")) {
        tree = match("AND");
      }

      return tree;
    }
    bool op3Pending() {
      return check("AND");
    }

    Lexeme op4() {
      Lexeme tree = null;

      if (check("EQUAL")) {
        tree = match("EQUAL");
      } else if (check("NOT_EQUAL")) {
        tree = match("NOT_EQUAL");
      }

      return tree;
    }
    bool op4Pending() {
      return check("EQUAL") || check("NOT_EQUAL");
    }

    Lexeme op5() {
      Lexeme tree = null;

      if (check("LESSTHAN")) {
        tree = match("LESSTHAN");
      } else if (check("LESSTHAN_EQUALTO")) {
        tree = match("LESSTHAN_EQUALTO");
      } else if (check("GREATERTHAN")) {
        tree = match("GREATERTHAN");
      } else if (check("GREATERTHAN_EQUALTO")) {
        tree = match("GREATERTHAN_EQUALTO");
      }

      return tree;
    }
    bool op5Pending() {
      return check("LESSTHAN") || check("LESSTHAN_EQUALTO") || check("GREATERTHAN") || check("GREATERTHAN_EQUALTO");
    }

    Lexeme op6() {
      Lexeme tree = null;

      if (check("PLUS")) {
        tree = match("PLUS");
      } else if (check("MINUS")) {
        tree = match("MINUS");
      }

      return tree;
    }
    bool op6Pending() {
      return check("PLUS") || check("MINUS");
    }

    Lexeme op7() {
      Lexeme tree = null;

      if (check("TIMES")) {
        tree = match("TIMES");
      } else if (check("DIVIDES")) {
        tree = match("DIVIDES");
      } else if (check("MOD")) {
        tree = match("MOD");
      }

      return tree;
    }
    bool op7Pending() {
      return check("TIMES") || check("DIVIDES") || check("MOD");
    }

    Lexeme op8() {
      Lexeme tree = null;

      if (check("EXPN")) {
        tree = match("EXPN");
      }

      return tree;
    }
    bool op8Pending() {
      return check("EXPN");
    }

    Lexeme op9() {
      Lexeme tree = null;

      if (check("INCREMENT")) {
        tree = match("INCREMENT");
      } else if (check("DECREMENT")) {
        tree = match("DECREMENT");
      }

        return tree;
    }
    bool op9Pending() {
      return check("INCREMENT") || check("DECREMENT");
    }

    Lexeme op10() {
      Lexeme tree = null;

      if (check("DOT")) {
        tree = match("DOT");
      }

      return tree;
    }
    bool op10Pending() {
      return check("DOT");
    }

    Lexeme whileLoop() {
      Lexeme tree;

      tree = match("WHILE");
      match("OPAREN");
      tree.Left = expr1();
      match("CPAREN");
      tree.Right = block();

      return tree;
    }
    bool whileLoopPending() {
      return check("WHILE");
    }

    Lexeme ifStatement() {
      Lexeme tree;

      tree = match("IF");
      match("OPAREN");
      tree.Left = new Lexeme("JOIN");
      tree.Left.Left = expr1();
      match("CPAREN");
      tree.Left.Right = block();
      tree.Right = optElif();

      return tree;
    }
    bool ifStatementPending() {
      return check("IF");
    }

    Lexeme optElif() {
      Lexeme tree;

      if (check("ELIF")) {
        tree = match("ELIF");
        match("OPAREN");
        tree.Left = new Lexeme("JOIN");
        tree.Left.Left = expr1();
        match("CPAREN");
        tree.Left.Right = block();
        tree.Right = optElif();
      } else {
        tree = optElse();
      }

      return tree;
    }

    Lexeme optElse() {
      Lexeme tree = null;

      if (check("ELSE")) {
        tree = match("ELSE");
        tree.Left = block();
      }

      return tree;
    }

    Lexeme varDef() {
      Lexeme tree;

      tree = match("ID");
      tree.Right = optAssign();

      return tree;
    }
    bool varDefPending() {
      return check("ID") && (checkAhead("SEMI", 1) || checkAhead("ASSIGN", 1));
    }

    Lexeme optAssign() {
      Lexeme tree = null;

      if (check("ASSIGN")) {
        tree = match("ASSIGN");
        tree.Right = expr1();
      }

      return tree;
    }

    Lexeme optArray() {
      Lexeme tree = new Lexeme("array");

      match("OBRACKET");
      if (arrayPending()) {
        tree.Left = array();
      }
      match("CBRACKET");

      return tree;
    }
    bool optArrayPending() {
      return check("OBRACKET");
    }

    Lexeme array() {
      Lexeme tree;

      tree = primary();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right = array();
      }

      return tree;
    }
    bool arrayPending() {
      return primaryPending();
    }

    Lexeme lambda() {
      Lexeme tree;

      match("OPAREN");
      tree = optArgList();
      match("CPAREN");

      var temp = tree;
      tree = match("LAMBDA_DEF");
      tree.Left = temp;

      tree.Right = block();

      return tree;
    }
    bool lambdaPending() {
      bool isLambdaPending = false;
      if (check("OPAREN")) {
        int i = 1;
        while (tokens.Count != i && tokens[i].type != "CPAREN") {
          i++;
        }
        if (tokens[i + 1].type == "LAMBDA_DEF") {
          isLambdaPending = true;
        }
      }
      return isLambdaPending;
    }

    Lexeme dictionary() {
      Lexeme tree;

      match("OBRACE");
      tree = optDictList();
      match("CBRACE");

      return tree;
    }
    bool dictionaryPending() {
      return check("OBRACE");
    }

    Lexeme optDictList() {
      Lexeme tree = new Lexeme("dictionary");

      if (check("STRING")) {
        tree.Left = dictList();
      }

      return tree;
    }

    Lexeme dictList() {
      Lexeme tree;

      tree = match("STRING");

      match("COLON");

      tree.Left = primary();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right = dictList();
      }

      return tree;
    }
  }
}
