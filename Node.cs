using System;

namespace dpl {
  public class Node {
    private Node left;
    public Node Left
    {
      get { return left;}
      set { left = value;}
    }
    private Node right;
    public Node Right
    {
      get { return right;}
      set { right = value;}
    }
    private Lexeme val;
    public Lexeme Value
    {
      get { return val;}
      set { val = value;}
    }
    
    public Node(Lexeme val) {
      Value = val;
    }
  }
}