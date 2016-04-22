build:
	mcs LexerScanner.cs Lexer.cs Lexeme.cs
	mcs RecognizerScanner.cs Recognizer.cs Lexer.cs Lexeme.cs
	mcs EnvironmentTest.cs Environment.cs Lexeme.cs

run:
	make build
	mono RecognizerScanner.exe simple.txt
	mono RecognizerScanner.exe program.txt
	
prettyPrinter:
	mcs PrettyPrinter.cs Recognizer.cs Lexer.cs Lexeme.cs Node.cs

clean:
	rm *.exe
