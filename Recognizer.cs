using System;
using System.IO;
using System.Collections.Generic;

namespace dpl {
  public class Recognizer {
    List<Lexeme> tokens;
    Lexeme CurrentLexeme {
      get { return tokens[0]; }
    }
    
    Node programTree;
    public Node ProgramTree {
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
    
    Node match(string type) {
      matchNoAdvance(type);
      var matchedLex = CurrentLexeme;
      advance();
      return new Node(matchedLex);
    }
    
    void matchNoAdvance(string type) {
      if (!check(type)) {
        throw new System.Exception(String.Format("Syntax error!\n" +
        "\tCurrentLexeme type : {0}\n" +
        "\tExpecting type : {1}" , CurrentLexeme.type, type));
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
    Node program() {
      var tree = new Node();
      
      tree = optStatementList();
      
      return tree;
    }
    
    Node funcDef() {
      var tree = new Node();
      
      tree = match("ID");
      match("OPAREN");
      tree.Right = optParamList();
      match("CPAREN");
      tree.Right = block();
      
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
    
    Node optParamList() {
      var tree = new Node();
      
      if (!check("CPAREN")) {
        tree = paramList();
      } else {
        tree = new Node();
      }
      
      return tree;
    }
    
    Node paramList() {
      var tree = new Node();
      
      tree = expr();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right = paramList();
      }
      
      return tree;
    }
    
    Node optArgList() {
      var tree = new Node();
      
      if (!check("CPAREN")) {
        tree = argList();
      }
      
      return tree;
    }
    
    Node argList() {
      var tree = new Node();
      
      tree = expr();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right = argList();
      }
      
      return tree;
    }
    
    Node expr() {
      var tree = new Node();
      
      tree = primary();
      if (opPending()) {
        var temp = tree;
        tree = op();
        tree.Left = temp;
        tree.Right = expr();
        tree.Right.Right = optExprList();
      }
      
      return tree;
    }
    bool exprPending() {
      return primaryPending();
    }
    
    Node optExprList() {
      var tree = new Node();
      
      if (check("AND")) {
        tree = match("AND");
        tree.Left = expr();
      } else if (check("OR")) {
        tree = match("OR");
        tree.Left = expr();
      }
      
      return tree;
    }
    
    Node primary() {
      var tree = new Node();
      
      if (check("ID")) {
        tree = match("ID");
        if (check("OPAREN")) {
          match("OPAREN");
          tree.Right = optArgList();
          match("CPAREN");
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
        tree.Left = match("INTEGER");
      } else if (dictionaryPending()) {
        tree = dictionary();
      } else if (optArrayPending()) {
        tree = optArray();
      }
      
      return tree;
    }
    bool primaryPending() {
      return check("ID") || check("STRING") || check("INTEGER") || check("OPAREN") ||
        lambdaPending() || funcDefPending() || check("MINUS") ||
        optArrayPending() || dictionaryPending();
    }
    
    Node block() {
      var tree = new Node();
      
      match("OBRACE");
      tree = optStatementList();
      match("CBRACE");
      
      return tree;
    }
    
    Node optStatementList() {
      var tree = new Node();
      
      if (!check("CBRACE")) {
        statementList();
      }
      
      return tree;
    }
    bool optStatementListPending() {
      return (!check("CBRACE"));
    }
    
    Node statementList() {
      var tree = new Node();
      
      tree = statement();
      if (statementListPending()) {
        tree.Right = statementList();
      }
      
      return tree;
    }
    bool statementListPending() {
      return statementPending();
    }
    
    Node statement() {
      var tree = new Node();
      
      if (funcDefPending()) {
        tree = funcDef();
      } else if (varDefPending()) {
        tree = varDef();
        match("SEMI");
      }
      else if (whileLoopPending()) {
        tree = whileLoop();
      } else if (exprPending()) {
        tree = expr();
        match("SEMI");
      } else if (ifStatementPending()) {
        tree = ifStatement();
      } else if (check("RETURN")) {
        tree = match("RETURN");
        tree.Left = expr();
        match("SEMI");
      }
      
      return tree;
    }
    bool statementPending() {
      return funcDefPending() || whileLoopPending() || exprPending() ||
      ifStatementPending() || check("RETURN") || varDefPending();
    }
    
    Node op() {
      var tree = new Node();
      
      if (check("PLUS")) {
        tree = match("PLUS");
      } else if (check("INCREMENT")) {
        tree = match("INCREMENT");
      } else if (check("PLUS_TO")) {
        tree = match("PLUS_TO");
      } else if (check("MINUS")) {
        tree = match("MINUS");
      } else if (check("DECREMENT")) {
        tree = match("DECREMENT");
      } else if (check("MINUS_TO")) {
        tree = match("MINUS_TO");
      } else if (check("TIMES")) {
        tree = match("TIMES");
      } else if (check("TIMES_TO")) {
        tree = match("TIMES_TO");
      } else if (check("DIVIDES")) {
        tree = match("DIVIDES");
      } else if (check("DIVIDES_TO")) {
        tree = match("DIVIDES_TO");
      } else if (check("MOD")) {
        tree = match("MOD");
      } else if (check("MOD_TO")) {
        tree = match("MOD_TO");
      } else if (check("EXPN")) {
        tree = match("EXPN");
      } else if (check("EXPN_TO")) {
        tree = match("EXPN_TO");
      } else if (check("ASSIGN")) {
        tree = match("ASSIGN");
      } else if (check("EQUAL")) {
        tree = match("EQUAL");
      } else if (check("NOT_EQUAL")) {
        tree = match("NOT_EQUAL");
      } else if (check("LESSTHAN")) {
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
    bool opPending() {
      return check("PLUS") || check("INCREMENT") || check("PLUS_TO") ||
        check("MINUS") || check("DECREMENT") || check("MINUS_TO") ||
        check("TIMES") || check("TIMES_TO") ||
        check("DIVIDES") || check("DIVIDES_TO") ||
        check("MOD") || check("MOD_TO") ||
        check("EXPN") || check("EXPN_TO") ||
        check("ASSIGN") ||
        check("EQUAL") || check("NOT_EQUAL") ||
        check("LESSTHAN") || check("LESSTHAN_EQUALTO") ||
        check("GREATERTHAN") || check("GREATERTHAN_EQUALTO");
    }
    
    Node whileLoop() {
      var tree = new Node();
      
      tree = match("WHILE");
      match("OPAREN");
      tree.Left = expr();
      match("CPAREN");
      tree.Right = block();
      
      return tree;
    }
    bool whileLoopPending() {
      return check("WHILE");
    }
    
    Node ifStatement() {
      var tree = new Node();
      
      tree= match("IF");
      match("OPAREN");
      tree.Left = expr();
      match("CPAREN");
      tree.Left.Right = block();
      tree.Right = optElif();
      
      return tree;
    }
    bool ifStatementPending() {
      return check("IF");
    }
    
    Node optElif() {
      var tree = new Node();
      
      if (check("ELIF")) {
        tree = match("ELIF");
        match("OPAREN");
        tree.Left = expr();
        match("CPAREN");
        tree.Left.Right = block();
        tree.Right = optElif();
      } else {
        tree = optElse();
      }
      
      return tree;
    }
    
    Node optElse() {
      var tree = new Node();
      
      if (check("ELSE")) {
        tree = match("ELSE");
        tree.Right = block();
      }
      
      return tree;
    }
    
    Node varDef() {
      var tree = new Node();
      
      tree = match("ID");
      tree.Right = optAssign();
      
      return tree;
    }
    bool varDefPending() {
      return check("ID") && (checkAhead("SEMI",1) || checkAhead("ASSIGN",1));
    }
    
    Node optAssign() {
      var tree = new Node();
      
      if (check("ASSIGN")) {
        tree = match("ASSIGN");
        tree.Right = expr();
      }
      
      return tree;
    }
    
    Node optArray() {
      var tree = new Node();
      
      match("OBRACKET");
      if (arrayPending()) {
        tree = array();
      }
      match("CBRACKET");
      
      return tree;
    }
    bool optArrayPending() {
      return check("OBRACKET");
    }
    
    Node array() {
      var tree = new Node();
      
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
    
    Node lambda() {
      var tree = new Node();
      
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
        if (tokens[i+1].type == "LAMBDA_DEF") {
          isLambdaPending = true;
        }
      }
      return isLambdaPending;
    }
    
    Node dictionary() {
      var tree = new Node();
      
      match("OBRACE");
      tree = optDictList();
      match("CBRACE");
      
      return tree;
    }
    bool dictionaryPending() {
      return check("OBRACE");
    }
    
    Node optDictList() {
      var tree = new Node();
      
      if (check("STRING")) {
        tree = dictList();
      }
      
      return tree;
    }
    
    Node dictList() {
      var tree = new Node();
      
      tree = match("STRING");
      
      var temp = tree;
      tree = match("COLON");
      tree.Left = temp;
      
      tree.Right = primary();
      if (check("COMMA")) {
        match("COMMA");
        tree.Right.Right = dictList();
      }
      
      return tree;
    }
  }
}
