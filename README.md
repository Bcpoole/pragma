#Pragma
*Written by Brandon Poole*

[TOC]

#Description
**Pragma** is a dynamic language built off of the [C#](https://msdn.microsoft.com/en-us/library/kx37x362.aspx?f=255&MSPPError=-2147217396) [Mono](http://www.mono-project.com/) (now sponsored by [Microsoft](https://www.microsoft.com/en-us/)) written as a final project for CS 403 - Programming Languages.

#Setup and Run
##Mono
As per [StackOverflow](http://askubuntu.com/questions/497358/how-to-install-mono-on-ubuntu-64-bit-v14-04):
Install with apt-get: *(Ubuntu, Debian, and derivatives)*
```shell
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list 

sudo apt-get update

sudo apt-get install mono-complete
```

Using a different distro or need help? [Try the Mono Website's Download instructions.](http://www.mono-project.com/docs/getting-started/install/linux/)

###I have Make!

*(if you want to speed up the build process, remove all but the last line under **make build**)*
```shell
#build - or -> make build
make

#run
make run <filename>
```

if you wish to clean up the directory run:
```shell
make clean
```

###I don't have Make :(

```shell
#build
mcs EvaluatorScanner.cs Evaluator.cs Environment.cs Recognizer.cs Lexer.cs Lexeme.cs ConsMethods.cs

#run
mono EvaluatorScanner.exe <filename>
#or
evaluator <filename>
```

##Visual Studio
*For those of you who are Mono-challenged, or simply don't want to.*

1. Throw the files into a project
2. Set the file you want to compile and run in the command-line args
3. Run!

#Features
##Variables
Variables don't need types because **Dynamic**! *They have to be set to something though!* While they are dynamic, the only backing types are string, integer, array, and function.
```
x = "42";
x = 42;
x = [4, 2];
x; //returns array [4,2];

//Situationally Bad:
y; //only works if declared beforehand
```
Also features aliasing!
```
a = 5;
b = a;
println(a); //5
println(b); //5
a = 10;
println(a); //10
println(b); //10
```

##Booleans
Currently booleans are represented as integers.
```
	42 == 42; //returns 1
	42 < 2; //returns 0 in most universes
```

##Blocks
Blocks are denoted by curcly braces { }
```
if (1) {

}
```

##Conditionals (If-Elif-Else)
Since things should be uniform, conditionals **MUST** have a block. Chaining conditionals is done through use of *elif* until you want an *else* (which of course are optional).
```
if ("red == "blue") {
	println("Hide the moon!");
} elif ("red" == "yellow") {
	println("Kill a fellow");
} else {
	println("There beith no rhymes here");
}

//good
if (1 == 1) {
	1;
}

//bad
if (1 ==1)
	1;
```

##Comparison
Standard comparisons.
```
1 == 1;
1 != 1;
1 > 1;
1 >= 1;
1 < 1;
1 <= 1;
```

##Iteration
Iteration is handled through *while* loops. Currently no *for* loops exist.
```
x = 5;
while (x >= 0) {
	println(x);
	x--;
}
```

##Comments
C-style comments.
```
//Single line

/*
Block
Comment
*/
```

##Arrays
Arrays are 0-based indexing. You can also index strings since they're... ARRAYS (of characters)!
```
emptyArr = [];
oneEleArr = [4];
finalCountdown = [3, 2, 1];

println(finalCount); //prints [3, 2, 1] because arrays should be printable :)
finalCountdown[1]; //returns 2;
finalCountdown[2] = 0; //sets index 2 to zero

x = "but wait! I'm a string!";
x[4]; //w
x[0] = "B"; //must be a single character
println(x); //"But wait! I'm a string!" Take that bad grammar!

//Bad:
emptyArr[0] = ":(";
```

##Arithmetic
###Math!
```
//Addition
x = 1 + 2; //3
x += 4; //7
x++; //8

//Subtraction
x = 50 - 25; //25
x -= 20; //5
x--; //4

//Mutliplication
x = 5 * 5; //25
x *= 5; //225

//Division
x = 100 / 4; //25
x /= 5; //5

//Negation
x = 100;
-x; //-100

//Modulo
x = 50 % 47; //3
x %= 2; //1

//Exponentiation
x = 2^4; //16
x ^= 3; //4096
```

###Strings!
```
//Concatenation with +
x = "CS" + "403"; //CS403
x += " woooo"; //CS403 woooo

//Removal of substrings with -
x = "the rain in spain falls mainly on the plain"
x = x - "in"; //"the ra  spa falls maly on the pla"
x -= "the "; //"ra  spa falls maly on pla"
```

##Scopes
###Functions
Functions should look very familiar. One thing to note: return is purely syntactical sugar. The last thing evaluated will always return. Also, functions need a body!
```
myFunc() {
	"Hello World";
}

boomerang1(x) { //returns x
	return x;
}

boomerang2(x) { //returns x
	x;
}

nonerang(x) { //returns 5
	x;
	5;
}

```

And function in functions!
```
outterFunc() {
  println("Hello World");
  
  innerFunc() {
    println("Goodbye Moon");
  }
  innerFunc();
}

outterFunc();
```

<img src="http://i.qkme.me/3stqet.jpg" alt="Billy Mays supports Pragma!" height="200px"/>
Variables can be set to functions.
```
myFunc() {
  x = "There's no way you'll ever see me without a function call!";
}

a = myFunc;

println(a()); //Gasp! We got x from calling a!
```

###Objects
Objects are functions that end with this. Pass in all arguments needed unless you want strange behavior! Non-parameter fields are static.
```
Wrapper(val) {
  type = "wrapper";
  this;
}

x = Wrapper(1);
x.val; //1
x.type; //"wrapper"

y = Wrapper(2);
y.val; //2
y.val = 5;
y.val; //5
y.type = "foo";

x.val; //1
y.val; //5
x.type; //"foo"
y.type; //"foo"
```

###Lambdas
You (should) know what lambdas are. Note: You must end them with a semicolon.
```
() => {
  println("Hello");
  println("Goodbye");
};
```

Variables can be set to lambdas too!
```
myLambda = () => {
	5;
};
myLambda(); //5
```

##Precedence
Following the [guidelines](https://en.wikipedia.org/wiki/Order_of_operations) from lowest to highest:

1.   PLUS_TO | MINUS_TO | TIMES_TO | DIVIDES_TO | MOD_TO | EXPN_TO | ASSIGN
2.  OR
3.  AND
4.  EQUAL | NOT_EQUAL
5.  LESSTHAN | LESSTHAN_EQUALTO | GREATERTHAN | GREATERTHAN_EQUALTO
6.  PLUS | MINUS
7.  TIMES | DIVIDES | MOD
8.  EXPN
9.  INCREMENT | DECREMENT
10.  DOT


##Builtins
###println()
Prints arguments to console followed by a newline.
```
println("I'm up here and you're down there!");
println("I wish I were up there :(");
```

###print()
Prints arguments to console. There are no automatic spaces between arguments.
```
//Quickly! Chant: "Spirits, demons, ghosts, monsters, quickly leave."
print("Yu Mo Gui Gwai Fai"); //umm....
print("Di");
print("Zao"); 
//Oh no! You ended with FaiDiZao which isn't a word!
```

###now
Returns date string in "M/d/yyyy - h:mm tt" format.
```
now; //ex "4/29/2016 3:58 PM"
```

###today
Returns date string in "M/d/yyyy" format.
```
today; //ex "4/29/2016"
```

###kill
Terminate the program (take that you error catchers!)
```
println("I'll be back!");
kill;
println("HERE COMES ANOTHER TERMINATOR MOVIE"); //No! Kill it! Good :)
```

###sleep()
Sleeps current thread in milliseconds.
```
sleep(2); //sleep for 2 milliseconds
```

###sleeplong()
Same as sleep, but in seconds.
```
sleeplong(2); //sleep for 2 seconds
```

###thread()
Creates and run threads with optional statements.
**Remember that main thread ending before other threads ends the program rule apply!**
*Currenty no wait() feature exists*
```
println("Wacky Races!");
thread(
	println("Here comes Dick Dastardly and Muttley in the Mean Machine #00 (The Double Zero)");
	sleep(80);
	println("#00 rounding the bend...");
	sleep(300); //taking longer than normal when out of sight... hmm....
	println("Dick Dastardly crosses the finish line!");
);
thread(
	println("Here comes The Red Max in the Crimson Haybaler #4");
	sleep(100);
	println("Oh no! #4 has fallen into a pitfall!");
	sleep(500);
	println("But the Crimson Haybaler flying out!");
	sleep(100);
	println("The Red Max crosses the finish line!");
);
sleep(1500);
println("Who won?"); //Tricky Dick [Dastardly]
```

###random()
Getting a random number should by simple, don't you think?
```
random(); //returns a value between 1-100 inclusive
random(x); //returns a value between 1-x inclusive
random(x,y); //returns a value between x-y inclusive
```
####coin
Wish your coin flip was more than just 0 or 1?
```
coin; //returns 'Heads' or 'Tails'
```
####dice
Rolling dice is easy too!
```
dice; //returns a value between 1-6 inclusive
```
 Supports standard RPG dice :D
```
d2; //see coin
d4; //returns a value between 1-4 inclusive
d6; //see dice
d8; //returns a value between 1-8 inclusive
d10; //returns a value between 1-10 inclusive
d12; //returns a value between 1-12 inclusive
d20; //returns a value between 1-20 inclusive
d100; //returns a value between 1-100 inclusive
```

###creator
Because why not?
```
creator; //returns "Brandon Poole"
```

###fib()
Programmers love Fibonacci right? Well Pragma has it built into the language! *Note: since Pragma doesn't support the ulong type, it can only go up to the 46th term without an int overflow.*
```
fib(0); //returns 0
fib(1); //returns 1
fib(2); //returns 1
fib(46); //returns 1836311903
```

###len()
Return the length of a string or array.
```
len("krakatoa"); //returns 8
len([1,2,3,4,9]); //returns 5
```

###compareStrings()
Compares strings based on their numeric value.
```
compareStrings("b","a"); //returns 1
compareStrings("b","b"); //returns 0
compareStrings("b","c"); //returns -1
compareStrings("b","B"); //returns -1
```